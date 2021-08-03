# videoapp

Website with movie trailer search engine.

Uses and aggregates data from IMDB and Youtube to provide a web API for searching for movie trailers.

## how to build (and run)

### prerequisites

You should have .NET 5 SDK installed. You can get it [here](https://dotnet.microsoft.com/download).

### building and running

```cmd
cd src\videoapp.api
dotnet restore
dotnet build
```

then, you could type:

```cmd
dotnet run
```

navigate to:

```
https://localhost:5001/
```

or browse API definition

```
https://localhost:5001/swagger/index.html
```
