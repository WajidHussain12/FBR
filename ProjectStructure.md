# FBR Digital Invoicing System — Project Structure

## Solution Overview

**Solution File:** `Fbr_Digital_Invoicing.slnx`
**Framework:** .NET 8.0
**Architecture:** Clean Architecture (Domain → Application → Infrastructure/Persistence → Presentation)
**UI Framework:** Tailwind CSS v3.4 + ASP.NET Core MVC
**Pattern:** CQRS (Command Query Responsibility Segregation) — folders ready

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                  FBR_DI.Web  (Presentation)             │
│           ASP.NET Core MVC · Tailwind CSS               │
└───────────────────────┬─────────────────────────────────┘
                        │ references
         ┌──────────────┼──────────────┐
         ▼                             ▼
┌─────────────────┐         ┌──────────────────────┐
│ FBR_DI.         │         │ FBR_DI.              │
│ Application     │         │ Infrastructure       │
│ (Use Cases,     │         │ (External Services,  │
│  CQRS, DTOs)    │         │  Repositories)       │
└────────┬────────┘         └──────────┬───────────┘
         │ references                  │ references
         ▼                             ▼
┌─────────────────────────────────────────────────────────┐
│                    FBR_DI.Domain                        │
│           (Entities · Enums · Business Rules)           │
└─────────────────────────────────────────────────────────┘
         ▲
         │ references
┌─────────────────┐
│ FBR_DI.         │
│ Persistence     │
│ (DbContext,     │
│  Migrations,    │
│  Identity)      │
└─────────────────┘
```

---

## Complete Folder & File Structure

```
Fbr_Digital_Invoicing_2/                         ← Solution Root
│
├── Fbr_Digital_Invoicing.slnx                   ← Solution file (.NET new format)
├── ProjectStructure.md                          ← This file
├── ProjectStructure.txt                         ← Auto-generated tree output (legacy)
│
├── FBR_DI.Domain/                               ← [Layer 1] Domain Layer
├── FBR_DI.Application/                          ← [Layer 2] Application Layer
├── FBR_DI.Infrastructure/                       ← [Layer 3] Infrastructure Layer
├── FBR_DI.Persistence/                          ← [Layer 3] Persistence Layer
└── FBR_DI.Web/                                  ← [Layer 4] Presentation Layer
```

---

## Project 1 — FBR_DI.Domain

> **Role:** Core business entities, enums, and domain rules. No dependencies on other layers.

```
FBR_DI.Domain/
│
├── FBR_DI.Domain.csproj                         ← Project file (net8.0, no external dependencies)
│
├── Common/                                      ← [EMPTY] Base classes for domain entities
│   └── (e.g. BaseEntity.cs, AuditableEntity.cs)
│
├── Entities/                                    ← [EMPTY] Core domain/business objects
│   └── (e.g. Invoice.cs, Taxpayer.cs, Item.cs)
│
└── Enums/                                       ← [EMPTY] Business-level enumerations
    └── (e.g. InvoiceStatus.cs, InvoiceType.cs)
```

**Dependencies:** None (pure domain — no external packages)

---

## Project 2 — FBR_DI.Application

> **Role:** Application use cases, CQRS commands/queries, DTOs, interfaces, validators, and mappings. Orchestrates domain logic.

```
FBR_DI.Application/
│
├── FBR_DI.Application.csproj                   ← Project file
│   ├── Package: Microsoft.Extensions.DependencyInjection 8.0.1
│   └── Reference: FBR_DI.Domain
│
├── ApplicationServiceExtensions.cs             ← DI registration entry point
│   └── AddApplication(IServiceCollection)      ← Called from Program.cs (currently empty stub)
│
├── Behaviors/                                  ← [EMPTY] MediatR pipeline behaviors
│   └── (e.g. LoggingBehavior.cs, ValidationBehavior.cs)
│
├── DTOs/                                       ← [EMPTY] Data Transfer Objects
│   └── (e.g. InvoiceDto.cs, CreateInvoiceDto.cs)
│
├── Enums/                                      ← [EMPTY] Application-level enumerations
│   └── (e.g. SortDirection.cs)
│
├── Exceptions/                                 ← [EMPTY] Custom application exceptions
│   └── (e.g. NotFoundException.cs, ValidationException.cs)
│
├── Features/
│   └── Admin/                                  ← CQRS feature folder for Admin module
│       ├── Commands/                           ← [EMPTY] Command definitions (write operations)
│       │   └── (e.g. CreateInvoiceCommand.cs)
│       ├── Handlers/
│       │   ├── CommandHandlers/                ← [EMPTY] Command handler implementations
│       │   │   └── (e.g. CreateInvoiceCommandHandler.cs)
│       │   └── QueryHandlers/                  ← [EMPTY] Query handler implementations
│       │       └── (e.g. GetInvoiceListQueryHandler.cs)
│       └── Queries/                            ← [EMPTY] Query definitions (read operations)
│           └── (e.g. GetInvoiceListQuery.cs)
│
├── Interfaces/                                 ← [EMPTY] Repository & service contracts
│   └── (e.g. IInvoiceRepository.cs, IEmailService.cs)
│
├── Mappings/                                   ← [EMPTY] AutoMapper profiles
│   └── (e.g. InvoiceMappingProfile.cs)
│
├── ResultPattern/                              ← [EMPTY] Standardized result wrappers
│   └── (e.g. Result.cs, Result<T>.cs)
│
├── Validators/                                 ← [EMPTY] FluentValidation validators
│   └── (e.g. CreateInvoiceValidator.cs)
│
└── Wrappers/                                   ← [EMPTY] API response wrapper classes
    └── (e.g. PagedResponse.cs, ApiResponse.cs)
