import { apiRequest } from './client';

const TOKEN_STORAGE_KEY = 'quizer.token';

/** Возвращает JWT-токен текущего пользователя из localStorage. */
function getToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_STORAGE_KEY);
  } catch {
    return null;
  }
}

/* ============================================================
 * Типы (контракты совпадают с бэкендом Quizer.Controllers)
 * ============================================================ */

/** Тип вопроса. */
export type QuestionType = 'Open' | 'SingleChoice' | 'MultipleChoice';

/** Статус квиза. */
export type QuizStatus = 'Draft' | 'Published';

/** Статус попытки прохождения. */
export type AttemptStatus = 'InProgress' | 'Completed';

/** Ответ с пагинацией. */
export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

/** Квиз. */
export interface Quiz {
  id: string;
  title: string;
  description: string;
  ownerId: string;
  status: QuizStatus;
  createdAt: string;
  updatedAt: string | null;
}

/** Вариант ответа. */
export interface Answer {
  id: string;
  text: string;
  isCorrect: boolean;
}

/** Вопрос квиза. */
export interface Question {
  id: string;
  text: string;
  type: QuestionType;
  order: number;
  points: number;
  answers: Answer[];
}

/** Ответ пользователя на вопрос в рамках попытки. */
export interface AttemptAnswer {
  id: string;
  questionId: string;
  textAnswer: string | null;
  selectedAnswerIds: string[];
  isCorrect: boolean;
}

/** Попытка прохождения квиза. */
export interface Attempt {
  id: string;
  quizId: string;
  userId: string;
  status: AttemptStatus;
  startedAt: string;
  completedAt: string | null;
  answers: AttemptAnswer[];
}

/** Результат по конкретному вопросу. */
export interface QuestionResult {
  questionId: string;
  text: string;
  type: QuestionType;
  points: number;
  isCorrect: boolean;
  textAnswer: string | null;
  selectedAnswerIds: string[];
}

/** Результат прохождения квиза. */
export interface AttemptResult {
  attemptId: string;
  quizId: string;
  userId: string;
  status: AttemptStatus;
  startedAt: string;
  completedAt: string | null;
  totalPoints: number;
  earnedPoints: number;
  questions: QuestionResult[];
}

/** Запись в таблице лидеров. */
export interface LeaderboardEntry {
  userId: string;
  userName: string;
  score: number;
  completedAt: string;
}

/* ============================================================
 * Запросы
 * ============================================================ */

export interface CreateQuizRequest {
  title: string;
  description: string;
}

export interface UpdateQuizRequest {
  title: string;
  description: string;
}

export interface AddQuestionRequest {
  text: string;
  type: QuestionType;
  order: number;
  points: number;
}

export interface UpdateQuestionRequest {
  text: string;
  type: QuestionType;
  order: number;
  points: number;
}

export interface AddAnswerRequest {
  text: string;
  isCorrect: boolean;
}

export interface UpdateAnswerRequest {
  text: string;
  isCorrect: boolean;
}

export interface AnswerQuestionRequest {
  textAnswer: string | null;
  selectedAnswerIds: string[] | null;
}

/* ============================================================
 * Методы API
 * ============================================================ */

/** Получить список квизов (с пагинацией). */
export function getQuizzes(page = 1, pageSize = 10): Promise<PagedResponse<Quiz>> {
  return apiRequest<PagedResponse<Quiz>>(`/api/v1/quizzes?page=${page}&pageSize=${pageSize}`, {
    token: getToken(),
  });
}

/** Получить квиз по идентификатору. */
export function getQuiz(id: string): Promise<Quiz> {
  return apiRequest<Quiz>(`/api/v1/quizzes/${id}`, { token: getToken() });
}

/** Создать квиз. Возвращает идентификатор созданного квиза. */
export function createQuiz(request: CreateQuizRequest): Promise<string> {
  return apiRequest<string>('/api/v1/quizzes', { method: 'POST', body: request, token: getToken() });
}

/** Обновить квиз. */
export function updateQuiz(id: string, request: UpdateQuizRequest): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${id}`, { method: 'PUT', body: request, token: getToken() });
}

/** Удалить квиз. */
export function deleteQuiz(id: string): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${id}`, { method: 'DELETE', token: getToken() });
}

