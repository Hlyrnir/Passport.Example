using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.Authorization.PassportVisa.ByPassport
{
	public sealed class PassportVisaByPassportIdQuery : IQuery<IMessageResult<PassportVisaByPassportIdResult>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PassportIdToFind { get; init; }
	}
}
