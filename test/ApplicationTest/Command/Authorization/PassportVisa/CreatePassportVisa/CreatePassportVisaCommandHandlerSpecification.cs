using Application.Command.Authorization.PassportVisa.Create;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportVisa.CreatePassportVisa
{
	public sealed class CreatePassportVisaCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public CreatePassportVisaCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenVisaIsCreated()
        {
            // Arrange
            CreatePassportVisaCommand cmdCreate = new CreatePassportVisaCommand()
            {
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                RestrictedPassportId = Guid.NewGuid()
            };

            CreatePassportVisaCommandHandler cmdHandler = new CreatePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            // Act
            IMessageResult<Guid> rsltPassportVisaId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            await rsltPassportVisaId.MatchAsync(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                async guPassportVisaId =>
                {
                    IRepositoryResult<IPassportVisa> rsltPassportVisa = await fxtAuthorizationData.PassportVisaRepository.FindByIdAsync(guPassportVisaId, CancellationToken.None);

                    return rsltPassportVisa.Match(
                        msgError =>
                        {
                            msgError.Should().BeNull();

                            return false;
                        },
                        ppPassportVisa =>
                        {
                            ppPassportVisa.Level.Should().Be(cmdCreate.Level);
                            ppPassportVisa.Name.Should().Be(cmdCreate.Name);

                            return true;
                        });
                });

            //Clean up
            IRepositoryResult<IPassportVisa> rsltPassportVisa = await fxtAuthorizationData.PassportVisaRepository.FindByNameAsync(cmdCreate.Name, cmdCreate.Level, CancellationToken.None);

            await rsltPassportVisa.MatchAsync(
                msgError => false,
                async ppVisa => await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldReturnRepositoryError_WhenVisaIsNotCreated()
        {
            // Arrange
            CreatePassportVisaCommand cmdCreate = new CreatePassportVisaCommand()
            {
                Level = -1,
                Name = Guid.NewGuid().ToString(),
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            CreatePassportVisaCommandHandler cmdHandler = new CreatePassportVisaCommandHandler(
                prvTime: prvTime,
                repoVisa: fxtAuthorizationData.PassportVisaRepository);

            IMessageResult<Guid> rsltPassportVisaId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

            // Assert
            rsltPassportVisaId.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(DomainError.Code.Method);
                    msgError.Description.Should().Be("Visa has not been created.");
                    return false;
                },
                guPassportVisaId =>
                {
                    guPassportVisaId.Should().BeEmpty();

                    return true;
                });
        }
    }
}