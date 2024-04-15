using Application.Interface.Result;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Xunit;

namespace InfrastructureTest.Passport.PassportVisa
{
    public class PassportVisaRepositorySpecification_ExistsAsync : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		private readonly ITimeProvider prvTime;

		public PassportVisaRepositorySpecification_ExistsAsync(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			this.prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Exists_ShouldReturnTrue_WhenAllVisaIdExist()
		{
			// Arrange
			IPassportVisa ppVisa_01 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_02 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_03 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_04 = DataFaker.PassportVisa.CreateDefault();

			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			ppPassport.TryAddVisa(ppVisa_01);
			ppPassport.TryAddVisa(ppVisa_02);
			ppPassport.TryAddVisa(ppVisa_03);
			ppPassport.TryAddVisa(ppVisa_04);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_01, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_02, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_03, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_04, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.ExistsAsync(ppPassport.VisaId, CancellationToken.None);

			// Assert
			rsltVisa.Match(
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
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_01, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_02, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_03, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_04, CancellationToken.None);
		}

		[Fact]
		public async Task Exists_ShouldReturnFalse_WhenNotAllVisaIdExist()
		{
			// Arrange
			IPassportVisa ppVisa_01 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_02 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_03 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_04 = DataFaker.PassportVisa.CreateDefault();
			IPassportVisa ppVisa_DoesNotExists = DataFaker.PassportVisa.CreateDefault();

			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			ppPassport.TryAddVisa(ppVisa_01);
			ppPassport.TryAddVisa(ppVisa_02);
			ppPassport.TryAddVisa(ppVisa_03);
			ppPassport.TryAddVisa(ppVisa_04);
			ppPassport.TryAddVisa(ppVisa_DoesNotExists);

			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_01, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_02, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_03, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa_04, prvTime.GetUtcNow(), CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			// Act
			IRepositoryResult<bool> rsltVisa = await fxtAuthorizationData.PassportVisaRepository.ExistsAsync(ppPassport.VisaId, CancellationToken.None);

			// Assert
			rsltVisa.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_01, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_02, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_03, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_04, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa_DoesNotExists, CancellationToken.None);
		}
	}
}