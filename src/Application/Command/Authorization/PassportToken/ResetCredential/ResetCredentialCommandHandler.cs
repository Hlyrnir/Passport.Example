using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.UnitOfWork;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.ResetCredential
{
	internal sealed class ResetCredentialCommandHandler : ICommandHandler<ResetCredentialCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;

		private readonly IUnitOfWork<IPassportDataAccess> uowUnitOfWork;

		private readonly IPassportSetting ppSetting;

		private readonly IPassportRepository repoPassport;
		private readonly IPassportTokenRepository repoToken;

		public ResetCredentialCommandHandler(
			ITimeProvider prvTime,
			IUnitOfWork<IPassportDataAccess> uowUnitOfWork,
			IPassportSetting ppSetting,
			IPassportRepository repoPassport,
			IPassportTokenRepository repoToken)
		{
			this.prvTime = prvTime;
			this.uowUnitOfWork = uowUnitOfWork;
			this.ppSetting = ppSetting;
			this.repoPassport = repoPassport;
			this.repoToken = repoToken;
		}

		public async ValueTask<IMessageResult<bool>> Handle(ResetCredentialCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportToken> rsltToken = await repoToken.FindTokenByCredentialAsync(msgMessage.CredentialToVerify, prvTime.GetUtcNow(), tknCancellation);

			return await rsltToken.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppToken =>
				{
					IRepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(ppToken.PassportId, tknCancellation);

					return await rsltPassport.MatchAsync(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						async ppPassport =>
						{
							ppPassport.TryExtendTerm(prvTime.GetUtcNow().Add(ppSetting.ExpiresAfterDuration), prvTime.GetUtcNow(), msgMessage.RestrictedPassportId);

							bool bIsCommited = false;

							await uowUnitOfWork.TransactionAsync(async () =>
							{
								await repoToken.ResetCredentialAsync(ppToken, msgMessage.CredentialToApply, prvTime.GetUtcNow(), tknCancellation);
								await repoPassport.UpdateAsync(ppPassport, prvTime.GetUtcNow(), tknCancellation);

								bIsCommited = uowUnitOfWork.TryCommit();

								if (bIsCommited == false)
									uowUnitOfWork.TryRollback();
							});

							if (bIsCommited == false)
								return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Transaction has not been committed." });

							return new MessageResult<bool>(true);
						});
				});
		}
	}
}