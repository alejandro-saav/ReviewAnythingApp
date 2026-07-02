# ReviewAnythingApp

A full-stack web application that lets users review anything. Built with an **ASP.NET Core Web API** backend, fully containerized with **Docker Compose** for easy local development and deployment.

🌐 **Live site:** [reviewanything.site](https://reviewanything.site)

---

> **Note on the frontend:** The Blazor frontend (`BlazorApp1/`) in this repo is the original implementation and is no longer actively maintained. The production frontend has been migrated to a separate React Router + TypeScript app:
> 👉 [ReviewAnythingFrontEnd](https://github.com/alejandro-saav/ReviewAnythingFrontEnd)

---

## Tech Stack

| Layer     | Technology                        |
|-----------|----------------------------------|
| Frontend  | React Router + TypeScript + Vite *(active)* |
| Frontend (legacy) | Blazor Server (ASP.NET Core) |
| Backend   | ASP.NET Core Web API (C#)        |
| Database  | PostgreSQL *(configurable)*      |
| Container | Docker & Docker Compose          |
| Hosting   | Netlify (frontend) |

---
## Features
- Add reviews
- Explore reviews
- Filter reviews by date, rating, tags, and category.
- Email Authentication
- Email Confirmation
- Password Reset
- Google OAuth 2.0
- Add comments to reviews.
- Up and down vote comments and reviews.
- Check user profiles.
- Update user profile data.
---

## Getting Started

### Prerequisites

- [Docker](https://www.docker.com/get-started) and Docker Compose
- [.NET SDK](https://dotnet.microsoft.com/download) (for local development without Docker)

### Run with Docker Compose

1. **Clone the repository:**
   ```bash
   git clone https://github.com/alejandro-saav/ReviewAnythingApp.git
   cd ReviewAnythingApp
   ```

2. **Set up environment files:**

   Create `ReviewAnythingAPI/config/development.env` with your API configuration:
   ```env
   ASPNETCORE_ENVIRONMENT=Development
   # Add any connection strings or secrets here
   ```

   Create `BlazorApp1/config/development.env` with your frontend configuration:
   ```env
   ASPNETCORE_ENVIRONMENT=Development
   ```

3. **Build and run all services:**
   ```bash
   docker compose up --build
   ```

4. **Access the app:**
   - Blazor frontend: [http://localhost:5002](http://localhost:5002)
   - API: [http://localhost:5000](http://localhost:5000)

### Run Locally (without Docker)

1. Open `ReviewAnythingApp.sln` in Visual Studio or Rider.
2. Set both `BlazorApp1` and `ReviewAnythingAPI` as startup projects.
3. Run the solution.

---

## Docker Services

The `docker-compose.yml` defines two main services:

- **`api`** — The ASP.NET Core Web API, running with `dotnet watch run` for hot reload during development. Exposed on port `5000`.
- **`blazor`** — The Blazor Server frontend. Depends on the API service and communicates with it via the internal Docker network (`http://api`). Exposed on port `5002`.

---

## Development Notes

- Both services use volume mounts so code changes are reflected immediately without rebuilding the image.
- NuGet packages are cached via a shared `~/.nuget` volume to speed up builds.
- The Blazor app talks to the API using the internal Docker hostname `api`, configured via the `Api__BaseUrl` environment variable.

---

## License

This project is open source. See the repository for details.
