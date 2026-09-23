import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AppLayout } from '../components/AppLayout';
import { Button } from '../components/common/Button';
import { Badge } from '../components/common/Badge';
import { Card } from '../components/common/Card';
import { Spinner } from '../components/common/Spinner';
import { Alert } from '../components/common/Alert';
import { Modal } from '../components/common/Modal';
import { Input } from '../components/common/Input';
import { Textarea } from '../components/common/Textarea';
import {
  getQuizzes,
  createQuiz,
  deleteQuiz,
  publishQuiz,
  type Quiz,
} from '../api/quizzes';
import { formatDate } from '../utils/format';
import './quizzes.css';

const PAGE_SIZE = 9;

export function QuizzesPage() {
  const navigate = useNavigate();
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [createOpen, setCreateOpen] = useState(false);
  const [createTitle, setCreateTitle] = useState('');
  const [createDescription, setCreateDescription] = useState('');
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);

  const [deleteTarget, setDeleteTarget] = useState<Quiz | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const [publishingId, setPublishingId] = useState<string | null>(null);

  const load = useCallback(async (targetPage: number) => {
    try {
      const data = await getQuizzes(targetPage, PAGE_SIZE);
      setQuizzes(data.items);
      setPage(data.page);
      setTotalPages(data.totalPages);
      setTotalCount(data.totalCount);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось загрузить квизы');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load(page);
  }, [load, page]);

  const handleCreate = async () => {
    if (!createTitle.trim()) {
      setCreateError('Введите название квиза');
      return;
    }
    setCreating(true);
    setCreateError(null);
    try {
      const id = await createQuiz({
        title: createTitle.trim(),
        description: createDescription.trim(),
      });
      setCreateOpen(false);
      setCreateTitle('');
      setCreateDescription('');
      navigate(`/quizzes/${id}/edit`);
    } catch (e) {
      setCreateError(e instanceof Error ? e.message : 'Не удалось создать квиз');
    } finally {
      setCreating(false);
    }
  };

  const handlePublish = async (quiz: Quiz) => {
    setPublishingId(quiz.id);
    setError(null);
    try {
      await publishQuiz(quiz.id);
      await load(page);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось опубликовать квиз');
    } finally {
      setPublishingId(null);
    }
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      await deleteQuiz(deleteTarget.id);
      setDeleteTarget(null);
      await load(page);
    } catch (e) {
      setDeleteError(e instanceof Error ? e.message : 'Не удалось удалить квиз');
    } finally {
      setDeleting(false);
    }
  };

  return (
    <AppLayout>
      <div className="quizzes">
        <div className="quizzes__header">
          <div>
            <h1 className="quizzes__title">Квизы</h1>
            <p className="quizzes__subtitle">
              {totalCount > 0 ? `Всего квизов: ${totalCount}` : 'Создайте свой первый квиз'}
            </p>
          </div>
          <Button onClick={() => setCreateOpen(true)}>Создать квиз</Button>
        </div>

        {error && (
          <div className="quizzes__alert">
            <Alert variant="error">{error}</Alert>
          </div>
        )}

        {loading ? (
          <div className="quizzes__loading">
            <Spinner size="lg" label="Загрузка квизов…" />
          </div>
        ) : quizzes.length === 0 ? (
          <Card className="quizzes__empty">
            <h2 className="quizzes__empty-title">Пока нет квизов</h2>
            <p className="quizzes__empty-text">
              Создайте первый квиз, чтобы начать собирать вопросы и делиться ими.
            </p>
            <Button onClick={() => setCreateOpen(true)}>Создать квиз</Button>
          </Card>
        ) : (
          <>
            <div className="quizzes__grid">
              {quizzes.map((quiz) => (
                <Card key={quiz.id} className="quiz-card">
                  <div className="quiz-card__top">
                    <Badge variant={quiz.status === 'Published' ? 'success' : 'warning'}>
                      {quiz.status === 'Published' ? 'Опубликован' : 'Черновик'}
                    </Badge>
                    <span className="quiz-card__date">{formatDate(quiz.createdAt)}</span>
                  </div>
                  <h2 className="quiz-card__title">{quiz.title}</h2>
                  <p className="quiz-card__desc">
                    {quiz.description || 'Описание отсутствует'}
                  </p>
                  <div className="quiz-card__actions">
                    <Link to={`/quizzes/${quiz.id}`} className="quiz-card__link">
                      Открыть
                    </Link>
                    {quiz.status === 'Published' && (
                      <Link to={`/quizzes/${quiz.id}/pass`} className="quiz-card__link">
                        Пройти
                      </Link>
                    )}
                    <Link to={`/quizzes/${quiz.id}/edit`} className="quiz-card__link">
                      Редактировать
                    </Link>
                    {quiz.status === 'Draft' && (
                      <button
                        type="button"
                        className="quiz-card__link quiz-card__link--accent"
                        disabled={publishingId === quiz.id}
                        onClick={() => handlePublish(quiz)}
                      >
                        {publishingId === quiz.id ? 'Публикация…' : 'Опубликовать'}
                      </button>
                    )}
                    <button
                      type="button"
                      className="quiz-card__link quiz-card__link--danger"
                      onClick={() => setDeleteTarget(quiz)}
                    >
                      Удалить
                    </button>
                  </div>
                </Card>
              ))}
            </div>

            {totalPages > 1 && (
              <div className="quizzes__pagination">
                <Button
                  variant="secondary"
                  disabled={page <= 1}
                  onClick={() => setPage((p) => p - 1)}
                >
                  Назад
                </Button>
                <span className="quizzes__pagination-info">
                  Страница {page} из {totalPages}
                </span>
                <Button
                  variant="secondary"
                  disabled={page >= totalPages}
                  onClick={() => setPage((p) => p + 1)}
                >
                  Вперёд
                </Button>
              </div>
            )}
          </>
        )}
      </div>

      <Modal
        open={createOpen}
        title="Новый квиз"
        onClose={() => setCreateOpen(false)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setCreateOpen(false)}>
              Отмена
            </Button>
            <Button loading={creating} onClick={handleCreate}>
              Создать
            </Button>
          </>
        }
      >
        <div className="quizzes__form">
          {createError && <Alert variant="error">{createError}</Alert>}
          <Input
            label="Название"
            value={createTitle}
            onChange={(e) => setCreateTitle(e.target.value)}
            placeholder="Например: Основы JavaScript"
          />
          <Textarea
            label="Описание"
            value={createDescription}
            onChange={(e) => setCreateDescription(e.target.value)}
            placeholder="Кратко опишите, о чём этот квиз"
            rows={3}
          />
        </div>
      </Modal>

      <Modal
        open={Boolean(deleteTarget)}
        title="Удалить квиз"
        onClose={() => setDeleteTarget(null)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setDeleteTarget(null)}>
              Отмена
            </Button>
            <Button variant="primary" loading={deleting} onClick={handleDelete}>
              Удалить
            </Button>
          </>
        }
      >
        <div className="quizzes__form">
          {deleteError && <Alert variant="error">{deleteError}</Alert>}
          <p className="quizzes__confirm">
            Вы действительно хотите удалить квиз «{deleteTarget?.title}»? Это действие
            необратимо.
          </p>
        </div>
      </Modal>
    </AppLayout>
  );
}
