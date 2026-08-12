# Week 1 — Planning & Setup

**Goal:** Requirements finalization, repo setup, architecture design, DB schema.
**Stack:** React (frontend) · C# ASP.NET Core (backend) · PostgreSQL (database)

---

## 1. Requirements Finalization


### Core user flow
1. User creates an account / logs in.
2. User uploads or pastes a resume (PDF, DOCX, or plain text).
3. User uploads or pastes a job posting (text).
4. System analyzes both and returns:
   - Match score (%)
   - Red flags (missing keywords, no metrics, formatting issues)
   - Suggestions
5. Result is saved to the user's scan history.
6. User can view past scans.

### Functional requirements (confirm with your team)
- [ ] Supported input formats: PDF, DOCX, plain text — confirm parsing library for each
- [ ] Match score algorithm: keyword/skill overlap % — define the exact formula (e.g., matched keywords ÷ total required keywords)
- [ ] Red-flag rules — list them explicitly, e.g.:
  - Missing keyword(s) from job posting
  - No quantifiable metrics (numbers, %, $ in bullet points)
  - Inconsistent fonts / broken formatting for ATS
  - Missing standard sections (Experience, Education, Skills)
- [ ] Suggestions — map each red flag to a specific suggestion template
- [ ] Auth: email/password? OAuth? (pick one for MVP)
- [ ] Scan history: how much data stored per scan (score, timestamp, file name, flags)?

### Non-functional requirements
- [ ] Response time target for a scan (e.g., under 5 seconds)
- [ ] Max file size for uploads
- [ ] Data retention / privacy note (resumes are sensitive personal data)

**Action for the team:** go through this list together and check off/adjust each item so everyone agrees before Week 2 backend work starts.

---

## 2. Repo Setup

### Recommended structure (monorepo)
```
resume-match/
├── client/              # React frontend
├── server/               # ASP.NET Core backend
│   ├── ResumeMatch.Api/
│   ├── ResumeMatch.Core/        # business logic (matching engine)
│   └── ResumeMatch.Infrastructure/  # DB access (EF Core + PostgreSQL)
├── docs/                 # architecture notes, ERD, meeting notes
├── .gitignore
└── README.md
```

### Steps
```bash
# 1.
git clone https://github.com/<your-org>/resume-match.git
cd resume-match

# 2. Scaffold backend (ASP.NET Core Web API)
dotnet new webapi -n ResumeMatch.Api -o server/ResumeMatch.Api
dotnet new classlib -n ResumeMatch.Core -o server/ResumeMatch.Core
dotnet new classlib -n ResumeMatch.Infrastructure -o server/ResumeMatch.Infrastructure

# create a solution file to tie them together
dotnet new sln -n ResumeMatch
dotnet sln add server/ResumeMatch.Api server/ResumeMatch.Core server/ResumeMatch.Infrastructure
dotnet add server/ResumeMatch.Api reference server/ResumeMatch.Core
dotnet add server/ResumeMatch.Api reference server/ResumeMatch.Infrastructure
dotnet add server/ResumeMatch.Infrastructure reference server/ResumeMatch.Core

# 3. Scaffold frontend (React + Vite is the fastest setup)
npm create vite@latest client -- --template react
cd client && npm install && cd ..

# 4. Add PostgreSQL EF Core packages to Infrastructure project
cd server/ResumeMatch.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
cd ../..
```

### `.gitignore` essentials
```
# .NET
bin/
obj/
*.user

# Node
node_modules/
dist/
.env

# IDE
.vs/
.vscode/
.idea/
```

### Branching convention (agree on one)
- `main` — always deployable
- `dev` — integration branch
- `feature/<name>` — per feature, PR into `dev`

---

## 3. Architecture Design

### High-level flow
```
[React Client] 
     |  HTTPS/JSON (REST API)
     v
[ASP.NET Core API]  --->  [ResumeMatch.Core: matching engine, red-flag rules]
     |
     v
[EF Core / Npgsql]
     |
     v
[PostgreSQL]
```

### Layers
- **client/** — React UI: upload forms, results dashboard, history view. Talks to the API via `fetch`/`axios`.
- **ResumeMatch.Api** — controllers/endpoints, request validation, auth (JWT recommended), orchestrates calls into Core.
- **ResumeMatch.Core** — pure business logic: text parsing helpers, keyword extraction, scoring algorithm, red-flag detection rules. No DB or web dependencies — keeps it testable.
- **ResumeMatch.Infrastructure** — EF Core `DbContext`, repositories, PostgreSQL access.

### Key API endpoints to plan for
| Method | Route | Purpose |
|---|---|---|
| POST | `/api/auth/register` | create account |
| POST | `/api/auth/login` | authenticate, return JWT |
| POST | `/api/scans` | submit resume + job posting, run analysis |
| GET | `/api/scans` | list current user's scan history |
| GET | `/api/scans/{id}` | get one scan's full detail |

Put this table (or your team's version) into `docs/api-design.md` in the repo.

---

## 4. Database Schema (PostgreSQL)

### Core tables

```sql
CREATE TABLE users (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email         VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE scans (
    id               UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id          UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    resume_filename  VARCHAR(255),
    job_title        VARCHAR(255),
    match_score      NUMERIC(5,2) NOT NULL,       -- e.g. 78.50
    created_at       TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE scan_red_flags (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    scan_id     UUID NOT NULL REFERENCES scans(id) ON DELETE CASCADE,
    flag_type   VARCHAR(100) NOT NULL,   -- e.g. 'missing_keyword', 'no_metrics', 'formatting'
    description TEXT NOT NULL
);

CREATE TABLE scan_suggestions (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    scan_id     UUID NOT NULL REFERENCES scans(id) ON DELETE CASCADE,
    suggestion  TEXT NOT NULL
);
```

### Notes
- Splitting red flags and suggestions into their own tables (rather than JSON blobs) makes it easy to query/filter/report later, and keeps things in 3NF.
- Add indexes once query patterns are clearer, e.g. `CREATE INDEX idx_scans_user_id ON scans(user_id);`
- Sketch this as an ERD (draw.io, dbdiagram.io, or even a hand-drawn photo) and drop it in `docs/` — a visual is worth having for the proposal/demo too.

### Wiring it up with EF Core (Infrastructure project)
```bash
cd server/ResumeMatch.Infrastructure
dotnet ef migrations add InitialCreate
dotnet ef database update
```
(Requires an `AppDbContext` with `DbSet<User>`, `DbSet<Scan>`, etc., and a PostgreSQL connection string in `appsettings.json` — e.g. `Host=localhost;Database=resumematch;Username=postgres;Password=...`)

---

- [ ] Backend solution + 3 projects scaffolded and building (`dotnet build`)
- [ ] Frontend scaffolded and running (`npm run dev`)
- [ ] Architecture diagram in `docs/`
- [ ] DB schema created + first migration applied
- [ ] Everyone can pull the repo and run both client and server locally
