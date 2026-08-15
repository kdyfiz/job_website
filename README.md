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
 Himalayas + demo fallback  PDF text extraction
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
2. `IJobSearchProvider` returns jobs. Live search uses the free Himalayas JSON API; demo jobs are the fallback.
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

Search uses the **Himalayas** public jobs API (`https://himalayas.app/jobs/api/search`). No API key and no paid plan. Those listings are mostly **remote / worldwide**, not JobStreet-style on-site Malaysia ads. If Himalayas is down or returns nothing, the API falls back to labelled demo jobs.

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

**Do not use a paid host.** Re-check current free-tier terms before deploying. No credit card is required for this path.

### 1. API — Render Free

1. Sign up at [dashboard.render.com/register](https://dashboard.render.com/register) with GitHub. Skip any paid plan.
2. **New + → Blueprint**. Connect `kdyfiz/job_website` and apply `render.yaml`.
   Or **New + → Web Service**, connect the same repo, then:
   - Language: **Docker**
   - Dockerfile path: `backend/Dockerfile`
   - Instance type: **Free**
   - Health check path: `/api/health`
3. Do **not** create a Render Postgres database.
4. Copy the API URL, e.g. `https://jobscout-api.onrender.com`.

Render Free sleeps after 15 minutes idle. The next request can take about a minute. Extra bandwidth can suspend the service if no payment method exists.

### 2. Frontend — Cloudflare Pages

1. Sign up at [dash.cloudflare.com](https://dash.cloudflare.com). No credit card for the free plan.
2. **Workers & Pages → Create → Pages → Import an existing Git repository**.
3. Select `kdyfiz/job_website` and use:
   - Root directory: `frontend`
   - Build command: `npm run build`
   - Build output directory: `dist`
   - Environment variable: `VITE_API_BASE_URL` = your Render API URL (no trailing slash)
4. Deploy. The site URL looks like `https://jobscout.pages.dev`.

`frontend/public/_redirects` keeps React Router paths working. CORS already allows `*.pages.dev`.

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

- **Job data:** Live results come from Himalayas remote listings. If that API fails, labelled demo jobs are shown instead. Availability must be checked on the original posting.
- **Match score:** An estimate from detected CV text and listing fields. Not a hiring guarantee.
- **CV extraction:** Fails for image-only or scanned PDFs.
- **External sources:** JobScout does not control third-party listings.
- **Hosting:** Free backend hosts can change terms, sleep, or suspend services.

## Future improvements

Not in V1:

- Saved jobs, alerts, accounts
- Optional AI (would need an explicit cost decision first)
- Firebase Firestore if persistence is actually required

## License

Use this project as a portfolio piece. Demo job companies and descriptions are fictional.
