FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["JurayKV.sln", "./"]
COPY ["PostmarkEmailService/PostmarkEmailService.csproj", "PostmarkEmailService/"]
COPY ["src/ServerApp/Core/JurayKV.Domain/JurayKV.Domain.csproj", "src/ServerApp/Core/JurayKV.Domain/"]
COPY ["src/ServerApp/Core/JurayKV.Application/JurayKV.Application.csproj", "src/ServerApp/Core/JurayKV.Application/"]
COPY ["src/ServerApp/Infrastructure/JurayKV.Infrastructure.Flutterwave/JurayKV.Infrastructure.Flutterwave.csproj", "src/ServerApp/Infrastructure/JurayKV.Infrastructure.Flutterwave/"]
COPY ["src/ServerApp/Infrastructure/JurayKV.Infrastructure.Opay/JurayKV.Infrastructure.Opay.csproj", "src/ServerApp/Infrastructure/JurayKV.Infrastructure.Opay/"]
COPY ["src/ServerApp/Infrastructure/JurayKV.Infrastructure.VTU/JurayKV.Infrastructure.VTU.csproj", "src/ServerApp/Infrastructure/JurayKV.Infrastructure.VTU/"]
COPY ["src/ServerApp/Infrastructure/JurayKvV.Infrastructure.Interswitch/JurayKvV.Infrastructure.Interswitch.csproj", "src/ServerApp/Infrastructure/JurayKvV.Infrastructure.Interswitch/"]
COPY ["src/ServerApp/Infrastructure/JurayKV.Persistence.Cache/JurayKV.Persistence.Cache.csproj", "src/ServerApp/Infrastructure/JurayKV.Persistence.Cache/"]
COPY ["src/ServerApp/Infrastructure/JurayKV.Persistence.RelationalDB/JurayKV.Persistence.RelationalDB.csproj", "src/ServerApp/Infrastructure/JurayKV.Persistence.RelationalDB/"]
COPY ["src/ServerApp/Infrastructure/JurayKV.Infrastructure.Services/JurayKV.Infrastructure.Services.csproj", "src/ServerApp/Infrastructure/JurayKV.Infrastructure.Services/"]
COPY ["src/ServerApp/Presentation/JurayKV.UI/JurayKV.UI.csproj", "src/ServerApp/Presentation/JurayKV.UI/"]

RUN dotnet restore "src/ServerApp/Presentation/JurayKV.UI/JurayKV.UI.csproj"
COPY . .
WORKDIR "/src/src/ServerApp/Presentation/JurayKV.UI"
RUN dotnet publish "JurayKV.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "JurayKV.UI.dll"]
