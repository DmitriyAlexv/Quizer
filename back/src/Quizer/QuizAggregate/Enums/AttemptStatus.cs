namespace Quizer.QuizAggregate.Enums;

/// <summary>
/// Статус попытки прохождения квиза.
/// </summary>
public enum AttemptStatus
{
    /// <summary>
    /// В процессе прохождения.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Завершена.
    /// </summary>
    Completed = 2,
}
