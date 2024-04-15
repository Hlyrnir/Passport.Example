using Application.Command.Authentication.BearerTokenByRefreshToken;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace ApplicationTest.Command.Authentication.BearerTokenByRefreshToken
{
    public class BearerTokenByRefreshTokenCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public BearerTokenByRefreshTokenCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
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

            BearerTokenByRefreshTokenCommand cmdToken = new BearerTokenByRefreshTokenCommand()
            {
                PassportId = ppToken.PassportId,
                Provider = ppToken.Provider,
                RefreshToken = ppToken.RefreshToken
            };

            BearerTokenByRefreshTokenCommandHandler cmdHandler = new BearerTokenByRefreshTokenCommandHandler(
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

            BearerTokenByRefreshTokenCommand cmdToken = new BearerTokenByRefreshTokenCommand()
            {
                PassportId = ppToken.PassportId,
                Provider = ppToken.Provider,
                RefreshToken = ppToken.RefreshToken
            };

            // Act
            BearerTokenByRefreshTokenCommandHandler cmdHandler = new BearerTokenByRefreshTokenCommandHandler(
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
    }
}
