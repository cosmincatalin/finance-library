# Installation

## dotnet-interactive

In a `dotnet-interactive` notebook, the library can be installed with

```csharp
#r "nuget: CosminSanda.Finance"
```

Although transient dependencies should be installed as well, in the notebook context that does not seem to the case, so ou might also need to run:

```csharp
#r "nuget: CsvHelper"
#r "nuget: ServiceStack.Text"
```

## .NET project

The ubiquitous `Install-Package CosminSanda.Finance`.
