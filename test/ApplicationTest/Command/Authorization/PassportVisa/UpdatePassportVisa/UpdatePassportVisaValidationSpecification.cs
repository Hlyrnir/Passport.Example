using Application.Command.Authorization.PassportVisa.Update;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;

namespace ApplicationTest.Command.Authorization.PassportVisa.UpdatePassportVisa
{
	public sealed class UpdatePassportVisaValidationSpecification : IClassFixture<PassportFixture>
    {
        private readonly PassportFixture fxtAuthorizationData;
        private readonly ITimeProvider prvTime;

        public UpdatePassportVisaValidationSpecification(PassportFixture fxtAuthorizationData)
        {
            this.fxtAuthorizationData = fxtAuthorizationData;
            prvTime = fxtAuthorizationData.TimeProvider;
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPassportVisaExists()
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdDelete = new UpdatePassportVisaCommand()
            {
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.Empty
            };

            IValidation<UpdatePassportVisaCommand> hndlValidation = new UpdatePassportVisaValidation(
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdDelete,
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
        public async Task Update_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
        {
            // Arrange
            UpdatePassportVisaCommand cmdDelete = new UpdatePassportVisaCommand()
            {
                Level = 0,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = Guid.NewGuid(),
                RestrictedPassportId = Guid.Empty
            };

            IValidation<UpdatePassportVisaCommand> hndlValidation = new UpdatePassportVisaValidation(
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdDelete,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain($"Passport visa {cmdDelete.PassportVisaId} does not exist.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().BeFalse();

                    return true;
                });
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, int.MaxValue)]
        public async Task Update_ShouldReturnTrue_WhenLevelIsValid(bool bExpectedResult, int iLevel)
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                Level = iLevel,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<UpdatePassportVisaCommand> hndlValidation = new UpdatePassportVisaValidation(
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }

        [Theory]
        [InlineData(false, (-1))]
        [InlineData(false, int.MinValue)]
        public async Task Update_ShouldReturnMessageError_WhenLevelIsInvalid(bool bExpectedResult, int iLevel)
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                Level = iLevel,
                Name = Guid.NewGuid().ToString(),
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            IValidation<UpdatePassportVisaCommand> hndlValidation = new UpdatePassportVisaValidation(
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            // Act
            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain("Level must be greater than or equal to zero.");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }

        [Theory]
        [InlineData(true, "THIS_IS_A_NAME")]
        [InlineData(true, "THIS_IS_A_VERY_VERY_VERY_VERY_VERY_VERY_VERY_VERY_VERY_VERY_LONG_NAME")]
        public async Task Update_ShouldReturnTrue_WhenNameIsValid(bool bExpectedResult, string sName)
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                Level = 0,
                Name = sName,
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            IValidation<UpdatePassportVisaCommand> hndlValidation = new UpdatePassportVisaValidation(
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
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
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }

        [Theory]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        public async Task Update_ShouldReturnMessageError_WhenNameIsInvalid(bool bExpectedResult, string sName)
        {
            // Arrange
            IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();
            await fxtAuthorizationData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

            UpdatePassportVisaCommand cmdUpdate = new UpdatePassportVisaCommand()
            {
                Level = 0,
                Name = sName,
                PassportVisaId = ppVisa.Id,
                RestrictedPassportId = Guid.NewGuid()
            };

            // Act
            IValidation<UpdatePassportVisaCommand> hndlValidation = new UpdatePassportVisaValidation(
                repoVisa: fxtAuthorizationData.PassportVisaRepository,
                srvValidation: fxtAuthorizationData.PassportValidation,
                prvTime: prvTime);

            IMessageResult<bool> rsltValidation = await hndlValidation.ValidateAsync(
                msgMessage: cmdUpdate,
                tknCancellation: CancellationToken.None);

            // Assert
            rsltValidation.Match(
                msgError =>
                {
                    msgError.Should().NotBeNull();
                    msgError.Code.Should().Be(ValidationError.Code.Method);
                    msgError.Description.Should().Contain("Name is invalid (empty).");

                    return false;
                },
                bResult =>
                {
                    bResult.Should().Be(bExpectedResult);

                    return true;
                });

            // Clean up
            await fxtAuthorizationData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
        }
    }
}