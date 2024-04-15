using Application.Common.Result.Message;
using Application.Error;
using Application.Factory;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportVisa.Create
{
	internal sealed class CreatePassportVisaCommandHandler : ICommandHandler<CreatePassportVisaCommand, IMessageResult<Guid>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportVisaRepository repoVisa;

		public CreatePassportVisaCommandHandler(
			ITimeProvider prvTime,
			IPassportVisaRepository repoVisa)
		{
			this.prvTime = prvTime;
			this.repoVisa = repoVisa;
		}

		public async ValueTask<IMessageResult<Guid>> Handle(CreatePassportVisaCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

			IPassportVisa? ppVisa = DomainFactory.Authorization.PassportVisa.Create(
				sName: msgMessage.Name,
				iLevel: msgMessage.Level);

			if (ppVisa is null)
				return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Visa has not been created." });

			IRepositoryResult<bool> rsltVisa = await repoVisa.InsertAsync(ppVisa, prvTime.GetUtcNow(), tknCancellation);

			return rsltVisa.Match(
				msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<Guid>(ppVisa.Id));
		}
	}
}
