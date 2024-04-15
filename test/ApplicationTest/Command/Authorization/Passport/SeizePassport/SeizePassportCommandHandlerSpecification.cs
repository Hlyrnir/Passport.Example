using Application.Command.Authorization.Passport.SeizePassport;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Passport.SeizePassport
{
    public sealed class SeizePassportCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public SeizePassportCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenVisaIsDeleted()
        {
            // Arrange
            IPassport ppPassport = DataFaker.Passport.CreateDefault();
            await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            SeizePassportCommand cmdDelete = new SeizePassportCommand()
            {
                PassportIdToSeize = ppPassport.Id,
                RestrictedPassportId = Guid.Empty
            };

            SeizePassportCommandHandler cmdHandler = new SeizePassportCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository);

            // Act
            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
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
        public async Task Delete_ShouldReturnRepositoryError_WhenHolderDoesNotExist()
        {
            // Arrange
            SeizePassportCommand cmdDelete = new SeizePassportCommand()
            {
                PassportIdToSeize = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            // Act
            SeizePassportCommandHandler cmdHandler = new SeizePassportCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.Passport.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.Passport.NotFound.Description);
                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });
        }

        [Fact]
        public async Task Delete_ShouldReturnRepositoryError_WhenHolderIsNotDeleted()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            SeizePassportCommand cmdDelete = new SeizePassportCommand()
            {
                PassportIdToSeize = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            // Act
            SeizePassportCommandHandler cmdHandler = new SeizePassportCommandHandler(
                prvTime: prvTime,
                repoPassport: fxtAuthorizationData.PassportRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.Passport.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.Passport.NotFound.Description);
                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }
    }
}