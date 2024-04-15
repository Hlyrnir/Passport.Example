using Application.Command.Authorization.PassportToken.ResetCredential;
using Application.Common.Authorization;
using Application.Error;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportToken.ResetCredential
{
    public class ResetCredentialAuthorizationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public ResetCredentialAuthorizationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportIdIsAuthorized()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Update);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryAddVisa(ppVisa);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.CreateDefault();

			ResetCredentialCommand cmdUpdate = new ResetCredentialCommand()
			{
				CredentialToApply = ppCredentialToApply,
				CredentialToVerify = ppCredentialToVerify,
				RestrictedPassportId = ppPassport.Id
			};

			IAuthorization<ResetCredentialCommand> hndlAuthorization = new ResetCredentialAuthorization(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdUpdate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
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

			//Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Update_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.Passport, AuthorizationDefault.Level.Update);
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			ppPassport.TryAddVisa(ppVisa);
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportToken ppToken = DataFaker.PassportToken.CreateDefault(ppPassport.Id);
			IPassportCredential ppCredentialToVerify = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredentialToVerify, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportCredential ppCredentialToApply = DataFaker.PassportCredential.CreateDefault();

			ResetCredentialCommand cmdUpdate = new ResetCredentialCommand()
			{
				CredentialToApply = ppCredentialToApply,
				CredentialToVerify = ppCredentialToVerify,
				RestrictedPassportId = ppPassport.Id
			};

			IAuthorization<ResetCredentialCommand> hndlAuthorization = new ResetCredentialAuthorization(fxtAuthorizationData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdUpdate,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();

					msgError.Code.Should().Be(AuthorizationError.Code.Method);
					msgError.Description.Should().Be("Passport has no valid visa for this request.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			//Clean up
			await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None);
			await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None);
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}
	}
}