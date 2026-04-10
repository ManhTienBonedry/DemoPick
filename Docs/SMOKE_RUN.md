# DemoPick Smoke Test Report

- Started: 2026-04-10 17:05:40
- Duration: 2,02s
- Machine: MANHTIEN
- User: Admin
- App: DemoPick.exe

## Login Used

- Identifier: smoke_eaea78f4626141bda272e2f3089f59a8@local
- Source: temp-user

## Steps

| Step | Result | Duration | Details |
|---|---|---:|---|
| DB init (schema + migrations) | SUCCESS | 21ms | OK |
| Obtain test credentials | SUCCESS | 866ms | Registered temp user: smoke_eaea78f4626141bda272e2f3089f59a8@local |
| Login | SUCCESS | 853ms | Signed in as smoke_eaea78f4626141bda272e2f3089f59a8@local (Staff) |
| Load courts | SUCCESS | 10ms | Courts: 19 |
| Create + cancel test booking | SUCCESS | 59ms | Created booking on CourtID=1 2026-04-13 06:00 (90m). Cancelled=True |
| Logout | SUCCESS | 0ms | OK |
| Cleanup temp account | SUCCESS | 3ms | Deleted rows: 1 |
| Logic tests (PriceCalculator + PendingOrders) | SUCCESS | 172ms | All logic assertions passed |
| Performance tests (micro-benchmark) | SUCCESS | 34ms | PriceCalc: 20000 loops in 18ms (1077099 ops/s, min 861679), PendingOrders: 20000 loops in 6ms (2968592 ops/s, min 2374874), checksum=6.900.000.000, report=D:\vstudio\BTL\DemoPick\DemoPick\Docs\Perf\PE… |

## Failures

No failures.

