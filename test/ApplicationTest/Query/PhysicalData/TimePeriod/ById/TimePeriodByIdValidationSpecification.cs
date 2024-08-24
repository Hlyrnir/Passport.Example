using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.PhysicalData.TimePeriod.ById;
using ApplicationTest.Common;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.TimePeriod.ById
{
    public sealed class TimePeriodByIdValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public TimePeriodByIdValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenTimePeriodIdExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			ITimePeriod pdTimePeriod = DataFaker.TimePeriod.CreateDefault(pdPhysicalDimension);
			await fxtPhysicalData.TimePeriodRepository.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), CancellationToken.None);

			TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
			{
				TimePeriodId = pdTimePeriod.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<TimePeriodByIdQuery> hndlValidation = new TimePeriodByIdValidation(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository,
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				prvTime: fxtPhysicalData.TimeProvider);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryById,
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
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenTimePeriodDoesNotExist()
		{
			// Arrange
			Guid guTimePeriodId = Guid.NewGuid();

			TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
			{
				TimePeriodId = guTimePeriodId,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<TimePeriodByIdQuery> hndlValidation = new TimePeriodByIdValidation(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository,
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				prvTime: fxtPhysicalData.TimeProvider);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryById,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Time period {guTimePeriodId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenTimePeriodIdIsEmpty()
		{
			// Arrange
			TimePeriodByIdQuery qryById = new TimePeriodByIdQuery()
			{
				TimePeriodId = Guid.Empty,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<TimePeriodByIdQuery> hndlValidation = new TimePeriodByIdValidation(
				repoTimePeriod: fxtPhysicalData.TimePeriodRepository,
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				prvTime: fxtPhysicalData.TimeProvider);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryById,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Time period identifier is invalid (empty).");

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
