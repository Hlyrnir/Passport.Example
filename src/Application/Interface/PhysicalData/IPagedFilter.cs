namespace Application.Interface.PhysicalData
{
    public interface IPagedFilter
    {
        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}
