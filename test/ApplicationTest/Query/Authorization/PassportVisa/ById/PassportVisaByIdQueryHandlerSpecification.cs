using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.Authorization.PassportVisa.ById;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.PassportVisa.ById
{
	public class PassportVisaByIdQueryHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportVisaByIdQueryHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnPassport_WhenPassportExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			PassportVisaByIdQuery qryById = new PassportVisaByIdQuery()
			{
				PassportVisaId = ppVisa.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportVisaByIdQueryHandler hdlQuery = new PassportVisaByIdQueryHandler(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<PassportVisaByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

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
					qryResult.PassportVisa.Should().BeEquivalentTo(ppVisa);

					return true;
				});

			//Clean up
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			PassportVisaByIdQuery qryById = new PassportVisaByIdQuery()
			{
				PassportVisaId = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportVisaByIdQueryHandler hdlQuery = new PassportVisaByIdQueryHandler(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<PassportVisaByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PassportVisa.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportVisa.NotFound.Description);

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