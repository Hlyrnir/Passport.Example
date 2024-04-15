using Application.Error;
using Application.Filter;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.PhysicalData.TimePeriod.ByFilter;
using ApplicationTest.Common;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.PhysicalData.TimePeriod.ByFilter
{
	public sealed class TimePeriodByFilterValidationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;
		private readonly ITimeProvider prvTime;

		public TimePeriodByFilterValidationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenFilterIsValid()
		{
			// Arrange
			TimePeriodByFilterQuery qryByFilter = new TimePeriodByFilterQuery()
			{
				Filter = new TimePeriodByFilterOption()
				{
					PhysicalDimensionId = null,
					Page = 1,
					PageSize = 10
				},
				RestrictedPassportId = Guid.Empty
			};

			IValidation<TimePeriodByFilterQuery> hndlValidation = new TimePeriodByFilterValidation(
				srvValidation: fxtPhysicalData.PhysicalDataValiation);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryByFilter,
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
		}
	}
}
