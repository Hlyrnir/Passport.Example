using Application.Filter;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Common;
using InfrastructureTest.PhysicalData.Common;
using Xunit;

namespace InfrastructureTest.PhysicalData.PhysicalDimension
{
    public class PhysicalDimensionRepositorySpecification_FindByFilterAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PhysicalDimensionRepositorySpecification_FindByFilterAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindByFilter_ShouldReturnPhysicalDimension_WhenIdExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension_01 = DataFaker.PhysicalDimension.CreateDefault();
			IPhysicalDimension pdPhysicalDimension_02 = DataFaker.PhysicalDimension.CreateDefault();

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_01, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension_02, prvTime.GetUtcNow(), CancellationToken.None);

			IPhysicalDimensionByFilterOption optFilter = new PhysicalDimensionByFilterOption()
			{
				ConversionFactorToSI = null,
				CultureName = null,
				ExponentOfAmpere = null,
				ExponentOfCandela = null,
				ExponentOfKelvin = null,
				ExponentOfKilogram = null,
				ExponentOfMetre = null,
				ExponentOfMole = null,
				ExponentOfSecond = null,
				Name = null,
				Symbol = null,
				Unit = null,
				Page = 1,
				PageSize = 10
			};

			// Act
			IRepositoryResult<IEnumerable<IPhysicalDimension>> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.FindByFilterAsync(optFilter, CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				enumPhysicalDimension =>
				{
					enumPhysicalDimension.Should().NotBeEmpty();
					enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_01);
					enumPhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension_02);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_01, CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension_02, CancellationToken.None);
		}

		[Fact]
		public async Task FindByFilter_ShouldReturnRepositoryError_WhenIdDoesNotExist()
		{
			// Arrange
			IPhysicalDimensionByFilterOption optFilter = new PhysicalDimensionByFilterOption()
			{
				ConversionFactorToSI = null,
				CultureName = null,
				ExponentOfAmpere = null,
				ExponentOfCandela = null,
				ExponentOfKelvin = null,
				ExponentOfKilogram = null,
				ExponentOfMetre = null,
				ExponentOfMole = null,
				ExponentOfSecond = null,
				Name = null,
				Symbol = null,
				Unit = null,
				Page = 1,
				PageSize = 10
			};

			// Act
			IRepositoryResult<IEnumerable<IPhysicalDimension>> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.FindByFilterAsync(optFilter, CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
					msgError.Description.Should().Be($"No data has been found.");

					return false;
				},
				enumPhysicalDimension =>
				{
					//enumPhysicalDimension.Should().BeEmpty();

					return true;
				});
		}
	}
}