/** Опубликовать квиз. */
export function publishQuiz(id: string): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${id}/publish`, { method: 'PATCH', token: getToken() });
}

/** Получить список вопросов квиза (с пагинацией). */
export function getQuestions(quizId: string, page = 1, pageSize = 50): Promise<PagedResponse<Question>> {
  return apiRequest<PagedResponse<Question>>(
    `/api/v1/quizzes/${quizId}/questions?page=${page}&pageSize=${pageSize}`,
    { token: getToken() },
  );
}

/** Получить вопрос по идентификатору. */
export function getQuestion(quizId: string, questionId: string): Promise<Question> {
  return apiRequest<Question>(`/api/v1/quizzes/${quizId}/questions/${questionId}`, { token: getToken() });
}

/** Добавить вопрос в квиз. Возвращает идентификатор созданного вопроса. */
export function addQuestion(quizId: string, request: AddQuestionRequest): Promise<string> {
  return apiRequest<string>(`/api/v1/quizzes/${quizId}/questions`, {
    method: 'POST',
    body: request,
    token: getToken(),
  });
}

/** Обновить вопрос. */
export function updateQuestion(quizId: string, questionId: string, request: UpdateQuestionRequest): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${quizId}/questions/${questionId}`, {
    method: 'PUT',
    body: request,
    token: getToken(),
  });
}

/** Удалить вопрос. */
export function deleteQuestion(quizId: string, questionId: string): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${quizId}/questions/${questionId}`, {
    method: 'DELETE',
    token: getToken(),
  });
}

/** Добавить вариант ответа к вопросу. */
export function addAnswer(quizId: string, questionId: string, request: AddAnswerRequest): Promise<Answer> {
  return apiRequest<Answer>(`/api/v1/quizzes/${quizId}/questions/${questionId}/answers`, {
    method: 'POST',
    body: request,
    token: getToken(),
  });
}

/** Обновить вариант ответа. */
export function updateAnswer(
  quizId: string,
  questionId: string,
  answerId: string,
  request: UpdateAnswerRequest,
): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${quizId}/questions/${questionId}/answers/${answerId}`, {
    method: 'PUT',
    body: request,
    token: getToken(),
  });
}

/** Удалить вариант ответа. */
export function removeAnswer(quizId: string, questionId: string, answerId: string): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${quizId}/questions/${questionId}/answers/${answerId}`, {
    method: 'DELETE',
    token: getToken(),
  });
}

/** Получить список попыток прохождения квиза (с пагинацией). */
export function getAttempts(quizId: string, page = 1, pageSize = 10): Promise<PagedResponse<Attempt>> {
  return apiRequest<PagedResponse<Attempt>>(
    `/api/v1/quizzes/${quizId}/attempts?page=${page}&pageSize=${pageSize}`,
    { token: getToken() },
  );
}

/** Получить попытку по идентификатору. */
export function getAttempt(quizId: string, attemptId: string): Promise<Attempt> {
  return apiRequest<Attempt>(`/api/v1/quizzes/${quizId}/attempts/${attemptId}`, { token: getToken() });
}

/** Получить результат прохождения квиза. */
export function getAttemptResult(quizId: string, attemptId: string): Promise<AttemptResult> {
  return apiRequest<AttemptResult>(`/api/v1/quizzes/${quizId}/attempts/${attemptId}/result`, {
    token: getToken(),
  });
}

/** Начать прохождение квиза. Возвращает созданную попытку. */
export function createAttempt(quizId: string): Promise<Attempt> {
  return apiRequest<Attempt>(`/api/v1/quizzes/${quizId}/attempts`, { method: 'POST', token: getToken() });
}

/** Ответить на вопрос в рамках попытки. */
export function answerQuestion(
  quizId: string,
  attemptId: string,
  questionId: string,
  request: AnswerQuestionRequest,
): Promise<AttemptAnswer> {
  return apiRequest<AttemptAnswer>(
    `/api/v1/quizzes/${quizId}/attempts/${attemptId}/questions/${questionId}/answer`,
    { method: 'POST', body: request, token: getToken() },
  );
}

/** Завершить попытку прохождения квиза. */
export function completeAttempt(quizId: string, attemptId: string): Promise<void> {
  return apiRequest<void>(`/api/v1/quizzes/${quizId}/attempts/${attemptId}/complete`, {
    method: 'POST',
    token: getToken(),
  });
}

/** Получить таблицу лидеров по квизу. */
export function getLeaderboard(quizId: string, limit = 10): Promise<LeaderboardEntry[]> {
  return apiRequest<LeaderboardEntry[]>(`/api/v1/quizzes/${quizId}/leaderboard?limit=${limit}`, {
    token: getToken(),
  });
}
