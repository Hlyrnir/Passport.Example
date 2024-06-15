using Application.Command.Authorization.PassportHolder.Delete;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportHolder.DeletePassportHolder
{
    public sealed class DeletePassportHolderValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public DeletePassportHolderValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenHolderExists()
        {
            // Arrange
            IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
            await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = ppHolder.Id,
                RestrictedPassportId = Guid.Empty
            };

            IValidation<DeletePassportHolderCommand> hndlValidation = new DeletePassportHolderValidation(
                repoHolder: fxtAuthorizationData.PassportHolderRepository,
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
            await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
        }

		[Fact]
		public async Task Delete_ShouldReturnMessageError_WhenHolderDoesNotExist()
		{
			// Arrange
			DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
			{
				PassportHolderId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IValidation<DeletePassportHolderCommand> hndlValidation = new DeletePassportHolderValidation(
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
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
                    msgError.Description.Should().Contain($"Passport holder {cmdDelete.PassportHolderId} does not exist.");

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
