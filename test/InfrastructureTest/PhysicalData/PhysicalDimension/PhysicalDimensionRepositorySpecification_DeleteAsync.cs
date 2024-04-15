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
    public class PhysicalDimensionRepositorySpecification_DeleteAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PhysicalDimensionRepositorySpecification_DeleteAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenPhysicalDimensionIsDeleted()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);

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
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();

			if (pdPhysicalDimension is null)
				throw new ArgumentNullException();

			// Act
			IRepositoryResult<bool> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
					msgError.Description.Should().Be($"Could not delete {pdPhysicalDimension.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenConcurrencyStampIsNotActual()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();

			if (pdPhysicalDimension is null)
				throw new ArgumentNullException();

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.UpdateAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPhysicalDimension = await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);

			// Assert
			rsltPhysicalDimension.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PhysicalDimensionError.Code.Method);
					msgError.Description.Should().Be($"Could not delete {pdPhysicalDimension.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			IRepositoryResult<IPhysicalDimension> rsltPhysicalDimensionToDelete = await fxtAuthorizationData.PhysicalDimensionRepository.FindByIdAsync(pdPhysicalDimension.Id, CancellationToken.None);

			await rsltPhysicalDimensionToDelete.MatchAsync(
				msgError => false,
				async pdPhysicalDimensionToDelete =>
				{
					await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimensionToDelete, CancellationToken.None);

					return true;
				});
		}
	}
}
