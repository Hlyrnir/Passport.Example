using Application.Command.Authorization.PassportHolder.Update;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportHolder.UpdatePassportHolder
{
    public sealed class UpdatePassportHolderValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public UpdatePassportHolderValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportExists()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			UpdatePassportHolderCommand cmdUpdate = new UpdatePassportHolderCommand()
			{
				ConcurrencyStamp = ppHolder.ConcurrencyStamp,
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
				FirstName = "Jane",
				LastName = "Doe",
				PassportHolderId = ppHolder.Id,
				PhoneNumber = "111",
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<UpdatePassportHolderCommand> hndlValidation = new UpdatePassportHolderValidation(
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				bResult =>
				{
					bResult.Should().BeTrue();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			UpdatePassportHolderCommand cmdUpdate = new UpdatePassportHolderCommand()
			{
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
				FirstName = "Jane",
				LastName = "Doe",
				PassportHolderId = Guid.NewGuid(),
				PhoneNumber = "111",
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<UpdatePassportHolderCommand> hndlValidation = new UpdatePassportHolderValidation(
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Passport holder {cmdUpdate.PassportHolderId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}
	}
}