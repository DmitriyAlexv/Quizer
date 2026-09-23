namespace Quizer.Controllers.Contracts.Common;

/// <summary>
/// Ответ с пагинацией.
/// </summary>
public record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
