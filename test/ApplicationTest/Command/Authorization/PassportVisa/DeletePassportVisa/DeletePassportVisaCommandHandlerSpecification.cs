using Application.Command.Authorization.PassportVisa.Delete;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportVisa.DeletePassportVisa
{
	public sealed class DeletePassportVisaCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public DeletePassportVisaCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenVisaIsDeleted()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            DeletePassportVisaCommand cmdDelete = new DeletePassportVisaCommand()
            {
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            DeletePassportVisaCommandHandler cmdHandler = new DeletePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

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
        public async Task Delete_ShouldReturnRepositoryError_WhenVisaDoesNotExist()
        {
            // Arrange
            DeletePassportVisaCommand cmdDelete = new DeletePassportVisaCommand()
            {
                PassportVisaId = Guid.NewGuid(),
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            DeletePassportVisaCommandHandler cmdHandler = new DeletePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
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
        public async Task Delete_ShouldReturnRepositoryError_WhenVisaIsNotDeleted()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            DeletePassportVisaCommand cmdDelete = new DeletePassportVisaCommand()
            {
                PassportVisaId = Guid.NewGuid(),
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            DeletePassportVisaCommandHandler cmdHandler = new DeletePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
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

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }
    }
}