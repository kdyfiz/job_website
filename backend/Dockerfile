FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore JobScout.API/JobScout.API.csproj
RUN dotnet publish JobScout.API/JobScout.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_HOSTBUILDER_RELOADCONFIGONCHANGE=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_EnableDiagnostics=0
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "exec dotnet JobScout.API.dll --urls http://0.0.0.0:${PORT:-8080}"]
