# MoneyTrace — Business Requirements

## 1. Overview

MoneyTrace is a personal finance tracking web application for a **single authenticated user**. The user can record and categorise all money movements across four financial transaction types, manage supporting reference data (banks, counterparties, currencies), and view a consolidated dashboard of their financial activity.

---

## 2. User Management

| # | Requirement |
|---|---|
| U-01 | A user can self-register with a unique username, full name, and password. |
| U-02 | A user can log in with their username and password and receive a session token. |
| U-03 | Closing the browser ends the session. The user must log in again on the next visit. |
| U-04 | If a session token expires during active use, any failing API call (401) should silently attempt to resume the session and retry. If resumption fails, the user is redirected to the login page. |
| U-05 | After a failed session, when the user logs in again, they are returned to the page they were on before being redirected. |

---

## 3. Reference Data Management

### 3.1 Counterparties

A counterparty is a person or entity the user transacts money with (e.g., a family member, a friend).

| # | Requirement |
|---|---|
| C-01 | User can create a counterparty with a name. |
| C-02 | User can view a paginated list of all their counterparties. |
| C-03 | User can edit a counterparty's name. |
| C-04 | User can delete (soft-delete) a counterparty. |

### 3.2 Banks

A bank represents a financial institution where the user holds accounts.

| # | Requirement |
|---|---|
| B-01 | User can create a bank with a name and optional SWIFT code. |
| B-02 | User can view a paginated list of all their banks. |
| B-03 | User can edit a bank's details. |
| B-04 | User can delete (soft-delete) a bank. |

### 3.3 Currencies

Currencies are used to denominate all financial transactions.

| # | Requirement |
|---|---|
| CY-01 | User can create a currency with a code (e.g. USD), optional name (e.g. US Dollar), optional symbol (e.g. $), and an exchange rate relative to the user's default currency. |
| CY-02 | User can view a paginated list of all their currencies. |
| CY-03 | User can edit a currency's details, including updating its exchange rate. |
| CY-04 | User can delete (soft-delete) a currency. |

---

## 4. Transaction Types

### 4.1 Cashflow

A cashflow represents a standard income or expense event in the user's day-to-day life (e.g., receiving salary, paying rent, buying groceries).

| # | Requirement |
|---|---|
| CF-01 | User can record a cashflow with: date, amount, currency, direction (income or expense), optional description, and optional comma-separated tags. |
| CF-02 | User can view a list of all their cashflows. |

### 4.2 Bank Transaction

A bank transaction records a deposit or withdrawal made to a bank account, with optional interest details.

| # | Requirement |
|---|---|
| BT-01 | User can record a bank transaction with: date, bank, account number, amount, currency, optional interest percentage (annual), optional interest period (Daily/Monthly/Yearly), and optional description and tags. |
| BT-02 | The system stores both the expected interest amount and the actual interest amount received. |
| BT-03 | User can view a list of all their bank transactions. |

### 4.3 Currency Exchange

A currency exchange records converting a sum from one currency to another.

| # | Requirement |
|---|---|
| EX-01 | User can record a currency exchange with: date, source amount, source currency, target amount, target currency, exchange rate, and optional description and tags. |
| EX-02 | User can view a list of all their exchange transactions. |
| EX-03 | In the dashboard, an exchange is treated as income if the target amount (in the user's default currency equivalent) exceeds the source amount. |

### 4.4 Transfer

A transfer records money moving between the user and a counterparty — lending, borrowing, repaying, or collecting.

| # | Requirement |
|---|---|
| TR-01 | User can record a transfer with: date, counterparty, amount, currency, transfer type, initial status (Pending), and optional description and tags. |
| TR-02 | Transfer types are: **Lend** (user gives money to counterparty), **Borrow** (user receives money from counterparty), **Repay** (user returns borrowed money), **Collect** (user receives back lent money). |
| TR-03 | Transfer statuses are: Pending, Completed, Cancelled. |
| TR-04 | In the dashboard, Borrow and Collect are treated as income; Lend and Repay are treated as expense. |
| TR-05 | User can view a list of all their transfers. |

---

## 5. Dashboard

| # | Requirement |
|---|---|
| D-01 | The dashboard displays a consolidated summary of all transaction types for the current user. |
| D-02 | User can filter the dashboard by a date range (start date / end date). |
| D-03 | The dashboard shows: Total Income, Total Expense, and Net Balance for the selected period. |
| D-04 | The dashboard shows a Recent Transactions table listing all transaction types with: date, type label, amount (coloured green for income, red for expense), description, and tags. |
| D-05 | Transactions are ordered by date descending. |

---

## 6. Transaction List Page

| # | Requirement |
|---|---|
| TL-01 | User can view a full list of all their transactions (all types merged). |
| TL-02 | User can filter by date range. |
| TL-03 | User can filter by transaction type (Cashflow, Bank, Exchange, Transfer). |
| TL-04 | User can sort by date (ascending/descending) and by amount. |
| TL-05 | User can open the transaction creation modal from this page. |

---

## 7. Settings

| # | Requirement |
|---|---|
| S-01 | User can set a default currency. This currency is used as the baseline for exchange rate calculations. |
| S-02 | User can change the display language between English and Vietnamese. The language change takes effect immediately without page reload. |
| S-03 | User can select a theme (Light or Dark). Dark mode is a planned future feature. |
| S-04 | Settings are persisted per user in the database. |

---

## 8. Non-Functional Requirements

| # | Requirement |
|---|---|
| NF-01 | **Security**: Passwords are stored as BCrypt hashes. JWTs are signed with HS256. |
| NF-02 | **Data integrity**: No hard deletes. All entities use soft-delete (`IsActive` flag). |
| NF-03 | **ID generation**: All entity IDs use time-sortable UUIDs (Guid Version 7). |
| NF-04 | **Timezone**: All dates are stored and transmitted in UTC. Conversion to local time is handled on the frontend. |
| NF-05 | **Database portability**: Application is built with EF Core in a database-agnostic way. SQLite is used for local development; PostgreSQL is the target for production. |
| NF-06 | **Scalability**: Monolith architecture for the current scope. Structure follows Clean Architecture to facilitate future service extraction if needed. |
| NF-07 | **Error handling**: All API errors are returned as structured JSON. 400 errors surface validation messages. 500 errors are caught by global middleware. The frontend displays global notifications for 400/500 errors. |
| NF-08 | **Responsiveness**: The UI is usable on mobile, tablet, and desktop screen widths. The sidebar collapses on small screens. |
| NF-09 | **Containerisation**: The application can be run locally with a single `docker-compose up` command. |

---

## 9. Out of Scope (Current Version)

- Multi-user support (each deployment is for a single user)
- Push / email notifications
- Recurring transactions / scheduled entries
- Budget planning / spending limits
- Charts and visualisations (planned next)
- CSV / Excel export (planned next)
- Refresh token rotation (sliding sessions — planned next)
- Dark mode (UI placeholder only)
