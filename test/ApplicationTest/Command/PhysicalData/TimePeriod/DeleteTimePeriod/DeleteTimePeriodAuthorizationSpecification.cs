using Application.Command.PhysicalData.TimePeriod.Delete;
using Application.Common.Authorization;
using Application.Common.Error;
using Application.Interface.Authorization;
using Application.Interface.Result;
using Application.Interface.Time;
using ApplicationTest.Common;
using Domain.Interface.Authorization;
using DomainFaker;
using FluentAssertions;
using Xunit;
using static ApplicationTest.Error.TestError;

namespace ApplicationTest.Command.PhysicalData.TimePeriod.DeleteTimePeriod
{
    public sealed class DeleteTimePeriodAuthorizationSpecification : IClassFixture<PhysicalDataFixture>
	{
		private readonly PhysicalDataFixture fxtPhysicalData;

		private readonly ITimeProvider prvTime;

		public DeleteTimePeriodAuthorizationSpecification(PhysicalDataFixture fxtPhysicalData)
		{
			this.fxtPhysicalData = fxtPhysicalData;
			this.prvTime = fxtPhysicalData.TimeProvider;
		}

		[Fact]
		public async Task Delete_ShouldReturnTrue_WhenPassportIdIsAuthorized()
		{
			// Arrange
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Delete);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppVisa.Id };

			DeleteTimePeriodCommand cmdDelete = new DeleteTimePeriodCommand()
			{
				TimePeriodId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IAuthorization<DeleteTimePeriodCommand> hndlAuthorization = new DeleteTimePeriodAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdDelete,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Should().BeNull();

					return true;
				},
				bResult =>
				{
					bResult.Should().BeTrue();

					return true;
				});

			//Clean up
			await fxtPhysicalData.PassportVisaRepository.DeleteAsync(ppVisa, CancellationToken.None);
		}

		[Fact]
		public async Task Delete_ShouldReturnMessageError_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			IEnumerable<Guid> enumPassportVisaId = Enumerable.Empty<Guid>();

			DeleteTimePeriodCommand cmdDelete = new DeleteTimePeriodCommand()
			{
				TimePeriodId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IAuthorization<DeleteTimePeriodCommand> hndlAuthorization = new DeleteTimePeriodAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdDelete,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			//Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Code.Should().Be(Repository.PassportVisa.NotFound.Code);
					msgError.Description.Should().Be(Repository.PassportVisa.NotFound.Description);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});
		}

		[Theory]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Create)]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Delete)]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Read)]
		[InlineData(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Update)]
		public async Task Delete_ShouldReturnMessageError_WhenPassportVisaDoesNotMatch(string sName, int iLevel)
		{
			// Arrange
			IPassportVisa ppAuthorizedVisa = DataFaker.PassportVisa.CreateDefault(AuthorizationDefault.Name.TimePeriod, AuthorizationDefault.Level.Delete);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppAuthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IPassportVisa ppUnauthorizedVisa = DataFaker.PassportVisa.CreateDefault(sName, iLevel);
			await fxtPhysicalData.PassportVisaRepository.InsertAsync(ppUnauthorizedVisa, prvTime.GetUtcNow(), CancellationToken.None);

			IEnumerable<Guid> enumPassportVisaId = new List<Guid>() { ppUnauthorizedVisa.Id };

			DeleteTimePeriodCommand cmdDelete = new DeleteTimePeriodCommand()
			{
				TimePeriodId = Guid.NewGuid(),
				RestrictedPassportId = Guid.Empty
			};

			IAuthorization<DeleteTimePeriodCommand> hndlAuthorization = new DeleteTimePeriodAuthorization(fxtPhysicalData.PassportVisaRepository);

			// Act
			IMessageResult<bool> rsltAuthorization = await hndlAuthorization.AuthorizeAsync(
				msgMessage: cmdDelete,
				enumPassportVisaId: enumPassportVisaId,
				tknCancellation: CancellationToken.None);

			// Assert
			rsltAuthorization.Match(
				msgError =>
				{
					msgError.Should().Be(AuthorizationError.PassportVisa.VisaDoesNotExist);

					return false;
				},
				bResult =>
				{
					bResult.Should().BeFalse();

					return true;
				});

			// Clean up
			await fxtPhysicalData.PassportVisaRepository.DeleteAsync(ppUnauthorizedVisa, CancellationToken.None);
			await fxtPhysicalData.PassportVisaRepository.DeleteAsync(ppAuthorizedVisa, CancellationToken.None);
		}
	}
}
