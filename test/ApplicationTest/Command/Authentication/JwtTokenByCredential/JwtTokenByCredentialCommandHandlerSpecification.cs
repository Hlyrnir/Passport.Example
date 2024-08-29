using Application.Command.Authentication.JwtTokenByCredential;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Token;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace ApplicationTest.Command.Authentication.JwtTokenByCredential
{
	public sealed class JwtTokenByCredentialCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public JwtTokenByCredentialCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async void ByCredential_ShouldReturnToken_WhenPassportIsEnabled()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryEnable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			JwtTokenByCredentialCommand cmdToken = new JwtTokenByCredentialCommand()
			{
				Credential = ppCredential
			};

			JwtTokenByCredentialCommandHandler cmdHandler = new JwtTokenByCredentialCommandHandler(
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
		public async void ByCredential_ShouldReturnMessageError_WhenPassportIsDisabled()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryDisable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			JwtTokenByCredentialCommand cmdToken = new JwtTokenByCredentialCommand()
			{
				Credential = ppCredential
			};

			// Act
			JwtTokenByCredentialCommandHandler cmdHandler = new JwtTokenByCredentialCommandHandler(
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

		[Fact]
		public async void ByCredential_ShouldReturnMessageError_WhenCredentialIsUnknown()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryEnable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppInvalidCredential = DataFaker.PassportCredential.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

			JwtTokenByCredentialCommand cmdToken = new JwtTokenByCredentialCommand()
			{
				Credential = ppInvalidCredential
			};

			// Act
			JwtTokenByCredentialCommandHandler cmdHandler = new JwtTokenByCredentialCommandHandler(
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
					msgError.Should().NotBeNull();

					msgError.Code.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Description);

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

		[Fact]
		public async void ByCredential_ShouldReturnMessageError_WhenSignatureIsInvalid()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryEnable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppInvalidCredential = DataFaker.PassportCredential.Create(ppCredential.Credential, Guid.NewGuid().ToString());

			JwtTokenByCredentialCommand cmdToken = new JwtTokenByCredentialCommand()
			{
				Credential = ppInvalidCredential
			};

			// Act
			JwtTokenByCredentialCommandHandler cmdHandler = new JwtTokenByCredentialCommandHandler(
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
					msgError.Should().NotBeNull();

					msgError.Code.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Description);

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

		[Fact]
		public async void ByCredential_ShouldReturnMessageError_WhenTooManyAttemps()
		{
			// Arrange
			IPassport ppAutority = DataFaker.Passport.CreateAuthority();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryEnable(ppAutority, prvTime.GetUtcNow());
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppInvalidCredential = DataFaker.PassportCredential.Create(ppCredential.Credential, Guid.NewGuid().ToString());

			JwtTokenByCredentialCommand cmdToken = new JwtTokenByCredentialCommand()
			{
				Credential = ppInvalidCredential
			};

			// Act
			JwtTokenByCredentialCommandHandler cmdHandler = new JwtTokenByCredentialCommandHandler(
				prvTime: prvTime,
				repoPassport: fxtAuthorizationData.PassportRepository,
				repoToken: fxtAuthorizationData.PassportTokenRepository,
				jwtTokenService: fxtAuthorizationData.JwtTokenService,
				optJwtToken: fxtAuthorizationData.JwtTokenSetting);

			IMessageResult<JwtTokenTransferObject>? rsltJwtToken = null;

			for (int i = 0; i < fxtAuthorizationData.PassportSetting.MaximalAllowedAccessAttempt + 1; i++)
			{
				rsltJwtToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);
			}

			if (rsltJwtToken is null)
				throw new NullReferenceException();

			// Assert
			rsltJwtToken.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();

					msgError.Code.Should().Be(AuthorizationError.Passport.TooManyAttempts.Code);
					msgError.Description.Should().Be(AuthorizationError.Passport.TooManyAttempts.Description);

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