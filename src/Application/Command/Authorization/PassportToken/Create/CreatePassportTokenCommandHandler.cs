using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.Create
{
    internal sealed class CreatePassportTokenCommandHandler : ICommandHandler<CreatePassportTokenCommand, IMessageResult<Guid>>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportTokenRepository repoToken;

        public CreatePassportTokenCommandHandler(
            ITimeProvider prvTime,
            IPassportTokenRepository repoToken)
        {
            this.prvTime = prvTime;
            this.repoToken = repoToken;
        }

        public async ValueTask<IMessageResult<Guid>> Handle(CreatePassportTokenCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

            IRepositoryResult<IPassportToken> rsltToken = await repoToken.FindTokenByCredentialAsync(msgMessage.CredentialToVerify, prvTime.GetUtcNow(), tknCancellation);

            return await rsltToken.MatchAsync(
                msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async ppTokenInRepository =>
                {
                    if (ppTokenInRepository.Provider == msgMessage.CredentialToAdd.Provider)
                        return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = $"Token at provider {msgMessage.CredentialToAdd.Provider} does already exist." });

                    IPassportToken? ppToken = Domain.Aggregate.Authorization.PassportToken.PassportToken.Create(
                        guPassportId: ppTokenInRepository.PassportId,
                        sProvider: msgMessage.CredentialToAdd.Provider,
                        bTwoFactorIsEnabled: false);

                    if (ppToken is null)
                        return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Token has not been created." });

                    IRepositoryResult<bool> rsltTokenId = await repoToken.InsertAsync(ppToken, msgMessage.CredentialToAdd, prvTime.GetUtcNow(), tknCancellation);

                    return rsltTokenId.Match(
                        msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<Guid>(ppToken.Id));
                });
        }
    }
}
