using Application.Command.PhysicalData.PhysicalDimension.Delete;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.DeletePhysicalDimension
{
    public sealed class DeletePhysicalDimensionValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public DeletePhysicalDimensionValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenPhysicalDimensionExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
			{
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.NewGuid(),
			};

			IValidation<DeletePhysicalDimensionCommand> hndlValidation = new DeletePhysicalDimensionValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdDelete,
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
		public async Task Delete_ShouldReturnMessageError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			Guid guPhysicalDimensionId = Guid.NewGuid();

			DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
			{
				PhysicalDimensionId = guPhysicalDimensionId,
				RestrictedPassportId = Guid.NewGuid(),
			};

			IValidation<DeletePhysicalDimensionCommand> hndlValidation = new DeletePhysicalDimensionValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation,
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: cmdDelete,
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
	}
}