# Changelog

## [1.0.0] - 2026-09-23

### Добавлено

- Доменная модель: агрегат `Quiz` с вопросами, ответами и попытками прохождения; базовые классы `Entity` и `AggregateRoot`; доменные события.
- Слой данных: `QuizerDbContext`, `UnitOfWork`, репозитории `QuizRepository` и `UserRepository`, EF Core конфигурации и миграции.
- API: контроллеры `QuizzesController` (CRUD квизов, вопросов, ответов, попытки, результат, таблица лидеров) и `UsersController` (регистрация и авторизация), контракты DTO, пагинация, обработка исключений.
- Аутентификация и авторизация: ASP.NET Core Identity, JWT-аутентификация, сервис `IdentityService`, проверка авторства ресурса.
- Инфраструктура: миддлвара отключения CORS для Development, OpenAPI, поддержка user secrets.
- Тесты: unit-тесты для агрегатов `Quiz` и `Attempt`.
- Фронтенд: страницы авторизации, регистрации и управления квизами.

## [0.1.0] - 2026-09-21

### Добавлено

- Инициализация структуры решения `Quizer.sln` и проектов: `Quizer`, `Quizer.Controllers`, `Quizer.Infrastructure`, `Quizer.Web`, `Quizer.UnitTests`.
- Добавлено игнорирование файлов IDE (`.gitignore`).

[1.0.0]: https://github.com/DmitriyAlexv/Quizer/releases/tag/1.0.0
