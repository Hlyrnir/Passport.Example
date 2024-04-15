using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.Authorization.PassportVisa.ById
{
	public sealed class PassportVisaByIdQuery : IQuery<IMessageResult<PassportVisaByIdResult>>, IRestrictedAuthorization
    {
        public required Guid RestrictedPassportId { get; init; }
        public required Guid PassportVisaId { get; init; }
    }
}
