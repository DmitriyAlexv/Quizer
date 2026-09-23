/**
 * Базовый API-клиент для взаимодействия с бэкендом.
 * Все запросы идут к API, адрес которого задаётся через VITE_API_URL.
 */

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5097';

export class ApiError extends Error {
  readonly status: number;
  readonly details: string;

  constructor(status: number, message: string, details = '') {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
  }
}

interface RequestOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
  body?: unknown;
  token?: string | null;
}

/**
 * Выполняет запрос к API и возвращает распарсенный JSON.
 * При ошибке HTTP-статуса бросает ApiError с человекочитаемым сообщением.
 */
export async function apiRequest<T>(
  path: string,
  { method = 'GET', body, token }: RequestOptions = {},
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    throw new ApiError(0, 'Не удалось подключиться к серверу. Проверьте, что бэкенд запущен.');
  }

  if (!response.ok) {
    const message = await extractErrorMessage(response);
    throw new ApiError(response.status, message);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function extractErrorMessage(response: Response): Promise<string> {
  const text = await response.text();
  if (!text) {
    return `Ошибка запроса (${response.status})`;
  }

  try {
    const data = JSON.parse(text) as Record<string, unknown>;
    if (typeof data.title === 'string') {
      return data.title;
    }
    if (typeof data.message === 'string') {
      return data.message;
    }
    if (typeof data.error === 'string') {
      return data.error;
    }
  } catch {
    // тело не является JSON — используем как есть
  }

  return text;
}
