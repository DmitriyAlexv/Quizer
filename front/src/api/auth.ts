import { apiRequest } from './client';

/** Контракт запроса на регистрацию (совпадает с RegisterRequest на бэкенде). */
export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
}

/** Контракт запроса на авторизацию (совпадает с AuthorizeRequest на бэкенде). */
export interface AuthorizeRequest {
  email: string;
  password: string;
}

/** Контракт ответа на авторизацию (совпадает с AuthorizeResponse на бэкенде). */
export interface AuthorizeResponse {
  token: string;
}

/**
 * Регистрирует нового пользователя.
 * Возвращает идентификатор созданного пользователя (Guid).
 */
export function registerUser(request: RegisterRequest): Promise<string> {
  return apiRequest<string>('/api/v1/users/register', {
    method: 'POST',
    body: request,
  });
}

/**
 * Авторизует пользователя и возвращает JWT-токен.
 */
export function authorizeUser(request: AuthorizeRequest): Promise<AuthorizeResponse> {
  return apiRequest<AuthorizeResponse>('/api/v1/users/authorize', {
    method: 'POST',
    body: request,
  });
}
