FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем все файлы решения
COPY . .

# Восстанавливаем пакеты и публикуем
RUN dotnet restore "StudentAccounting.WebApi/StudentAccounting.WebApi.csproj"
RUN dotnet publish "StudentAccounting.WebApi/StudentAccounting.WebApi.csproj" -c Release -o /app

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080

# Важно для Render
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "StudentAccounting.WebApi.dll"]