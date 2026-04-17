#requires -Version 5.1

[CmdletBinding()]
param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$ConfigPath = "App.config",
    [string]$BinPath = "bin\\Debug",
    [switch]$Fix,
    [switch]$Quiet
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-PathAgainstRoot {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Root,
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $Root $Path))
}

function Get-PublicKeyTokenHex {
    param(
        [byte[]]$TokenBytes
    )

    if ($null -eq $TokenBytes -or $TokenBytes.Length -eq 0) {
        return ""
    }

    return ([System.BitConverter]::ToString($TokenBytes)).Replace("-", "").ToLowerInvariant()
}

function Add-RedirectEntry {
    param(
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlDocument]$Xml,
        [Parameter(Mandatory = $true)]
        [System.Xml.XmlElement]$AssemblyBindingNode,
        [Parameter(Mandatory = $true)]
        [string]$NamespaceUri,
        [Parameter(Mandatory = $true)]
        [string]$AssemblyName,
        [Parameter(Mandatory = $true)]
        [string]$PublicKeyToken,
        [Parameter(Mandatory = $true)]
        [string]$Version
    )

    $dep = $Xml.CreateElement("dependentAssembly", $NamespaceUri)
    $id = $Xml.CreateElement("assemblyIdentity", $NamespaceUri)
    $id.SetAttribute("name", $AssemblyName)
    $id.SetAttribute("publicKeyToken", $PublicKeyToken)
    $id.SetAttribute("culture", "neutral")

    $redirect = $Xml.CreateElement("bindingRedirect", $NamespaceUri)
    $redirect.SetAttribute("oldVersion", "0.0.0.0-$Version")
    $redirect.SetAttribute("newVersion", $Version)

    [void]$dep.AppendChild($id)
    [void]$dep.AppendChild($redirect)
    [void]$AssemblyBindingNode.AppendChild($dep)

    return $dep
}

$projectRootFull = Resolve-PathAgainstRoot -Root (Get-Location).Path -Path $ProjectRoot
$configFullPath = Resolve-PathAgainstRoot -Root $projectRootFull -Path $ConfigPath
$binFullPath = Resolve-PathAgainstRoot -Root $projectRootFull -Path $BinPath

if (-not (Test-Path $configFullPath)) {
    throw "Config file not found: $configFullPath"
}

if (-not (Test-Path $binFullPath)) {
    throw "Bin path not found: $binFullPath"
}

[xml]$xml = Get-Content -Path $configFullPath -Raw

$assemblyBindingNs = "urn:schemas-microsoft-com:asm.v1"
$ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
$ns.AddNamespace("asm", $assemblyBindingNs)

$configurationNode = $xml.SelectSingleNode("/configuration")
if ($null -eq $configurationNode) {
    throw "Invalid config file: missing /configuration root node."
}

$runtimeNode = $xml.SelectSingleNode("/configuration/runtime")
if ($null -eq $runtimeNode) {
    if (-not $Fix) {
        throw "Missing /configuration/runtime node. Run with -Fix to create it."
    }

    $runtimeNode = $xml.CreateElement("runtime")
    [void]$configurationNode.AppendChild($runtimeNode)
}

$assemblyBindingNode = $xml.SelectSingleNode("/configuration/runtime/asm:assemblyBinding", $ns)
if ($null -eq $assemblyBindingNode) {
    if (-not $Fix) {
        throw "Missing assemblyBinding node. Run with -Fix to create it."
    }

    $assemblyBindingNode = $xml.CreateElement("assemblyBinding", $assemblyBindingNs)
    [void]$runtimeNode.AppendChild($assemblyBindingNode)
}

$existingEntries = @{}
$dependentNodes = $assemblyBindingNode.SelectNodes("asm:dependentAssembly", $ns)
foreach ($depNode in $dependentNodes) {
    $idNode = $depNode.SelectSingleNode("asm:assemblyIdentity", $ns)
    $redirectNode = $depNode.SelectSingleNode("asm:bindingRedirect", $ns)
    if ($null -eq $idNode -or $null -eq $redirectNode) {
        continue
    }

    $name = $idNode.GetAttribute("name")
    if ([string]::IsNullOrWhiteSpace($name)) {
        continue
    }

    $existingEntries[$name] = [PSCustomObject]@{
        Dependent = $depNode
        Identity  = $idNode
        Redirect  = $redirectNode
    }
}

$trackedAssemblies = @(
    "System.Runtime.CompilerServices.Unsafe",
    "System.Buffers",
    "System.Memory",
    "System.Numerics.Vectors",
    "System.Reflection.Metadata",
    "System.Collections.Immutable",
    "System.Resources.Extensions",
    "Microsoft.Bcl.HashCode"
)

$results = New-Object System.Collections.Generic.List[object]
$configChanged = $false

