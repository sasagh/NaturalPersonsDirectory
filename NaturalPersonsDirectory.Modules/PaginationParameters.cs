namespace NaturalPersonsDirectory.Modules
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        private string _orderBy;
        public string OrderBy
        {
            get => _orderBy ?? "Id";
            set => _orderBy = value;
        }
    }
}
