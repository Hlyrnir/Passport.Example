using Application.Command.Authorization.Passport.Register;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Passport.RegisterPassport
{
    public class RegisterPassportCommandHandlerSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public RegisterPassportCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Register_ShouldReturnTrue_WhenPassportIsCreated()
        {
            // Arrange
            IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

            RegisterPassportCommand cmdRegister = new RegisterPassportCommand()
            {
                CredentialToRegister = ppCredential,
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
                FirstName = "Jane",
                LastName = "Doe",
                IssuedBy = Guid.NewGuid(),
				PhoneNumber = "111",
				RestrictedPassportId = Guid.NewGuid()
            };

            RegisterPassportCommandHandler hdlCommand = new RegisterPassportCommandHandler(
                prvTime: prvTime,
                uowUnitOfWork: fxtAuthorizationData.UnitOfWork,
                ppSetting: fxtAuthorizationData.PassportSetting,
                repoPassport: fxtAuthorizationData.PassportRepository,
                repoHolder: fxtAuthorizationData.PassportHolderRepository,
                repoToken: fxtAuthorizationData.PassportTokenRepository);

            // Act
            IMessageResult<Guid> rsltPassportId = await hdlCommand.Handle(cmdRegister, CancellationToken.None);

            Guid guPassportId = Guid.Empty;
            Guid guPassportHolderId = Guid.Empty;
            Guid guPassportTokenId = Guid.Empty;

            //Assert
            await rsltPassportId.MatchAsync(
                msgError =>
                {
                    msgError.Should().BeNull();
                    return false;
                },
                async guPassportIdInRepository =>
                {
                    guPassportId = guPassportIdInRepository;

                    IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(guPassportIdInRepository, CancellationToken.None);

                    return await rsltPassport.MatchAsync(
                        msgError =>
                        {
                            msgError.Should().BeNull();
                            return false;
                        },
                        async ppPassportInRepository =>
                        {
                            ppPassportInRepository.Id.Should().Be(guPassportIdInRepository);
                            ppPassportInRepository.ExpiredAt.Should().Be(prvTime.GetUtcNow().Add(fxtAuthorizationData.PassportSetting.ExpiresAfterDuration));
                            ppPassportInRepository.IsAuthority.Should().BeFalse();
                            ppPassportInRepository.IsEnabled.Should().BeFalse();
                            ppPassportInRepository.IssuedBy.Should().Be(cmdRegister.IssuedBy);
                            ppPassportInRepository.LastCheckedAt.Should().Be(prvTime.GetUtcNow());
                            ppPassportInRepository.LastCheckedBy.Should().Be(cmdRegister.IssuedBy);
                            ppPassportInRepository.VisaId.Should().BeEmpty();

                            IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppPassportInRepository.HolderId, CancellationToken.None);

                            rsltHolder.Match(
                                msgError =>
                                {
                                    return false;
                                },
                                ppHolder =>
                                {
                                    guPassportHolderId = ppHolder.Id;

                                    ppHolder.CultureName.Should().Be(cmdRegister.CultureName);
                                    ppHolder.EmailAddress.Should().Be(cmdRegister.EmailAddress);
                                    ppHolder.EmailAddressIsConfirmed.Should().BeFalse();
                                    ppHolder.FirstName.Should().Be(cmdRegister.FirstName);
                                    ppHolder.LastName.Should().Be(cmdRegister.LastName);
                                    ppHolder.PhoneNumber.Should().Be(cmdRegister.PhoneNumber);
                                    ppHolder.PhoneNumberIsConfirmed.Should().BeFalse();

                                    return true;
                                });

                            IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

                            rsltToken.Match(
                                msgError =>
                                {
                                    return false;
                                },
                                ppToken =>
                                {
                                    guPassportTokenId = ppToken.Id;

                                    ppToken.PassportId.Should().Be(guPassportId);
                                    ppToken.Provider.Should().Be(ppCredential.Provider);
                                    ppToken.TwoFactorIsEnabled.Should().BeFalse();
                                    return true;
                                });

                            return true;
                        });
                });

            //Clean up
            IRepositoryResult<IPassport> rsltPassportToDelete = await fxtAuthorizationData.PassportRepository.FindByIdAsync(guPassportId, CancellationToken.None);

            await rsltPassportToDelete.MatchAsync(
                msgError => false,
                async ppPassport => await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None));

            IRepositoryResult<IPassportHolder> rsltHolderToDelete = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(guPassportHolderId, CancellationToken.None);

            await rsltHolderToDelete.MatchAsync(
                msgError => false,
                async ppHolder => await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None));

            IRepositoryResult<IPassportToken> rsltTokenToDelete = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

            await rsltTokenToDelete.MatchAsync(
                msgError => false,
                async ppToken => await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None));
        }

		//[Fact]
		//public async Task Register_ShouldReturnMessageError_WhenCredentialExists()
		//{
		//	// Arrange
		//	IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();

		//	RegisterPassportCommand cmdRegister = new RegisterPassportCommand()
		//	{
		//		CredentialToRegister = ppCredential,
		//		CultureName = "en-GB",
		//		EmailAddress = "default@ema.il",
		//		FirstName = "Jane",
		//		LastName = "Doe",
		//		IssuedBy = Guid.NewGuid(),
		//		PhoneNumber = "111",
		//		RestrictedPassportId = Guid.NewGuid()
		//	};

		//	RegisterPassportCommandHandler hdlCommand = new RegisterPassportCommandHandler(
		//		prvTime: prvTime,
		//		uowUnitOfWork: fxtAuthorizationData.UnitOfWork,
		//		ppSetting: fxtAuthorizationData.PassportSetting,
		//		repoPassport: fxtAuthorizationData.PassportRepository,
		//		repoHolder: fxtAuthorizationData.PassportHolderRepository,
		//		repoToken: fxtAuthorizationData.PassportTokenRepository);

		//	// Act
		//	IMessageResult<Guid> rsltPassportId = await hdlCommand.Handle(cmdRegister, CancellationToken.None);

		//	Guid guPassportId = Guid.Empty;
		//	Guid guPassportHolderId = Guid.Empty;
		//	Guid guPassportTokenId = Guid.Empty;

		//	//Assert
		//	await rsltPassportId.MatchAsync(
		//		msgError =>
		//		{
		//			msgError.Should().BeNull();
		//			return false;
		//		},
		//		async guPassportIdInRepository =>
		//		{
		//			guPassportId = guPassportIdInRepository;

		//			IRepositoryResult<IPassport> rsltPassport = await fxtAuthorizationData.PassportRepository.FindByIdAsync(guPassportIdInRepository, CancellationToken.None);

		//			return await rsltPassport.MatchAsync(
		//				msgError =>
		//				{
		//					msgError.Should().BeNull();
		//					return false;
		//				},
		//				async ppPassportInRepository =>
		//				{
		//					ppPassportInRepository.Id.Should().Be(guPassportIdInRepository);
		//					ppPassportInRepository.ExpiredAt.Should().Be(prvTime.GetUtcNow().Add(fxtAuthorizationData.PassportSetting.ExpiresAfterDuration));
		//					ppPassportInRepository.IsAuthority.Should().BeFalse();
		//					ppPassportInRepository.IsEnabled.Should().BeFalse();
		//					ppPassportInRepository.IssuedBy.Should().Be(cmdRegister.IssuedBy);
		//					ppPassportInRepository.LastCheckedAt.Should().Be(prvTime.GetUtcNow());
		//					ppPassportInRepository.LastCheckedBy.Should().Be(cmdRegister.IssuedBy);
		//					ppPassportInRepository.VisaId.Should().BeEmpty();

		//					IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppPassportInRepository.HolderId, CancellationToken.None);

		//					rsltHolder.Match(
		//						msgError =>
		//						{
		//							return false;
		//						},
		//						ppHolder =>
		//						{
		//							guPassportHolderId = ppHolder.Id;

		//							ppHolder.CultureName.Should().Be(cmdRegister.CultureName);
		//							ppHolder.EmailAddress.Should().Be(cmdRegister.EmailAddress);
		//							ppHolder.EmailAddressIsConfirmed.Should().BeFalse();
		//							ppHolder.FirstName.Should().Be(cmdRegister.FirstName);
		//							ppHolder.LastName.Should().Be(cmdRegister.LastName);
		//							ppHolder.PhoneNumber.Should().Be(cmdRegister.PhoneNumber);
		//							ppHolder.PhoneNumberIsConfirmed.Should().BeFalse();

		//							return true;
		//						});

		//					IRepositoryResult<IPassportToken> rsltToken = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

		//					rsltToken.Match(
		//						msgError =>
		//						{
		//							return false;
		//						},
		//						ppToken =>
		//						{
		//							guPassportTokenId = ppToken.Id;

		//							ppToken.PassportId.Should().Be(guPassportId);
		//							ppToken.Provider.Should().Be(ppCredential.Provider);
		//							ppToken.TwoFactorIsEnabled.Should().BeFalse();
		//							return true;
		//						});

		//					return true;
		//				});
		//		});

		//	//Clean up
		//	IRepositoryResult<IPassport> rsltPassportToDelete = await fxtAuthorizationData.PassportRepository.FindByIdAsync(guPassportId, CancellationToken.None);

		//	await rsltPassportToDelete.MatchAsync(
		//		msgError => false,
		//		async ppPassport => await fxtAuthorizationData.PassportRepository.DeleteAsync(ppPassport, CancellationToken.None));

		//	IRepositoryResult<IPassportHolder> rsltHolderToDelete = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(guPassportHolderId, CancellationToken.None);

		//	await rsltHolderToDelete.MatchAsync(
		//		msgError => false,
		//		async ppHolder => await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None));

		//	IRepositoryResult<IPassportToken> rsltTokenToDelete = await fxtAuthorizationData.PassportTokenRepository.FindTokenByCredentialAsync(ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

		//	await rsltTokenToDelete.MatchAsync(
		//		msgError => false,
		//		async ppToken => await fxtAuthorizationData.PassportTokenRepository.DeleteAsync(ppToken, CancellationToken.None));
		//}
	}
}