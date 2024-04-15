using Application.Command.Authorization.Passport.RegisterPassport;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Passport.RegisterPassport
{
    public sealed class RegisterPassportValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public RegisterPassportValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Register_ShouldReturnTrue_WhenCredentialDoesNotExist()
        {
            // Arrange
            IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

            RegisterPassportCommand cmdRegister = new RegisterPassportCommand()
            {
                CredentialToRegister = ppCredential,
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
                FirstName = "Jane",
                LastName = "Doe",
                IssuedBy = Guid.NewGuid(),
				PhoneNumber = "111",
				RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<RegisterPassportCommand> hndlValidation = new RegisterPassportValidation(
                repoToken: fxtAuthorizationData.PassportTokenRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdRegister,
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
        }

		[Fact]
		public async Task Register_ShouldReturnMessageError_WhenCredentialDoesExist()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			RegisterPassportCommand cmdRegister = new RegisterPassportCommand()
			{
				CredentialToRegister = ppCredential,
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
				FirstName = "Jane",
				LastName = "Doe",
				IssuedBy = Guid.NewGuid(),
				PhoneNumber = "111",
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<RegisterPassportCommand> hndlValidation = new RegisterPassportValidation(
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: prvTime);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdRegister,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Credential {cmdRegister.CredentialToRegister.Credential} does already exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}