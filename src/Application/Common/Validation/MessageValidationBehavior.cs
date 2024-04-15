using Application.Common.Result.Message;
using Application.Interface.Result;
using Application.Interface.Validation;
using Mediator;

namespace Application.Common.Validation
{
    internal sealed class MessageValidationBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, IMessageResult<TResponse>>
        where TMessage : notnull, IMessage
    {
        private readonly IValidation<TMessage> msgValidation;

        public MessageValidationBehaviour(IValidation<TMessage> msgValidation)
        {
            this.msgValidation = msgValidation;
        }

        public async ValueTask<IMessageResult<TResponse>> Handle(TMessage msgMessage, CancellationToken tknCancellation, MessageHandlerDelegate<TMessage, IMessageResult<TResponse>> dlgMessageHandler)
        {
            IMessageResult<bool> msgResult = await msgValidation.ValidateAsync(msgMessage, tknCancellation);

            return await msgResult.MatchAsync(
                msgError => new MessageResult<TResponse>(msgError),
                async bResult =>
                {
                    return await dlgMessageHandler(msgMessage, tknCancellation);
                });
        }
    }
}
