using Application.Command.Authorization.Passport.SeizePassport;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Passport.SeizePassport
{
    public sealed class SeizePassportValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public SeizePassportValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPassportExists()
        {
            // Arrange
            IPassport ppPassport = DataFaker.Passport.CreateDefault();
            await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            SeizePassportCommand cmdDelete = new SeizePassportCommand()
            {
                PassportIdToSeize = ppPassport.Id,
                RestrictedPassportId = Guid.Empty
            };

            IValidation<SeizePassportCommand> hndlValidation = new SeizePassportValidation(
                repoPassport: fxtAuthorizationData.PassportRepository,
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
            await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
        }

		[Fact]
		public async Task Delete_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			SeizePassportCommand cmdDelete = new SeizePassportCommand()
			{
				PassportIdToSeize = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IValidation<SeizePassportCommand> hndlValidation = new SeizePassportValidation(
				repoPassport: fxtAuthorizationData.PassportRepository,
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
                    msgError.Description.Should().Contain($"Passport {cmdDelete.PassportIdToSeize} does not exist.");

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
