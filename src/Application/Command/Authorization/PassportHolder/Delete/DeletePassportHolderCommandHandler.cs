using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.Delete
{
    internal sealed class DeletePassportHolderCommandHandler : ICommandHandler<DeletePassportHolderCommand, IMessageResult<bool>>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportHolderRepository repoHolder;

        public DeletePassportHolderCommandHandler(
            ITimeProvider prvTime,
            IPassportHolderRepository repoHolder)
        {
            this.prvTime = prvTime;
            this.repoHolder = repoHolder;
        }

        public async ValueTask<IMessageResult<bool>> Handle(DeletePassportHolderCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            IRepositoryResult<IPassportHolder> rsltPassport = await repoHolder.FindByIdAsync(msgMessage.PassportHolderId, tknCancellation);

            return await rsltPassport.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async ppHolder =>
                {
                    IRepositoryResult<bool> rsltDelete = await repoHolder.DeleteAsync(ppHolder, tknCancellation);

                    return rsltDelete.Match(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<bool>(bResult));
                });
        }
    }
}