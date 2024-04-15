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
    public class PhysicalDimensionSpecification_CreateAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PhysicalDimensionSpecification_CreateAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionIsCreated()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();

			// Act
			IRepositoryResult<bool> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
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
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Create_ShouldReturnRepositoryError_WhenPhysicalDimensionExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
					msgError.Description.Should().Be($"Could not create {pdPhysicalDimension.Name}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}
	}
}
