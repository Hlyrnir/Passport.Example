using Application.Command.Authentication.JwtTokenByRefreshToken;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authentication.JwtTokenByCredential
{
	public class JwtTokenByRefreshTokenValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public JwtTokenByRefreshTokenValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async void ByCredential_ShouldReturnTrue_WhenCredentialIsValid()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);

			JwtTokenByRefreshTokenCommand cmdToken = new JwtTokenByRefreshTokenCommand()
			{
				PassportId = ppPassport.Id,
				Provider = ppToken.Provider,
				RefreshToken = ppToken.RefreshToken
			};

			IValidation<JwtTokenByRefreshTokenCommand> hndlValidation = new JwtTokenByRefreshTokenValidation(srvValidation: fxtAuthorizationData.PassportValidation);

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
