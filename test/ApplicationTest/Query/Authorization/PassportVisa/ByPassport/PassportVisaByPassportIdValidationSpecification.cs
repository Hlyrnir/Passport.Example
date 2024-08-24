using Application.Common.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.Authorization.PassportVisa.ByPassport;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.PassportVisa.ById
{
    public sealed class PassportVisaByPassportIdValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportVisaByPassportIdValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenPassportIdExists()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			await fxtAuthorizationData.PassportRepository.InsertAsync(ppPassport, prvTime.GetUtcNow(), CancellationToken.None);

			PassportVisaByPassportIdQuery qryById = new PassportVisaByPassportIdQuery()
			{
				PassportIdToFind = ppPassport.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportVisaByPassportIdQuery> hndlValidation = new PassportVisaByPassportIdValidation(
				repoPassport: fxtAuthorizationData.PassportRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: fxtAuthorizationData.TimeProvider);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryById,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
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
		public async Task Read_ShouldReturnMessageError_WhenPassportDoesNotExist()
		{
			// Arrange
			Guid guPassportId = Guid.NewGuid();

			PassportVisaByPassportIdQuery qryById = new PassportVisaByPassportIdQuery()
			{
				PassportIdToFind = guPassportId,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportVisaByPassportIdQuery> hndlValidation = new PassportVisaByPassportIdValidation(
				repoPassport: fxtAuthorizationData.PassportRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: fxtAuthorizationData.TimeProvider);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryById,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Passport {guPassportId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPassportIdIsEmpty()
		{
			// Arrange
			PassportVisaByPassportIdQuery qryById = new PassportVisaByPassportIdQuery()
			{
				PassportIdToFind = Guid.Empty,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportVisaByPassportIdQuery> hndlValidation = new PassportVisaByPassportIdValidation(
				repoPassport: fxtAuthorizationData.PassportRepository,
				srvValidation: fxtAuthorizationData.PassportValidation,
				prvTime: fxtAuthorizationData.TimeProvider);

			// Act
			IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
				msgMessage: qryById,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Passport identifier is invalid (empty).");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}
	}
}
