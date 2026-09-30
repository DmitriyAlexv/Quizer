using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными вопроса.
/// </summary>
/// <param name="Id">Идентификатор вопроса.</param>
/// <param name="Text">Текст вопроса.</param>
/// <param name="Type">Тип вопроса.</param>
/// <param name="Order">Порядковый номер вопроса в квизе.</param>
/// <param name="Points">Количество баллов за вопрос.</param>
/// <param name="Answers">Список вариантов ответа.</param>
public record QuestionResponse(
    Guid Id,
    string Text,
    QuestionType Type,
    int Order,
    int Points,
    IReadOnlyCollection<AnswerResponse> Answers);
