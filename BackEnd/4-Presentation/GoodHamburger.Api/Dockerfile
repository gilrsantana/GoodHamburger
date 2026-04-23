FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BackEnd/4-Presentation/GoodHamburger.Api/GoodHamburger.Api.csproj", "BackEnd/4-Presentation/GoodHamburger.Api/"]
COPY ["BackEnd/3-Infrastructure/GoodHamburger.Database/GoodHamburger.Database.csproj", "BackEnd/3-Infrastructure/GoodHamburger.Database/"]
COPY ["BackEnd/3-Infrastructure/GoodHamburger.Infrastructure/GoodHamburger.Infrastructure.csproj", "BackEnd/3-Infrastructure/GoodHamburger.Infrastructure/"]
COPY ["BackEnd/1-Domain/GoodHamburger.Domain/GoodHamburger.Domain.csproj", "BackEnd/1-Domain/GoodHamburger.Domain/"]
COPY ["BackEnd/0-Shared/GoodHamburger.Shared/GoodHamburger.Shared.csproj", "BackEnd/0-Shared/GoodHamburger.Shared/"]
RUN dotnet restore "BackEnd/4-Presentation/GoodHamburger.Api/GoodHamburger.Api.csproj"
COPY . .
WORKDIR "/src/BackEnd/4-Presentation/GoodHamburger.Api"
RUN dotnet build "./GoodHamburger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GoodHamburger.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoodHamburger.Api.dll"]
