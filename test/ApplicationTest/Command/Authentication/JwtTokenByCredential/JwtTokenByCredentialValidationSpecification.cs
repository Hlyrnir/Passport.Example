using Application.Command.Authentication.JwtTokenByCredential;
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
	public class JwtTokenByCredentialValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public JwtTokenByCredentialValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async void ByCredential_ShouldReturnTrue_WhenCredentialIsValid()
		{
			// Arrange
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			JwtTokenByCredentialCommand cmdToken = new JwtTokenByCredentialCommand()
			{
				Credential = ppCredential
			};

			IValidation<JwtTokenByCredentialCommand> hndlValidation = new JwtTokenByCredentialValidation(srvValidation: fxtAuthorizationData.PassportValidation);

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
