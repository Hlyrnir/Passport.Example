using Application.Command.PhysicalData.PhysicalDimension.Delete;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.PhysicalData.PhysicalDimension.DeletePhysicalDimension
{
	public sealed class DeletePhysicalDimensionCommandHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public DeletePhysicalDimensionCommandHandlerSpecification(PhysicalDataFixture fxtPhysicalDimension)
		{
			fxtPhysicalData = fxtPhysicalDimension;
			prvTime = fxtPhysicalDimension.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenPhysicalDimensionIsDeleted()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateTimeDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			DeletePhysicalDimensionCommand cmdDelete = new DeletePhysicalDimensionCommand()
			{
				PhysicalDimensionId = pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			DeletePhysicalDimensionCommandHandler cmdHandler = new DeletePhysicalDimensionCommandHandler(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

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
		}

		[Fact]
		public async Task Delete_ShouldReturnRepositoryError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			DeletePhysicalDimensionCommand cmdCreate = new DeletePhysicalDimensionCommand()
			{
				PhysicalDimensionId = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			// Act
			DeletePhysicalDimensionCommandHandler cmdHandler = new DeletePhysicalDimensionCommandHandler(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			IMessageResult<bool> rsltDelete = await cmdHandler.Handle(cmdCreate, CancellationToken.None);

			// Assert
			rsltDelete.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);

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