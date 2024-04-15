using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.Authorization.Passport.ById
{
	public sealed class PassportByIdQuery : IQuery<IMessageResult<PassportByIdResult>>, IRestrictedAuthorization
    {
        public required Guid RestrictedPassportId { get; init; }
        public required Guid PassportId { get; init; }
    }
}
