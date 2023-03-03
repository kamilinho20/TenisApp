 FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
 WORKDIR /app
 EXPOSE 80
 FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
 WORKDIR /src
 COPY . .
 RUN ls
 RUN dotnet restore TenisApp.Api/TenisApp.Api.csproj
 RUN dotnet build TenisApp.Api/TenisApp.Api.csproj -c Release -o /app/build
 FROM build AS publish
 RUN dotnet publish TenisApp.Api/TenisApp.Api.csproj -c Release -o /app/publish
 FROM base AS final
 WORKDIR /app
 COPY --from=publish /app/publish .
 ENTRYPOINT ["dotnet", "TenisApp.Api.dll"]
