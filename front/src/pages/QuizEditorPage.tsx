import { useCallback, useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
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
  getQuiz,
  updateQuiz,
  publishQuiz,
  getQuestions,
  addQuestion,
  updateQuestion,
  deleteQuestion,
  addAnswer,
  updateAnswer,
  removeAnswer,
  type Quiz,
  type Question,
  type QuestionType,
} from '../api/quizzes';
import './quiz-editor.css';

const QUESTION_TYPES: { value: QuestionType; label: string }[] = [
  { value: 'Open', label: 'Открытый' },
  { value: 'SingleChoice', label: 'Один вариант' },
  { value: 'MultipleChoice', label: 'Несколько вариантов' },
];

interface QuestionDraft {
  id: string | null;
  text: string;
  type: QuestionType;
  points: number;
  answers: AnswerDraft[];
}

interface AnswerDraft {
  id: string | null;
  text: string;
  isCorrect: boolean;
}

export function QuizEditorPage() {
  const { id } = useParams<{ id: string }>();

  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState(false);

  const [publishing, setPublishing] = useState(false);

  const [questionModalOpen, setQuestionModalOpen] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<QuestionDraft | null>(null);
  const [questionSaving, setQuestionSaving] = useState(false);
  const [questionError, setQuestionError] = useState<string | null>(null);

  const [deleteQuestionTarget, setDeleteQuestionTarget] = useState<Question | null>(null);
  const [deletingQuestion, setDeletingQuestion] = useState(false);

  const load = useCallback(async () => {
    if (!id) return;
    try {
      const [quizData, questionsData] = await Promise.all([
        getQuiz(id),
        getQuestions(id, 1, 100),
      ]);
      setQuiz(quizData);
      setTitle(quizData.title);
      setDescription(quizData.description);
      setQuestions(questionsData.items);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось загрузить квиз');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    load();
  }, [load]);

  const handleSave = async () => {
    if (!id) return;
    if (!title.trim()) {
      setSaveError('Введите название квиза');
      return;
    }
    setSaving(true);
    setSaveError(null);
    setSaveSuccess(false);
    try {
      await updateQuiz(id, { title: title.trim(), description: description.trim() });
      setSaveSuccess(true);
      await load();
    } catch (e) {
      setSaveError(e instanceof Error ? e.message : 'Не удалось сохранить квиз');
    } finally {
      setSaving(false);
    }
  };

  const handlePublish = async () => {
    if (!id) return;
    setPublishing(true);
    setSaveError(null);
    try {
      await publishQuiz(id);
      await load();
    } catch (e) {
      setSaveError(e instanceof Error ? e.message : 'Не удалось опубликовать квиз');
    } finally {
      setPublishing(false);
    }
  };

  const openNewQuestion = () => {
    setEditingQuestion({
      id: null,
      text: '',
      type: 'SingleChoice',
      points: 1,
      answers: [
        { id: null, text: '', isCorrect: false },
        { id: null, text: '', isCorrect: false },
      ],
    });
    setQuestionError(null);
    setQuestionModalOpen(true);
  };

  const openEditQuestion = (q: Question) => {
    setEditingQuestion({
      id: q.id,
      text: q.text,
      type: q.type,
      points: q.points,
      answers: q.answers.map((a) => ({ id: a.id, text: a.text, isCorrect: a.isCorrect })),
    });
    setQuestionError(null);
    setQuestionModalOpen(true);
  };

  const handleSaveQuestion = async () => {
    if (!id || !editingQuestion) return;
    if (!editingQuestion.text.trim()) {
      setQuestionError('Введите текст вопроса');
      return;
    }
    if (editingQuestion.type !== 'Open') {
      const validAnswers = editingQuestion.answers.filter((a) => a.text.trim());
      if (validAnswers.length < 2) {
        setQuestionError('Добавьте минимум два варианта ответа');
        return;
      }
      if (!validAnswers.some((a) => a.isCorrect)) {
        setQuestionError('Отметьте хотя бы один правильный вариант');
        return;
      }
    }

    setQuestionSaving(true);
    setQuestionError(null);
    try {
      if (editingQuestion.id) {
        await updateQuestion(id, editingQuestion.id, {
          text: editingQuestion.text.trim(),
          type: editingQuestion.type,
          order: questions.findIndex((q) => q.id === editingQuestion.id) + 1,
          points: editingQuestion.points,
        });
        // Обновляем ответы: удаляем отсутствующие, добавляем новые, обновляем существующие
        const existing = questions.find((q) => q.id === editingQuestion.id);
        const existingAnswers = existing?.answers ?? [];
        const keptIds = editingQuestion.answers
          .filter((a) => a.id)
          .map((a) => a.id as string);

        for (const ans of existingAnswers) {
          if (!keptIds.includes(ans.id)) {
            await removeAnswer(id, editingQuestion.id, ans.id);
          }
        }
        for (const ans of editingQuestion.answers) {
          if (ans.id) {
            await updateAnswer(id, editingQuestion.id, ans.id, {
              text: ans.text.trim(),
              isCorrect: ans.isCorrect,
            });
          } else if (ans.text.trim()) {
            await addAnswer(id, editingQuestion.id, {
              text: ans.text.trim(),
              isCorrect: ans.isCorrect,
            });
          }
        }
      } else {
        const newQuestionId = await addQuestion(id, {
          text: editingQuestion.text.trim(),
          type: editingQuestion.type,
          order: questions.length + 1,
          points: editingQuestion.points,
        });
        for (const ans of editingQuestion.answers) {
          if (ans.text.trim()) {
            await addAnswer(id, newQuestionId, {
              text: ans.text.trim(),
              isCorrect: ans.isCorrect,
            });
          }
        }
      }
      setQuestionModalOpen(false);
      await load();
    } catch (e) {
      setQuestionError(e instanceof Error ? e.message : 'Не удалось сохранить вопрос');
    } finally {
      setQuestionSaving(false);
    }
  };

  const handleDeleteQuestion = async () => {
    if (!id || !deleteQuestionTarget) return;
    setDeletingQuestion(true);
    try {
      await deleteQuestion(id, deleteQuestionTarget.id);
      setDeleteQuestionTarget(null);
      await load();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Не удалось удалить вопрос');
    } finally {
      setDeletingQuestion(false);
    }
  };

  if (loading) {
    return (
      <AppLayout>
        <div className="editor__loading">
          <Spinner size="lg" label="Загрузка квиза…" />
        </div>
      </AppLayout>
    );
  }

  if (error && !quiz) {
    return (
      <AppLayout>
        <div className="editor__error">
          <Alert variant="error">{error}</Alert>
          <Link to="/quizzes" className="editor__back">
            ← К списку квизов
          </Link>
        </div>
      </AppLayout>
    );
  }

  return (
    <AppLayout>
      <div className="editor">
        <div className="editor__topbar">
          <Link to="/quizzes" className="editor__back">
            ← К списку квизов
          </Link>
          <div className="editor__topbar-actions">
            {quiz?.status === 'Published' ? (
              <Badge variant="success">Опубликован</Badge>
            ) : (
              <Badge variant="warning">Черновик</Badge>
            )}
            {quiz?.status === 'Draft' && (
              <Button variant="secondary" loading={publishing} onClick={handlePublish}>
                Опубликовать
              </Button>
            )}
          </div>
        </div>

        <h1 className="editor__title">Редактирование квиза</h1>

        {saveError && (
          <div className="editor__alert">
            <Alert variant="error">{saveError}</Alert>
          </div>
        )}
        {saveSuccess && (
          <div className="editor__alert">
            <Alert variant="success">Изменения сохранены</Alert>
          </div>
        )}

        <Card className="editor__meta">
          <h2 className="editor__section-title">Основная информация</h2>
          <div className="editor__meta-fields">
            <Input
              label="Название"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Название квиза"
            />
            <Textarea
              label="Описание"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Описание квиза"
              rows={3}
            />
          </div>
          <div className="editor__meta-actions">
            <Button loading={saving} onClick={handleSave}>
              Сохранить
            </Button>
          </div>
        </Card>

        <div className="editor__questions-header">
          <h2 className="editor__section-title">Вопросы</h2>
          <Button variant="secondary" onClick={openNewQuestion}>
            Добавить вопрос
          </Button>
        </div>

        {questions.length === 0 ? (
          <Card className="editor__empty">
            <p className="editor__empty-text">
              Вопросов пока нет. Добавьте первый вопрос, чтобы наполнить квиз.
            </p>
            <Button onClick={openNewQuestion}>Добавить вопрос</Button>
          </Card>
        ) : (
          <div className="editor__questions">
            {questions.map((q, index) => (
              <Card key={q.id} className="question-card">
                <div className="question-card__header">
                  <span className="question-card__num">Вопрос {index + 1}</span>
                  <Badge variant="default">
                    {QUESTION_TYPES.find((t) => t.value === q.type)?.label ?? q.type}
                  </Badge>
                  <span className="question-card__points">{q.points} б.</span>
                </div>
                <p className="question-card__text">{q.text}</p>
                {q.type !== 'Open' && (
                  <ul className="question-card__answers">
                    {q.answers.map((a) => (
                      <li
                        key={a.id}
                        className={`question-card__answer ${
                          a.isCorrect ? 'question-card__answer--correct' : ''
                        }`}
                      >
                        {a.text}
                        {a.isCorrect && <span className="question-card__correct-mark">✓</span>}
                      </li>
                    ))}
                  </ul>
                )}
                <div className="question-card__actions">
                  <button
                    type="button"
                    className="question-card__action"
                    onClick={() => openEditQuestion(q)}
                  >
                    Редактировать
                  </button>
                  <button
                    type="button"
                    className="question-card__action question-card__action--danger"
                    onClick={() => setDeleteQuestionTarget(q)}
                  >
                    Удалить
                  </button>
                </div>
              </Card>
            ))}
          </div>
        )}
      </div>

      <Modal
        open={questionModalOpen}
        title={editingQuestion?.id ? 'Редактировать вопрос' : 'Новый вопрос'}
        onClose={() => setQuestionModalOpen(false)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setQuestionModalOpen(false)}>
              Отмена
            </Button>
            <Button loading={questionSaving} onClick={handleSaveQuestion}>
              Сохранить
            </Button>
          </>
        }
      >
        {editingQuestion && (
          <div className="editor__question-form">
            {questionError && <Alert variant="error">{questionError}</Alert>}
            <Textarea
              label="Текст вопроса"
              value={editingQuestion.text}
              onChange={(e) =>
                setEditingQuestion({ ...editingQuestion, text: e.target.value })
              }
              rows={2}
            />
            <div className="editor__question-row">
              <div className="editor__question-type">
                <label className="field__label">Тип вопроса</label>
                <select
                  className="editor__select"
                  value={editingQuestion.type}
                  onChange={(e) =>
                    setEditingQuestion({
                      ...editingQuestion,
                      type: e.target.value as QuestionType,
                    })
                  }
                >
                  {QUESTION_TYPES.map((t) => (
                    <option key={t.value} value={t.value}>
                      {t.label}
                    </option>
                  ))}
                </select>
              </div>
              <div className="editor__question-points">
                <Input
                  label="Баллы"
                  type="number"
                  min={1}
                  value={String(editingQuestion.points)}
                  onChange={(e) =>
                    setEditingQuestion({
                      ...editingQuestion,
                      points: Number(e.target.value) || 1,
                    })
                  }
                />
              </div>
            </div>

            {editingQuestion.type !== 'Open' && (
              <div className="editor__answers">
                <label className="field__label">Варианты ответа</label>
                {editingQuestion.answers.map((ans, i) => (
                  <div key={i} className="editor__answer-row">
                    <input
                      type="checkbox"
                      className="editor__answer-check"
                      checked={ans.isCorrect}
                      onChange={(e) => {
                        const answers = [...editingQuestion.answers];
                        if (editingQuestion.type === 'SingleChoice') {
                          answers.forEach((a, idx) => {
                            answers[idx] = { ...a, isCorrect: idx === i };
                          });
                        } else {
                          answers[i] = { ...ans, isCorrect: e.target.checked };
                        }
                        setEditingQuestion({ ...editingQuestion, answers });
                      }}
                      aria-label="Правильный ответ"
                    />
                    <input
                      type="text"
                      className="editor__answer-input"
                      value={ans.text}
                      placeholder={`Вариант ${i + 1}`}
                      onChange={(e) => {
                        const answers = [...editingQuestion.answers];
                        answers[i] = { ...ans, text: e.target.value };
                        setEditingQuestion({ ...editingQuestion, answers });
                      }}
                    />
                    <button
                      type="button"
                      className="editor__answer-remove"
                      onClick={() => {
                        const answers = editingQuestion.answers.filter((_, idx) => idx !== i);
                        setEditingQuestion({ ...editingQuestion, answers });
                      }}
                      aria-label="Удалить вариант"
                    >
                      ×
                    </button>
                  </div>
                ))}
                <Button
                  variant="ghost"
                  onClick={() =>
                    setEditingQuestion({
                      ...editingQuestion,
                      answers: [...editingQuestion.answers, { id: null, text: '', isCorrect: false }],
                    })
                  }
                >
                  + Добавить вариант
                </Button>
              </div>
            )}
          </div>
        )}
      </Modal>

      <Modal
        open={Boolean(deleteQuestionTarget)}
        title="Удалить вопрос"
        onClose={() => setDeleteQuestionTarget(null)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setDeleteQuestionTarget(null)}>
              Отмена
            </Button>
            <Button loading={deletingQuestion} onClick={handleDeleteQuestion}>
              Удалить
            </Button>
          </>
        }
      >
        <p className="editor__confirm">
          Удалить вопрос «{deleteQuestionTarget?.text}»? Это действие необратимо.
        </p>
      </Modal>
    </AppLayout>
  );
}
