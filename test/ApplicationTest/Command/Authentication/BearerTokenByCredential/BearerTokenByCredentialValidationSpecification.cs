using Application.Command.Authentication.BearerTokenByCredential;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authentication.BearerTokenByCredential
{
    public class BearerTokenByCredentialValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public BearerTokenByCredentialValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async void ByCredential_ShouldReturnTrue_WhenCredentialIsValid()
        {
            // Arrange
            IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

            BearerTokenByCredentialCommand cmdToken = new BearerTokenByCredentialCommand()
            {
                Credential = ppCredential
            };

            IValidation<BearerTokenByCredentialCommand> hndlValidation = new BearerTokenByCredentialValidation(srvValidation: fxtAuthorizationData.PassportValidation);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdToken,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltValidation.Match(
                msgError =>
                {
                    return false;
                },
                bResult =>
                {
                    bResult.Should().Be(true);
                    return true;
                });
        }
    }
}
