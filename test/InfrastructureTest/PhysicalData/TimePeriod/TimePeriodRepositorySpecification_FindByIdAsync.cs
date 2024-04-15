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
    public class TimePeriodRepositorySpecification_FindByIdAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public TimePeriodRepositorySpecification_FindByIdAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindById_ShouldReturnTimePeriod_WhenIdExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<ITimePeriod> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.FindByIdAsync(pdTimePeriod.Id, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				pdTimePeriodById =>
				{
					pdTimePeriodById.Should().BeEquivalentTo(pdTimePeriod);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task FindById_ShouldReturnRepositoryError_WhenIdDoesNotExist()
		{
			// Arrange
			Guid guId = Guid.NewGuid();

			// Act
			IRepositoryResult<ITimePeriod> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.FindByIdAsync(guId, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TimePeriodError.Code.Method);
					msgError.Description.Should().Be($"Time period {guId} has not been found.");

					return false;
				},
				pdTimePeriod =>
				{
					pdTimePeriod.Should().BeNull();

					return true;
				});
		}
	}
}
