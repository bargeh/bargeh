FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Source/Services/Sms.Api/Sms.Api.csproj", "Source/Services/Sms.Api/"]
RUN dotnet restore "Source/Services/Sms.Api/Sms.Api.csproj"
COPY . .
WORKDIR "/src/Source/Services/Sms.Api"
RUN dotnet build "Sms.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sms.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sms.Api.dll"]
