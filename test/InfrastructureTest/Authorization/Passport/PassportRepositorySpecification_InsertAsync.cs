using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Infrastructure.Error;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.Passport
{
    public class PassportRepositorySpecification_InsertAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportRepositorySpecification_InsertAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnTrue_WhenPassportIsCreated()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPassport.Match(
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
		}

		[Fact]
		public async Task InsertAsync_ShouldReturnRepositoryError_WhenPassportIdExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltPassport = await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltPassport.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not create passport {ppPassport.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
		}

		[Fact]
		public async Task InsertAsync_ShouldContainPassportVisaId_WhenPassportVisaIsAdded()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppPassport.TryAddVisa(ppVisa);
			await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			// Assert
			rsltPassport.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
					ppPassportInRepository.Should().BeEquivalentTo(ppPassport, options => options.Excluding(x => x.ConcurrencyStamp));
					ppPassportInRepository.VisaId.Should().Contain(ppVisa.Id);

					return true;
				});

			// Clean up
			IRepositoryResult<IPassport> rsltPassportToDelete = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			await rsltPassportToDelete.MatchAsync(
				msgError => false,
				async ppPassportToDelete =>
				{
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassportToDelete, CancellationToken.None);

					return true;
				});

			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}
	}
}