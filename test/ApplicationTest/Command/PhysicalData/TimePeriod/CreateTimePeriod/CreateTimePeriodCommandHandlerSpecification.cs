using Application.Command.PhysicalData.TimePeriod.Create;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.TimePeriod.CreateTimePeriod
{
	public sealed class CreateTimePeriodCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public CreateTimePeriodCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
		{
			this.fxtPhysicalData = fxtPhysicalDimension;
			this.prvTime = fxtPhysicalDimension.TimeProvider;
		}

		[Fact]
		public async Task Create_ShouldReturnTrue_WhenTimePeriodIsCreated()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			double dMagnitude = 0.0;

			CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
			{
				PhysicalDimensionId = pdPhysicalDimension.Id,
				Magnitude = new double[] { dMagnitude },
				Offset = 0.0,
				RestrictedPassportId = Guid.NewGuid()
			};

			CreateTimePeriodCommandHandler cmdHandler = new CreateTimePeriodCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<Guid> rsltTimePeriodId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			await rsltTimePeriodId.MatchAsync(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				async guTimePeriodId =>
				{
					IRepositoryResult<ITimePeriod> rsltTimePeriod = await fxtPhysicalData.TimePeriodRepository.FindByIdAsync(guTimePeriodId, CancellationToken.None);

					rsltTimePeriod.Match(
						msgError =>
						{
							msgError.Should().BeNull();

							return false;
						},
						pdTimePeriod =>
						{
							pdTimePeriod.Magnitude.Should().ContainEquivalentOf(dMagnitude);
							pdTimePeriod.Offset.Should().Be(cmdCreate.Offset);

							return true;
						});

					//Clean up
					await rsltTimePeriod.MatchAsync(
						msgError => false,
						async pdTimePeriod => await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None));

					return true;
				});
		}

		[Fact]
		public async Task Create_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			CreateTimePeriodCommand cmdCreate = new CreateTimePeriodCommand()
			{
				PhysicalDimensionId = Guid.NewGuid(),
				Magnitude = new double[] { 0.0 },
				Offset = 0.0,
				RestrictedPassportId = Guid.NewGuid()
			};

			// Act
			CreateTimePeriodCommandHandler cmdHandler = new CreateTimePeriodCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			IMessageResult<Guid> rsltTimePeriodId = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			rsltTimePeriodId.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);
					return false;
				},
				guTimePeriodId =>
				{
					guTimePeriodId.Should().BeEmpty();

					return true;
				});
		}
	}
}