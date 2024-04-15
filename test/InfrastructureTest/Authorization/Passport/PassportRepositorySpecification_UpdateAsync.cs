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
    public class PassportRepositorySpecification_UpdateAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportRepositorySpecification_UpdateAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportIsUpdated()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppPassport.TryDisable(ppPassport, prvTime.GetUtcNow());

			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
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
			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			await rsltPassport.MatchAsync<bool>(
				msgError => false,
				async ppPassport =>
				{
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
					return true;
				});
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportWithVisaIsUpdated()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppPassport.TryAddVisa(ppVisa);

			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
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
			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			await rsltPassport.MatchAsync<bool>(
				msgError => false,
				async ppPassport =>
				{
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
					return true;
				});

			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldChangeConcurrencyStamp_WhenPassportIsUpdated()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			ppPassport.TryDisable(ppPassport, prvTime.GetUtcNow());

			await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			IRepositoryResult<IPassport> rsltUpdate = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
					ppPassportInRepository.ConcurrencyStamp.Should().NotBe(ppPassport.ConcurrencyStamp);
					ppPassportInRepository.ExpiredAt.Should().Be(ppPassport.ExpiredAt);
					ppPassportInRepository.HolderId.Should().Be(ppPassport.HolderId);
					ppPassportInRepository.Id.Should().Be(ppPassport.Id);
					ppPassportInRepository.IssuedBy.Should().Be(ppPassport.IssuedBy);
					ppPassportInRepository.LastCheckedAt.Should().Be(ppPassport.LastCheckedAt);
					ppPassportInRepository.LastCheckedBy.Should().Be(ppPassport.LastCheckedBy);
					return true;
				});

			// Clean up
			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			await rsltPassport.MatchAsync<bool>(
				msgError => false,
				async ppPassport =>
				{
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
					return true;
				});
		}

		[Fact]
		public async Task Update_ShouldReturnFalse_WhenConcurrencyStampIsDifferent()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassport ppPassportToUpdate = DataFaker.Passport.Clone(ppPassportToClone: ppPassport, bResetConcurrencyStamp: true);

			// Act
			IRepositoryResult<bool> rsltUpdate = await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassportToUpdate, prvTime.GetUtcNow(), CancellationToken.None);

			// Assert
			rsltUpdate.Match<bool>(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(PassportError.Code.Method);
					msgError.Description.Should().Be($"Could not update passport {ppPassportToUpdate.Id}.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassportToUpdate.Id, CancellationToken.None);

			await rsltPassport.MatchAsync<bool>(
				msgError => false,
				async ppPassport =>
				{
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
					return true;
				});
		}

		[Fact]
		public async Task Update_ShouldNotContainVisaId_WhenVisaIsRemoved()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassportVisa ppVisa_01 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_02 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_03 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_04 = DataFaker.PassportVisa.CreateDefault();

			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_01, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_02, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_03, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_04, prvTime.GetUtcNow(), CancellationToken.None);

			ppPassport.TryAddVisa(ppVisa_01);
			ppPassport.TryAddVisa(ppVisa_02);
			ppPassport.TryAddVisa(ppVisa_03);
			ppPassport.TryAddVisa(ppVisa_04);

			await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			await rsltPassport.MatchAsync<bool>(
				msgError => false,
				async ppPassportInRepository =>
				{
					ppPassportInRepository.TryRemoveVisa(ppVisa_01);
					
					await fxtAuthorizationData.PassportRepository.UpdateAsync(ppPassportInRepository, prvTime.GetUtcNow(), CancellationToken.None);

					return true;
				});

			// Assert
			IRepositoryResult<IPassport> rsltUpdate = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			rsltUpdate.Match<bool>(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				ppPassportInRepository =>
				{
					ppPassportInRepository.Should().NotBeNull();
					ppPassportInRepository.VisaId.Should().NotContain(ppVisa_01.Id);
					ppPassportInRepository.VisaId.Should().Contain(ppVisa_02.Id);
					ppPassportInRepository.VisaId.Should().Contain(ppVisa_03.Id);
					ppPassportInRepository.VisaId.Should().Contain(ppVisa_04.Id);

					return true;
				});

			// Clean up
			IRepositoryResult<IPassport> rsltPassportToDelete = await fxtAuthorizationData.PassportRepository.FindByIdAsync(ppPassport.Id, CancellationToken.None);

			await rsltPassportToDelete.MatchAsync<bool>(
				msgError => false,
				async ppPassportToDelete =>
				{
					await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassportToDelete, CancellationToken.None);
					return true;
				});

			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_01, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_02, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_03, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_04, CancellationToken.None);
		}
	}
}
