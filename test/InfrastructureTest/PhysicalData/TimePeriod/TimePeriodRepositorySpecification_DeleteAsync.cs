using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Common;
using InfrastructureTest.PhysicalData.Common;
using Xunit;

namespace InfrastructureTest.PhysicalData.TimePeriod
{
    public class TimePeriodRepositorySpecification_DeleteAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public TimePeriodRepositorySpecification_DeleteAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenTimePeriodIsDeleted()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
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
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TimePeriodError.Code.Method);
					msgError.Description.Should().Be($"Could not delete {pdTimePeriod.Id}.");

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

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenConcurrencyStampIsNotActual()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.UpdateAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TimePeriodError.Code.Method);
					msgError.Description.Should().Be($"Could not delete {pdTimePeriod.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			IRepositoryResult<ITimePeriod> rsltTimePeriodToDelete = await fxtAuthorizationData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

			await rsltTimePeriodToDelete.MatchAsync(
				msgError => false,
				async pdTimePeriodToDelete =>
				{
					await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriodToDelete, CancellationToken.None);

					return true;
				});
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}
	}
}