```

**Dependencies:**
- `Microsoft.Extensions.DependencyInjection` 8.0.1
- `FBR_DI.Domain` (project reference)

---

## Project 3 — FBR_DI.Infrastructure

> **Role:** Concrete implementations of application interfaces — external services (email, SMS), repository implementations, and seed data.

```
FBR_DI.Infrastructure/
│
├── FBR_DI.Infrastructure.csproj                ← Project file
│   ├── References: FBR_DI.Application, FBR_DI.Domain
│   └── (No external packages yet)
│
├── InfrastructureServiceExtensions.cs          ← DI registration entry point
│   └── AddInfrastructure(IServiceCollection)   ← Called from Program.cs (currently empty stub)
│
├── Repositories/                               ← [EMPTY] Repository implementations
│   └── (e.g. InvoiceRepository.cs implementing IInvoiceRepository)
│
├── SeedData/                                   ← [EMPTY] Static/initial data seeding
│   └── (e.g. InvoiceSeeder.cs)
│
└── Services/                                   ← [EMPTY] External service implementations
    └── (e.g. EmailService.cs, SmsService.cs)
```

**Dependencies:**
- `FBR_DI.Application` (project reference)
- `FBR_DI.Domain` (project reference)

---

## Project 4 — FBR_DI.Persistence

> **Role:** Database context (EF Core), entity configurations, migrations, identity models, and seed logic.

```
FBR_DI.Persistence/
│
├── FBR_DI.Persistence.csproj                   ← Project file
│   ├── Package: Microsoft.Extensions.Configuration 8.0.0
│   └── References: FBR_DI.Application, FBR_DI.Domain
│
├── PersistenceServiceExtensions.cs             ← DI registration entry point
│   └── AddPersistence(IServiceCollection,      ← Called from Program.cs (currently empty stub)
│         IConfiguration)
│
├── Configurations/                             ← [EMPTY] EF Core entity type configurations
│   └── (e.g. InvoiceConfiguration.cs : IEntityTypeConfiguration<Invoice>)
│
├── Context/                                    ← [EMPTY] DbContext definition
│   └── (e.g. AppDbContext.cs)
│
├── EmailTemplates/                             ← [EMPTY] HTML email template files
│   └── (e.g. WelcomeEmail.html)
│
├── HelperClasses/                              ← [EMPTY] Persistence utility/helper classes
│   └── (e.g. QueryExtensions.cs)
│
├── IdentityModels/                             ← [EMPTY] ASP.NET Core Identity models
│   └── (e.g. ApplicationUser.cs, ApplicationRole.cs)
│
├── Migrations/                                 ← [EMPTY] EF Core migration files
│   └── (auto-generated by dotnet ef migrations add)
│
└── Seed/                                       ← [EMPTY] Database seed logic
    └── (e.g. DatabaseSeeder.cs)
