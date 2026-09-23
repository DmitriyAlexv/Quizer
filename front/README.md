# Quizer — Frontend

Фронтенд-часть сервиса **Quizer** (создание и прохождение небольших квизов).
Реализована как SPA на **React + TypeScript** (сборщик — Vite).

## Стек

- React 19
- TypeScript
- Vite
- React Router (маршрутизация)
- CSS (отдельные файлы для сложных стилей)

## Структура

```
src/
├── api/                    # Работа с бэкенд-API
│   ├── client.ts           # Базовый HTTP-клиент (fetch + обработка ошибок)
│   └── auth.ts             # Функции авторизации/регистрации
├── components/
│   ├── common/             # Общие переиспользуемые компоненты
│   │   ├── Button.tsx      # Кнопка (primary/secondary/ghost, loading)
│   │   ├── Input.tsx       # Поле ввода (label, иконка, ошибка, пароль)
│   │   ├── Logo.tsx        # Логотип Quizer
│   │   └── Alert.tsx       # Сообщения (ошибка/успех/инфо)
│   ├── AuthLayout.tsx      # Каркас страниц входа/регистрации
│   └── ProtectedRoute.tsx  # Защита маршрутов (требует авторизации)
├── context/
│   └── AuthContext.tsx     # Состояние авторизации (JWT-токен)
├── pages/
│   ├── LoginPage.tsx       # Страница входа
│   ├── RegisterPage.tsx    # Страница регистрации
│   └── HomePage.tsx        # Заглушка после входа
├── App.tsx                 # Маршрутизация
├── main.tsx                # Точка входа (BrowserRouter + AuthProvider)
└── index.css               # Дизайн-токены (CSS-переменные)
```

## Запуск

```bash
npm install
npm run dev
```

## Конфигурация API

Базовый URL бэкенда задаётся через переменную окружения `VITE_API_URL`
(по умолчанию — `http://localhost:5097`). Пример — в файле `.env.example`.

## Контракты API

| Метод | Путь                     | Тело запроса                          | Ответ            |
|-------|--------------------------|---------------------------------------|------------------|
| POST  | `/api/v1/users/register` | `{ email, password, name }`           | `Guid` (userId)  |
| POST  | `/api/v1/users/authorize`| `{ email, password }`                 | `{ token }`      |

## Скрипты

- `npm run dev` — запуск dev-сервера
- `npm run build` — сборка для продакшена
- `npm run lint` — проверка линтером
- `npm run preview` — предпросмотр собранной версии
