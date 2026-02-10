# Stop script immediately on error
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Starting local dev environment..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Navigate to solution directory
$appsPath = "..\APPS"

if (-Not (Test-Path $appsPath)) {
    Write-Error " Failed to change directory to $appsPath"
    exit 1
}

Set-Location $appsPath

# Check if Azure Functions Core Tools is installed
Write-Host "`n Checking Azure Functions Core Tools..." -ForegroundColor Yellow
try {
    $funcVersion = func --version 2>$null
    Write-Host " Azure Functions Core Tools found: $funcVersion" -ForegroundColor Green
} catch {
    Write-Host " WARNING: Azure Functions Core Tools not found!" -ForegroundColor Red
    Write-Host " Please install it using: npm install -g azure-functions-core-tools@4 --unsafe-perm" -ForegroundColor Yellow
}

Write-Host "`n Building solution..." -ForegroundColor Yellow
dotnet build

Write-Host "`n Build succeeded" -ForegroundColor Green

# Run services in parallel with Start-Process
Write-Host "`n Starting backend services..." -ForegroundColor Yellow

# Start AgenticAIService
Write-Host " Starting AgenticAIService on http://localhost:5000" -ForegroundColor Cyan
Start-Process -FilePath "dotnet" -ArgumentList "run --project BackendServices\AgenticAIService\AgenticAIService.csproj" -WindowStyle Normal

# Start WebAPIService
Write-Host " Starting WebAPIService on http://localhost:5001" -ForegroundColor Cyan
Start-Process -FilePath "dotnet" -ArgumentList "run --project BackendServices\WebAPIService\WebApiService.csproj" -WindowStyle Normal

# Start ReportingService (Azure Functions)
Write-Host " Starting ReportingService (Function App) on http://localhost:7071" -ForegroundColor Cyan
try {
    Start-Process -FilePath "func" -ArgumentList "start" -WorkingDirectory "BackendServices\ReportingService" -WindowStyle Normal
} catch {
    Write-Host " WARNING: Could not start ReportingService. Please start it manually:" -ForegroundColor Yellow
    Write-Host " cd .\BackendServices\ReportingService && func start" -ForegroundColor Yellow
}

# Start React App
Write-Host "`n Starting React Web App..." -ForegroundColor Yellow
Set-Location 'ReactAppUI'

if (-Not (Test-Path "node_modules")) {
    Write-Host " Installing npm dependencies..." -ForegroundColor Cyan
    npm install
}
## Print message with all service URLs
Set-Location 'src'
Write-Host " React app starting on http://localhost:3000" -ForegroundColor Cyan
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " All services are running!" -ForegroundColor Green
Write-Host " AgenticAIService: http://localhost:5000" -ForegroundColor Green
Write-Host " WebAPIService: http://localhost:5001" -ForegroundColor Green
Write-Host " ReportingService: http://localhost:7071" -ForegroundColor Green
Write-Host " React App: http://localhost:3000" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

npm start
