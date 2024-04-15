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

namespace InfrastructureTest.PhysicalData.TimePeriod
{
    public class TimePeriodRepositorySpecification_FindByFilterAsync : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public TimePeriodRepositorySpecification_FindByFilterAsync(PhysicalDataFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindByFilter_ShouldReturnTimePeriod_WhenIdExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();

			ITimePeriod pdTimePeriod_01 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			ITimePeriod pdTimePeriod_02 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			ITimePeriod pdTimePeriod_03 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			ITimePeriod pdTimePeriod_04 = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);

			await fxtAuthorizationData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod_01, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod_02, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod_03, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.InsertAsync(pdTimePeriod_04, prvTime.GetUtcNow(), CancellationToken.None);

			ITimePeriodByFilterOption optFilter = new TimePeriodByFilterOption()
			{
				PhysicalDimensionId = null,
				Page = 1,
				PageSize = 10
			};

			// Act
			IRepositoryResult<IEnumerable<ITimePeriod>> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.FindByFilterAsync(optFilter, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				enumTimePeriod =>
				{
					enumTimePeriod.Should().NotBeEmpty();
					enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_01);
					enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_02);
					enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_03);
					enumTimePeriod.Should().ContainEquivalentOf(pdTimePeriod_04);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod_01, CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod_02, CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod_03, CancellationToken.None);
			await fxtAuthorizationData.TimePeriodRepository.DeleteAsync(pdTimePeriod_04, CancellationToken.None);
			await fxtAuthorizationData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task FindByFilter_ShouldReturnRepositoryError_WhenIdDoesNotExist()
		{
			// Arrange
			Guid guId = Guid.NewGuid();

			ITimePeriodByFilterOption optFilter = new TimePeriodByFilterOption()
			{
				PhysicalDimensionId = null,
				Page = 1,
				PageSize = 10
			};

			// Act
			IRepositoryResult<IEnumerable<ITimePeriod>> rsltTimePeriod = await fxtAuthorizationData.TimePeriodRepository.FindByFilterAsync(optFilter, CancellationToken.None);

			// Assert
			rsltTimePeriod.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TimePeriodError.Code.Method);
					msgError.Description.Should().Be($"No data has been found.");

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
