using Application.Command.PhysicalData.TimePeriod.Delete;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.TimePeriod.DeleteTimePeriod
{
	public sealed class DeleteTimePeriodCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public DeleteTimePeriodCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
		{
			this.fxtPhysicalData = fxtPhysicalDimension;
			this.prvTime = fxtPhysicalDimension.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenTimePeriodIsDeleted()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			DeleteTimePeriodCommand cmdDelete = new DeleteTimePeriodCommand()
			{
				TimePeriodId = pdTimePeriod.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			DeleteTimePeriodCommandHandler cmdHandler = new DeleteTimePeriodCommandHandler(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdDelete, CancellationToken.None);

			// Assert
			rsltDelete.Match(
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
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenTimePeriodDoesNotExist()
		{
			// Arrange
			DeleteTimePeriodCommand cmdCreate = new DeleteTimePeriodCommand()
			{
				TimePeriodId = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			// Act
			DeleteTimePeriodCommandHandler cmdHandler = new DeleteTimePeriodCommandHandler(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			rsltDelete.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.TimePeriod.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.TimePeriod.NotFound.Description);
					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}
	}
}