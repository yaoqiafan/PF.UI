@echo off
chcp 65001 >nul
pushd "%~dp0"
color 0A

set "NUPKG_DIR=%~dp0nupkg"

echo [1/3] Clean output folder...
if exist "%NUPKG_DIR%" rmdir /s /q "%NUPKG_DIR%"
mkdir "%NUPKG_DIR%"
dotnet clean -c Release >nul

echo.
echo [2/3] Restore...
dotnet restore
if %ERRORLEVEL% NEQ 0 goto :ERROR_EXIT

echo.
echo [3/3] Build and pack (Controls / Resources / Shared only)...
dotnet build -c Release --no-restore
if %ERRORLEVEL% NEQ 0 goto :ERROR_EXIT

dotnet pack "PF.UI.Controls\PF.UI.Controls.csproj" -c Release --no-build -o "%NUPKG_DIR%"
if %ERRORLEVEL% NEQ 0 goto :ERROR_EXIT

dotnet pack "PF.UI.Resources\PF.UI.Resources.csproj" -c Release --no-build -o "%NUPKG_DIR%"
if %ERRORLEVEL% NEQ 0 goto :ERROR_EXIT

dotnet pack "PF.UI.Shared\PF.UI.Shared.csproj" -c Release --no-build -o "%NUPKG_DIR%"
if %ERRORLEVEL% NEQ 0 goto :ERROR_EXIT

if not exist "%NUPKG_DIR%\*.nupkg" goto :ERROR_EXIT
echo [OK] Pack complete. Output: nupkg

color 0B
echo Done! Controls / Resources / Shared packed.
popd
pause
exit /b 0

:ERROR_EXIT
color 0C
echo [ERROR] Build or pack failed.
popd
pause
exit /b 1
