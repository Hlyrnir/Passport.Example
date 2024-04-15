namespace Contract.v01.Common
{
	public class PagedResponse
	{
		public required int Page { get; init; }
		public required int PageSize { get; init; }
		public required int ResultCount { get; init; }
	}
}
