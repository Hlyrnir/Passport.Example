using Application.Command.Authorization.Passport.UpdatePassport;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Passport.UpdatePassport
{
    public sealed class UpdatePassportValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public UpdatePassportValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
            {
                ExpiredAt = ppPassport.ExpiredAt,
                IsAuthority = false,
                IsEnabled = false,
                LastCheckedAt = prvTime.GetUtcNow(),
                LastCheckedBy = Guid.NewGuid(),
                PassportIdToUpdate = ppPassport.Id,
                PassportVisaId = Enumerable.Empty<Guid>(),
                RestrictedPassportId = Guid.NewGuid()
            };

			IValidation<UpdatePassportCommand> hndlValidation = new UpdatePassportValidation(
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
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
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
			{
				ExpiredAt = prvTime.GetUtcNow(),
				IsAuthority = false,
				IsEnabled = false,
				LastCheckedAt = prvTime.GetUtcNow(),
				LastCheckedBy = Guid.NewGuid(),
				PassportIdToUpdate = Guid.NewGuid(),
				PassportVisaId = Enumerable.Empty<Guid>(),
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<UpdatePassportCommand> hndlValidation = new UpdatePassportValidation(
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
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
					msgError.Description.Should().Contain($"Passport {cmdUpdate.PassportIdToUpdate} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
        public async Task Update_ShouldReturnTrue_WhenExpiredAtDateIsLater()
        {
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			DateTimeOffset dtDate = prvTime.GetUtcNow().AddDays(1);

            UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
            {
                ExpiredAt = dtDate,
                IsAuthority = false,
                IsEnabled = false,
                LastCheckedAt = prvTime.GetUtcNow(),
                LastCheckedBy = Guid.NewGuid(),
                PassportIdToUpdate = ppPassport.Id,
                PassportVisaId = Enumerable.Empty<Guid>(),
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<UpdatePassportCommand> hndlValidation = new UpdatePassportValidation(
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
                tknCancellation: CancellationToken.None);

            //Assert
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
        public async Task Update_ShouldReturnMessageError_WhenExpiredAtIsTooEarly()
        {
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			DateTimeOffset dtDate = prvTime.GetUtcNow().AddDays(-1);

            UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
            {
                ExpiredAt = dtDate,
                IsAuthority = false,
                IsEnabled = false,
                LastCheckedAt = prvTime.GetUtcNow(),
                LastCheckedBy = Guid.NewGuid(),
                PassportIdToUpdate = ppPassport.Id,
                PassportVisaId = Enumerable.Empty<Guid>(),
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<UpdatePassportCommand> hndlValidation = new UpdatePassportValidation(
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain("Expiration date must be in the future.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
    }
}