using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Authorization;
using Application.Interface.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Common.Authorization
{
    internal sealed class MessageAuthorizationBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, IMessageResult<TResponse>>
        where TMessage : notnull, IMessage, IRestrictedAuthorization
    {
        private readonly IAuthorization<TMessage> msgAuthorization;
        private readonly ITimeProvider prvTime;
        private readonly IPassportRepository repoPassport;

        public MessageAuthorizationBehaviour(
            IAuthorization<TMessage> msgAuthorization,
            ITimeProvider prvTime,
            IPassportRepository repoPassport)
        {
            this.msgAuthorization = msgAuthorization;
            this.prvTime = prvTime;
            this.repoPassport = repoPassport;
        }

        public async ValueTask<IMessageResult<TResponse>> Handle(TMessage msgMessage, CancellationToken tknCancellation, MessageHandlerDelegate<TMessage, IMessageResult<TResponse>> dlgMessageHandler)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<TResponse>(DefaultMessageError.TaskAborted);

            IRepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(msgMessage.RestrictedPassportId, tknCancellation);

            return await rsltPassport.MatchAsync(
                msgError => new MessageResult<TResponse>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async ppPassport =>
                {
                    if (msgMessage is IVerifiedAuthorization)
                    {
                        if (ppPassport.IsAuthority == false)
                            return new MessageResult<TResponse>(new MessageError() { Code = AuthorizationError.Code.Method, Description = "Passport has not been approved for this request." });
                    }

					if (ppPassport.IsEnabled == false)
                        return new MessageResult<TResponse>(new MessageError() { Code = AuthorizationError.Code.Method, Description = "Passport is not enabled." });

                    if (ppPassport.IsExpired(prvTime.GetUtcNow()) == true)
                        return new MessageResult<TResponse>(new MessageError() { Code = AuthorizationError.Code.Method, Description = "Passport is expired." });

                    IMessageResult<bool> rsltAuthorization = await msgAuthorization.AuthorizeAsync(msgMessage, ppPassport.VisaId, tknCancellation);

                    return await rsltAuthorization.MatchAsync(
                        msgError => new MessageResult<TResponse>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        async bResult =>
                        {
                            return await dlgMessageHandler(msgMessage, tknCancellation);
                        });
                });
        }
	}
}