@echo off
setlocal
set EXE=%~dp0..\go\bin\fuel-latest.exe

:: Required environment variables
if "%EIA_API_KEY%"=="" (
    echo [ERROR] EIA_API_KEY environment variable is not set.
    echo Please set it in Windows Environment Variables and try again.
    pause
    exit /b 1
)

if "%FUEL_DSN%"=="" (
    echo [ERROR] FUEL_DSN environment variable is not set.
    echo Please set it in Windows Environment Variables and try again.
    pause
    exit /b 1
)

:: Optional defaults
if "%FUEL_OUT%"=="" set FUEL_OUT=%USERPROFILE%\Desktop\fuel-latest.csv
if "%FUEL_AREA%"=="" set FUEL_AREA=NUS

"%EXE%"
pause