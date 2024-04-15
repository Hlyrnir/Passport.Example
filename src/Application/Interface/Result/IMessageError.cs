namespace Application.Interface.Result
{
	public interface IMessageError
	{
		string? Code { get; init; }
		string? Description { get; init; }
	}
}