# MoneyTrace — Development Documentation

> Personal finance tracking application supporting cash flow, bank transactions, counterparty transfers, and currency exchange.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 9, ASP.NET Core Minimal API |
| Architecture | Clean Architecture (Monolith) |
| ORM | EF Core + SQLite (PostgreSQL-ready) |
| Auth | JWT Bearer Tokens |
| CQRS | MediatR |
| Validation | FluentValidation |
| Frontend | React + Vite + TypeScript |
| UI | Ant Design + TailwindCSS |
| HTTP Client | Axios |
| Server State | Tanstack Query |
| i18n | react-i18next (EN + VI) |
| Routing | React Router v6 |
| Dev Environment | Docker Compose |

---

## Project Structure

```
MoneyTraceAI/
├── backend/
│   ├── MoneyTrace.Domain/          # Entities, Enums, Result<T>, Interfaces
│   ├── MoneyTrace.Application/     # CQRS Commands/Queries, DTOs, Validators
│   ├── MoneyTrace.Infrastructure/  # EF Core DbContext, Repositories, Migrations
│   └── MoneyTrace.Api/             # Minimal API Endpoints, JWT, Middleware
├── frontend/
│   ├── src/
│   │   ├── api/         # Axios instance + interceptors
│   │   ├── components/
│   │   │   ├── atoms/
│   │   │   ├── molecules/
│   │   │   ├── organisms/   # TransactionModal
│   │   │   ├── templates/
│   │   │   └── layouts/     # MainLayout (sidebar + header)
│   │   ├── hooks/
│   │   ├── i18n/        # i18next configuration (EN + VI)
│   │   ├── pages/       # Login, Dashboard, Transactions, Banks, Currencies, Counterparties, Settings
│   │   ├── router/      # React Router + ProtectedRoute
│   │   └── utils/
│   └── tailwind.config.js
├── docker-compose.yml
├── .gitignore
└── DEVELOPMENT.md
```

---

## Backend Architecture

### Clean Architecture Layers

**Domain** (`MoneyTrace.Domain`)
- `Entity` base class with `Guid` ID (using `Guid.CreateVersion7()`)
- `Result<T>` and `Error` primitives for railway-oriented error handling
- Entities: `User`, `Counterparty`, `Bank`, `Currency`, `UserSettings`, `Cashflow`, `BankTransaction`, `ExchangeTransaction`, `TransferTransaction`
- All datetime fields are UTC. Soft delete via `IsActive` boolean.

**Application** (`MoneyTrace.Application`)
- CQRS via MediatR (`IRequest<Result<T>>`)
- Validators via FluentValidation
- Interfaces: `IRepository<T>`, `IUnitOfWork`, `ITokenService`
- DTOs: `TransactionDto` (unified view across all transaction types)

**Infrastructure** (`MoneyTrace.Infrastructure`)
- `MoneyTraceDbContext` (EF Core)
- SQLite by default, connection string is fully swappable to PostgreSQL
- Generic `Repository<T>` + `UnitOfWork` implementations
- EF Core Migrations in `Migrations/`
- `JwtTokenService` for token generation

**API** (`MoneyTrace.Api`)
- Minimal API endpoints grouped by feature
- JWT configured via `appsettings.json` `Jwt` section
- `ExceptionHandlingMiddleware` — catches unhandled exceptions → 500 JSON response
- CORS: `AllowAll` policy (tighten for production)

### API Endpoints

| Method | Path | Description |
|---|---|---|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login → returns JWT token |
| GET/POST/PUT/DELETE | `/api/counterparties` | Counterparty CRUD |
| GET/POST/PUT/DELETE | `/api/banks` | Bank CRUD |
| GET/POST/PUT/DELETE | `/api/currencies` | Currency CRUD |
| GET/POST | `/api/settings` | User settings |
| POST | `/api/transactions/cashflow` | Create cashflow transaction |
| GET | `/api/transactions/cashflows` | Get user cashflows |
| POST | `/api/transactions/bank` | Create bank transaction |
| GET | `/api/transactions/banks` | Get user bank transactions |
| POST | `/api/transactions/exchange` | Create exchange transaction |
| GET | `/api/transactions/exchanges` | Get user exchange transactions |
| POST | `/api/transactions/transfer` | Create transfer transaction |
| GET | `/api/transactions/transfers` | Get user transfer transactions |
| GET | `/api/dashboard/transactions` | Unified transaction list with optional date filter |

