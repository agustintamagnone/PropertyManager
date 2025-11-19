Write-Host "1/3: Cleaning previous coverage data..."
dotnet clean

Write-Host "2/3: Running tests with coverage collection..."
dotnet test --collect:"XPlat Code Coverage"

Write-Host "3/3: Generating HTML coverage report..."
& reportgenerator `
    -reports:PropertyManager.Tests/TestResults/**/coverage.cobertura.xml `
    -targetdir:coverage_html_report `
    -reporttypes:Html
Write-Host "Update complete. Open 'coverage_html_report/index.html' in your explorer to view the report."