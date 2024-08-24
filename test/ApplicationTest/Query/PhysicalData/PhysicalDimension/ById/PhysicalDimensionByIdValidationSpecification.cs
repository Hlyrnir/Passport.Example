using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.PhysicalData.PhysicalDimension.ById;
using ApplicationTest.Common;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.PhysicalDimension.ById
{
    public sealed class PhysicalDimensionByIdValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public PhysicalDimensionByIdValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenPhysicalDimensionIdExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
			{
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PhysicalDimensionByIdQuery> hndlValidation = new PhysicalDimensionByIdValidation(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
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
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			Guid guPhysicalDimensionId = Guid.NewGuid();
			
			PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
			{
				PhysicalDimensionId = guPhysicalDimensionId,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PhysicalDimensionByIdQuery> hndlValidation = new PhysicalDimensionByIdValidation(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
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
					msgError.Description.Should().Contain($"Physical dimension {guPhysicalDimensionId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPhysicalDimensionIdIsEmpty()
		{
			// Arrange
			PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
			{
				PhysicalDimensionId = Guid.Empty,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PhysicalDimensionByIdQuery> hndlValidation = new PhysicalDimensionByIdValidation(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository,
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
					msgError.Description.Should().Contain($"Physical dimension identifier is invalid (empty).");

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
