using Domain.Interface.Authorization;

namespace Application.Query.Authorization.PassportHolder.ById
{
	public sealed class PassportHolderByIdResult
	{
		public required IPassportHolder PassportHolder { get; init; }
	}
}
