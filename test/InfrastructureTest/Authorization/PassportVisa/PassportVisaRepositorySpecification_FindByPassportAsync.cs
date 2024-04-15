using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportVisa
{
    public class PassportVisaRepositorySpecification_FindByPassportAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportVisaRepositorySpecification_FindByPassportAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindByName_ShouldFindVisa_WhenVisaIsAddedToPassport()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			ppPassport.TryAddVisa(ppVisa);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IEnumerable<IPassportVisa>> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByPassportAsync(ppPassport.Id, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				enumVisaInRepository =>
				{
					enumVisaInRepository.Should().ContainEquivalentOf(ppVisa);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task FindByName_ShouldFindAllVisa_WhenVisaAreAddedToPassport()
		{
			// Arrange
			IPassportVisa ppVisa_01 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_02 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_03 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_04 = DataFaker.PassportVisa.CreateDefault();

			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			ppPassport.TryAddVisa(ppVisa_01);
			ppPassport.TryAddVisa(ppVisa_02);
			ppPassport.TryAddVisa(ppVisa_03);
			ppPassport.TryAddVisa(ppVisa_04);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_01, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_02, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_03, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_04, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IEnumerable<IPassportVisa>> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByPassportAsync(ppPassport.Id, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				enumVisaInRepository =>
				{
					enumVisaInRepository.Should().ContainEquivalentOf(ppVisa_01);
					enumVisaInRepository.Should().ContainEquivalentOf(ppVisa_02);
					enumVisaInRepository.Should().ContainEquivalentOf(ppVisa_03);
					enumVisaInRepository.Should().ContainEquivalentOf(ppVisa_04);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_01, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_02, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_03, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_04, CancellationToken.None);
		}

		[Fact]
		public async Task FindByName_ShouldFindNoVisa_WhenNoVisaIsAddedToPassport()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IEnumerable<IPassportVisa>> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.FindByPassportAsync(ppPassport.Id, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				enumVisaInRepository =>
				{
					enumVisaInRepository.Should().BeEmpty();
					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}
	}
}