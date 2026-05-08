 # это первая стадия (build), которая будет использовать образ .NET SDK версии 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# Устанавливает рабочую директорию внутри контейнера в /src
WORKDIR /src

# Копируем только файл проекта .csproj из хост-системы в текущую рабочую директорию (./)
COPY ["BookStories.csproj", "./"]
# Восстанавливаем все NuGet-пакеты, указанные в .csproj
# Благодаря тому, что скопировали только .csproj, Docker может закэшировать этот слой
# и не переустанавливать пакеты, если .csproj не менялся
RUN dotnet restore "BookStories.csproj"

# Копируем все остальные исходные файлы проекта в контейнер
COPY . .
# Публикуем (собираем) проект в Release-конфигурации
RUN dotnet publish "BookStories.csproj" -c Release -o /app/publish /p:Configuration=Release

# Начинаем вторую стадию (runtime), используя легковесный образ .NET Runtime 10.0
# (без SDK, компиляторов и т.д. - только для выполнения приложения)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
# Устанавливаем рабочую директорию в /app (где будет лежать приложение)
WORKDIR /app
# Обновляем список пакетов apt и устанавливаем утилиты wget и curl
# нужны для healthcheck, иначе при docker ps статус будет unhealthy
RUN apt-get update && apt-get install -y --no-install-recommends wget curl \
    && rm -rf /var/lib/apt/lists/*

# Копируем опубликованные файлы из первой стадии (build)
# Это не включает SDK, исходники, промежуточные файлы (.obj, .bin) - только готовое приложение
COPY --from=build /app/publish .

# Устанавливаем переменную окружения: приложение будет слушать HTTP-запросы на порту 8016
ENV ASPNETCORE_URLS=http://+:8016
# Сообщаем Docker, что контейнер будет слушать трафик на порту 8016
EXPOSE 8016
# Команда по умолчанию при запуске контейнера: выполнить dotnet BookStories.dll
ENTRYPOINT ["dotnet", "BookStories.dll"]