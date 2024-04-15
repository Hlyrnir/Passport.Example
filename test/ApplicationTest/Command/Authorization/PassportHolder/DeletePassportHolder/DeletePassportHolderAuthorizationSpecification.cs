using Application.Command.Authorization.PassportHolder.DeletePassportHolder;
using Application.Common.Authorization;
using Application.Error;
using Application.Interface.Authorization;
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
    public sealed class DeletePassportHolderAuthorizationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public DeletePassportHolderAuthorizationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Delete);
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<DeletePassportHolderCommand> hndlAuthorization = new DeletePassportHolderAuthorization(fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdDelete,
                enumPassportVisaId: enumPassportVisaId,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltAuthorization.Match(
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

            //Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }

        [Fact]
        public async Task Delete_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
        {
            // Arrange
            IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<DeletePassportHolderCommand> hndlAuthorization = new DeletePassportHolderAuthorization(fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdDelete,
                enumPassportVisaId: enumPassportVisaId,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltAuthorization.Match(
                msgError =>
                {
                    msgError.Code.Should().Be(TestError.Repository.PassportVisa.NotFound.Code);
                    msgError.Description.Should().Be(TestError.Repository.PassportVisa.NotFound.Description);

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }

        [Theory]
        [InlineData(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Create)]
        [InlineData(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Delete)]
        [InlineData(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Read)]
        [InlineData(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Update)]
        public async Task Delete_ShouldReturnMessageError_WhenPassportVisaDoesNotMatch(string sName, int iLevel)
        {
            // Arrange
            IPassportVisa ppAuthorizedVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Delete);
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppAuthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

            IPassportVisa ppUnauthorizedVisa = DataFaker.PassportVisa.CreateDefault(sName, iLevel);
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppUnauthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

            IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppUnauthorizedVisa.Id };

            DeletePassportHolderCommand cmdDelete = new DeletePassportHolderCommand()
            {
                PassportHolderId = Guid.Empty,
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<DeletePassportHolderCommand> hndlAuthorization = new DeletePassportHolderAuthorization(fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdDelete,
                enumPassportVisaId: enumPassportVisaId,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltAuthorization.Match(
                msgError =>
                {
                    msgError.Should().Be(AuthorizationError.PassportVisa.VisaDoesNotExist);

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppUnauthorizedVisa, CancellationToken.None);
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppAuthorizedVisa, CancellationToken.None);
        }
    }
}