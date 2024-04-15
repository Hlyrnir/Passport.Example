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
    public class TimePeriodRepositorySpecification_CreateAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public TimePeriodRepositorySpecification_CreateAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenPhysicalDimensionIsCreated()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

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
		public async Task Create_ShouldReturnRepositoryError_WhenTimePeriodExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TimePeriodError.Code.Method);
					msgError.Description.Should().Be($"Could not create {pdTimePeriod.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}
	}
}
