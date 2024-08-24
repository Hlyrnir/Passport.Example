using Application.Command.Authorization.PassportVisa.Create;
using Application.Common.Authorization;
using Application.Common.Error;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;
using static ApplicationTest.Error.TestError;

namespace ApplicationTest.Command.Authorization.PassportVisa.CreatePassportVisa
{
    public sealed class CreatePassportVisaAuthorizationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public CreatePassportVisaAuthorizationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenPassportIdIsAuthorized()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Create);
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

            CreatePassportVisaCommand cmdCreate = new CreatePassportVisaCommand()
            {
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<CreatePassportVisaCommand> hndlAuthorization = new CreatePassportVisaAuthorization(fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdCreate,
                enumPassportVisaId: enumPassportVisaId,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltAuthorization.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return true;
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
        public async Task Create_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
        {
            // Arrange
            IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

            CreatePassportVisaCommand cmdCreate = new CreatePassportVisaCommand()
            {
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<CreatePassportVisaCommand> hndlAuthorization = new CreatePassportVisaAuthorization(fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdCreate,
                enumPassportVisaId: enumPassportVisaId,
                tknCancellation: CancellationToken.None);

            //Assert
            rsltAuthorization.Match(
                msgError =>
                {
                    msgError.Code.Should().Be(Repository.PassportVisa.NotFound.Code);
                    msgError.Description.Should().Be(Repository.PassportVisa.NotFound.Description);

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
        public async Task Create_ShouldReturnMessageError_WhenPassportVisaDoesNotMatch(string sName, int iLevel)
        {
            // Arrange
            IPassportVisa ppAuthorizedVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Create);
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppAuthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

            IPassportVisa ppUnauthorizedVisa = DataFaker.PassportVisa.CreateDefault(sName, iLevel);
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppUnauthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

            IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppUnauthorizedVisa.Id };

            CreatePassportVisaCommand cmdCreate = new CreatePassportVisaCommand()
            {
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                RestrictedPassportId = Guid.Empty
            };

            IAuthorization<CreatePassportVisaCommand> hndlAuthorization = new CreatePassportVisaAuthorization(fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
                msgMessage: cmdCreate,
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
