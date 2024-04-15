using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.PassportVisa.Update
{
	internal sealed class UpdatePassportVisaValidation : IValidation<UpdatePassportVisaCommand>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportValidation srvValidation;

		private readonly IPassportVisaRepository repoVisa;

		public UpdatePassportVisaValidation(IPassportVisaRepository repoVisa, IPassportValidation srvValidation, ITimeProvider prvTime)
		{
			this.prvTime = prvTime;
			this.srvValidation = srvValidation;

			this.repoVisa = repoVisa;
		}

		async ValueTask<IMessageResult<bool>> IValidation<UpdatePassportVisaCommand>.ValidateAsync(UpdatePassportVisaCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			if (string.IsNullOrWhiteSpace(msgMessage.Name) == true)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Name is invalid (empty)." });

			if (msgMessage.Level < 0)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Level must be greater than or equal to zero." });

			if (srvValidation.IsValid == true)
			{
				IRepositoryResult<bool> rsltVisa = await repoVisa.ExistsAsync(msgMessage.PassportVisaId, tknCancellation);

				rsltVisa.Match(
					msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
					bResult =>
					{
						if (bResult == false)
							srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Passport visa {msgMessage.PassportVisaId} does not exist." });

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
