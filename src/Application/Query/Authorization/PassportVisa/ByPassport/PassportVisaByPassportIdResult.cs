using Domain.Interface.Authorization;

namespace Application.Query.Authorization.PassportVisa.ByPassport
{
	public sealed class PassportVisaByPassportIdResult
	{
		public required IEnumerable<IPassportVisa> PassportVisa { get; init; }
	}
}
