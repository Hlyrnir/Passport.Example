using Application.Command.Authorization.Passport.UpdatePassport;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Passport.UpdatePassport
{
    public class UpdatePassportCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public UpdatePassportCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPassportIsUpdated()
        {
            // Arrange
            IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

            IPassport ppPassport = DataFaker.Passport.CreateDefault();
            ppPassport.TryEnable(ppAuthority, prvTime.GetUtcNow());
            ppPassport.TryJoinToAuthority(ppAuthority, prvTime.GetUtcNow());
            await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
            {
                ExpiredAt = ppPassport.ExpiredAt.AddDays(1),
                IsAuthority = ppPassport.IsAuthority,
                IsEnabled = ppPassport.IsEnabled,
                LastCheckedAt = prvTime.GetUtcNow(),
                LastCheckedBy = Guid.NewGuid(),
                PassportIdToUpdate = ppPassport.Id,
                PassportVisaId = ppPassport.VisaId,
                RestrictedPassportId = ppPassport.Id
            };

            UpdatePassportCommandHandler hdlCommand = new UpdatePassportCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

            //Assert
            await rsltUpdate.MatchAsync(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                async bResult =>
                {
                    bResult.Should().BeTrue();

                    IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

                    return rsltPassport.Match(
                        msgError => false,
                        ppPassportInRepository =>
                        {
                            ppPassportInRepository.IsExpired(ppPassport.ExpiredAt.AddDays(1).AddSeconds(1)).Should().Be(true);
                            ppPassportInRepository.IsAuthority.Should().Be(true);
                            ppPassportInRepository.IsEnabled.Should().Be(true);
                            ppPassportInRepository.LastCheckedAt.Should().Be(prvTime.GetUtcNow());
                            ppPassportInRepository.LastCheckedBy.Should().Be(ppPassport.Id);
                            ppPassportInRepository.VisaId.Should().BeEmpty();

                            return true;
                        });
                });

            //Clean up
            await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnMessageError_WhenTryToEnablePassportWithoutAuthorization()
        {
            // Arrange
            IPassport ppPassport = DataFaker.Passport.CreateDefault();
            await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
            {
                ExpiredAt = ppPassport.ExpiredAt.AddDays(1),
                IsAuthority = ppPassport.IsAuthority,
                IsEnabled = true,
                LastCheckedAt = prvTime.GetUtcNow(),
                LastCheckedBy = Guid.NewGuid(),
                PassportIdToUpdate = ppPassport.Id,
                PassportVisaId = ppPassport.VisaId,
                RestrictedPassportId = ppPassport.Id
            };

            UpdatePassportCommandHandler hdlCommand = new UpdatePassportCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

            //Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(DomainError.Code.Method);
                    msgError.Description.Should().Be($"Passport {ppPassport.Id} is not enabled.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });

            //Clean up
            await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnMessageError_WhenTryToJoinAuthorityWithoutAuthorization()
        {
            // Arrange
            IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

            IPassport ppPassport = DataFaker.Passport.CreateDefault();
            ppPassport.TryEnable(ppAuthority, prvTime.GetUtcNow());
            await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportCommand cmdUpdate = new UpdatePassportCommand()
            {
                ExpiredAt = ppPassport.ExpiredAt.AddDays(1),
                IsAuthority = true,
                IsEnabled = ppPassport.IsEnabled,
                LastCheckedAt = prvTime.GetUtcNow(),
                LastCheckedBy = Guid.NewGuid(),
                PassportIdToUpdate = ppPassport.Id,
                PassportVisaId = ppPassport.VisaId,
                RestrictedPassportId = ppPassport.Id
            };

            UpdatePassportCommandHandler hdlCommand = new UpdatePassportCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

            //Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();

                    msgError.Code.Should().Be(DomainError.Code.Method);
                    msgError.Description.Should().Be($"Passport {ppPassport.Id} could not join to authority.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });

            //Clean up
            await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
        }
    }
}