```

**Dependencies:**
- `Microsoft.Extensions.Configuration` 8.0.0
- `FBR_DI.Application` (project reference)
- `FBR_DI.Domain` (project reference)

---

## Project 5 — FBR_DI.Web

> **Role:** ASP.NET Core MVC presentation layer. Handles HTTP requests, renders views, serves static assets. Uses Tailwind CSS for styling.

```
FBR_DI.Web/
│
├── FBR_DI.Web.csproj                           ← Project file (net8.0 Web SDK)
│   ├── References: FBR_DI.Application, FBR_DI.Infrastructure
│   ├── MSBuild Target: NpmInstall              ← Runs npm install if node_modules missing
│   └── MSBuild Target: BuildTailwind           ← Auto-generates tailwind.css on every build
│
├── FBR_DI.Web.csproj.user                      ← User-specific project settings (not committed)
│
├── Program.cs                                  ← Application entry point & middleware pipeline
│   ├── builder.Services.AddControllersWithViews()
│   ├── app.UseExceptionHandler("/Home/Error")   (production only)
│   ├── app.UseHsts()                            (production only)
│   ├── app.UseHttpsRedirection()
│   ├── app.UseStaticFiles()
│   ├── app.UseRouting()
│   ├── app.UseAuthorization()
│   └── Default Route: {controller=Home}/{action=Index}/{id?}
│
├── appsettings.json                            ← Production app configuration
│   ├── Logging: Information level (default), Warning (Microsoft.AspNetCore)
│   └── AllowedHosts: "*"
│
├── appsettings.Development.json                ← Development overrides
│   └── Logging: Debug level (default), Warning (Microsoft.AspNetCore)
│
├── package.json                                ← npm config for Tailwind CSS
│   ├── build:css → npx tailwindcss ... --minify
│   └── watch:css → npx tailwindcss ... --watch
│
├── tailwind.config.js                          ← Tailwind configuration
│   └── content: ["./Views/**/*.cshtml",        ← Auto-scans ALL views (current + future)
│                  "./Areas/**/*.cshtml"]        ← Auto-scans ALL area views (current + future)
│
│
├── Properties/
│   └── launchSettings.json                     ← Dev server launch profiles
│       ├── Profile: http  → http://localhost:5024
│       ├── Profile: https → https://localhost:7107
│       └── Profile: IIS Express → port 44348
│
│
├── Controllers/
│   └── HomeController.cs                       ← Default MVC controller
│       ├── GET Index()    → Views/Home/Index.cshtml
│       ├── GET Privacy()  → Views/Home/Privacy.cshtml
│       └── GET Error()    → Views/Shared/Error.cshtml
│                             [ResponseCache: no-store]
│
│
├── Models/
│   └── ErrorViewModel.cs                       ← Model for error view
│       ├── string? RequestId
│       └── bool ShowRequestId                  ← true if RequestId is not null/empty
│
│
├── Views/
│   │
│   ├── _ViewStart.cshtml                       ← Sets default layout = "_Layout" for all views
│   ├── _ViewImports.cshtml                     ← Global view imports
│   │   ├── @using FBR_DI.Web
│   │   ├── @using FBR_DI.Web.Models
│   │   └── @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
│   │
│   ├── Home/
│   │   ├── Index.cshtml                        ← Home page (Welcome heading)
│   │   │   └── Tailwind classes: text-center, text-5xl, font-bold
│   │   └── Privacy.cshtml                      ← Privacy policy placeholder page
│   │
│   └── Shared/                                 ← Shared layout & partial views
│       ├── _Layout.cshtml                      ← MASTER LAYOUT (used by all views)
│       │   ├── <head>  Tailwind CSS, site.css, FBR_DI.Web.styles.css
│       │   ├── <nav>   Responsive navbar with mobile hamburger toggle (pure JS)
│       │   ├── <main>  @RenderBody() inside max-w-7xl container
│       │   ├── <footer> Copyright 2026, Privacy link
│       │   └── <scripts> jQuery, site.js, @RenderSectionAsync("Scripts")
│       │
│       ├── _Layout.cshtml.css                  ← Scoped CSS for layout (compiled to FBR_DI.Web.styles.css)
│       │   ├── a.navbar-brand → white-space, word-break
│       │   ├── a → color #0077cc
│       │   └── button.accept-policy → font-size
│       │
│       ├── _ValidationScriptsPartial.cshtml    ← jQuery validation scripts partial
│       │   ├── jquery.validate.min.js
│       │   └── jquery.validate.unobtrusive.min.js
│       │
│       └── Error.cshtml                        ← Error page
│           ├── Heading: text-red-600 (Tailwind)
│           └── Shows RequestId in Development mode
│
│
├── Areas/
│   └── Admin/                                  ← Admin Area (scaffolded, awaiting implementation)
│       ├── Controllers/                        ← [EMPTY] Admin controllers go here
│       ├── Data/                               ← [EMPTY] Admin-specific data access
│       ├── Models/                             ← [EMPTY] Admin view models
│       └── Views/                              ← [EMPTY] Admin views go here
│           └── (Add _ViewStart.cshtml here to set area layout when needed)
│
│
└── wwwroot/                                    ← Static files root (served publicly)
    │
    ├── favicon.ico                             ← Browser tab icon
    │
    ├── css/
    │   ├── tailwind-input.css                  ← Tailwind CSS source (input)
    │   │   ├── @tailwind base
    │   │   ├── @tailwind components
    │   │   └── @tailwind utilities
    │   ├── tailwind.css                        ← Compiled & minified Tailwind CSS (~6.9 KB)
    │   │                                          AUTO-GENERATED — do not edit manually
    │   └── site.css                            ← Custom global CSS overrides
    │       ├── html font-size: 14px (16px on ≥768px)
    │       ├── html position: relative, min-height: 100%
    │       └── body margin-bottom: 60px (footer spacing)
    │
    ├── js/
    │   └── site.js                             ← Custom JavaScript placeholder
    │
    └── lib/                                    ← Third-party frontend libraries
        ├── jquery/
        │   └── dist/
        │       ├── jquery.js                   ← jQuery full source
        │       ├── jquery.min.js               ← jQuery minified (used in _Layout)
        │       └── jquery.min.map              ← Source map
        │
        ├── jquery-validation/
        │   ├── LICENSE.md
        │   └── dist/
        │       ├── jquery.validate.js          ← Validation plugin full source
        │       ├── jquery.validate.min.js      ← Validation plugin minified
        │       ├── additional-methods.js       ← Extended validation rules
        │       └── additional-methods.min.js   ← Extended rules minified
        │
        └── jquery-validation-unobtrusive/
            ├── LICENSE.txt
            ├── jquery.validate.unobtrusive.js      ← ASP.NET unobtrusive adapter
            └── jquery.validate.unobtrusive.min.js  ← Minified (used in _ValidationScriptsPartial)
