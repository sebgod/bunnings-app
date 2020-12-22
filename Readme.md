Example app
===========

## Building

Requirements: NET 5.0 and PowerShell 7.0 or later installed

  - On Windows, run `build.cmd`
  - On MacOS X or Linux, run `pwsh build.ps1`

The build script will ask for an API Key, this is provided via E-Mail (please ask if that is not the case)

## Run locally

Easiest way is to use Visual Studio Code and open workspace.code-workspace.
There is a launch configuration `Launch App & Api` which starts both Api and App.

On succes, this should open up https://localhost:5001. There go to upload and press `Submit`.
This will trigger a login process, which requires setting up a local user account.
An entry should be submitted for further processing, and after a couple of minutes and pressing F5, a picture should appear on the page.

## Run using docker-compose

Use the [following instructions](https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-5.0) to
create a password-based PFX developer certificate:

```
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p { password here }
dotnet dev-certs https --trust
```

Now you can run:

```
ASPNETCORE_Kestrel__Certificates__Default__Password=<password> PlateSolverOptions__ApiKey=<api key> docker-compose up
```
Which now will make the App available at https://localhost:8001