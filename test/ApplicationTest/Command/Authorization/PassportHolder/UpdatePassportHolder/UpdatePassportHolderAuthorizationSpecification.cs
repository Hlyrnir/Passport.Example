﻿using Application.Command.Authorization.PassportHolder.UpdatePassportHolder;
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

namespace ApplicationTest.Command.Authorization.PassportHolder.UpdatePassportHolder
{
    public class UpdatePassportHolderAuthorizationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public UpdatePassportHolderAuthorizationSpecification(PassportFixture fxtAuthorizationData)
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
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			UpdatePassportHolderCommand cmdUpdate = new UpdatePassportHolderCommand()
			{
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
				FirstName = "Jane",
				LastName = "Doe",
				PassportHolderId = Guid.NewGuid(),
				PhoneNumber = "111",
				RestrictedPassportId = ppPassport.Id
			};

			IAuthorization<UpdatePassportHolderCommand> hndlAuthorization = new UpdatePassportHolderAuthorization(fxtAuthorizationData.PassportVisaRepository);

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
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateDefault();
			await fxtAuthorizationData.PassportTokenRepository.InsertAsync(ppToken, ppCredential, prvTime.GetUtcNow(), CancellationToken.None);

			UpdatePassportHolderCommand cmdUpdate = new UpdatePassportHolderCommand()
			{
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				CultureName = "en-GB",
				EmailAddress = "default@ema.il",
				FirstName = "Jane",
				LastName = "Doe",
				PassportHolderId = Guid.NewGuid(),
				PhoneNumber = "111",
				RestrictedPassportId = ppPassport.Id
			};

			IAuthorization<UpdatePassportHolderCommand> hndlAuthorization = new UpdatePassportHolderAuthorization(fxtAuthorizationData.PassportVisaRepository);

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