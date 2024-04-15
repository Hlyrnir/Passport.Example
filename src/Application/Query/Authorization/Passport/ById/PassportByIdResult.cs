using Domain.Interface.Authorization;

namespace Application.Query.Authorization.Passport.ById
{
	public sealed class PassportByIdResult
	{
		public required IPassport Passport { get; init; }
	}
}
