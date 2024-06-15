using Application.Interface.Message;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.Create
{
    public sealed class CreatePassportTokenCommand : ICommand<IMessageResult<Guid>>, IRestrictedAuthorization
    {
        public required Guid RestrictedPassportId { get; init; }

        public required IPassportCredential CredentialToVerify { get; init; }
        public required IPassportCredential CredentialToAdd { get; init; }
    }
}
