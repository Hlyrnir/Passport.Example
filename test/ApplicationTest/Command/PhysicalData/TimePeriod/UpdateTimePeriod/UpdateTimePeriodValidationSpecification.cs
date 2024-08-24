using Application.Command.PhysicalData.TimePeriod.Update;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.TimePeriod.UpdateTimePeriod
{
    public sealed class UpdateTimePeriodValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public UpdateTimePeriodValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			this.prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenTimePeriodExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
			{
				ConcurrencyStamp = pdTimePeriod.ConcurrencyStamp,
				Magnitude = new double[] { 0.0 },
				Offset = 0.0,
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				TimePeriodId = pdTimePeriod.Id
			};

			IValidation<UpdateTimePeriodCommand> hndlValidation = new UpdateTimePeriodValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
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
		public async Task Update_ShouldReturnMessageError_WhenTimePeriodDoesNotExist()
		{
			// Arrange
			UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
			{
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				Magnitude = new double[] { 0.0 },
				Offset = 0.0,
				PhysicalDimensionId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty,
				TimePeriodId = Guid.NewGuid()
			};

			IValidation<UpdateTimePeriodCommand> hndlValidation = new UpdateTimePeriodValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().ContainEquivalentOf($"Physical dimension {cmdUpdate.PhysicalDimensionId} does not exist.");
					msgError.Description.Should().ContainEquivalentOf($"Time period {cmdUpdate.TimePeriodId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Theory]
		[InlineData(true, 0)]
		[InlineData(true, double.MaxValue)]
		[InlineData(true, double.MinValue)]
		public async Task Update_ShouldReturnTrue_WhenOffseetIsValid(bool bExpectedResult, double dOffset)
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			UpdateTimePeriodCommand cmdUpdate = new UpdateTimePeriodCommand()
			{
				ConcurrencyStamp = pdTimePeriod.ConcurrencyStamp,
				Magnitude = new double[] { 0.0 },
				Offset = dOffset,
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.Empty,
				TimePeriodId = pdTimePeriod.Id
			};

			IValidation<UpdateTimePeriodCommand> hndlValidation = new UpdateTimePeriodValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdUpdate,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				bResult =>
				{
					bResult.Should().Be(bExpectedResult);

					return true;
				});

			// Clean up
			await fxtPhysicalData.TimePeriodRepository.DeleteAsync(pdTimePeriod, CancellationToken.None);
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}
	}
}