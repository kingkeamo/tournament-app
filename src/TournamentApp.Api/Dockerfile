FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TournamentApp.Api/TournamentApp.Api.csproj", "src/TournamentApp.Api/"]
COPY ["src/TournamentApp.Application/TournamentApp.Application.csproj", "src/TournamentApp.Application/"]
COPY ["src/TournamentApp.Domain/TournamentApp.Domain.csproj", "src/TournamentApp.Domain/"]
COPY ["src/TournamentApp.Infrastructure/TournamentApp.Infrastructure.csproj", "src/TournamentApp.Infrastructure/"]
RUN dotnet restore "src/TournamentApp.Api/TournamentApp.Api.csproj"
COPY . .
WORKDIR "/src/src/TournamentApp.Api"
RUN dotnet build "TournamentApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TournamentApp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TournamentApp.Api.dll"]

