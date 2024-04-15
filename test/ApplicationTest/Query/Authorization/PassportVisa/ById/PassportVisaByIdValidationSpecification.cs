using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.Authorization.PassportVisa.ById;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.PassportVisa.ById
{
	public sealed class PassportVisaByIdValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportVisaByIdValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenPassportVisaIdExists()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
			await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			PassportVisaByIdQuery qryById = new PassportVisaByIdQuery()
			{
				PassportVisaId = ppVisa.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportVisaByIdQuery> hndlValidation = new PassportVisaByIdValidation(
				repoVisa: fxtAuthorizationData.PassportVisaRepository,
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
			await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			Guid guPassportVisaId = Guid.NewGuid();

			PassportVisaByIdQuery qryById = new PassportVisaByIdQuery()
			{
				PassportVisaId = guPassportVisaId,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportVisaByIdQuery> hndlValidation = new PassportVisaByIdValidation(
				repoVisa: fxtAuthorizationData.PassportVisaRepository,
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
					msgError.Description.Should().Contain($"Passport visa {guPassportVisaId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPassportVisaIdIsEmpty()
		{
			// Arrange
			PassportVisaByIdQuery qryById = new PassportVisaByIdQuery()
			{
				PassportVisaId = Guid.Empty,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportVisaByIdQuery> hndlValidation = new PassportVisaByIdValidation(
				repoVisa: fxtAuthorizationData.PassportVisaRepository,
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
					msgError.Description.Should().Contain($"Passport visa identifier is invalid (empty).");

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
