# MoneyTrace 💰

A personal finance tracking web app for recording and visualising daily money movements — cash flow, bank transactions, currency exchanges, and counterparty transfers.

---

## Features

- **Cashflow** — income and expense entries
- **Bank Transactions** — deposits/withdrawals with interest tracking
- **Currency Exchange** — convert between currencies at a specified rate
- **Transfers** — lend/borrow/repay/collect with named counterparties
- **Dashboard** — unified transaction view with income/expense/net summaries and date filtering
- **Settings** — default currency, language (EN/VI), theme

---

## Tech Stack

| | |
|---|---|
| **Backend** | .NET 9 · ASP.NET Core Minimal API · EF Core · SQLite · JWT |
| **Frontend** | React · Vite · TypeScript · Ant Design · TailwindCSS · Tanstack Query |
| **Tests** | xUnit + NSubstitute + FluentAssertions · Vitest + jsdom |

---

## Prerequisites

| Tool | Version |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 9.0+ |
| [Node.js](https://nodejs.org/) | 20+ |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | Optional (for Docker run) |

---

## Running Locally (Without Docker)

### 1. Clone the Repository

```bash
git clone https://github.com/hao-du/MoneyTraceAI.git
cd MoneyTraceAI
```

### 2. Start the Backend

```bash
cd backend/MoneyTrace.Api

# Apply database migrations (first time only)
dotnet ef database update --project ../MoneyTrace.Infrastructure

# Start the API
dotnet run
```

> API will be available at **http://localhost:5044**

### 3. Start the Frontend

Open a **new terminal**:

```bash
cd frontend

# Install dependencies (first time only)
npm install

# Start the dev server
npm run dev
```

> App will be available at **http://localhost:5173**

### 4. Open the App

Navigate to **http://localhost:5173** in your browser.

- Click **Register** to create an account
- Log in and start adding transactions!

---

## Running with Docker

```bash
# From the repo root
docker-compose up --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000 |

---

## Running Tests

### Backend Tests (25 tests)

```bash
cd backend
dotnet test MoneyTrace.Tests

# With detailed output
dotnet test MoneyTrace.Tests --logger "console;verbosity=normal"
```

### Frontend Tests (21 tests)

```bash
cd frontend
npm test                  # run once
npm run test:watch        # watch mode
npm run test:coverage     # with coverage report
```

---

## Project Structure

```
MoneyTraceAI/
├── backend/
│   ├── MoneyTrace.Domain/        # Entities, Result<T>, Enums
│   ├── MoneyTrace.Application/   # CQRS handlers, DTOs, validators
│   ├── MoneyTrace.Infrastructure/ # EF Core, repositories, migrations
│   ├── MoneyTrace.Api/           # Minimal API endpoints, JWT, middleware
│   └── MoneyTrace.Tests/         # xUnit unit tests
├── frontend/
│   └── src/
│       ├── api/                  # Axios instance + interceptors
│       ├── components/           # Atomic Design components + layouts
│       ├── pages/                # Login, Dashboard, Transactions, CRUD pages
│       ├── router/               # React Router + ProtectedRoute
│       ├── i18n/                 # English & Vietnamese translations
│       └── test/                 # Vitest tests
├── docker-compose.yml
├── REQUIREMENTS.md               # Business requirements
├── DEVELOPMENT.md                # Architecture & design decisions
└── PROMPT.md                     # AI scaffold prompt (re-generate the app)
```

---

## Configuration

### Backend — `backend/MoneyTrace.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=moneytrace.db"
  },
  "Jwt": {
    "Secret": "your-secret-key-min-256-bits",
    "Issuer": "MoneyTraceApi",
    "Audience": "MoneyTraceClient"
  }
}
```

### Frontend — `frontend/.env` (optional override)

```
VITE_API_URL=http://localhost:5044/api
```

---

## Documentation

| File | Contents |
|---|---|
| [REQUIREMENTS.md](./REQUIREMENTS.md) | Full business requirements with numbered IDs |
| [DEVELOPMENT.md](./DEVELOPMENT.md) | Architecture, API endpoints, design decisions |
| [PROMPT.md](./PROMPT.md) | AI prompt to scaffold the entire app from scratch |
