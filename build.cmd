@setlocal enabledelayedexpansion
@pushd %~dp0
@echo Requires NET 5.0 and PowerShell 7.0 or later installed (dotnet tool update --global PowerShell)
pwsh build.ps1
popd