namespace act_ms_transaction.Api.DTOs.Parameters
{
    public class PaginationQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? OrderBy { get; set; }
        public bool Asc { get; set; } = true;
    }
}