foreach ($assemblyName in $trackedAssemblies) {
    $dllPath = Join-Path $binFullPath ($assemblyName + ".dll")
    if (-not (Test-Path $dllPath)) {
        if ($existingEntries.ContainsKey($assemblyName)) {
            $results.Add([PSCustomObject]@{
                Assembly        = $assemblyName
                ActualVersion   = "(missing)"
                RedirectVersion = $existingEntries[$assemblyName].Redirect.GetAttribute("newVersion")
                Status          = "WARN"
                Note            = "Redirect exists but DLL not found in bin path."
            })
        }

        continue
    }

    $asm = [System.Reflection.AssemblyName]::GetAssemblyName($dllPath)
    $actualVersion = $asm.Version.ToString()
    $actualPkt = Get-PublicKeyTokenHex -TokenBytes $asm.GetPublicKeyToken()

    if (-not $existingEntries.ContainsKey($assemblyName)) {
        if ($Fix) {
            $added = Add-RedirectEntry -Xml $xml -AssemblyBindingNode $assemblyBindingNode -NamespaceUri $assemblyBindingNs -AssemblyName $assemblyName -PublicKeyToken $actualPkt -Version $actualVersion
            $existingEntries[$assemblyName] = [PSCustomObject]@{
                Dependent = $added
                Identity  = $added.SelectSingleNode("asm:assemblyIdentity", $ns)
                Redirect  = $added.SelectSingleNode("asm:bindingRedirect", $ns)
            }

            $configChanged = $true
            $results.Add([PSCustomObject]@{
                Assembly        = $assemblyName
                ActualVersion   = $actualVersion
                RedirectVersion = $actualVersion
                Status          = "FIXED"
                Note            = "Missing redirect added."
            })
        }
        else {
            $results.Add([PSCustomObject]@{
                Assembly        = $assemblyName
                ActualVersion   = $actualVersion
                RedirectVersion = "(missing)"
                Status          = "ERROR"
                Note            = "Missing binding redirect."
            })
        }

        continue
    }

    $entry = $existingEntries[$assemblyName]
    $redirectNode = $entry.Redirect
    $idNode = $entry.Identity

    $currentNewVersion = $redirectNode.GetAttribute("newVersion")
    $currentOldVersion = $redirectNode.GetAttribute("oldVersion")
    $expectedOldVersion = "0.0.0.0-$actualVersion"
    $pktFromConfig = $idNode.GetAttribute("publicKeyToken")

    $versionMismatch = ($currentNewVersion -ne $actualVersion)
    $rangeMismatch = ($currentOldVersion -ne $expectedOldVersion)
    $pktMismatch = (-not [string]::IsNullOrWhiteSpace($pktFromConfig) -and $pktFromConfig.ToLowerInvariant() -ne $actualPkt)

    if ($versionMismatch -or $rangeMismatch -or $pktMismatch) {
        if ($Fix) {
            $redirectNode.SetAttribute("newVersion", $actualVersion)
            $redirectNode.SetAttribute("oldVersion", $expectedOldVersion)
            $idNode.SetAttribute("publicKeyToken", $actualPkt)

            $configChanged = $true
            $results.Add([PSCustomObject]@{
                Assembly        = $assemblyName
                ActualVersion   = $actualVersion
                RedirectVersion = $actualVersion
                Status          = "FIXED"
                Note            = "Redirect updated to match bin DLL."
            })
        }
        else {
            $noteParts = New-Object System.Collections.Generic.List[string]
            if ($versionMismatch) { $noteParts.Add("newVersion=$currentNewVersion") }
            if ($rangeMismatch) { $noteParts.Add("oldVersion=$currentOldVersion") }
            if ($pktMismatch) { $noteParts.Add("publicKeyToken=$pktFromConfig") }

            $results.Add([PSCustomObject]@{
                Assembly        = $assemblyName
                ActualVersion   = $actualVersion
                RedirectVersion = $currentNewVersion
                Status          = "ERROR"
                Note            = "Mismatch: $($noteParts -join ', ')"
            })
        }
    }
    else {
        $results.Add([PSCustomObject]@{
            Assembly        = $assemblyName
            ActualVersion   = $actualVersion
            RedirectVersion = $currentNewVersion
            Status          = "OK"
            Note            = "Aligned"
        })
    }
}

if ($Fix -and $configChanged) {
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.NewLineChars = "`r`n"
    $settings.NewLineHandling = [System.Xml.NewLineHandling]::Replace
    $settings.OmitXmlDeclaration = $false

    $writer = [System.Xml.XmlWriter]::Create($configFullPath, $settings)
    try {
        $xml.Save($writer)
    }
    finally {
        $writer.Dispose()
    }
}

$results = $results | Sort-Object Assembly

if (-not $Quiet) {
    Write-Host "Checked config: $configFullPath"
    Write-Host "Checked bin path: $binFullPath"
    Write-Host ""
    $results | Format-Table Assembly, ActualVersion, RedirectVersion, Status, Note -AutoSize
    Write-Host ""
}

$errorCount = @($results | Where-Object { $_.Status -eq "ERROR" }).Count
$warnCount = @($results | Where-Object { $_.Status -eq "WARN" }).Count
$fixedCount = @($results | Where-Object { $_.Status -eq "FIXED" }).Count

if ($Fix) {
    if (-not $Quiet) {
        if ($configChanged) {
            Write-Host "Updated App.config ($fixedCount item(s) fixed)."
        }
        else {
            Write-Host "No changes needed."
        }
    }

    exit 0
}

if ($errorCount -gt 0) {
    Write-Error "Found $errorCount redirect mismatch(es). Run this script with -Fix to auto-correct."
    exit 1
}

if (-not $Quiet) {
    Write-Host "Validation passed. Errors: $errorCount. Warnings: $warnCount."
}

exit 0