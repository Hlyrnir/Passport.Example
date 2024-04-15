using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.Passport.SeizePassport
{
	internal sealed class SeizePassportCommandHandler : ICommandHandler<SeizePassportCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportRepository repoPassport;

		public SeizePassportCommandHandler(
			ITimeProvider prvTime,
			IPassportRepository repoPassport)
		{
			this.prvTime = prvTime;
			this.repoPassport = repoPassport;
		}

		public async ValueTask<IMessageResult<bool>> Handle(SeizePassportCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(msgMessage.PassportIdToSeize, tknCancellation);

			return await rsltPassport.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppPassport =>
				{
					IRepositoryResult<bool> rsltDelete = await repoPassport.DeleteAsync(ppPassport, tknCancellation);

					return rsltDelete.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}