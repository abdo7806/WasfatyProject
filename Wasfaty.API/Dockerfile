# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Wasfaty.API/Wasfaty.API.csproj", "Wasfaty.API/"]
COPY ["Wasfaty.Application/Wasfaty.Application.csproj", "Wasfaty.Application/"]
COPY ["Wasfaty.Domain/Wasfaty.Domain.csproj", "Wasfaty.Domain/"]
COPY ["Wasfaty.Infrastructure/Wasfaty.Infrastructure.csproj", "Wasfaty.Infrastructure/"]

RUN dotnet restore "Wasfaty.API/Wasfaty.API.csproj"

COPY . .

WORKDIR "/src/Wasfaty.API"
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Wasfaty.API.dll"]