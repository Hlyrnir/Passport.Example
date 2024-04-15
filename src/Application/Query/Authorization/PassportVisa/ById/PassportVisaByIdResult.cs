using Domain.Interface.Authorization;

namespace Application.Query.Authorization.PassportVisa.ById
{
	public sealed class PassportVisaByIdResult
	{
		public required IPassportVisa PassportVisa { get; init; }
	}
}
