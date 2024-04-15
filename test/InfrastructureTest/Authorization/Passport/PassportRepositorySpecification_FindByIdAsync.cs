using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.Passport
{
    public class PassportRepositorySpecification_FindByIdAsync : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;

        private readonly ITimeProvider prvTime;

        public PassportRepositorySpecification_FindByIdAsync(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            this.prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task FindById_ShouldFindPassport_WhenPassportIdExists()
        {
            // Arrange
            IPassport ppPassport = DataFaker.Passport.CreateDefault();

            await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

            // Act
            IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

            // Assert
            rsltPassport.Match(
                msgError =>
                {
                    msgError.Should().BeNull();

                    return false;
                },
                ppPassportInRepository =>
                {
                    ppPassportInRepository.Should().NotBeNull();
                    ppPassportInRepository.Should().BeEquivalentTo(ppPassport);

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
        }

        [Fact]
        public async Task FindById_ShouldReturnRepositoryError_WhenPassportIdDoesNotExist()
        {
            // Arrange
            Guid guPassportId = Guid.NewGuid();

            // Act
            IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(guPassportId, CancellationToken.None);

            // Assert
            rsltPassport.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(PassportError.Code.Method);
                    msgError.Description.Should().Be($"No data for {guPassportId} has been found.");

                    return false;
                },
                ppPassportInRepository =>
                {
                    ppPassportInRepository.Should().BeNull();

                    return true;
                });
        }

		[Fact]
		public async Task FindById_VisaIdShouldNotBeEmpty_WhenVisaExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            Guid guPassportToDelete = Guid.Empty;

			// Act
			ppPassport.TryAddVisa(ppVisa);
			await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			// Assert
			rsltPassport.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
                    ppPassportInRepository.VisaId.Should().NotBeEmpty();

					return true;
				});

			// Clean up
			IRepositoryResult<IPassport> rsltPassportToDelete = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

            await rsltPassportToDelete.MatchAsync(
                msgError => false,
                async ppPassportToDelete =>
                {
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassportToDelete, CancellationToken.None);

                    return true;
                });

			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}
	}
}
