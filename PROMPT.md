# MoneyTrace — AI Scaffold Prompt

> Use this prompt with a coding AI assistant (e.g. Antigravity / Gemini / Copilot) to scaffold or re-generate the MoneyTrace application from scratch.

---

## Prompt

```
App Name: MoneyTrace

Description:
MoneyTrace is a personal finance web application that allows a single authenticated user to record and track all their daily money movements. It supports four types of financial transactions:
1. Cashflow — standard income or expense entry (e.g. salary, grocery purchase)
2. Bank Transaction — depositing or withdrawing money from a bank account, supporting interest tracking
3. Currency Exchange — converting an amount from one currency to another at a specified exchange rate
4. Transfer — lending or borrowing money to/from a named counterparty (family, friends), with status tracking (Lend, Borrow, Repay, Collect)

---

Tech Stack:

Backend:
- Language: C# / .NET 9
- Architecture: Clean Architecture (Monolith) — Domain, Application, Infrastructure, Api layers
- Web framework: ASP.NET Core with Minimal API only (no Controllers)
- Authentication: JWT Bearer tokens. Token stored on the frontend in localStorage. Stateless — no sessions.
- ORM: EF Core. Database: SQLite for local dev. Design queries to be PostgreSQL-compatible for future migration.
- CQRS: MediatR (IRequest<Result<T>>, IRequestHandler)
- Validation: FluentValidation
- Error handling: Result<T> pattern in Domain and Application, global ExceptionHandlingMiddleware in API
- IDs: All entity IDs use Guid.CreateVersion7() (time-sortable)
- Datetimes: All stored and returned in UTC
- Soft delete: All entities have an IsActive bool (no hard deletes)
- Password hashing: BCrypt.Net-Next
- Containerisation: Dockerfile for backend, docker-compose.yml at repo root

Frontend:
- Scaffold: React + Vite + TypeScript
- UI library: Ant Design (antd) for components (Tables, Forms, Modals, DatePickers)
- Styling: TailwindCSS for layout/utilities. Configure a custom theme token (primary = indigo-600)
- HTTP: Axios. Create a single axios instance with:
    - Request interceptor: attach Authorization: Bearer <token> from localStorage
    - Response interceptor: on 401 → clear token, save current path to localStorage.redirectUrl, dispatch 'auth-unauthorized' event. On 400/500 → show global antd notification.error with the error detail.
- Server state: @tanstack/react-query (QueryClientProvider wrapping the app)
- Routing: react-router-dom v6. All routes under "/" are protected by a ProtectedRoute component that checks localStorage for a token and redirects to /login if absent. On successful login, redirect to localStorage.redirectUrl or /dashboard.
- i18n: react-i18next. Support English (en) and Vietnamese (vi). Language can be changed in Settings.
- Component structure: Atomic Design — atoms, molecules, organisms, templates, pages
- Global config: App.tsx wraps the app in QueryClientProvider + Ant Design ConfigProvider (custom theme) + RouterProvider

---

Backend Entities (Domain layer):

User:        Id, Username, PasswordHash, FullName, Description?, IsActive
Counterparty: Id, UserId, Name, IsActive
Bank:         Id, UserId, Name, SwiftCode?, IsActive
Currency:     Id, UserId, Code (e.g. "USD"), Name?, Symbol?, RateToDefault (decimal), IsActive
UserSettings: Id, UserId, DefaultCurrencyId?, Language (default "en"), Theme (default "light")

Cashflow:     Id, UserId, DateUtc, Amount, CurrencyId, IsIncome (bool), Description?, Tags?, IsActive
BankTransaction: Id, UserId, BankId, AccountNumber, DateUtc, Amount, CurrencyId, InterestPercentage, InterestPeriod (enum: Daily/Monthly/Yearly), InterestAmount, ActualInterestAmount, Description?, Tags?, IsActive
ExchangeTransaction: Id, UserId, DateUtc, SourceAmount, SourceCurrencyId, TargetAmount, TargetCurrencyId, ExchangeRate, Description?, Tags?, IsActive
TransferTransaction: Id, UserId, CounterpartyId, DateUtc, Amount, CurrencyId, TransferType (enum: Lend/Borrow/Repay/Collect), Status (enum: Pending/Completed/Cancelled), Description?, Tags?, IsActive

---

Backend: Application Layer

For each entity, implement CQRS:
- Commands: Create (and optionally Update/Delete) → return Result<Guid> or Result<T>
- Queries: GetAll for the authenticated user → return Result<List<T>>
- All handlers inject IRepository<T> and IUnitOfWork
- Use FluentValidation for command validation (register validators via DI)

Implement a unified dashboard query:
- GetDashboardTransactionsQuery(UserId, StartDateUtc?, EndDateUtc?) → Result<List<TransactionDto>>
- TransactionDto: { Id, DateUtc, Amount, CurrencyId, Description?, Tags?, Type (string: "Cashflow"/"Bank"/"Exchange"/"Transfer"), IsIncome (bool) }
- Fetches all four transaction types in parallel, merges, sorts by DateUtc descending
- IsIncome: Cashflow uses IsIncome field; Bank always false; Exchange = TargetAmount > SourceAmount; Transfer = Borrow or Collect

---

Backend: Infrastructure Layer

- MoneyTraceDbContext with DbSet for all entities
- EF Core entity configurations: explicit column names, value conversions for enums, shadow FK properties
- Generic Repository<T> implementing IRepository<T> with Add, GetByIdAsync, GetAllAsync (filtered by UserId + IsActive), Remove methods
- UnitOfWork wrapping DbContext.SaveChangesAsync
- JwtTokenService: generate JWT with claims (sub = userId, name = username), signed with HS256, configurable expiry from appsettings

---

Backend: API Layer

Minimal API endpoint groups (IEndpointRouteBuilder extensions):
- /api/auth — POST /register (CreateUserCommand), POST /login (LoginUserQuery → returns { token })
- /api/counterparties — GET, POST, PUT/{id}, DELETE/{id} — all require auth, filter by userId from ClaimsPrincipal
- /api/banks — same pattern as counterparties
- /api/currencies — same pattern
- /api/settings — GET and POST (upsert) for user settings
- /api/transactions — POST /cashflow, GET /cashflows, POST /bank, GET /banks, POST /exchange, GET /exchanges, POST /transfer, GET /transfers
- /api/dashboard/transactions — GET with optional ?startDateUtc=&endDateUtc= query params

All authenticated endpoints extract userId from HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier).

---

Frontend: Pages

Login (/login):
- Two tabs: Login (Username + Password) and Register (Username + Full Name + Password)
- On register success, auto-login and redirect to /dashboard
- On login success, redirect to localStorage.redirectUrl or /dashboard

Dashboard (/dashboard):
- Date range picker to filter transactions
- Summary cards: Total Income, Total Expense, Net Balance
- Recent Transactions table with columns: Date, Type (tag), Amount (green +/red -), Description, Tags

Transactions (/transactions):
- Same data as dashboard but full table with sort (date, amount) + filter by Type
- "New Transaction" button opens TransactionModal

TransactionModal (organism):
- Radio button group to select type: Cashflow / Bank / Exchange / Transfer
- Common fields: Date (DatePicker with time), Currency
- Dynamic fields per type (see entity fields above)
- Common trailing fields: Description, Tags (comma separated)
- On success: invalidate dashboard_transactions and transactions_list query caches

Counterparties, Banks, Currencies (/counterparties, /banks, /currencies):
- Antd Table with pagination (15/page)
- Add button → Modal with Form for create
- Edit (pencil icon) and Delete (trash icon, with confirm dialog) per row
- Use useMutation + queryClient.invalidateQueries for optimistic UI

Settings (/settings):
- Default Currency (select from currencies list)
- Language (English / Vietnamese) — change applies instantly via i18n.changeLanguage()
- Theme (Light / Dark placeholder)
- Save via POST /api/settings

---

Routing:
/login         → Login page (public)
/dashboard     → Dashboard (protected)
/transactions  → Transactions list (protected)
/counterparties → Counterparties CRUD (protected)
/banks         → Banks CRUD (protected)
/currencies    → Currencies CRUD (protected)
/settings      → Settings (protected)

MainLayout (protected pages):
- Ant Design Layout with a collapsible Sider (sidebar)
- Sidebar menu items: Dashboard, Transactions, Counterparties, Banks, Currencies, Settings
- Logout button at the bottom of the sidebar (clears token, redirects to /login)
- Header shows current page title

---

Docker:
- backend/Dockerfile: multi-stage build (dotnet restore → publish → runtime image)
- frontend/Dockerfile: build Vite app → serve with nginx
- docker-compose.yml:
    - backend on port 5000:8080 with SQLite volume mount
    - frontend on port 3000:80, depends_on backend
```
