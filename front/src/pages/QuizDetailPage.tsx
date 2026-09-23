import { useCallback, useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { AppLayout } from '../components/AppLayout';
import { Button } from '../components/common/Button';
import { Badge } from '../components/common/Badge';
import { Card } from '../components/common/Card';
import { Spinner } from '../components/common/Spinner';
import { Alert } from '../components/common/Alert';
import {
  getQuiz,
  getQuestions,
  getAttempts,
  getLeaderboard,
  type Quiz,
  type Question,
  type Attempt,
  type LeaderboardEntry,
} from '../api/quizzes';
import { formatDate, formatDateTime } from '../utils/format';
import './quiz-detail.css';

const QUESTIONS_PAGE_SIZE = 50;
const ATTEMPTS_PAGE_SIZE = 10;

export function QuizDetailPage() {
  const { id } = useParams<{ id: string }>();

  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [attempts, setAttempts] = useState<Attempt[]>([]);
  const [attemptsTotal, setAttemptsTotal] = useState(0);
  const [attemptsPage, setAttemptsPage] = useState(1);
  const [attemptsTotalPages, setAttemptsTotalPages] = useState(1);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    if (!id) return;
    try {
      const [quizData, questionsData, attemptsData, leaderboardData] = await Promise.all([
        getQuiz(id),
        getQuestions(id, 1, QUESTIONS_PAGE_SIZE),
        getAttempts(id, attemptsPage, ATTEMPTS_PAGE_SIZE),
        getLeaderboard(id, 10),
      ]);
      setQuiz(quizData);
      setQuestions(questionsData.items);
      setAttempts(attemptsData.items);
      setAttemptsTotal(attemptsData.totalCount);
      setAttemptsTotalPages(attemptsData.totalPages);
      setLeaderboard(leaderboardData);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось загрузить квиз');
    } finally {
      setLoading(false);
    }
  }, [id, attemptsPage]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
  }, [load]);

  if (loading) {
    return (
      <AppLayout>
        <div className="detail__loading">
          <Spinner size="lg" label="Загрузка квиза…" />
        </div>
      </AppLayout>
    );
  }

  if (error || !quiz) {
    return (
      <AppLayout>
        <div className="detail__error">
          <Alert variant="error">{error ?? 'Квиз не найден'}</Alert>
          <Link to="/quizzes" className="detail__back">
            ← К списку квизов
          </Link>
        </div>
      </AppLayout>
    );
  }

  const totalPoints = questions.reduce((sum, q) => sum + q.points, 0);

  return (
    <AppLayout>
      <div className="detail">
        <div className="detail__header">
          <Link to="/quizzes" className="detail__back">
            ← К списку квизов
          </Link>
          <div className="detail__title-row">
            <h1 className="detail__title">{quiz.title}</h1>
            <Badge variant={quiz.status === 'Published' ? 'success' : 'warning'}>
              {quiz.status === 'Published' ? 'Опубликован' : 'Черновик'}
            </Badge>
          </div>
          {quiz.description && <p className="detail__desc">{quiz.description}</p>}
          <div className="detail__meta">
            <span>Создан: {formatDate(quiz.createdAt)}</span>
            {quiz.updatedAt && <span>Обновлён: {formatDate(quiz.updatedAt)}</span>}
          </div>
          <div className="detail__actions">
            {quiz.status === 'Published' && (
              <Link to={`/quizzes/${quiz.id}/pass`}>
                <Button>Пройти квиз</Button>
              </Link>
            )}
            <Link to={`/quizzes/${quiz.id}/edit`}>
              <Button variant="secondary">Редактировать</Button>
            </Link>
          </div>
        </div>

        <div className="detail__grid">
          <div className="detail__main">
            <Card className="detail__section">
              <div className="detail__section-header">
                <h2 className="detail__section-title">Вопросы</h2>
                <span className="detail__section-count">
                  {questions.length} · {totalPoints} б.
                </span>
              </div>
              {questions.length === 0 ? (
                <p className="detail__empty">В этом квизе пока нет вопросов.</p>
              ) : (
                <ol className="detail__questions">
                  {questions.map((q, index) => (
                    <li key={q.id} className="detail-question">
                      <span className="detail-question__num">{index + 1}.</span>
                      <div className="detail-question__body">
                        <p className="detail-question__text">{q.text}</p>
                        <div className="detail-question__meta">
                          <Badge variant="default">
                            {q.type === 'Open'
                              ? 'Открытый'
                              : q.type === 'SingleChoice'
                                ? 'Один вариант'
                                : 'Несколько вариантов'}
                          </Badge>
                          <span className="detail-question__points">{q.points} б.</span>
                        </div>
                      </div>
                    </li>
                  ))}
                </ol>
              )}
            </Card>

            <Card className="detail__section">
              <div className="detail__section-header">
                <h2 className="detail__section-title">Попытки</h2>
                <span className="detail__section-count">{attemptsTotal}</span>
              </div>
              {attempts.length === 0 ? (
                <p className="detail__empty">Попыток прохождения пока нет.</p>
              ) : (
                <div className="detail__attempts">
                  {attempts.map((attempt) => (
                    <div key={attempt.id} className="detail-attempt">
                      <div className="detail-attempt__info">
                        <Badge
                          variant={attempt.status === 'Completed' ? 'success' : 'warning'}
                        >
                          {attempt.status === 'Completed' ? 'Завершена' : 'В процессе'}
                        </Badge>
                        <span className="detail-attempt__date">
                          {formatDateTime(attempt.startedAt)}
                        </span>
                      </div>
                      {attempt.status === 'Completed' && (
                        <Link
                          to={`/quizzes/${quiz.id}/attempts/${attempt.id}/result`}
                          className="detail-attempt__link"
                        >
                          Результат →
                        </Link>
                      )}
                    </div>
                  ))}
                </div>
              )}
              {attemptsTotalPages > 1 && (
                <div className="detail__pagination">
                  <Button
                    variant="secondary"
                    disabled={attemptsPage <= 1}
                    onClick={() => setAttemptsPage((p) => p - 1)}
                  >
                    Назад
                  </Button>
                  <span className="detail__pagination-info">
                    Страница {attemptsPage} из {attemptsTotalPages}
                  </span>
                  <Button
                    variant="secondary"
                    disabled={attemptsPage >= attemptsTotalPages}
                    onClick={() => setAttemptsPage((p) => p + 1)}
                  >
                    Вперёд
                  </Button>
                </div>
              )}
            </Card>
          </div>

          <div className="detail__side">
            <Card className="detail__section">
              <h2 className="detail__section-title">Лидеры</h2>
              {leaderboard.length === 0 ? (
                <p className="detail__empty">Лидеров пока нет.</p>
              ) : (
                <ol className="detail__leaderboard">
                  {leaderboard.map((entry, index) => (
                    <li key={entry.userId} className="detail-leader">
                      <span className="detail-leader__rank">{index + 1}</span>
                      <span className="detail-leader__name">{entry.userName}</span>
                      <span className="detail-leader__score">{entry.score}</span>
                    </li>
                  ))}
                </ol>
              )}
            </Card>
          </div>
        </div>
      </div>
    </AppLayout>
  );
}
