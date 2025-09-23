# Этап 1: Сборка и публикация
# Используем LTS-версию .NET 9.0 для стабильности
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем .csproj файлы для всех проектов
COPY ["Server/Server.csproj", "Server/"]
COPY ["BaseLibrary/BaseLibrary.csproj", "BaseLibrary/"]
COPY ["ServerLibrary/ServerLibrary.csproj", "ServerLibrary/"]

# Восстанавливаем зависимости. Этот слой будет кэширован, если .csproj файлы не менялись
RUN dotnet restore "Server/Server.csproj"

# Копируем весь остальной код (с учетом .dockerignore)
COPY . .
WORKDIR "/src/Server"

# Собираем приложение
RUN dotnet build "Server.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

# Публикуем приложение
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Server.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

# Этап 2: Создание финального образа
# Используем легковесный aspnet runtime, соответствующий версии SDK
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Создаем пользователя без прав root для запуска приложения
ENV APP_USER=appuser
RUN useradd -m ${APP_USER}

WORKDIR /app
# Копируем опубликованные файлы. Сейчас они принадлежат root.
COPY --from=publish /app/publish .

# Меняем владельца файлов на нашего нового пользователя
RUN chown -R ${APP_USER}:${APP_USER} .

# Переключаемся на пользователя без прав root
USER ${APP_USER}

# EXPOSE — это просто метаданные для информации, реальное сопоставление портов происходит в docker-compose
EXPOSE 8080

# Команда для запуска приложения
ENTRYPOINT ["dotnet", "Server.dll"]