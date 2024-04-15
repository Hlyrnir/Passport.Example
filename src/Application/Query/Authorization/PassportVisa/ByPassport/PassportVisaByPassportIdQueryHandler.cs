using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Query.Authorization.PassportVisa.ByPassport
{
	internal sealed class PassportVisaByPassportIdQueryHandler : IQueryHandler<PassportVisaByPassportIdQuery, IMessageResult<PassportVisaByPassportIdResult>>
	{
		private readonly IPassportVisaRepository repoVisa;

		public PassportVisaByPassportIdQueryHandler(IPassportVisaRepository repoVisa)
		{
			this.repoVisa = repoVisa;
		}

		public async ValueTask<IMessageResult<PassportVisaByPassportIdResult>> Handle(PassportVisaByPassportIdQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<PassportVisaByPassportIdResult>(DefaultMessageError.TaskAborted);

			RepositoryResult<IEnumerable<IPassportVisa>> rsltPassportVisa = await repoVisa.FindByPassportAsync(msgMessage.PassportIdToFind, tknCancellation);

			return rsltPassportVisa.Match(
				msgError => new MessageResult<PassportVisaByPassportIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				enumPassportVisa =>
				{
					PassportVisaByPassportIdResult qryResult = new PassportVisaByPassportIdResult()
					{
						PassportVisa = enumPassportVisa
					};

					return new MessageResult<PassportVisaByPassportIdResult>(qryResult);
				});
		}
	}
}