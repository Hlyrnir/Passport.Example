using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.Authorization.PassportVisa.ByPassport;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.PassportVisa.ByPassport
{
	public class PassportVisaByPassportIdQueryHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportVisaByPassportIdQueryHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnPassportVisa_WhenPassportExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryAddVisa(ppVisa);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			PassportVisaByPassportIdQuery qryByPassportId = new PassportVisaByPassportIdQuery()
			{
				PassportIdToFind = ppPassport.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportVisaByPassportIdQueryHandler hdlQuery = new PassportVisaByPassportIdQueryHandler(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<PassportVisaByPassportIdResult> rsltQuery = await hdlQuery.Handle(qryByPassportId, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				qryResult =>
				{
					qryResult.PassportVisa.Should().NotBeNull();
					qryResult.PassportVisa.Should().ContainEquivalentOf(ppVisa);

					return true;
				});

			//Clean up
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			PassportVisaByPassportIdQuery qryByPassportId = new PassportVisaByPassportIdQuery()
			{
				PassportIdToFind = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportVisaByPassportIdQueryHandler hdlQuery = new PassportVisaByPassportIdQueryHandler(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<PassportVisaByPassportIdResult> rsltQuery = await hdlQuery.Handle(qryByPassportId, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PassportVisa.VisaRegister.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportVisa.VisaRegister.Description);

					return false;
				},
				ppPassportVisaInRepository =>
				{
					ppPassportVisaInRepository.Should().BeNull();

					return true;
				});
		}
	}
}