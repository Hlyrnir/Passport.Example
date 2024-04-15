using Application.Filter;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.PhysicalData.TimePeriod.ByFilter;
using ApplicationTest.Common;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.TimePeriod.ByFilter
{
	public class TimePeriodByFilterQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public TimePeriodByFilterQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTimePeriod_WhenTimePeriodExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			TimePeriodByFilterQuery qryByFilter = new TimePeriodByFilterQuery()
			{
				Filter = new TimePeriodByFilterOption()
				{
					PhysicalDimensionId = pdPhysicalDimension.Id,
					Page = 1,
					PageSize = 10
				},
				RestrictedPassportId = Guid.Empty
			};

			TimePeriodByFilterQueryHandler hdlQuery = new TimePeriodByFilterQueryHandler(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<TimePeriodByFilterResult> rsltQuery = await hdlQuery.Handle(qryByFilter, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				rsltTimePeriod =>
				{
					rsltTimePeriod.TimePeriod.Should().NotBeNull();
					rsltTimePeriod.TimePeriod.Should().ContainEquivalentOf(pdTimePeriod);

					return true;
				});

			//Clean up
			await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
		}
	}
}