```

---

## Dependency Graph (Project References)

```
FBR_DI.Domain
    ↑ (referenced by)
    ├── FBR_DI.Application
    │       ↑
    │       ├── FBR_DI.Infrastructure
    │       │       ↑
    │       │       └── FBR_DI.Web
    │       │
    │       ├── FBR_DI.Persistence
    │       │
    │       └── FBR_DI.Web
    │
    └── FBR_DI.Persistence
```

---

## NuGet Package Summary

| Project | Package | Version |
|---|---|---|
| FBR_DI.Application | Microsoft.Extensions.DependencyInjection | 8.0.1 |
| FBR_DI.Persistence | Microsoft.Extensions.Configuration | 8.0.0 |
| FBR_DI.Web | (none — uses Web SDK built-ins) | — |
| FBR_DI.Domain | (none) | — |
| FBR_DI.Infrastructure | (none) | — |

---

## npm / Frontend Package Summary

| Package | Version | Purpose |
|---|---|---|
| tailwindcss | ^3.4.0 | Utility-first CSS framework |

**npm Scripts:**

| Script | Command | When to use |
|---|---|---|
| `npm run build:css` | `npx tailwindcss -i ... -o ... --minify` | One-time production build |
| `npm run watch:css` | `npx tailwindcss -i ... -o ... --watch` | During active UI development |

> **Note:** `dotnet build` automatically runs Tailwind CSS generation via MSBuild targets — manual `npm run build:css` is only needed for isolated CSS work.

---

## How Tailwind CSS Auto-Applies to ALL Views

### Current Views (automatic)
All views use `_ViewStart.cshtml` which sets `Layout = "_Layout"`. Since `_Layout.cshtml` links `tailwind.css`, every view automatically gets Tailwind styles — no per-file configuration needed.

### Future Views in Root Views/ (automatic)
Any new `.cshtml` file added under `Views/` will inherit `_Layout.cshtml` via `_ViewStart.cshtml`. Tailwind CSS applies automatically.

### Future Area Views (automatic)
`tailwind.config.js` content path `./Areas/**/*.cshtml` ensures that Tailwind CSS classes used in ANY Area view are included in the compiled `tailwind.css` on next build.

To add Tailwind-aware views to a new Area:
1. Create `Areas/{AreaName}/Views/_ViewStart.cshtml` pointing to `_Layout` (or a custom area layout that includes `tailwind.css`)
2. Add views — Tailwind classes will work automatically at next build

---

## Implementation Status

| Layer | Status | Notes |
|---|---|---|
| FBR_DI.Domain | Scaffolded — empty | Entities, Enums, Common folders ready |
| FBR_DI.Application | Scaffolded — empty | CQRS folders, DI stub ready |
| FBR_DI.Infrastructure | Scaffolded — empty | Repositories, Services, SeedData ready |
| FBR_DI.Persistence | Scaffolded — empty | DbContext, Migrations, Identity ready |
| FBR_DI.Web | Base setup done | MVC + Tailwind CSS configured and running |
| Admin Area | Scaffolded — empty | Awaiting controllers + views |

---

## Development Quick Reference

```bash
# Run the web application
dotnet run --project FBR_DI.Web

# Watch Tailwind CSS changes during UI development
cd FBR_DI.Web && npm run watch:css

# Build entire solution
dotnet build

# Add EF Core migration (when Persistence is configured)
dotnet ef migrations add InitialCreate --project FBR_DI.Persistence --startup-project FBR_DI.Web

# Update database
dotnet ef database update --project FBR_DI.Persistence --startup-project FBR_DI.Web
```

---

*Last updated: April 2026*
