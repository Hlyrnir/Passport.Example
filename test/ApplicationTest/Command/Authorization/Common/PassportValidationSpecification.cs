using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Validation.Passport;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.Common
{
    public sealed class PassportValidationSpecification : IClassFixture<PassportFixture>
	{
		private readonly PassportFixture fxtAuthorizationData;

		public PassportValidationSpecification(PassportFixture fxtAuthorizationData)
		{
			this.fxtAuthorizationData = fxtAuthorizationData;
		}

		[Fact]
		public void Validate_ShouldReturnTrue_WhenNoValidationErrorExists()
		{
			// Arrange

			// Act
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Assert
			srvValidation.IsValid.Should().BeTrue();
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenValidationHasFailed()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			string sErrorDescription = Guid.NewGuid().ToString();

			// Act
			srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = sErrorDescription });

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain(sErrorDescription);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenCredentialIsEmpty()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create("", ".");

			// Act
			srvValidation.ValidateCredential(ppCredential.Credential, "Credential");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Credential is not valid (empty).");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenCredentialHasTooLessCharacters()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create(".", ".");

			// Act
			srvValidation.ValidateCredential(ppCredential.Credential, "Credential");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Credential has less than {fxtAuthorizationData.PassportSetting.RequiredMinimalCredentialLength} characters.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenCredentialHasTooManyCharacters()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create("THIS_CREDENTIAL_IS_TOO_LONG_AND_HAS_TOO_MANY_CHARACTERS!_PLEASE_CHOOSE_ANOTHER_ONE!", ".");

			// Act
			srvValidation.ValidateCredential(ppCredential.Credential, "Credential");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Credential has more than {fxtAuthorizationData.PassportSetting.MaximalCredentialLength} characters.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenProviderIsEmpty()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateAtProvider(".", "", ".");

			// Act
			srvValidation.ValidateProvider(ppCredential.Provider, "Provider");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Provider is not valid (empty).");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenProviderIsUnknown()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.CreateAtProvider(".", "UNKNOWN", ".");

			// Act
			srvValidation.ValidateProvider(ppCredential.Provider, "Provider");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Provider is unknown.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenSignatureIsEmpty()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create(".", "");

			// Act
			srvValidation.ValidateSignature(ppCredential.Signature, "Signature");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Signature is not valid (empty).");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenSignatureHasTooLessCharacters()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create(".", ".");

			// Act
			srvValidation.ValidateSignature(ppCredential.Signature, "Signature");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Signature has less than {fxtAuthorizationData.PassportSetting.RequiredMinimalSignatureLength} characters.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenSignatureHasTooManyCharacters()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);
			IPassportCredential ppCredential = DataFaker.PassportCredential.Create(".", "THIS_SIGNATURE_IS_TOO_LONG_AND_HAS_TOO_MANY_CHARACTERS!_PLEASE_CHOOSE_ANOTHER_ONE!");

			// Act
			srvValidation.ValidateSignature(ppCredential.Signature, "Signature");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Signature has more than {fxtAuthorizationData.PassportSetting.MaximalSignatureLength} characters.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenEmailAddressIsEmpty()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Act
			srvValidation.ValidateEmailAddress("", "Email address");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Email address is not valid (empty).");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenEmailAddressHasNoAt()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Act
			srvValidation.ValidateEmailAddress("defaultATpassport.org", "Email address");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Email address does not contain the '@' character.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenEmailAddressHasNoDot()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Act
			srvValidation.ValidateEmailAddress("default@passportDOTorg", "Email address");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Email address does not contain the '.' character.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenPhoneNumberIsEmpty()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Act
			srvValidation.ValidatePhoneNumber("", "Phone number");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Phone number is not valid (empty).");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenPhoneNumberHasTooLessCharacters()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Act
			srvValidation.ValidatePhoneNumber("0", "Phone number");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain($"Phone number has less than {fxtAuthorizationData.PassportSetting.MinimalPhoneNumberLength} characters.");

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();
					return true;
				});
		}

		[Fact]
		public void Validate_ShouldReturnValidationError_WhenPhoneNumberHasNoDigits()
		{
			// Arrange
			IPassportValidation srvValidation = new PassportValidation(fxtAuthorizationData.PassportSetting);

			// Act
			srvValidation.ValidatePhoneNumber("PHONE_NUMBER", "Phone number");

			// Assert
			srvValidation.Match(
				msgError =>
				{
					msgError.Should().NotBeNull();
					msgError.Code.Should().Be(ValidationError.Code.Method);
					msgError.Description.Should().Contain("Phone number contains no digits.");

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