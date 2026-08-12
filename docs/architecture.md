# Architecture Design — Resu Fit

**Stack:** React (frontend) · C# ASP.NET Core (backend) · PostgreSQL (database)

## High-Level Flow

```
[React Client]
     |  HTTPS/JSON (REST API)
     v
[ASP.NET Core API]  --->  [Core: matching engine, red-flag rules]
     |
     v
[EF Core / Npgsql]
     |
     v
[PostgreSQL]
```

## Layers

- **client/** — React UI: upload forms, results dashboard, history view. Talks to the API via `fetch`/`axios`.
- **server/ResumeMatch.Api** — controllers/endpoints, request validation, auth (JWT recommended), orchestrates calls into Core.
- **server/ResumeMatch.Core** — pure business logic: text parsing helpers, keyword extraction, scoring algorithm, red-flag detection rules. No DB or web dependencies — keeps it testable.
- **server/ResumeMatch.Infrastructure** — EF Core `DbContext`, repositories, PostgreSQL access.

## Key API Endpoints

| Method | Route | Purpose |
|---|---|---|
| POST | `/api/auth/register` | create account |
| POST | `/api/auth/login` | authenticate, return JWT |
| POST | `/api/scans` | submit resume + job posting, run analysis |
| GET | `/api/scans` | list current user's scan history |
| GET | `/api/scans/{id}` | get one scan's full detail |
