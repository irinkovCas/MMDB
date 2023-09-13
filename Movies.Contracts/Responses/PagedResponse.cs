namespace Movies.Contracts.Responses; 

public class PagedResponse<TResponse> {
   public IEnumerable<TResponse> Items { get; set; } = Enumerable.Empty<TResponse>();
   public int Page { get; set; }
   public int PageSize { get; set; }
   public int Total { get; set; }
   public bool HasNextPage => Total > (Page * PageSize);
}