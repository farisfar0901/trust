# Trust Welfare Trust

Full-stack setup:

- Frontend: React-based Next.js app in `trust-ngo`
- Backend: ASP.NET Core Web API in `trust-api`
- Database: PostgreSQL via `docker-compose.yml`

## Run locally

1. Start PostgreSQL:

```bash
docker compose up -d
```

2. Start the backend:

```bash
cd trust-api
dotnet run
```

3. Start the frontend:

```bash
cd trust-ngo
npm run dev
```

## API surface

- `/api/events`
- `/api/gallery`
- `/api/blogs`
- `/api/volunteers`
- `/api/donations`
- `/api/contact-messages`
- `/api/admin/dashboard`

## Database

The API is configured for PostgreSQL with the connection string in `trust-api/appsettings.json`.