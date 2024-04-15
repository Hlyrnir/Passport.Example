﻿using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.PassportHolder.ConfirmEmailAddress
{
	internal sealed class ConfirmEmailAddressValidation : IValidation<ConfirmEmailAddressCommand>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportValidation srvValidation;

		private readonly IPassportHolderRepository repoHolder;

		public ConfirmEmailAddressValidation(IPassportHolderRepository repoHolder, IPassportValidation srvValidation, ITimeProvider prvTime)
		{
			this.prvTime = prvTime;
			this.srvValidation = srvValidation;

			this.repoHolder = repoHolder;
		}

		async ValueTask<IMessageResult<bool>> IValidation<ConfirmEmailAddressCommand>.ValidateAsync(ConfirmEmailAddressCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateEmailAddress(msgMessage.EmailAddress, "Email address");

			IRepositoryResult<bool> rsltHolder = await repoHolder.ExistsAsync(msgMessage.PassportHolderId, tknCancellation);

			rsltHolder.Match(
				msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult =>
				{
					if (bResult == false)
						srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Passport holder {msgMessage.PassportHolderId} does not exist." });

					return bResult;
				});

			return await Task.FromResult(
				srvValidation.Match(
					msgError => new MessageResult<bool>(msgError),
					bResult => new MessageResult<bool>(bResult))
				);
		}
	}
}