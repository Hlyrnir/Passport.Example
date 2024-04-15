using Application.Command.PhysicalData.TimePeriod.Update;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.TimePeriod.UpdateTimePeriod
{
	public sealed class UpdateTimePeriodCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public UpdateTimePeriodCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
		{
			this.fxtPhysicalData = fxtPhysicalDimension;
			this.prvTime = fxtPhysicalDimension.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenTimePeriodIsUpdated()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
			{
				Magnitude = new double[] { 0.0 },
				Offset = 0.0,
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				TimePeriodId = pdTimePeriod.Id,
			};

			UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
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
			await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnRepositoryError_WhenTimePeriodDoesNotExist()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
			{
				Magnitude = new double[] { 0.0 },
				Offset = 0.0,
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				TimePeriodId = Guid.NewGuid(),
			};

			// Act
			UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
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

					return false;
				});

			// Clean up
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnRepositoryError_WhenTimePeriodIsNotUpdated()
		{
			// Arrange
			IPhysicalDimension pdValidPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdValidPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdValidPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			IPhysicalDimension pdInvalidPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdInvalidPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
			{
				Magnitude = new double[] { 0.0 },
				Offset = 0.0,
				PhysicalDimensionId = pdInvalidPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				TimePeriodId = pdTimePeriod.Id,
			};

			// Act
			UpdateTimePeriodCommandHandler cmdHandler = new UpdateTimePeriodCommandHandler(
				prvTime: prvTime,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			IMessageResult<bool> rsltUpdate = await cmdHandler.Handle(cmdUpdate, CancellationToken.None);

			// Assert
			rsltUpdate.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(DomainError.Code.Method);
					msgError.Description.Should().Be("Physical dimension could not be changed.");
					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return false;
				});

			// Clean up
			await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdValidPhysicalDimension, CancellationToken.None);
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdInvalidPhysicalDimension, CancellationToken.None);
		}
	}
}