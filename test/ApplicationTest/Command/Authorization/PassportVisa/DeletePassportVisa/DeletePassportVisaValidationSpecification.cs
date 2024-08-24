using Application.Command.Authorization.PassportVisa.Delete;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportVisa.DeletePassportVisa
{
    public sealed class DeletePassportVisaValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public DeletePassportVisaValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenPassportVisaExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			DeletePassportVisaCommand cmdDelete = new DeletePassportVisaCommand()
			{
				PassportVisaId = ppVisa.Id,
				RestrictedPassportId = Guid.Empty
			};

			IValidation<DeletePassportVisaCommand> hndlValidation = new DeletePassportVisaValidation(
				repoVisa: fxtAuthorizationData.PassportVisaRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdDelete,
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
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Delete_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			DeletePassportVisaCommand cmdDelete = new DeletePassportVisaCommand()
			{
				PassportVisaId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IValidation<DeletePassportVisaCommand> hndlValidation = new DeletePassportVisaValidation(
				repoVisa: fxtAuthorizationData.PassportVisaRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdDelete,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Passport visa {cmdDelete.PassportVisaId} does not exist.");

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
