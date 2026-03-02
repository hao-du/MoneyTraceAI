$ErrorActionPreference = "Stop"

Write-Host "Setting up backend..."
cd d:\Hao\Code\MoneyTraceAI
if (!(Test-Path backend)) { mkdir backend }
cd backend

if (!(Test-Path MoneyTrace.sln)) { dotnet new sln -n MoneyTrace }

if (!(Test-Path MoneyTrace.Api)) { dotnet new webapi -n MoneyTrace.Api -f net9.0 }
if (!(Test-Path MoneyTrace.Application)) { dotnet new classlib -n MoneyTrace.Application -f net9.0 }
if (!(Test-Path MoneyTrace.Domain)) { dotnet new classlib -n MoneyTrace.Domain -f net9.0 }
if (!(Test-Path MoneyTrace.Infrastructure)) { dotnet new classlib -n MoneyTrace.Infrastructure -f net9.0 }

dotnet sln add MoneyTrace.Api/MoneyTrace.Api.csproj
dotnet sln add MoneyTrace.Application/MoneyTrace.Application.csproj
dotnet sln add MoneyTrace.Domain/MoneyTrace.Domain.csproj
dotnet sln add MoneyTrace.Infrastructure/MoneyTrace.Infrastructure.csproj

cd MoneyTrace.Api
dotnet add reference ../MoneyTrace.Application/MoneyTrace.Application.csproj ../MoneyTrace.Infrastructure/MoneyTrace.Infrastructure.csproj

cd ../MoneyTrace.Infrastructure
dotnet add reference ../MoneyTrace.Application/MoneyTrace.Application.csproj

cd ../MoneyTrace.Application
dotnet add reference ../MoneyTrace.Domain/MoneyTrace.Domain.csproj

cd ../..
Write-Host "Backend setup complete."

Write-Host "Setting up frontend..."
# Bypass npm strict config errors by setting config inline or using npx with no-install if possible, 
# or use npm config set temporarily. 
# `npx --yes create-vite frontend --template react-ts`
npx --yes create-vite@latest frontend --template react-ts

Write-Host "Frontend setup complete."
