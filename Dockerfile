# Используем официальный образ .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["authorization.csproj", "./"]
RUN dotnet restore "./authorization.csproj"

# Копируем остальные файлы проекта
COPY . .

# Строим проект
WORKDIR "/src/."
RUN dotnet build "authorization.csproj" -c Release -o /app/build

FROM build AS publish
# Публикуем проект
RUN dotnet publish "authorization.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
# Копируем опубликованные файлы в финальный образ
COPY --from=publish /app/publish .
# Указываем команду для запуска приложения
ENTRYPOINT ["dotnet", "authorization.dll"]
