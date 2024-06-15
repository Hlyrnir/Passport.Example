using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.ConfirmPhoneNumber
{
	internal sealed class ConfirmPhoneNumberCommandHandler : ICommandHandler<ConfirmPhoneNumberCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportHolderRepository repoHolder;
		private readonly IPassportSetting ppSetting;

		public ConfirmPhoneNumberCommandHandler(
			ITimeProvider prvTime,
			IPassportHolderRepository repoHolder,
			IPassportSetting ppSetting)
		{
			this.prvTime = prvTime;
			this.repoHolder = repoHolder;
			this.ppSetting = ppSetting;
		}

		public async ValueTask<IMessageResult<bool>> Handle(ConfirmPhoneNumberCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportHolder> rsltHolder = await repoHolder.FindByIdAsync(msgMessage.PassportHolderId, tknCancellation);

			return await rsltHolder.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppHolder =>
				{
                    if (ppHolder.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
                        return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

                    if (ppHolder.PhoneNumberIsConfirmed == false)
					{
						if (ppHolder.TryConfirmPhoneNumber(msgMessage.PhoneNumber, ppSetting) == false)
							return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Phone number could not be confirmed." });

						IRepositoryResult<bool> rsltUpdate = await repoHolder.UpdateAsync(ppHolder, prvTime.GetUtcNow(), tknCancellation);

						return rsltUpdate.Match(
							msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
							bResult => new MessageResult<bool>(bResult));
					}

					return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Phone number is already confirmed." });
				});
		}
	}
}