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
    public class PhysicalDimensionRepositorySpecification_FindByIdAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PhysicalDimensionRepositorySpecification_FindByIdAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindById_ShouldReturnPhysicalDimension_WhenIdExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.FindByIdAsync(pdPhysicalDimension.Id, CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				pdPhysicalDimensionById =>
				{
					pdPhysicalDimensionById.Should().BeEquivalentTo(pdPhysicalDimension);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task FindById_ShouldReturnRepositoryError_WhenIdDoesNotExist()
		{
			// Arrange
			Guid guId = Guid.NewGuid();

			// Act
			IRepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.FindByIdAsync(guId, CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
					msgError.Description.Should().Be($"Physical dimension {guId} has not been found.");

					return false;
				},
				pdPhysicalDimension =>
				{
					pdPhysicalDimension.Should().BeNull();

					return true;
				});
		}
	}
}
