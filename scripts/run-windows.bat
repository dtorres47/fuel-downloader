@echo off
REM ───────────────────────────────────────────────────────────
REM 1) Jump up to the project root (one level above scripts/)
pushd "%~dp0\.."

REM ───────────────────────────────────────────────────────────
REM 2) Load KEY=VALUE lines from config.env into environment vars
for /f "usebackq tokens=1,* delims==" %%A in ("config.env") do (
    set "%%A=%%B"
)

REM ───────────────────────────────────────────────────────────
REM 3) Run your Go app (in the /go folder)
go run .\go

REM ───────────────────────────────────────────────────────────
REM 4) Return to where we started (cleanup) and pause so you can see any output
popd
pause