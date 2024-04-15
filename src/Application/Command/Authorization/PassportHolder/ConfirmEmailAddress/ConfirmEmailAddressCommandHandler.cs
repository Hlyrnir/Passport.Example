using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.ConfirmEmailAddress
{
	internal sealed class ConfirmEmailAddressCommandHandler : ICommandHandler<ConfirmEmailAddressCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportHolderRepository repoHolder;
		private readonly IPassportSetting ppSetting;

		public ConfirmEmailAddressCommandHandler(
			ITimeProvider prvTime,
			IPassportHolderRepository repoHolder,
			IPassportSetting ppSetting)
		{
			this.prvTime = prvTime;
			this.repoHolder = repoHolder;
			this.ppSetting = ppSetting;
		}

		public async ValueTask<IMessageResult<bool>> Handle(ConfirmEmailAddressCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportHolder> rsltHolder = await repoHolder.FindByIdAsync(msgMessage.PassportHolderId, tknCancellation);

			return await rsltHolder.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppHolder =>
				{
					if (ppHolder.EmailAddressIsConfirmed == false)
					{
						if (ppHolder.TryConfirmEmailAddress(msgMessage.EmailAddress, ppSetting) == false)
							return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Email address could not be confirmed." });

						IRepositoryResult<bool> rsltUpdate = await repoHolder.UpdateAsync(ppHolder, prvTime.GetUtcNow(), tknCancellation);

						return rsltUpdate.Match(
							msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
							bResult => new MessageResult<bool>(bResult));
					}

					return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Email address is already confirmed." });
				});
		}
	}
}