namespace Application.Interface.Result
{
    public interface IRepositoryError
    {
        string Code { get; init; }
        string Description { get; init; }
    }
}