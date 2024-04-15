using Application.Common.Authorization;
using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Authorization;
using Application.Interface.Passport;
using Application.Interface.Result;
using Domain.Interface.Authorization;

namespace Application.Query.Authorization.PassportVisa.ByPassport
{
	internal sealed class PassportVisaByPassportIdAuthorization : IAuthorization<PassportVisaByPassportIdQuery>
	{
		private readonly IPassportVisaRepository repoVisa;

		public PassportVisaByPassportIdAuthorization(IPassportVisaRepository repoVisa)
		{
			this.repoVisa = repoVisa;
		}

		async ValueTask<IMessageResult<bool>> IAuthorization<PassportVisaByPassportIdQuery>.AuthorizeAsync(PassportVisaByPassportIdQuery msgMessage, IEnumerable<Guid> enumPassportVisaId, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportVisa> rsltVisa = await repoVisa.FindByNameAsync(
				AuthorizationDefault.Name.Passport,
				AuthorizationDefault.Level.Read,
				tknCancellation);

			return rsltVisa.Match(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				ppVisa =>
				{
					foreach (Guid guPassportVisaId in enumPassportVisaId)
					{
						if (Equals(guPassportVisaId, ppVisa.Id) == true)
							return new MessageResult<bool>(true);
					}

					return new MessageResult<bool>(AuthorizationError.PassportVisa.VisaDoesNotExist);
				});
		}
	}
}
