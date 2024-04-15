using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportVisa.Delete
{
	internal sealed class DeletePassportVisaCommandHandler : ICommandHandler<DeletePassportVisaCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportVisaRepository repoVisa;

		public DeletePassportVisaCommandHandler(
			ITimeProvider prvTime,
			IPassportVisaRepository repoVisa)
		{
			this.prvTime = prvTime;
			this.repoVisa = repoVisa;
		}

		public async ValueTask<IMessageResult<bool>> Handle(DeletePassportVisaCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportVisa> rsltVisa = await repoVisa.FindByIdAsync(msgMessage.PassportVisaId, tknCancellation);

			return await rsltVisa.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppVisa =>
				{
					IRepositoryResult<bool> rsltDelete = await repoVisa.DeleteAsync(ppVisa, tknCancellation);

					return rsltDelete.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}