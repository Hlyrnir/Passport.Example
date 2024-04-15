using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.DeletePassportToken
{
	internal sealed class DeletePassportTokenCommandHandler : ICommandHandler<DeletePassportTokenCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportTokenRepository repoToken;

		public DeletePassportTokenCommandHandler(
			ITimeProvider prvTime,
			IPassportTokenRepository repoToken)
		{
			this.prvTime = prvTime;
			this.repoToken = repoToken;
		}

		public async ValueTask<IMessageResult<bool>> Handle(DeletePassportTokenCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportToken> rsltToken = await repoToken.FindTokenByCredentialAsync(msgMessage.CredentialToVerify, prvTime.GetUtcNow(), tknCancellation);

			return await rsltToken.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppToken =>
				{
					if (ppToken.Id != msgMessage.PassportTokenId)
						return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = $"Token identifier {msgMessage.PassportTokenId} does not match with credential." });

					IRepositoryResult<bool> rsltDelete = await repoToken.DeleteAsync(ppToken, tknCancellation);

					return rsltDelete.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}