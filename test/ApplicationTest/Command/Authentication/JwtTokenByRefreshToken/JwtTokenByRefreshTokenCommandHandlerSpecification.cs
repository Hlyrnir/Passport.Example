using Application.Command.Authentication.JwtTokenByRefreshToken;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Token;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace ApplicationTest.Command.Authentication.JwtTokenByRefreshToken
{
	public class JwtTokenByRefreshTokenCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public JwtTokenByRefreshTokenCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async void ByRefreshToken_ShouldReturnToken_WhenPassportIsEnabled()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryEnable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			JwtTokenByRefreshTokenCommand cmdToken = new JwtTokenByRefreshTokenCommand()
			{
				PassportId = ppToken.PassportId,
				Provider = ppToken.Provider,
				RefreshToken = ppToken.RefreshToken
			};

			JwtTokenByRefreshTokenCommandHandler cmdHandler = new JwtTokenByRefreshTokenCommandHandler(
				prvTime: prvTime,
				repoPassport: fxtAuthorizationData.PassportRepository,
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				jwtTokenService: fxtAuthorizationData.JwtTokenService,
				optJwtToken: fxtAuthorizationData.JwtTokenSetting);

			// Act
			IMessageResult<JwtTokenTransferObject> rsltJwtToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);

			// Assert
			rsltJwtToken.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				dtoJwtToken =>
				{
					dtoJwtToken.ExpiredAt.Should().Be(ppPassport.ExpiredAt);
					dtoJwtToken.Provider.Should().Be(ppToken.Provider);
					dtoJwtToken.RefreshToken.Should().NotBe(ppToken.RefreshToken);

					IEnumerable<Claim> enumClaim = ReadToken(dtoJwtToken.Token);

					foreach (Claim clmActual in enumClaim)
					{
						switch (clmActual.Type)
						{
							case JwtTokenClaim.Id:
								clmActual.Value.Should().Be(ppPassport.Id.ToString());
								break;

							default:

								break;
						}
					}

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		private IEnumerable<Claim> ReadToken(string sToken)
		{
			JwtSecurityTokenHandler tknHandler = new JwtSecurityTokenHandler();
			JwtSecurityToken tknSecurity = tknHandler.ReadJwtToken(sToken);

			return tknSecurity.Claims;
		}

		[Fact]
		public async void ByRefreshToken_ShouldReturnMessageError_WhenPassportIsDisabled()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryDisable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			JwtTokenByRefreshTokenCommand cmdToken = new JwtTokenByRefreshTokenCommand()
			{
				PassportId = ppToken.PassportId,
				Provider = ppToken.Provider,
				RefreshToken = ppToken.RefreshToken
			};

			// Act
			JwtTokenByRefreshTokenCommandHandler cmdHandler = new JwtTokenByRefreshTokenCommandHandler(
				prvTime: prvTime,
				repoPassport: fxtAuthorizationData.PassportRepository,
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				jwtTokenService: fxtAuthorizationData.JwtTokenService,
				optJwtToken: fxtAuthorizationData.JwtTokenSetting);

			IMessageResult<JwtTokenTransferObject> rsltJwtToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);

			// Assert
			rsltJwtToken.Match(
				msgError =>
				{
					msgError.Should().Be(AuthorizationError.Passport.IsDisabled);

					return false;
				},
				dtoJwtToken =>
				{
					dtoJwtToken.Should().BeNull();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}
