using Application.Filter;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.PhysicalData.PhysicalDimension.ByFilter;
using ApplicationTest.Common;
using Domain.Interface.PhysicalData;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.PhysicalDimension.ByFilter
{
	public class PhysicalDimensionByFilterQueryHandlerSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public PhysicalDimensionByFilterQueryHandlerSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnPhysicalDimension_WhenPhysicalDimensionExists()
		{
			// Arrange
			IPhysicalDimension pdPhysicalDimension = DataFaker.PhysicalDimension.CreateDefault();
			await fxtPhysicalData.PhysicalDimensionRepository.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), CancellationToken.None);

			PhysicalDimensionByFilterQuery qryByFilter = new PhysicalDimensionByFilterQuery()
			{
				Filter = new PhysicalDimensionByFilterOption()
				{
					ConversionFactorToSI = pdPhysicalDimension.ConversionFactorToSI,
					CultureName = null,
					ExponentOfAmpere = null,
					ExponentOfCandela = null,
					ExponentOfKelvin = null,
					ExponentOfKilogram = null,
					ExponentOfMetre = null,
					ExponentOfMole = null,
					ExponentOfSecond = null,
					Name = null,
					Symbol = null,
					Unit = null,
					Page = 1,
					PageSize = 10
				},
				RestrictedPassportId = Guid.Empty
			};

			PhysicalDimensionByFilterQueryHandler hdlQuery = new PhysicalDimensionByFilterQueryHandler(
				repoPhysicalDimension: fxtPhysicalData.PhysicalDimensionRepository);

			// Act
			IMessageResult<PhysicalDimensionByFilterResult> rsltQuery = await hdlQuery.Handle(qryByFilter, CancellationToken.None);

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
					rsltPhysicalDimension.PhysicalDimension.Should().ContainEquivalentOf(pdPhysicalDimension);

					return true;
				});

			//Clean up
			await fxtPhysicalData.PhysicalDimensionRepository.DeleteAsync(pdPhysicalDimension, CancellationToken.None);
		}
	}
}