using Application.Command.Authorization.PassportHolder.DeletePassportHolder;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportHolder.DeletePassportHolder
{
    public sealed class DeletePassportHolderCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public DeletePassportHolderCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenHolderIsDeleted()
        {
            // Arrange
            IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
            await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = ppHolder.Id,
                RestrictedPassportId = Guid.Empty
            };

            DeletePassportHolderCommandHandler cmdHandler = new DeletePassportHolderCommandHandler(
                prvTime: prvTime,
                repoHolder: fxtAuthorizationData.PassportHolderRepository);

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
            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            // Act
            DeletePassportHolderCommandHandler cmdHandler = new DeletePassportHolderCommandHandler(
                prvTime: prvTime,
                repoHolder: fxtAuthorizationData.PassportHolderRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.PassportHolder.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PassportHolder.NotFound.Description);
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
            IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
            await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            // Act
            DeletePassportHolderCommandHandler cmdHandler = new DeletePassportHolderCommandHandler(
                prvTime: prvTime,
                repoHolder: fxtAuthorizationData.PassportHolderRepository);

            IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

            // Assert
            rsltDelete.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(TestError.Repository.PassportHolder.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PassportHolder.NotFound.Description);
                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });

            // Clean up
            await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
        }
    }
}