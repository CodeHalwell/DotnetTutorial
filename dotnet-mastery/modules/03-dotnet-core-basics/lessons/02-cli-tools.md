# Module 03: .NET Core Basics - CLI Tools

## 📘 Mastering the .NET Command Line Interface

The .NET CLI is your primary tool for creating, building, testing, and publishing .NET applications. Mastering these commands is essential for productive development.

## 🎯 Learning Objectives

- Master all essential dotnet commands
- Create and manage projects and solutions
- Work with NuGet packages efficiently
- Understand build configurations
- Use advanced CLI features
- Automate tasks with CLI

## 🔧 Core Commands

### dotnet --version / --info

```bash
# Show SDK version
dotnet --version
# Output: 10.0.100

# Show detailed information
dotnet --info
# Output:
# .NET SDK:
#  Version:   10.0.100
#  Commit:    abc123def
#
# Runtime Environment:
#  OS Name:     Windows
#  OS Version:  11
#  OS Platform: Windows
#  RID:         win-x64
```

### dotnet new

**List available templates:**
```bash
dotnet new list

# Output (partial):
# Template Name            Short Name      Language    Tags
# ---------------------------------------------------------
# Console App              console         [C#],F#,VB  Common/Console
# Class Library            classlib        [C#],F#,VB  Common/Library
# ASP.NET Core Web API     webapi          [C#],F#     Web/WebAPI
# xUnit Test Project       xunit           [C#],F#,VB  Test/xUnit
# Worker Service           worker          [C#],F#     Common/Worker
# Blazor WebAssembly App   blazorwasm      [C#]        Web/Blazor/WebAssembly
```

**Create projects:**
```bash
# Console application
dotnet new console -n MyConsoleApp

# With framework specification
dotnet new console -n MyApp -f net10.0

# Class library
dotnet new classlib -n MyLibrary

# Web API
dotnet new webapi -n MyApi

# xUnit test project
dotnet new xunit -n MyTests

# Create in current directory
dotnet new console

# Use different language
dotnet new console -n MyFSharpApp -lang F#

# Dry run (show what would be created)
dotnet new console -n MyApp --dry-run
```

**Advanced options:**
```bash
# Console with specific options
dotnet new console -n MyApp \
  --framework net10.0 \
  --langVersion latest \
  --use-program-main  # Use traditional Main method

# Web API with authentication
dotnet new webapi -n MyApi \
  --auth IndividualB2C

# Disable nullable reference types
dotnet new console -n MyApp --nullable disable
```

### dotnet restore

**Purpose:** Downloads NuGet packages defined in project file

```bash
# Restore current project
dotnet restore

# Restore specific project
dotnet restore MyProject.csproj

# Restore entire solution
dotnet restore MySolution.sln

# Force re-download of packages
dotnet restore --force

# Restore from specific package source
dotnet restore --source https://api.nuget.org/v3/index.json

# Restore with verbose logging
dotnet restore --verbosity detailed
```

**When restore happens automatically:**
- `dotnet build` (if needed)
- `dotnet run` (if needed)
- `dotnet test` (if needed)

**When to run manually:**
- After cloning a repository
- After modifying .csproj file
- After switching branches
- When packages are corrupted

### dotnet build

**Purpose:** Compiles project to IL assemblies

```bash
# Build current project (Debug configuration)
dotnet build

# Build with Release configuration
dotnet build -c Release
# or
dotnet build --configuration Release

# Build specific project
dotnet build MyProject.csproj

# Build solution
dotnet build MySolution.sln

# Build without restore
dotnet build --no-restore

# Build with specific framework
dotnet build -f net10.0

# Output to specific directory
dotnet build -o ./build-output

# Continuous build (watch for changes)
dotnet watch build
```

**Build verbosity levels:**
```bash
dotnet build --verbosity quiet      # Minimal output
dotnet build --verbosity minimal    # Essential info
dotnet build --verbosity normal     # Default
dotnet build --verbosity detailed   # Detailed info
dotnet build --verbosity diagnostic # Maximum detail
```

**Common build properties:**
```bash
# Define preprocessor symbol
dotnet build -p:DefineConstants=DEBUG

# Set assembly version
dotnet build -p:Version=1.2.3

# Treat warnings as errors
dotnet build -p:TreatWarningsAsErrors=true

# Generate documentation XML
dotnet build -p:GenerateDocumentationFile=true
```

### dotnet run

**Purpose:** Runs application (builds if necessary)

