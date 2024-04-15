using Application.Command.Authentication.BearerTokenByCredential;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace ApplicationTest.Command.Authentication.BearerTokenByCredential
{
    public sealed class BearerTokenByCredentialCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public BearerTokenByCredentialCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
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

            BearerTokenByCredentialCommand cmdToken = new BearerTokenByCredentialCommand()
            {
                Credential = ppCredential
            };

            BearerTokenByCredentialCommandHandler cmdHandler = new BearerTokenByCredentialCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoToken: fxtAuthorizationData.PassportTokenRepository,
                optJwtToken: fxtAuthorizationData.JwtTokenSetting);

            // Act
            IMessageResult<string> rsltBearerToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);

            // Assert
            rsltBearerToken.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                sBearerToken =>
                {
                    IEnumerable<Claim> enumClaim = ReadToken(sBearerToken);

                    foreach (Claim clmActual in enumClaim)
                    {
                        switch (clmActual.Type)
                        {
                            case ClaimTypes.NameIdentifier:
                                clmActual.Value.Should().Be(ppPassport.Id.ToString());
                                break;

                            case ClaimTypes.Expiration:
                                clmActual.Value.Should().Be(ppPassport.ExpiredAt.ToString("O"));
                                break;

                            case ClaimTypes.Authentication:
                                clmActual.Value.Should().Be(ppToken.Provider);
                                break;

                            case ClaimTypes.Expired:
                                clmActual.Value.Should().NotBe(ppToken.RefreshToken);
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

        private IEnumerable<Claim> ReadToken(string sBearerToken)
        {
            JwtSecurityTokenHandler tknHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken tknSecurity = tknHandler.ReadJwtToken(sBearerToken);

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

            BearerTokenByCredentialCommand cmdToken = new BearerTokenByCredentialCommand()
            {
                Credential = ppCredential
            };

            // Act
            BearerTokenByCredentialCommandHandler cmdHandler = new BearerTokenByCredentialCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoToken: fxtAuthorizationData.PassportTokenRepository,
                optJwtToken: fxtAuthorizationData.JwtTokenSetting);

            IMessageResult<string> rsltBearerToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);

            // Assert
            rsltBearerToken.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(AuthorizationError.Code.Method);
                    msgError.Description.Should().Be("Passport is not enabled or expired.");

                    return false;
                },
                sBearerToken =>
                {
                    sBearerToken.Should().BeNullOrWhiteSpace();

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

            BearerTokenByCredentialCommand cmdToken = new BearerTokenByCredentialCommand()
            {
                Credential = ppInvalidCredential
            };

            // Act
            BearerTokenByCredentialCommandHandler cmdHandler = new BearerTokenByCredentialCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoToken: fxtAuthorizationData.PassportTokenRepository,
                optJwtToken: fxtAuthorizationData.JwtTokenSetting);

            IMessageResult<string> rsltBearerToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);

            // Assert
            rsltBearerToken.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Description);

                    return false;
                },
                sBearerToken =>
                {
                    sBearerToken.Should().BeNullOrWhiteSpace();

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

            BearerTokenByCredentialCommand cmdToken = new BearerTokenByCredentialCommand()
            {
                Credential = ppInvalidCredential
            };

            // Act
            BearerTokenByCredentialCommandHandler cmdHandler = new BearerTokenByCredentialCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoToken: fxtAuthorizationData.PassportTokenRepository,
                optJwtToken: fxtAuthorizationData.JwtTokenSetting);

            IMessageResult<string> rsltBearerToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);

            // Assert
            rsltBearerToken.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PassportToken.Credential.NotFound.Description);

                    return false;
                },
                sBearerToken =>
                {
                    sBearerToken.Should().BeNullOrWhiteSpace();

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

            BearerTokenByCredentialCommand cmdToken = new BearerTokenByCredentialCommand()
            {
                Credential = ppInvalidCredential
            };

            // Act
            BearerTokenByCredentialCommandHandler cmdHandler = new BearerTokenByCredentialCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoToken: fxtAuthorizationData.PassportTokenRepository,
                optJwtToken: fxtAuthorizationData.JwtTokenSetting);

            IMessageResult<string>? rsltBearerToken = null;

            for (int i = 0; i < fxtAuthorizationData.PassportSetting.MaximalAllowedAccessAttempt + 1; i++)
            {
                rsltBearerToken = await cmdHandler.Handle(cmdToken, CancellationToken.None);
            }

            if (rsltBearerToken is null)
                throw new NullReferenceException();

            // Assert
            rsltBearerToken.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(AuthorizationError.Passport.TooManyAttempts.Code);
                    msgError.Description.Should().Be(AuthorizationError.Passport.TooManyAttempts.Description);

                    return false;
                },
                sBearerToken =>
                {
                    sBearerToken.Should().BeNullOrWhiteSpace();

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
            await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
        }
    }
}