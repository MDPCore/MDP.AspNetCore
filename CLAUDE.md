# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

MDP.AspNetCore is a set of .NET 8 NuGet libraries that layer configuration-driven authentication/authorization modules on top of ASP.NET Core. It builds on the `MDP.NetCore` package (separate repo, referenced as a NuGet `PackageReference` in [src/MDP.AspNetCore/MDP.AspNetCore.csproj](src/MDP.AspNetCore/MDP.AspNetCore.csproj)), which supplies the config-driven DI/registration framework (`MDP.Registration`, `MDP.Configuration`, `MDP.Hosting`). README, docs and commit messages are written in Traditional Chinese.

## Commands

```
dotnet build src/MDP.AspNetCore.sln                 # build all libraries + Lab apps
dotnet run --project src/MDP.AspNetCore.Authentication.Line.Lab   # run a module's demo site
dotnet build samples/MDP.AspNetCore.Samples.sln     # samples (OAuthSSO client/server)
```

- Unit tests: `dotnet test src/MDP.AspNetCore.Authorization.Tests` (xUnit; currently the only test project). Release script excludes projects ending in `.Lab` / `.Tests`.
- Each library `X` has a sibling `X.Lab` web app (e.g. `MDP.AspNetCore.Authentication.Google.Lab`) that is the manual verification harness; its `appsettings.json` mounts the module under test.
- Release: [build/NuGet-Delivery.bat](build/NuGet-Delivery.bat) packs every `*.csproj` under `src/` except those ending in `.Lab`, using the hardcoded `version=` variable, and pushes to nuget.org. All packages share one version number; bump it in that file when releasing.

## Architecture

**Host bootstrap.** Every app's `Program.Main` just calls `MDP.AspNetCore.Host.Run(args)`, which runs `WebApplicationBuilder.ConfigureMDP()` and `WebApplication.ConfigureMDP()` ([src/MDP.AspNetCore/Hosting/](src/MDP.AspNetCore/Hosting/)). `ConfigureMDP` loads config files via MDP's `FileConfigurationProvider`, then calls `ServiceFactoryRegister.RegisterModule` / `ContainerRegister.RegisterModule` from MDP.NetCore, which discover factories and services by reflection across all application assemblies. `MvcRegister` likewise adds every application assembly as an MVC ApplicationPart and exposes embedded `wwwroot` resources, so controllers/views/static files inside a library (Razor SDK projects) appear automatically in the host app.

**Config-driven module mounting.** A module is activated purely by the presence of its config section. Each module ships a `Hosting/*Factory.cs` deriving from `ServiceFactory<WebApplicationBuilder, TSetting>` with a constructor like `base("Authentication", "Line", false)` — i.e. it binds `Authentication:Line` from appsettings into `Setting` and calls `ConfigureService`. Adding a new provider module means: a factory with this pattern, an `AuthenticationHandler`/`Options`/`Defaults`/`Extensions` set (see `MDP.AspNetCore.Authentication.Line` as the template), and a matching `.Lab` project. Application services are registered with `[MDP.Registration.Service<TInterface>(singleton: true)]` plus a config entry under their namespace/class name.

**Authentication flow** ([src/MDP.AspNetCore.Authentication/](src/MDP.AspNetCore.Authentication/)). `AuthenticationFactory` (bound to the root `Authentication` section) wires several cookie/virtual schemes:
- **Policy** scheme — default authenticate/challenge scheme; forwards each request to the first registered `PolicyAuthenticationSelector` whose `Check(context)` matches (token modules register these, e.g. for a header), falling back to `DefaultScheme`.
- **Remote** scheme — cookie holding the identity returned by an external OAuth provider before it's mapped to a local user. OAuth modules call `options.ConfigureSignIn()` so their result signs into Remote instead of directly into the app.
- **Local** scheme — the app's real login cookie.

Controller extensions in `Extensions/ControllerExtensions.cs` (`LoginAsync`, `LinkAsync`, `RefreshAsync`, `LogoutAsync`, `SignInAsync`) drive the flow. Apps customize the Remote→Local mapping by registering a subclass of `AuthenticationProvider` overriding `RemoteExchange` (return null → redirect to `Authentication:RegisterPath` or access denied) and `RemoteLink` (bind an extra OAuth identity to a logged-in user).

**Other projects.**
- `Authentication.Tokens` — JWT / API-token schemes from `Authentication:Jwt:Credentials[]` (HS/RS algorithms, configurable header/prefix).
- `Authentication.AzureAD.Users` / `.Services` — Azure AD for users vs. service-to-service.
- `Authentication.OAuthSSO` / `.OAuthSSO.Server` — client and server for the project's own OAuth SSO provider (samples in `samples/OAuthSSOSamples`).
- `Authorization` — role/resource-based authorization (`RoleAssignmentProvider`, `AccessPermissionProvider`, `AccessResourceProvider`, `RoleAuthorizationHandler`); domain model in `docs/MDP.AspNetCore.Authorization-領域模型.drawio`.
- `DataProtection.MSSql` — `IKeyRepository` implementation so DataProtection keys are shared across hosts; `ConfigureMDP` picks up any registered `IKeyRepository`.

`docs/` is a Jekyll (just-the-docs) GitHub Pages site published at https://mdpcore.github.io/MDP.AspNetCore/.

## Conventions

- Projects use `ImplicitUsings` and `Nullable` **disabled**; follow the existing style: `#region Contracts` blocks with `ArgumentNullException($"{nameof(x)}=null")`, `// Section` comments above each step, explicit `== true/false` comparisons.
- Commit message format: `[src] <Project>: <description>` for code changes, `[nuget] Release MDP.AspNetCore <version>` for releases.