---

## Frontend Architecture

### Authentication Flow
- JWT stored in `localStorage` under key `token`
- Axios request interceptor attaches `Authorization: Bearer <token>` on every request
- On **401**: token is removed, current path is saved to `localStorage.redirectUrl`, and user is redirected to `/login`
- On successful login: app reads `redirectUrl` and navigates back to the original destination
- On **400/500**: global Antd `notification.error` is shown with error details

### Routing
- `ProtectedRoute` wrapper checks for token in localStorage
- Unauthenticated users are redirected to `/login`
- Routes: `/dashboard`, `/transactions`, `/counterparties`, `/banks`, `/currencies`, `/settings`

### Pages
| Page | Description |
|---|---|
| `Login.tsx` | Login + Register tabs. Auto-login after registration. |
| `Dashboard.tsx` | Income/Expense/Net stats cards + Recent Transactions table with date range filter |
| `Transactions.tsx` | Full transaction list with sort/filter + "New Transaction" modal |
| `Counterparties.tsx` | CRUD table for counterparties |
| `Banks.tsx` | CRUD table for banks with SWIFT code |
| `Currencies.tsx` | CRUD table for currencies with exchange rate |
| `Settings.tsx` | Default currency, language, theme preferences |

### Transaction Modal (`TransactionModal.tsx`)
A unified entry form that dynamically changes fields based on transaction type:
- **Cashflow**: Amount, Currency, Income/Expense direction
- **Bank**: Amount, Currency, Bank, Account Number, Interest %
- **Exchange**: Source amount/currency, Target amount/currency, Exchange Rate
- **Transfer**: Amount, Currency, Counterparty, Transfer Type (Lend/Borrow/Repay/Collect)

---

## Running Locally

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)

### Backend
```bash
cd backend/MoneyTrace.Api
dotnet ef database update --project ../MoneyTrace.Infrastructure
dotnet run
# API available at http://localhost:5044
```

### Frontend
```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:5173
```

### Docker (both services)
```bash
docker-compose up --build
# Frontend: http://localhost:3000
# Backend:  http://localhost:5000
```

---

## Configuration

### Backend (`appsettings.json`)
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

### Frontend (`.env`)
```
VITE_API_URL=http://localhost:5044/api
```

---

## Key Design Decisions

1. **Guid V7 for IDs** — time-sortable UUIDs for natural ordering without a separate `CreatedAt` sort in most queries.
2. **Result Pattern** — all domain factory methods and handlers return `Result<T>` instead of throwing, enabling railway-oriented error propagation.
3. **SQLite → PostgreSQL** — all EF Core queries avoid SQLite-specific functions (no raw SQL, LIKE over Contains, etc.). Switching requires only changing the connection string and EF Core provider package.
4. **Soft Delete** — entities have `IsActive` field; no hard deletes are performed.
5. **CQRS** — commands mutate state, queries read state. Handlers live in the Application layer, decoupled from API.
6. **Atomic Design** — frontend components organized into atoms → molecules → organisms → templates → pages to encourage reuse and separation.

---

## Next Steps / Roadmap

- [ ] Refresh token support (sliding sessions without re-login)
- [ ] Backend unit tests (xUnit)
- [ ] Frontend unit tests (Vitest + React Testing Library)
- [ ] Playwright E2E tests (login, create transaction, filter)
- [ ] PostgreSQL migration for production
- [ ] Charts on Dashboard (income vs expense over time)
- [ ] Export transactions to CSV/Excel
