using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportToken
{
    public class PassportTokenRepositorySpecification_DeleteAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportTokenRepositorySpecification_DeleteAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenCredentialIsDeleted()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();
			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppAuthority, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltCredential = await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);

			// Assert
			rsltCredential.Match(
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
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppAuthority, CancellationToken.None);
		}
	}
}