```bash
# Run current project
dotnet run

# Run with arguments
dotnet run -- arg1 arg2 arg3

# Run specific project
dotnet run --project MyApp/MyApp.csproj

# Run with configuration
dotnet run -c Release

# Run without building (assumes already built)
dotnet run --no-build

# Run with environment variable
dotnet run --environment Production

# Watch mode (auto-restart on changes)
dotnet watch run
```

**Example with arguments:**
```bash
# Program.cs
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Arguments: {string.Join(", ", args)}");
    }
}

# Run with arguments
dotnet run -- hello world 123
# Output: Arguments: hello, world, 123
```

### dotnet test

**Purpose:** Runs unit tests

```bash
# Run all tests in current project
dotnet test

# Run tests in specific project
dotnet test MyTests/MyTests.csproj

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName=MyTests.MyClass.MyTest"

# Run tests matching name pattern
dotnet test --filter "Name~Calculator"

# Run tests in specific category
dotnet test --filter "Category=Integration"

# Collect code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests with no build
dotnet test --no-build

# Settings file
dotnet test --settings test.runsettings
```

**Test filters:**
```bash
# By name
dotnet test --filter "Name=MyTest"

# By fully qualified name
dotnet test --filter "FullyQualifiedName~MyNamespace.MyClass"

# By category/trait
dotnet test --filter "Category=Unit"

# Multiple conditions (AND)
dotnet test --filter "Category=Unit&Priority=High"

# Multiple conditions (OR)
dotnet test --filter "(Category=Unit)|(Category=Integration)"
```

**Code coverage:**
```bash
# Install coverage tool
dotnet tool install --global coverlet.console

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Output format
dotnet test --collect:"XPlat Code Coverage" \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

### dotnet publish

**Purpose:** Prepares application for deployment

```bash
# Publish for current platform
dotnet publish -c Release

# Publish for specific platform
dotnet publish -c Release -r win-x64

# Self-contained (includes runtime)
dotnet publish -c Release -r linux-x64 --self-contained

# Framework-dependent (requires runtime installed)
dotnet publish -c Release --no-self-contained

# Single file executable
dotnet publish -c Release -r win-x64 \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfContained=true

# Trimmed (remove unused code)
dotnet publish -c Release -r linux-x64 \
  --self-contained \
  -p:PublishTrimmed=true

# ReadyToRun (AOT compilation)
dotnet publish -c Release -r win-x64 \
  -p:PublishReadyToRun=true

# Native AOT
dotnet publish -c Release -r linux-x64 \
  -p:PublishAot=true

# Output directory
dotnet publish -c Release -o ./publish
```

**Publish profiles:**
```xml
<!-- Properties/PublishProfiles/Production.pubxml -->
<Project>
  <PropertyGroup>
    <Configuration>Release</Configuration>
    <Platform>Any CPU</Platform>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
    <SelfContained>true</SelfContained>
    <PublishSingleFile>true</PublishSingleFile>
    <PublishTrimmed>true</PublishTrimmed>
  </PropertyGroup>
</Project>
```

```bash
# Use publish profile
dotnet publish -p:PublishProfile=Production
```

### dotnet clean

**Purpose:** Cleans build artifacts

```bash
# Clean current project
dotnet clean

# Clean with configuration
dotnet clean -c Release

# Clean output directory
dotnet clean -o ./bin/Release
```

## 📦 NuGet Package Management

### dotnet add package

```bash
# Add latest version
dotnet add package Newtonsoft.Json

# Add specific version
dotnet add package Newtonsoft.Json --version 13.0.3

# Add with version range
dotnet add package Newtonsoft.Json --version "13.*"

# Add to specific project
dotnet add MyProject.csproj package Serilog

# Add prerelease version
dotnet add package SomePackage --prerelease

# Add from specific source
dotnet add package MyPackage \
  --source https://my-nuget-server.com/v3/index.json
```

### dotnet remove package

```bash
# Remove package
dotnet remove package Newtonsoft.Json

# Remove from specific project
dotnet remove MyProject.csproj package Serilog
```

### dotnet list package

```bash
# List packages in project
dotnet list package

# Include transitive dependencies
dotnet list package --include-transitive

# Show outdated packages
dotnet list package --outdated

# Show deprecated packages
dotnet list package --deprecated

# Show vulnerable packages
dotnet list package --vulnerable
```

### dotnet nuget

```bash
# Add NuGet source
dotnet nuget add source https://my-nuget-server.com/v3/index.json \
  --name MyNuGetServer

