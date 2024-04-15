using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.PhysicalData.PhysicalDimension.ById;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.PhysicalDimension.ById
{
	public class PhysicalDimensionByIdQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public PhysicalDimensionByIdQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnPhysicalDimension_WhenPhysicalDimensionExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
			{
				PhysicalDimensionId=pdPhysicalDimension.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			PhysicalDimensionByIdQueryHandler hdlQuery = new PhysicalDimensionByIdQueryHandler(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<PhysicalDimensionByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				rsltPhysicalDimension =>
				{
					rsltPhysicalDimension.PhysicalDimension.Should().NotBeNull();
					rsltPhysicalDimension.PhysicalDimension.Should().BeEquivalentTo(pdPhysicalDimension);

					return true;
				});

			//Clean up
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPhysicalDimensionDoesNotExist()
		{
			// Arrange
			PhysicalDimensionByIdQuery qryById = new PhysicalDimensionByIdQuery()
			{
				PhysicalDimensionId = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			PhysicalDimensionByIdQueryHandler hdlQuery = new PhysicalDimensionByIdQueryHandler(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<PhysicalDimensionByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PhysicalDimension.NotFound.Description);

					return false;
				},
				rsltPhysicalDimension =>
				{
					rsltPhysicalDimension.Should().BeNull();

					return true;
				});
		}
	}
}