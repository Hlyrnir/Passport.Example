using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.PassportVisa.Create
{
    internal sealed class CreatePassportVisaValidation : IValidation<CreatePassportVisaCommand>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportValidation srvValidation;

		private readonly IPassportVisaRepository repoVisa;

		public CreatePassportVisaValidation(IPassportVisaRepository repoVisa, IPassportValidation srvValidation, ITimeProvider prvTime)
		{
			this.prvTime = prvTime;
			this.srvValidation = srvValidation;

			this.repoVisa = repoVisa;
		}

		async ValueTask<IMessageResult<bool>> IValidation<CreatePassportVisaCommand>.ValidateAsync(CreatePassportVisaCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			if (string.IsNullOrWhiteSpace(msgMessage.Name) == true)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Name is invalid (empty)." });

			if (msgMessage.Level < 0)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Level must be greater than or equal to zero." });

			if (srvValidation.IsValid == true)
			{
				IRepositoryResult<bool> rsltVisa = await repoVisa.ByNameAtLevelExistsAsync(
					sName: msgMessage.Name,
					iLevel: msgMessage.Level,
					tknCancellation: tknCancellation);

				rsltVisa.Match(
					msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
					bResult =>
					{
						if (bResult == true)
							srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Visa of name {msgMessage.Name} at level {msgMessage.Level} does already exist." });

						return bResult;
					});
			}

			return await Task.FromResult(
				srvValidation.Match(
					msgError => new MessageResult<bool>(msgError),
					bResult => new MessageResult<bool>(bResult))
				);
		}
	}
}