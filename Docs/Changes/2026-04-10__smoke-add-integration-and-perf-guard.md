# Add tests: auto-member integration + performance baseline guard

Date: 2026-04-10

## Scope
Extended smoke testing in `Services/SmokeTestRunner.cs` with:
- DB integration assertions for auto-creating `Members` during checkout.
- Machine-profile performance baseline and regression guard.
- Dedicated per-module performance reports for long-term tracking.

## 1) Logic test: auto create Member on checkout
Added integration test path in logic suite:
- Creates a live booking on an available court with guest format `Name - Phone`.
- Runs `PosService.Checkout(memberId: 0, ...)`.
- Asserts:
  - `Invoices.MemberID` is created and valid.
  - `Members.Phone` matches expected phone.
  - `Members.TotalSpent` is accumulated.
  - `Members.TotalHoursPurchased` is accumulated.
  - `Bookings.MemberID` is linked and `Bookings.Status = Paid`.
- Cleans up smoke artifacts (invoice details, invoice, booking, member) best-effort.

## 2) Performance baseline + regression guard
Added machine-profile baseline file:
- `Docs/PERF_BASELINES.csv`
- On first run for a machine, baseline is bootstrapped at 80% of measured ops/s.
- On subsequent runs, guard fails if current ops/s < baseline threshold.

Guarded modules:
- `PriceCalculator`
- `PendingOrders` in-memory ops

## 3) Per-module performance reporting (long-term)
Added outputs:
- Last run report: `Docs/Perf/PERF_LAST_RUN.md`
- History log: `Docs/Perf/PERF_MODULES_HISTORY.csv`

Each run records:
- timestamp, machine, module
- iterations, elapsed ms, ops/s
- baseline threshold, pass/fail, mode

## Validation
Ran smoke mode:
- `DemoPick.exe --smoke`
- Result: all steps passed, including new integration assertions and performance guard.
