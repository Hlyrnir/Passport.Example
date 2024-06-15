using Application.Command.Authorization.PassportVisa.Update;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportVisa.UpdatePassportVisa
{
    public sealed class UpdatePassportVisaCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public UpdatePassportVisaCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenVisaIsUpdated()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                ConcurrencyStamp = ppVisa.ConcurrencyStamp,
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            UpdatePassportVisaCommandHandler cmdHandler = new UpdatePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
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

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenVisaDoesNotExist()
        {
            // Arrange
            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = Guid.Empty,
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            UpdatePassportVisaCommandHandler cmdHandler = new UpdatePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.PassportVisa.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PassportVisa.NotFound.Description);

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });
        }

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenConcurrencyStampDoNotMatch()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            string sObsoleteConcurrencyStamp = Guid.NewGuid().ToString();

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                ConcurrencyStamp = sObsoleteConcurrencyStamp,
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            UpdatePassportVisaCommandHandler cmdHandler = new UpdatePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Should().Be(DefaultMessageError.ConcurrencyViolation);

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

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenVisaIsNotUpdated()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                ConcurrencyStamp = ppVisa.ConcurrencyStamp,
                Level = -1,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            UpdatePassportVisaCommandHandler cmdHandler = new UpdatePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

            // Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(DomainError.Code.Method);
                    msgError.Description.Should().Be("Level could not be changed.");

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