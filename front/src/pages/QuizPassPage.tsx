import { useCallback, useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { AppLayout } from '../components/AppLayout';
import { Button } from '../components/common/Button';
import { Badge } from '../components/common/Badge';
import { Card } from '../components/common/Card';
import { Spinner } from '../components/common/Spinner';
import { Alert } from '../components/common/Alert';
import { Textarea } from '../components/common/Textarea';
import {
  getQuiz,
  getQuestions,
  createAttempt,
  answerQuestion,
  completeAttempt,
  type Quiz,
  type Question,
  type Attempt,
} from '../api/quizzes';
import './quiz-pass.css';

export function QuizPassPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [attempt, setAttempt] = useState<Attempt | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [currentIndex, setCurrentIndex] = useState(0);
  const [textAnswer, setTextAnswer] = useState('');
  const [selectedAnswers, setSelectedAnswers] = useState<string[]>([]);
  const [saving, setSaving] = useState(false);
  const [completing, setCompleting] = useState(false);

  const currentQuestion = questions[currentIndex];
  const isLast = currentIndex === questions.length - 1;

  const load = useCallback(async () => {
    if (!id) return;
    try {
      const [quizData, questionsData] = await Promise.all([
        getQuiz(id),
        getQuestions(id, 1, 100),
      ]);
      setQuiz(quizData);
      setQuestions(questionsData.items);
      const newAttempt = await createAttempt(id);
      setAttempt(newAttempt);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось начать прохождение квиза');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
  }, [load]);

  const resetAnswer = () => {
    setTextAnswer('');
    setSelectedAnswers([]);
  };

  const handleNext = async () => {
    if (!id || !attempt || !currentQuestion) return;
    setSaving(true);
    setError(null);
    try {
      await answerQuestion(id, attempt.id, currentQuestion.id, {
        textAnswer: currentQuestion.type === 'Open' ? textAnswer : null,
        selectedAnswerIds:
          currentQuestion.type === 'Open' ? null : selectedAnswers,
      });
      if (isLast) {
        setCompleting(true);
        await completeAttempt(id, attempt.id);
        navigate(`/quizzes/${id}/attempts/${attempt.id}/result`);
        return;
      }
      setCurrentIndex((i) => i + 1);
      resetAnswer();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось сохранить ответ');
    } finally {
      setSaving(false);
      setCompleting(false);
    }
  };

  const toggleAnswer = (answerId: string) => {
    if (currentQuestion?.type === 'SingleChoice') {
      setSelectedAnswers([answerId]);
    } else {
      setSelectedAnswers((prev) =>
        prev.includes(answerId) ? prev.filter((a) => a !== answerId) : [...prev, answerId],
      );
    }
  };

  if (loading) {
    return (
      <AppLayout>
        <div className="pass__loading">
          <Spinner size="lg" label="Подготовка квиза…" />
        </div>
      </AppLayout>
    );
  }

  if (error && !quiz) {
    return (
      <AppLayout>
        <div className="pass__error">
          <Alert variant="error">{error}</Alert>
          <Link to="/quizzes" className="pass__back">
            ← К списку квизов
          </Link>
        </div>
      </AppLayout>
    );
  }

  if (!currentQuestion) {
    return (
      <AppLayout>
        <div className="pass__error">
          <Alert variant="info">В этом квизе пока нет вопросов.</Alert>
          <Link to="/quizzes" className="pass__back">
            ← К списку квизов
          </Link>
        </div>
      </AppLayout>
    );
  }

  const progress = ((currentIndex + 1) / questions.length) * 100;

  return (
    <AppLayout>
      <div className="pass">
        <div className="pass__header">
          <Link to="/quizzes" className="pass__back">
            ← Выйти
          </Link>
          <div className="pass__title-wrap">
            <h1 className="pass__title">{quiz?.title}</h1>
            <Badge variant="default">
              Вопрос {currentIndex + 1} из {questions.length}
            </Badge>
          </div>
        </div>

        <div className="pass__progress">
          <div className="pass__progress-bar" style={{ width: `${progress}%` }} />
        </div>

        {error && (
          <div className="pass__alert">
            <Alert variant="error">{error}</Alert>
          </div>
        )}

        <Card className="pass__question">
          <div className="pass__question-meta">
            <Badge variant="accent">
              {currentQuestion.type === 'Open'
                ? 'Открытый'
                : currentQuestion.type === 'SingleChoice'
                  ? 'Один вариант'
                  : 'Несколько вариантов'}
            </Badge>
            <span className="pass__points">{currentQuestion.points} б.</span>
          </div>
          <h2 className="pass__question-text">{currentQuestion.text}</h2>

          {currentQuestion.type === 'Open' ? (
            <Textarea
              label="Ваш ответ"
              value={textAnswer}
              onChange={(e) => setTextAnswer(e.target.value)}
              rows={4}
              placeholder="Введите ответ"
            />
          ) : (
            <div className="pass__answers">
              {currentQuestion.answers.map((answer) => {
                const checked = selectedAnswers.includes(answer.id);
                return (
                  <button
                    key={answer.id}
                    type="button"
                    className={`pass__answer ${checked ? 'pass__answer--selected' : ''}`}
                    onClick={() => toggleAnswer(answer.id)}
                  >
                    <span className="pass__answer-check" aria-hidden="true">
                      {currentQuestion.type === 'SingleChoice' ? (
                        <span className="pass__radio" />
                      ) : (
                        <span className="pass__checkbox" />
                      )}
                    </span>
                    <span className="pass__answer-text">{answer.text}</span>
                  </button>
                );
              })}
            </div>
          )}
        </Card>

        <div className="pass__actions">
          <Button
            variant="secondary"
            disabled={currentIndex === 0}
            onClick={() => {
              setCurrentIndex((i) => i - 1);
              resetAnswer();
            }}
          >
            Назад
          </Button>
          <Button loading={saving || completing} onClick={handleNext}>
            {isLast ? 'Завершить' : 'Далее'}
          </Button>
        </div>
      </div>
    </AppLayout>
  );
}
