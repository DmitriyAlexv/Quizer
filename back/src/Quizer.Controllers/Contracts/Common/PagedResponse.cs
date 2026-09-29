namespace Quizer.Controllers.Contracts.Common;

/// <summary>
/// Ответ с пагинацией.
/// </summary>
/// <param name="Items">Коллекция элементов текущей страницы.</param>
/// <param name="TotalCount">Общее количество элементов.</param>
/// <param name="Page">Номер текущей страницы (начиная с 1).</param>
/// <param name="PageSize">Количество элементов на странице.</param>
/// <param name="TotalPages">Общее количество страниц.</param>
/// <param name="HasPreviousPage">Указывает, существует ли предыдущая страница.</param>
/// <param name="HasNextPage">Указывает, существует ли следующая страница.</param>
public record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
