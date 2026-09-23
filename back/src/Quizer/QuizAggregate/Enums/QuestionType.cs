namespace Quizer.QuizAggregate.Enums;

/// <summary>
/// Тип вопроса.
/// </summary>
public enum QuestionType
{
    /// <summary>
    /// Вопрос с открытым ответом.
    /// </summary>
    Open = 1,

    /// <summary>
    /// Вопрос с одним вариантом ответа.
    /// </summary>
    SingleChoice = 2,

    /// <summary>
    /// Вопрос с несколькими вариантами ответа.
    /// </summary>
    MultipleChoice = 3,
}
