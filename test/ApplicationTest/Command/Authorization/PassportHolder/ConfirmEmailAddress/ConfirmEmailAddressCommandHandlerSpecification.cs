using Application.Command.Authorization.PassportHolder.ConfirmEmailAddress;
using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportHolder.ConfirmEmailAddress
{
    public class ConfirmEmailAddressCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public ConfirmEmailAddressCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenEmailAddressIsConfirmed()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			ConfirmEmailAddressCommand cmdUpdate = new ConfirmEmailAddressCommand()
			{
				ConcurrencyStamp = ppHolder.ConcurrencyStamp,
				EmailAddress = ppHolder.EmailAddress,
				PassportHolderId = ppHolder.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			ConfirmEmailAddressCommandHandler hdlCommand = new ConfirmEmailAddressCommandHandler(
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
							ppHolderInRepository.EmailAddressIsConfirmed.Should().BeTrue();
							ppHolderInRepository.FirstName.Should().Be(ppHolder.FirstName);
							ppHolderInRepository.LastName.Should().Be(ppHolder.LastName);
							ppHolderInRepository.PhoneNumber.Should().Be(ppHolder.PhoneNumber);
							ppHolderInRepository.PhoneNumberIsConfirmed.Should().BeFalse();

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

			ConfirmEmailAddressCommand cmdUpdate = new ConfirmEmailAddressCommand()
			{
				ConcurrencyStamp = sObsoleteConcurrencyStamp,
				EmailAddress = ppHolder.EmailAddress,
				PassportHolderId = ppHolder.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

            ConfirmEmailAddressCommandHandler hdlCommand = new ConfirmEmailAddressCommandHandler(
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