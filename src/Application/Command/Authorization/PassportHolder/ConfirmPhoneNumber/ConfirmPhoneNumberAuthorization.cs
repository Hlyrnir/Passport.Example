using Application.Common.Authorization;
using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Authorization;
using Application.Interface.Passport;
using Application.Interface.Result;
using Domain.Interface.Authorization;

namespace Application.Command.Authorization.PassportHolder.ConfirmPhoneNumber
{
    internal sealed class ConfirmPhoneNumberAuthorization : IAuthorization<ConfirmPhoneNumberCommand>
	{
		private readonly IPassportVisaRepository repoVisa;

		public ConfirmPhoneNumberAuthorization(IPassportVisaRepository repoVisa)
		{
			this.repoVisa = repoVisa;
		}

		async ValueTask<IMessageResult<bool>> IAuthorization<ConfirmPhoneNumberCommand>.AuthorizeAsync(ConfirmPhoneNumberCommand msgMessage, IEnumerable<Guid> enumPassportVisaId, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportVisa> rsltVisa = await repoVisa.FindByNameAsync(
				AuthorizationDefault.Name.Passport,
				AuthorizationDefault.Level.Update,
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
