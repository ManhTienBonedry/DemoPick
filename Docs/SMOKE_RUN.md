# DemoPick Smoke Test Report

- Started: 2026-04-14 22:44:37
- Duration: 2,04s
- Machine: MANHTIEN
- User: Admin
- App: DemoPick.exe

## Login Used

- Identifier: smoke_2cd4e1393f224f4e9925b8cc0e737f39@local
- Source: temp-user

## Steps

| Step | Result | Duration | Details |
|---|---|---:|---|
| DB init (schema + migrations) | SUCCESS | 15ms | OK |
| Obtain test credentials | SUCCESS | 956ms | Registered temp user: smoke_2cd4e1393f224f4e9925b8cc0e737f39@local |
| Login | SUCCESS | 825ms | Signed in as smoke_2cd4e1393f224f4e9925b8cc0e737f39@local (Staff) |
| Load courts | SUCCESS | 10ms | Courts: 19 |
| Create + cleanup test booking | SUCCESS | 64ms | Created booking on CourtID=1 2026-04-17 06:00 (90m). Removed=True |
| Logout | SUCCESS | 0ms | OK |
| Cleanup temp account | SUCCESS | 4ms | Deleted rows: 1 |
| Cleanup legacy SMOKE artifacts | SUCCESS | 19ms | Bookings=0, Members=0, StaffAccounts=0 |
| Logic tests (PriceCalculator + PendingOrders) | SUCCESS | 124ms | All logic assertions passed |
| Performance tests (micro-benchmark) | SUCCESS | 27ms | PriceCalc: 20000 loops in 14ms (1388561 ops/s, min 861679), PendingOrders: 20000 loops in 5ms (3718371 ops/s, min 2374874), checksum=6.900.000.000, report=D:\vstudio\BTL\DemoPick\DemoPick\Docs\Perf\PE… |

## Failures

No failures.

