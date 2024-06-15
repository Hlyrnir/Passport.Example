using Application.Command.Authorization.PassportHolder.UpdatePassportHolder;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportHolder.UpdatePassportHolder
{
    public class UpdatePassportHolderCommandHandlerSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public UpdatePassportHolderCommandHandlerSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Update_ShouldReturnTrue_WhenPassportHolderIsUpdated()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			UpdatePassportHolderCommand cmdUpdate = new UpdatePassportHolderCommand()
			{
				ConcurrencyStamp = ppHolder.ConcurrencyStamp,
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
				FirstName = "Jane",
				LastName = "Doe",
				PassportHolderId = ppHolder.Id,
				PhoneNumber = "111",
				RestrictedPassportId = Guid.NewGuid()
			};

			UpdatePassportHolderCommandHandler hdlCommand = new UpdatePassportHolderCommandHandler(
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
							ppHolderInRepository.CultureName.Should().Be(cmdUpdate.CultureName);
							ppHolderInRepository.EmailAddress.Should().Be(cmdUpdate.EmailAddress);
							ppHolderInRepository.EmailAddressIsConfirmed.Should().BeFalse();
							ppHolderInRepository.FirstName.Should().Be(cmdUpdate.FirstName);
							ppHolderInRepository.LastName.Should().Be(cmdUpdate.LastName);
							ppHolderInRepository.PhoneNumber.Should().Be(cmdUpdate.PhoneNumber);
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

            UpdatePassportHolderCommand cmdUpdate = new UpdatePassportHolderCommand()
            {
                ConcurrencyStamp = sObsoleteConcurrencyStamp,
                CultureName = "en-GB",
                EmailAddress = "default@ema.il",
                FirstName = "Jane",
                LastName = "Doe",
                PassportHolderId = ppHolder.Id,
                PhoneNumber = "111",
                RestrictedPassportId = Guid.NewGuid()
            };

            UpdatePassportHolderCommandHandler hdlCommand = new UpdatePassportHolderCommandHandler(
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