import { useCallback, useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { AppLayout } from '../components/AppLayout';
import { Button } from '../components/common/Button';
import { Badge } from '../components/common/Badge';
import { Card } from '../components/common/Card';
import { Spinner } from '../components/common/Spinner';
import { Alert } from '../components/common/Alert';
import { getAttemptResult, type AttemptResult } from '../api/quizzes';
import { formatPercent } from '../utils/format';
import './quiz-result.css';

export function QuizResultPage() {
  const { id, attemptId } = useParams<{ id: string; attemptId: string }>();
  const [result, setResult] = useState<AttemptResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    if (!id || !attemptId) return;
    try {
      const data = await getAttemptResult(id, attemptId);
      setResult(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось загрузить результат');
    } finally {
      setLoading(false);
    }
  }, [id, attemptId]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
  }, [load]);

  if (loading) {
    return (
      <AppLayout>
        <div className="result__loading">
          <Spinner size="lg" label="Загрузка результата…" />
        </div>
      </AppLayout>
    );
  }

  if (error || !result) {
    return (
      <AppLayout>
        <div className="result__error">
          <Alert variant="error">{error ?? 'Результат не найден'}</Alert>
          <Link to="/quizzes" className="result__back">
            ← К списку квизов
          </Link>
        </div>
      </AppLayout>
    );
  }

  const percent =
    result.totalPoints > 0 ? (result.earnedPoints / result.totalPoints) * 100 : 0;

  return (
    <AppLayout>
      <div className="result">
        <div className="result__header">
          <Link to="/quizzes" className="result__back">
            ← К списку квизов
          </Link>
          <h1 className="result__title">Результат прохождения</h1>
        </div>

        <Card className="result__summary">
          <div className="result__score">
            <span className="result__score-value">
              {result.earnedPoints}
              <span className="result__score-total"> / {result.totalPoints}</span>
            </span>
            <span className="result__score-label">баллов</span>
          </div>
          <div className="result__percent">
            <div className="result__percent-bar">
              <div className="result__percent-fill" style={{ width: `${percent}%` }} />
            </div>
            <span className="result__percent-text">{formatPercent(percent)}</span>
          </div>
          <Badge variant={percent >= 50 ? 'success' : 'danger'}>
            {percent >= 50 ? 'Пройден' : 'Не пройден'}
          </Badge>
        </Card>

        <h2 className="result__section-title">Разбор вопросов</h2>
        <div className="result__questions">
          {result.questions.map((q, index) => (
            <Card key={q.questionId} className="result-question">
              <div className="result-question__header">
                <span className="result-question__num">Вопрос {index + 1}</span>
                <Badge variant={q.isCorrect ? 'success' : 'danger'}>
                  {q.isCorrect ? 'Верно' : 'Неверно'}
                </Badge>
                <span className="result-question__points">{q.points} б.</span>
              </div>
              <p className="result-question__text">{q.text}</p>
              {q.type === 'Open' ? (
                <p className="result-question__answer">
                  Ваш ответ: <span className="result-question__answer-text">{q.textAnswer || '—'}</span>
                </p>
              ) : (
                <p className="result-question__answer">
                  Выбрано: <span className="result-question__answer-text">{q.selectedAnswerIds.length}</span>
                </p>
              )}
            </Card>
          ))}
        </div>

        <div className="result__actions">
          <Link to={`/quizzes/${id}/pass`}>
            <Button variant="secondary">Пройти ещё раз</Button>
          </Link>
          <Link to={`/quizzes/${id}`}>
            <Button>К квизу</Button>
          </Link>
        </div>
      </div>
    </AppLayout>
  );
}
