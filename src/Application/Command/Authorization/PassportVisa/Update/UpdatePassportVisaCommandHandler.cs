using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportVisa.Update
{
    internal sealed class UpdatePassportVisaCommandHandler : ICommandHandler<UpdatePassportVisaCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportVisaRepository repoVisa;

		public UpdatePassportVisaCommandHandler(
			ITimeProvider prvTime,
			IPassportVisaRepository repoVisa)
		{
			this.prvTime = prvTime;
			this.repoVisa = repoVisa;
		}

		public async ValueTask<IMessageResult<bool>> Handle(UpdatePassportVisaCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportVisa> rsltVisa = await repoVisa.FindByIdAsync(msgMessage.PassportVisaId, tknCancellation);

			return await rsltVisa.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppVisa =>
				{
                    if (ppVisa.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
                        return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

                    if (ppVisa.Name != msgMessage.Name)
					{
						if (ppVisa.TryChangeName(msgMessage.Name) == false)
							return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Name could not be changed." });
					}

					if (ppVisa.Level != msgMessage.Level)
					{
						if (ppVisa.TryChangeLevel(msgMessage.Level) == false)
							return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Level could not be changed." });
					}

					IRepositoryResult<bool> rsltUpdate = await repoVisa.UpdateAsync(ppVisa, prvTime.GetUtcNow(), tknCancellation);

					return rsltUpdate.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}