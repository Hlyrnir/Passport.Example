using Application.Command.Authorization.PassportHolder.ConfirmPhoneNumber;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportHolder.ConfirmPhoneNumber
{
    public class ConfirmPhoneNumberCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public ConfirmPhoneNumberCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPhoneNumberIsConfirmedIsUpdated()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			ConfirmPhoneNumberCommand cmdUpdate = new ConfirmPhoneNumberCommand()
			{
				ConcurrencyStamp = ppHolder.ConcurrencyStamp,
				PassportHolderId = ppHolder.Id,
				PhoneNumber = ppHolder.PhoneNumber,
				RestrictedPassportId = Guid.NewGuid()
			};

			ConfirmPhoneNumberCommandHandler hdlCommand = new ConfirmPhoneNumberCommandHandler(
				prvTime: prvTime,
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
				ppSetting: fxtAuthorizationData.PassportSetting);

			// Act
			IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

			//Assert
			await rsltUpdate.MatchAsync(
				msgError =>
				{
					msgError.Should().BeNull();

					return false;
				},
				async bResult =>
				{
					bResult.Should().BeTrue();

					IRepositoryResult<IPassportHolder> rsltHolder = await fxtAuthorizationData.PassportHolderRepository.FindByIdAsync(ppHolder.Id, CancellationToken.None);

					return rsltHolder.Match(
						msgError => false,
						ppHolderInRepository =>
						{
							ppHolderInRepository.CultureName.Should().Be(ppHolder.CultureName);
							ppHolderInRepository.EmailAddress.Should().Be(ppHolder.EmailAddress);
							ppHolderInRepository.EmailAddressIsConfirmed.Should().BeFalse();
							ppHolderInRepository.FirstName.Should().Be(ppHolder.FirstName);
							ppHolderInRepository.LastName.Should().Be(ppHolder.LastName);
							ppHolderInRepository.PhoneNumber.Should().Be(ppHolder.PhoneNumber);
							ppHolderInRepository.PhoneNumberIsConfirmed.Should().BeTrue();

							return true;
						});
				});

			//Clean up
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}

        [Fact]
        public async Task Update_ShouldReturnRepositoryError_WhenConcurrencyStampDoNotMatch()
        {
            // Arrange
            IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
            await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

            string sObsoleteConcurrencyStamp = Guid.NewGuid().ToString();


            ConfirmPhoneNumberCommand cmdUpdate = new ConfirmPhoneNumberCommand()
            {
                ConcurrencyStamp = sObsoleteConcurrencyStamp,
                PassportHolderId = ppHolder.Id,
                PhoneNumber = ppHolder.PhoneNumber,
                RestrictedPassportId = Guid.NewGuid()
            };

            ConfirmPhoneNumberCommandHandler hdlCommand = new ConfirmPhoneNumberCommandHandler(
                prvTime: prvTime,
                repoHolder: fxtAuthorizationData.PassportHolderRepository,
                ppSetting: fxtAuthorizationData.PassportSetting);

            // Act
            IMessageResult<bool> rsltUpdate = await hdlCommand.Handle(cmdUpdate, CancellationToken.None);

            //Assert
            rsltUpdate.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Should().Be(DefaultMessageError.ConcurrencyViolation);

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return false;
                });

            //Clean up
            await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
        }
    }
}