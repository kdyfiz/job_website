# JobScout

Find jobs. Match your skills. No account required.

JobScout is a free job discovery and CV-matching website. Search by title and location, filter results, optionally upload a PDF CV, and see an estimated skill-match score. There is no registration, no login, and no payment.

This repository is a portfolio full-stack application: React + TypeScript on the frontend, ASP.NET Core on the backend.

## Features

- Search jobs by title, location, experience, and work arrangement
- Job result cards and a job details page
- Optional PDF CV upload with temporary in-memory processing
- Deterministic estimated match scores, matching skills, and potential gaps
- About and Privacy pages
- Swagger / OpenAPI at `/swagger`
- No user accounts

## Screenshots

Run the app locally and capture:

1. Home search
2. Results list (including the **Demo data** badge)
3. Job details
4. CV match results

Do not present demo listings as live jobs.

## Tech stack

| Layer | Technology |
| --- | --- |
| Frontend | React, TypeScript, Vite, Tailwind CSS |
| Backend | C#, ASP.NET Core, .NET 10 LTS |
| PDF text extraction | Docnet.Core (MIT, PDFium) |
| Tests | xUnit, ASP.NET Core WebApplicationFactory |
| CI | GitHub Actions (free runners) |

## Architecture

```
                 USER
                   │
                   ▼
          React + TypeScript
                   │
                   │ REST API
                   ▼
          ASP.NET Core Web API
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
 Job Search Service       CV Service
        │                     │
        ▼                     ▼
 Demo Job Provider       PDF text extraction
        │                     │
        └──────────┬──────────┘
                   ▼
             Match Engine
                   │
                   ▼
              Job Results
```

There is **no database** in V1. Firebase is not used.

## How it works

1. The browser calls the ASP.NET Core API.
2. `IJobSearchProvider` returns jobs. V1 uses `DemoJobSearchProvider`.
3. Results are normalized to a shared `Job` model.
4. The UI shows cards and details. Apply on the original source — or, for demo rows, read the demo-data note.

## CV matching

1. Upload a PDF (max 5 MB).
2. The API validates type and size, extracts text, and detects skills from `skills.json`.
3. Jobs are scored with configurable weights (default: skills 60%, experience 20%, location 10%, keywords 10%).
4. The file is discarded when the request ends. It is not stored.

Image-only / scanned PDFs cannot be read. The API will say so instead of inventing skills.

PDF text extraction uses **Docnet.Core** (MIT, PDFium). The historical PdfPig NuGet listing currently only exposes prerelease/custom versions, so it was not used.

## Job search

V1 uses **demo data**, labelled in the UI. Live Malaysia job APIs that are free, keyless, and safe to use were not assumed. The provider interface allows a later swap without rewriting the API.

Do not scrape sites that require login, CAPTCHA, or forbid automated access.

## API

| Method | Path | Purpose |
| --- | --- | --- |
| `GET` | `/api/health` | Health check |
| `GET` | `/api/jobs/search` | Search jobs |
| `GET` | `/api/jobs/{id}` | Job details |
| `POST` | `/api/cv/analyze` | Extract skills from a PDF |
| `POST` | `/api/jobs/match` | Analyze a CV and score jobs |

Swagger UI: `http://localhost:5080/swagger`

Example:

```
GET /api/jobs/search?query=software%20developer&location=Malaysia
```

## Running locally

Prerequisites: .NET 10 SDK, Node.js 20+.

Terminal 1 — API:

```bash
export PATH="$HOME/.dotnet:$PATH"   # if the SDK was installed with the official script
cd backend
dotnet run --project JobScout.API
```

API: http://localhost:5080

Terminal 2 — frontend:

```bash
cd frontend
npm install
npm run dev
```

App: http://localhost:5173

In local Vite dev, `/api` is proxied to the API. You do not need `VITE_API_BASE_URL` unless the frontend and API are on different hosts.

## Environment variables

See `.env.example` and `frontend/.env.example`.

| Variable | Where | Purpose |
| --- | --- | --- |
| `VITE_API_BASE_URL` | Frontend build | API origin in production. Leave unset in Vite dev to use the proxy. |
| `Cors__AllowedOrigins__0` | Backend | Allowed browser origin. Default: `http://localhost:5173` |

Never commit secrets. V1 has no API keys.

## Testing

```bash
cd backend
dotnet test
```

Coverage includes matching, CV validation/extraction, search, and API integration tests.

## Deployment

**Do not use a paid host.** Re-check current free-tier terms before deploying.

Suggested RM0 path:

- Frontend: Cloudflare Pages (Vite, output `dist`, SPA redirects). No credit card for the free plan.
- Backend: keep local until you explicitly approve a public host. Render Free can run ASP.NET Core without a card today, but it sleeps after 15 minutes idle, cold-starts ~1 minute, and extra bandwidth can suspend the service if no payment method exists. Render’s free Postgres expires in 30 days — JobScout does not use it.

This repository is **not** deployed as part of V1 scaffolding.

## Docker

Docker is for **local** runs, not paid hosting.

```bash
docker compose up --build
```

- App: http://localhost:8088
- API: http://localhost:5080
- Swagger: http://localhost:5080/swagger

## Privacy

- No accounts or passwords
- CV files are processed in memory for the request only
- Match scores are estimates
- Job availability must be verified on the original listing
- Demo jobs are not live vacancies

## Limitations

- **Job data:** V1 is demo data. Availability depends on the original source when a live provider is added.
- **Match score:** An estimate from detected CV text and listing fields. Not a hiring guarantee.
- **CV extraction:** Fails for image-only or scanned PDFs.
- **External sources:** JobScout does not control third-party listings.
- **Hosting:** Free backend hosts can change terms, sleep, or suspend services.

## Future improvements

Not in V1:

- A live free job provider (only if still free and ToS-safe)
- Saved jobs, alerts, accounts
- Optional AI (would need an explicit cost decision first)
- Firebase Firestore if persistence is actually required

## License

Use this project as a portfolio piece. Demo job companies and descriptions are fictional.
