# Zero Trust Security Audit (DemoPick)

## Scope & threat model
This audit assumes a “Zero Trust” posture for the workstation and the application folder:
- Local attacker may read/modify files next to the application (including `.config` and `.sql`).
- Local attacker may run the app and interact with UI.
- SQL Server is reachable via the configured connection string.

Out of scope (not enough evidence in this repo alone): enterprise identity, network perimeter controls, endpoint protection policy.

## High-level summary
**Top risk:** a default admin credential is automatically seeded into the database on first-run.

Additional material risks:
- Compiled binaries (bin/obj outputs) and `nuget.exe` are present in the repo tree.
- SQL bootstrap + migrations are executed at startup. In the current implementation they are embedded resources (tampering then requires modifying the built binaries, not just editing `.sql` files next to the app).

---

## A) Critical issues (fix before any real deployment)

### A1. Default admin credential is seeded with a hardcoded password
**Evidence**
- Hardcoded password: [Services/AuthService.cs](../Services/AuthService.cs#L158)

**Impact**
- If the app reaches a fresh database (or a reset DB), an attacker can immediately authenticate as Admin with a known password.
- This defeats any role gating in the UI and also enables destructive “dev tool” paths if other gates are bypassed.

**Where**
- `Services/AuthService.cs` (see `SeedAdminIfEmpty`).

**Recommendation**
Choose one of these patterns (ordered by security):
1) **No auto-seed in Release:** only seed in DEBUG; require normal registration + admin promotion out-of-band.
2) **One-time bootstrap secret:** read initial admin password from a secure channel (environment variable, installer-time secret), then force change on first login and/or disable bootstrap afterward.
3) **Random password on first run:** generate a strong random password, store only the hash, and show the password once to the operator (avoid logging it to DB/files).

---

## B) Suspicious / high-risk surfaces (likely benign, but worth tightening)

### B1. Runtime executes SQL scripts at startup (bootstrap + migrations)
**Evidence**
- Bootstrap script is executed from embedded resources: [Services/SchemaInstaller.cs](../Services/SchemaInstaller.cs)
- Migrations are discovered and executed from embedded resources: [Services/MigrationsRunner.cs](../Services/MigrationsRunner.cs)
- SQL resources are embedded by the project: [DemoPick.csproj](../DemoPick.csproj)

**Impact**
- If an attacker can modify the deployed folder, they can still replace/patch the built binaries and get arbitrary SQL executed at startup under the app’s DB privileges.
- In a Zero Trust model, “shipping bootstrap + migration logic” is still effectively “shipping privileged code”; embedding reduces (but does not eliminate) tamper risk compared to loose `.sql` files.

**Recommendation**
- Treat scripts as privileged code: embed as resources and/or verify integrity (e.g., hardcoded SHA256 allowlist) before execution.
- Ensure DB principal used by the app has least privilege.

### B2. Checked-in binaries and tooling increase the chance of supply-chain/style attacks
**Evidence**
- Tooling binary exists in-tree: [nuget.exe](../nuget.exe)
- Build outputs may appear locally after a build (examples: `bin/`, `obj/`, `.vs/`).
- `.gitignore` exists and already ignores build outputs.
- NuGet restore path is configured outside the project directory (sibling `packages` folder): [NuGet.Config](../NuGet.Config#L5)
- Project references packages via relative HintPath into that external packages folder: [DemoPick.csproj](../DemoPick.csproj#L42-L56)

**Impact**
- Binaries in source trees are a classic hiding place for unwanted payloads.
- Accidentally committing outputs can propagate unreviewed binaries.

**Recommendation**
- Add `.gitignore` for `bin/`, `obj/`, `.vs/` and any package cache.
- Remove checked-in build outputs and prefer restoring/building from source.
- If `nuget.exe` is needed, pin to an official source, verify signature, and keep it out of the main repo if possible.

### B3. P/Invoke (DllImport) usage in UI
**Evidence**
- `DllImport` usage (rounded corners / drag support):
   - [Views/UCLogin.cs](../Views/UCLogin.cs)
   - [Views/UCRegister.cs](../Views/UCRegister.cs)
   - [Views/FrmUserMenu.cs](../Views/FrmUserMenu.cs#L62-L63)
   - [Views/FrmDatSanCoDinh.WindowChrome.cs](../Views/FrmDatSanCoDinh.WindowChrome.cs)
   - [Services/NativeMethods.cs](../Services/NativeMethods.cs)

**Impact**
- P/Invoke increases the attack surface and makes static analysis harder; however the usage here appears limited to standard window-dragging and rounded corners.

**Recommendation**
- Keep P/Invoke usage minimal and centralized.
- Consider replacing with managed equivalents where feasible.

---

## C) Weak practices / hardening opportunities

### C1. Diagnostics may reveal environment details and internal exception messages
**Evidence**
- DB init error message includes server/database/auth details: [Services/DbDiagnostics.cs](../Services/DbDiagnostics.cs#L10-L23)
- Technical exception details are appended to the message: [Services/DbDiagnostics.cs](../Services/DbDiagnostics.cs#L48-L86)

**Impact**
- In shared environments/screenshots/log collection, this can leak internal topology and error details.

**Recommendation**
- In Release builds, show a user-safe message and log technical details separately (with access controls).

### C2. Connection string uses Integrated Security (deployment risk)
**Impact**
- Running the app as a high-privilege Windows user may implicitly give high DB privileges.

**Recommendation**
- For non-dev deployment, use a dedicated SQL principal (or constrained Windows identity) with least privilege.

**Where**
- Connection string: [App.config](../App.config#L8)

### C3. No rate limiting / lockout for login attempts
**Impact**
- Enables online brute-force attempts against `StaffAccounts`.

**Recommendation**
- Implement basic lockout/backoff and audit logging for repeated failures.

---

## D) Remediation checklist (practical next steps)

1) Remove hardcoded seed password (pick a safe bootstrap strategy) and rotate any existing admin credentials.
2) Add repo hygiene controls:
   - `.gitignore` for build outputs.
   - Remove `bin/` and `obj/` from version control.
3) Reduce script tampering risk:
   - Embed/verify SQL scripts before executing.
   - Run with least-privilege DB credentials.
4) Add automated checks (CI or local scripts):
   - Secret scanning (prevent committing credentials).
   - Dependency scanning / allowlisting.

## Verification
- Fresh DB creation no longer creates a predictable admin password.
- Running with modified SQL scripts is detected (integrity check) and aborts safely.
- No compiled binaries are stored in source control.
