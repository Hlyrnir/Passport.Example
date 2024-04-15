using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportHolder
{
    public class PassportHolderRepositorySpecification_FindAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportHolderRepositorySpecification_FindAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task FindById_ShouldFindPassport_WhenPassportIdExists()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);

			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppHolder.Id, CancellationToken.None);

			// Assert
			rsltHolder.Match<bool>(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
					ppPassportInRepository.Should().BeEquivalentTo(ppHolder);

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}

		[Fact]
		public async Task FindById_ShouldReturnRepositoryError_WhenPassportIdDoesNotExist()
		{
			// Arrange
			Guid guHolderId = Guid.NewGuid();

			// Act
			IRepositoryResult<IPassportHolder> rsltPassport = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(guHolderId, CancellationToken.None);

			// Assert
			rsltPassport.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"No data for {guHolderId} has been found.");

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