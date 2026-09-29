using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Запрос на добавление вопроса в квиз.
/// </summary>
/// <param name="Text">Текст вопроса.</param>
/// <param name="Type">Тип вопроса.</param>
/// <param name="Order">Порядковый номер вопроса в квизе.</param>
/// <param name="Points">Количество баллов за вопрос.</param>
public record AddQuestionRequest(string Text, QuestionType Type, int Order, int Points);
