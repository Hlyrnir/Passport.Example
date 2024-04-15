using Application.Interface.Result;
using Application.Interface.Time;
using Application.Query.Authorization.PassportHolder.ById;
using ApplicationTest.Common;
using ApplicationTest.Error;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.PassportHolder.ById
{
	public class PassportHolderByIdQueryHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportHolderByIdQueryHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnPassportHolder_WhenHolderExists()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			PassportHolderByIdQuery qryById = new PassportHolderByIdQuery()
			{
				PassportHolderId = ppHolder.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportHolderByIdQueryHandler hdlQuery = new PassportHolderByIdQueryHandler(fxtAuthorizationData.PassportHolderRepository);

			// Act
			IMessageResult<PassportHolderByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				qryResult =>
				{
					qryResult.PassportHolder.Should().NotBeNull();
					qryResult.PassportHolder.Should().BeEquivalentTo(ppHolder);

					return true;
				});

			//Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenHolderDoesNotExist()
		{
			// Arrange
			PassportHolderByIdQuery qryById = new PassportHolderByIdQuery()
			{
				PassportHolderId = Guid.NewGuid(),
				RestrictedPassportId = Guid.NewGuid()
			};

			PassportHolderByIdQueryHandler hdlQuery = new PassportHolderByIdQueryHandler(fxtAuthorizationData.PassportHolderRepository);

			// Act
			IMessageResult<PassportHolderByIdResult> rsltQuery = await hdlQuery.Handle(qryById, CancellationToken.None);

			//Assert
			rsltQuery.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(TestError.Repository.PassportHolder.NotFound.Code);
					msgError.Description.Should().Be(TestError.Repository.PassportHolder.NotFound.Description);

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