using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.PhysicalData.TimePeriod.ById;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.TimePeriod.ById
{
	public class TimePeriodByIdQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public TimePeriodByIdQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTimePeriod_WhenPeriodExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
			{
				RestrictedPassportId = Guid.NewGuid(),
				TimePeriodId = pdTimePeriod.Id
			};

			TimePeriodByIdQueryHandler hdlQuery = new TimePeriodByIdQueryHandler(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<TimePeriodByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

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
					rsltTimePeriod.TimePeriod.Should().BeEquivalentTo(pdTimePeriod);

					return true;
				});

			//Clean up
			await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenTimePeriodDoesNotExist()
		{
			// Arrange
			TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
			{
				RestrictedPassportId = Guid.NewGuid(),
				TimePeriodId = Guid.NewGuid()
			};

			TimePeriodByIdQueryHandler hdlQuery = new TimePeriodByIdQueryHandler(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<TimePeriodByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.TimePeriod.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.TimePeriod.NotFound.Description);

					return false;
				},
				rsltTimePeriod =>
				{
					rsltTimePeriod.Should().BeNull();

					return true;
				});
		}
	}
}