namespace EsperancaSolidaria.BuildingBlocks.Queries;

/// <summary>
/// Resultado paginado para consultas que retornam listas de itens.
/// </summary>
public class PaginatedResult<T>
{
    public int PageSize { get; set; }
    public int Page { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public PaginatedResult(int page, int pageSize, int totalItems, IEnumerable<T> items)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
    }
}
