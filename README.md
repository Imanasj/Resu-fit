# Resu-fit

Resu-Fit is a resume and job-match scanner. Paste your resume and a job
posting, and it returns a match score, the keywords your resume matched or is
missing, and a few quick quality checks on the resume itself.

## What it does

- Compares a resume against a job posting and produces a percentage match score
- Shows which keywords from the job posting were found in the resume, and which are missing
- Runs simple resume checks (word count, missing Experience/Education/Skills sections)
- Lets you copy the missing keywords to your clipboard
- Lets you download a scan result as a `.txt` report
- Keeps a local scan history you can revisit, stored in the browser

## Tech stack

| Layer | Technology |
|---|---|
| Frontend | React (Vite) |
| Backend | C# / ASP.NET Core (minimal API) |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Hosting | GitHub Pages (frontend only — backend runs locally) |

## Project structure

```
Resu-fit/
├── client/                     React frontend (Vite)
│   ├── src/
│   │   ├── App.jsx             Main app component
│   │   ├── App.css             Styles
│   │   └── ScoreCircle.jsx     Score circle component
│   └── index.html
├── server/
│   ├── ResumeMatch.Api/        ASP.NET Core API
│   ├── ResumeMatch.Core/       Core domain logic
│   └── ResumeMatch.Infrastructure/   EF Core + PostgreSQL
├── docs/                       Planning docs, schema, architecture notes
└── README.md
```

## Running it locally

You need two things running at once: the backend API and the frontend dev server.

### 1. Backend

```
cd server/ResumeMatch.Api
dotnet run
```

This starts the API at `http://localhost:5151`. It needs a local PostgreSQL
database running, with your own connection details configured in a local,
git-ignored `appsettings.Local.json` file inside `server/ResumeMatch.Api/`.
Ask a teammate for the expected database name and schema if you don't have
it set up yet — do not commit real credentials anywhere in this repo.

Endpoints:
- `GET /api/health` — confirms the API is running
- `GET /api/db-health` — confirms the database connection is working
- `POST /api/scan` — runs a resume-vs-job-posting scan

### 2. Frontend

```
cd client
npm install
npm run dev
```

This starts the dev server at `http://localhost:5173`, with a proxy that
forwards `/api/*` requests to the backend on port 5151 (configured in
`client/vite.config.js`).

## Deploying the frontend

The live site (`https://imanasj.github.io/Resu-fit/`) is a static build of
the frontend only — there's no live backend, so the "Analyze Resume" button
won't work there. It's just for showing the UI.

To push a new build:

```
cd client
npm run deploy
```

This runs `vite build` and pushes the output to the `gh-pages` branch.

## Database

See `docs/database/README.md` for the full database design: schema, ERD,
views, functions, and example queries.

## Team

- Iman — frontend UI, styling, project setup
- Saranya — backend API, matching logic, PostgreSQL integration
- Jun — frontend scaffolding, scan history

## Notes

- Never commit real database credentials to this repo. Local-only config
  files are excluded via `.gitignore` — keep it that way.