# List sources
dotnet nuget list source

# Remove source
dotnet nuget remove source MyNuGetServer

# Enable source
dotnet nuget enable source MyNuGetServer

# Disable source
dotnet nuget disable source MyNuGetServer

# Clear local cache
dotnet nuget locals all --clear

# Push package to NuGet
dotnet nuget push MyPackage.1.0.0.nupkg \
  --source https://api.nuget.org/v3/index.json \
  --api-key YOUR_API_KEY
```

## 🗂️ Solution Management

### dotnet sln

```bash
# Create solution
dotnet new sln -n MySolution

# List projects in solution
dotnet sln list

# Add project to solution
dotnet sln add MyProject/MyProject.csproj

# Add multiple projects
dotnet sln add **/*.csproj

# Remove project from solution
dotnet sln remove MyProject/MyProject.csproj

# Create folder structure in solution
dotnet sln add MyProject/MyProject.csproj --solution-folder src
```

### Project References

```bash
# Add project reference
dotnet add MyApp.csproj reference MyLibrary/MyLibrary.csproj

# Add multiple references
dotnet add reference Lib1.csproj Lib2.csproj

# List project references
dotnet list reference

# Remove reference
dotnet remove reference MyLibrary/MyLibrary.csproj
```

## 🛠️ Tool Management

### dotnet tool

```bash
# Install global tool
dotnet tool install --global dotnet-ef

# Install local tool (project-specific)
dotnet tool install dotnet-ef --tool-path ./tools

# List installed global tools
dotnet tool list --global

# Update tool
dotnet tool update --global dotnet-ef

# Uninstall tool
dotnet tool uninstall --global dotnet-ef

# Restore tools (from manifest)
dotnet tool restore
```

**Common tools:**
```bash
# Entity Framework Core tools
dotnet tool install --global dotnet-ef

# Code formatter
dotnet tool install --global dotnet-format

# SQL Server CLI
dotnet tool install --global dotnet-sql-cache

# Certificate tool
dotnet tool install --global dotnet-dev-certs

# User secrets manager
dotnet tool install --global dotnet-user-secrets
```

**Tool manifest (dotnet-tools.json):**
```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "dotnet-ef": {
      "version": "10.0.0",
      "commands": ["dotnet-ef"]
    },
    "dotnet-format": {
      "version": "5.1.0",
      "commands": ["dotnet-format"]
    }
  }
}
```

```bash
# Create manifest
dotnet new tool-manifest

# Install tool from manifest
dotnet tool install dotnet-ef

# Restore all tools from manifest
dotnet tool restore
```

## 🔍 Diagnostic & Development Tools

### dotnet watch

**Auto-restart on file changes:**
```bash
# Watch and run
dotnet watch run

# Watch and test
dotnet watch test

# Watch specific files
dotnet watch --project MyApp.csproj run
```

**.NET 10+ Hot Reload:**
```bash
# Run with hot reload enabled (default in .NET 10)
dotnet watch

# Apply code changes without restart!
# Edit code while app is running
```

### dotnet dev-certs

**Manage development certificates:**
```bash
# Trust HTTPS certificate
dotnet dev-certs https --trust

# Export certificate
dotnet dev-certs https --export-path ./cert.pfx --password YourPassword

# Clean certificates
dotnet dev-certs https --clean

# Check certificate status
dotnet dev-certs https --check
```

### dotnet user-secrets

**Manage sensitive configuration:**
```bash
# Initialize user secrets
dotnet user-secrets init

# Set secret
dotnet user-secrets set "ApiKey" "secret-key-value"

# List all secrets
dotnet user-secrets list

# Remove secret
dotnet user-secrets remove "ApiKey"

# Clear all secrets
dotnet user-secrets clear
```

**Access in code:**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Automatically loaded in Development
var apiKey = builder.Configuration["ApiKey"];
```

### dotnet format

**Code formatter:**
```bash
# Format code
dotnet format

# Check without making changes
dotnet format --verify-no-changes

# Format with specific options
dotnet format --include MyProject/

# Format solution
dotnet format MySolution.sln
```

## 🎯 Advanced CLI Patterns

### Chaining Commands

```bash
# Clean, restore, build, test
dotnet clean && dotnet restore && dotnet build && dotnet test

# Build and run if successful
dotnet build && dotnet run

# Conditional execution (PowerShell)
dotnet build; if ($?) { dotnet run }
```

### Environment Variables

```bash
# Windows (CMD)
set ASPNETCORE_ENVIRONMENT=Production
dotnet run

# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Production
dotnet run

# Inline (Linux/macOS)
ASPNETCORE_ENVIRONMENT=Production dotnet run
```

### Response Files

**Create response file (build-options.rsp):**
```
-c
Release
-r
linux-x64
--self-contained
-p:PublishSingleFile=true
```

```bash
# Use response file
dotnet publish @build-options.rsp
```

### Scripting Examples

**Build script (build.sh):**
```bash
#!/bin/bash
set -e  # Exit on error

echo "Cleaning..."
dotnet clean

echo "Restoring packages..."
dotnet restore

echo "Building..."
dotnet build -c Release

echo "Running tests..."
dotnet test --no-build -c Release

echo "Publishing..."
dotnet publish -c Release -o ./publish

echo "Build complete!"
```

**Windows PowerShell (build.ps1):**
```powershell
$ErrorActionPreference = "Stop"

Write-Host "Cleaning..."
dotnet clean

Write-Host "Restoring packages..."
dotnet restore

Write-Host "Building..."
dotnet build -c Release

Write-Host "Running tests..."
dotnet test --no-build -c Release

Write-Host "Publishing..."
dotnet publish -c Release -o ./publish

Write-Host "Build complete!"
```

## 🎨 Custom Templates

### Create Custom Template

```bash
# Install template
dotnet new install MyCompany.Templates

# Use custom template
dotnet new my-template -n MyProject

# Uninstall template
dotnet new uninstall MyCompany.Templates
```

**Template structure:**
```
MyTemplate/
├── .template.config/
│   └── template.json
├── Company.ProjectName.csproj
├── Program.cs
└── README.md
```

**template.json:**
```json
{
  "author": "Your Name",
  "classifications": ["Web", "API"],
  "name": "My Company Web API",
  "identity": "MyCompany.WebApi",
  "shortName": "myapi",
  "tags": {
    "language": "C#",
    "type": "project"
  },
  "sourceName": "Company.ProjectName",
  "preferNameDirectory": true
}
```

## 💡 Pro Tips

### 1. Global.json for SDK Version

**Control SDK version per project:**
```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestMinor"
  }
}
```

```bash
# Create global.json
dotnet new globaljson --sdk-version 10.0.100
```

### 2. Directory.Build.props

**Share properties across projects:**
```xml
<!-- Directory.Build.props (in solution root) -->
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Company>Your Company</Company>
  </PropertyGroup>
</Project>
```

### 3. Useful Aliases

**Bash/Zsh:**
```bash
# Add to ~/.bashrc or ~/.zshrc
alias dnb='dotnet build'
alias dnr='dotnet run'
alias dnt='dotnet test'
alias dnp='dotnet publish -c Release'
alias dnc='dotnet clean'
alias dnw='dotnet watch run'
```

**PowerShell:**
```powershell
# Add to $PROFILE
function dnb { dotnet build }
function dnr { dotnet run }
function dnt { dotnet test }
function dnp { dotnet publish -c Release }
function dnc { dotnet clean }
function dnw { dotnet watch run }
```

## 📚 Command Reference Summary

| Command | Purpose | Common Usage |
|---------|---------|--------------|
| `dotnet new` | Create project | `dotnet new console -n MyApp` |
| `dotnet restore` | Download packages | `dotnet restore` |
| `dotnet build` | Compile project | `dotnet build -c Release` |
| `dotnet run` | Run application | `dotnet run` |
| `dotnet test` | Run tests | `dotnet test` |
| `dotnet publish` | Package for deployment | `dotnet publish -c Release -r linux-x64` |
| `dotnet add package` | Add NuGet package | `dotnet add package Serilog` |
| `dotnet sln` | Manage solution | `dotnet sln add MyProject.csproj` |
| `dotnet tool` | Manage tools | `dotnet tool install --global dotnet-ef` |
| `dotnet watch` | Auto-reload | `dotnet watch run` |

## ⏭️ Next Lesson

Proceed to **[Lesson 03: NuGet Package Management](03-nuget-packages.md)** to learn about:
- Finding and evaluating packages
- Package versioning strategies
- Creating your own packages
- Private package feeds
- Dependency management

## 📚 Additional Resources

- [.NET CLI Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [dotnet Command Reference](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet)
- [.NET Templates](https://github.com/dotnet/templating)
- [Global Tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools)

---

*"The CLI is where professionals live. Master it, and you'll be unstoppable." - Developer wisdom*
