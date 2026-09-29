using Quizer.QuizAggregate.Enums;

namespace Quizer.Controllers.Contracts.Quizzes.V1;

/// <summary>
/// Ответ с данными квиза.
/// </summary>
/// <param name="Id">Идентификатор квиза.</param>
/// <param name="Title">Название квиза.</param>
/// <param name="Description">Описание квиза.</param>
/// <param name="OwnerId">Идентификатор владельца квиза.</param>
/// <param name="Status">Статус квиза.</param>
/// <param name="CreatedAt">Дата и время создания квиза.</param>
/// <param name="UpdatedAt">Дата и время последнего обновления квиза (null, если квиз не обновлялся).</param>
public record QuizResponse(
    Guid Id,
    string Title,
    string Description,
    Guid OwnerId,
    QuizStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
