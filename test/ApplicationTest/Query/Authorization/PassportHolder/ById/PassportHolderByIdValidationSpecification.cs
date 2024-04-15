using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using Application.Query.Authorization.PassportHolder.ById;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Query.Authorization.PassportHolder.ById
{
	public sealed class PassportHolderByIdValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;
		private readonly ITimeProvider prvTime;

		public PassportHolderByIdValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
			prvTime = fxtAuthorizationData.TimeProvider;
		}

		[Fact]
		public async Task Read_ShouldReturnTrue_WhenPassportHolderIdExists()
		{
			// Arrange
			IPassportHolder ppHolder = DataFaker.PassportHolder.CreateDefault(fxtAuthorizationData.PassportSetting);
			await fxtAuthorizationData.PassportHolderRepository.InsertAsync(ppHolder, prvTime.GetUtcNow(), CancellationToken.None);

			PassportHolderByIdQuery qryById = new PassportHolderByIdQuery()
			{
				PassportHolderId = ppHolder.Id,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportHolderByIdQuery> hndlValidation = new PassportHolderByIdValidation(
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
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
			await fxtAuthorizationData.PassportHolderRepository.DeleteAsync(ppHolder, CancellationToken.None);
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPassportHolderDoesNotExist()
		{
			// Arrange
			Guid guPassportHolderId = Guid.NewGuid();

			PassportHolderByIdQuery qryById = new PassportHolderByIdQuery()
			{
				PassportHolderId = guPassportHolderId,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportHolderByIdQuery> hndlValidation = new PassportHolderByIdValidation(
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
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
					msgError.Description.Should().Contain($"Passport holder {guPassportHolderId} does not exist.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Fact]
		public async Task Read_ShouldReturnMessageError_WhenPassportHolderIdIsEmpty()
		{
			// Arrange
			PassportHolderByIdQuery qryById = new PassportHolderByIdQuery()
			{
				PassportHolderId = Guid.Empty,
				RestrictedPassportId = Guid.NewGuid()
			};

			IValidation<PassportHolderByIdQuery> hndlValidation = new PassportHolderByIdValidation(
				repoHolder: fxtAuthorizationData.PassportHolderRepository,
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
					msgError.Description.Should().Contain($"Passport holder identifier is invalid (empty).");

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
