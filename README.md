# Quizer
Сервис по созданию и участию в викторинах

# Запуск
1. Клонируйте репозиторий
2. Установить зависимости `npm install`
3. Запустите фронт `npm run`
4. Соберите решение `dotnet build`
5. Установите переменные UserSecrets (или в appsettings.json) для строк подключения и ключа jwt
6. Примените миграции `dotnet ef database update -p src/Quizer.Infrastructure -s src/Quizer.Web -c QuizerIdentityDbContext`
7. Примените миграции `dotnet ef database update -p src/Quizer.Infrastructure -s src/Quizer.Web -c QuizerDbContext`
8. Запустите проект `dotnet run -c Release`

# Тесты
Запустите `dotnet test`
