using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.Authorization.Passport.ById;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.Passport.ById
{
    public class PassportByIdQueryHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportByIdQueryHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnPassport_WhenPassportExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			PassportByIdQuery qryById = new PassportByIdQuery()
			{
				PassportId = ppPassport.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportByIdQueryHandler hdlQuery = new PassportByIdQueryHandler(fxtAuthorizationData.PassportRepository);

			// Act
			IMessageResult<PassportByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				qryResult =>
				{
					qryResult.Passport.Should().NotBeNull();
					qryResult.Passport.Should().BeEquivalentTo(ppPassport);

					return true;
				});

			//Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			PassportByIdQuery qryById = new PassportByIdQuery()
			{
				PassportId = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportByIdQueryHandler hdlQuery = new PassportByIdQueryHandler(fxtAuthorizationData.PassportRepository);

			// Act
			IMessageResult<PassportByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.Passport.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.Passport.NotFound.Description);

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().BeNull();

					return true;
				});
		}
	}
}