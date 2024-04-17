using Domain.Interface.Authorization;
using Domain.Interface.Transfer;
using DomainFaker;
using DomainFaker.Implementation;
using FluentAssertions;
using Xunit;


namespace DomainTest.Passport
{
	public class PassportSpecification
	{
		public PassportSpecification()
		{

		}

		[Fact]
		public void Initialize_ShouldReturnPassport_WhenTransferObjectIsValid()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();

			// Act
			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(dtoPassport, Enumerable.Empty<Guid>());

			if (ppPassport is null)
				return;

			// Assert
			ppPassport.ConcurrencyStamp.Should().Be(dtoPassport.ConcurrencyStamp);
			ppPassport.ExpiredAt.Should().Be(dtoPassport.ExpiredAt);
			ppPassport.HolderId.Should().Be(dtoPassport.HolderId);
			ppPassport.Id.Should().Be(dtoPassport.Id);
			ppPassport.IsAuthority.Should().Be(dtoPassport.IsAuthority);
			ppPassport.IsEnabled.Should().Be(dtoPassport.IsEnabled);
			ppPassport.IssuedBy.Should().Be(dtoPassport.IssuedBy);
			ppPassport.LastCheckedAt.Should().Be(dtoPassport.LastCheckedAt);
			ppPassport.LastCheckedBy.Should().Be(dtoPassport.LastCheckedBy);
		}

		[Fact]
		public void Write_ShouldReturnTransferObject_WhenPassportExists()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();

			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(dtoPassport, Enumerable.Empty<Guid>());

			if (ppPassport is null)
				return;

			// Act
			IPassportTransferObject dtoPassportToRead = ppPassport.WriteTo<PassportTransferObjectFaker>();

			// Assert
			dtoPassportToRead.Should().BeEquivalentTo(dtoPassport);
		}

		[Fact]
		public void Initialize_ShouldContainPassportVisa_WhenPassportVisaWereInitialized()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();
			IList<Guid> lstPassportVisaId = new List<Guid>()
			{
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid()
			};

			// Act
			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(dtoPassport: dtoPassport, enumPassportVisaId: lstPassportVisaId.AsEnumerable());

			if (ppPassport is null)
				return;

			// Assert
			ppPassport.VisaId.Should().Contain(lstPassportVisaId[0]);
			ppPassport.VisaId.Should().Contain(lstPassportVisaId[1]);
			ppPassport.VisaId.Should().Contain(lstPassportVisaId[2]);
			ppPassport.VisaId.Should().Contain(lstPassportVisaId[3]);
		}

		[Fact]
		public void HasVisa_ShouldBeTrue_WhenPassportVisaExists()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			IList<Guid> lstPassportVisaId = new List<Guid>()
			{
				ppVisa.Id
			};

			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(dtoPassport: dtoPassport, enumPassportVisaId: lstPassportVisaId.AsEnumerable());

			if (ppPassport is null)
				return;

			// Act
			bool bResult = ppPassport.HasVisa(ppVisa);

			// Assert
			bResult.Should().BeTrue();
		}

		[Fact]
		public void TryAddVisa_ShouldBeTrue_WhenPassportVisaIdIsUnique()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			IList<Guid> lstPassportVisaId = new List<Guid>()
			{
				Guid.NewGuid()
			};

			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
				dtoPassport: dtoPassport,
				enumPassportVisaId: lstPassportVisaId.AsEnumerable());

			if (ppPassport is null)
				return;

			// Act
			bool bResult = ppPassport.TryAddVisa(ppVisa);

			// Assert
			bResult.Should().BeTrue();
		}

		[Fact]
		public void TryAddVisa_ShouldBeFalse_WhenPassportVisaIdIsNotUnique()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			IList<Guid> lstPassportVisaId = new List<Guid>()
			{
				ppVisa.Id
			};

			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
				dtoPassport: dtoPassport,
				enumPassportVisaId: lstPassportVisaId.AsEnumerable());

			if (ppPassport is null)
				return;

			// Act
			bool bResult = ppPassport.TryAddVisa(ppVisa);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void RemoveVisa_ShouldBeTrue_WhenPassportVisaExists()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			IList<Guid> lstPassportVisaId = new List<Guid>()
			{
				ppVisa.Id
			};

			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
				dtoPassport: dtoPassport,
				enumPassportVisaId: lstPassportVisaId.AsEnumerable());

			if (ppPassport is null)
				return;

			// Act
			bool bResult = ppPassport.TryRemoveVisa(ppVisa);

			// Assert
			bResult.Should().BeTrue();
		}

		[Fact]
		public void RemoveVisa_ShouldBeFalse_WhenPassportVisaDoesNotExist()
		{
			// Arrange
			IPassportTransferObject dtoPassport = DataFaker.PassportTransferObject.Create();
			IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

			IList<Guid> lstPassportVisaId = new List<Guid>()
			{
				Guid.NewGuid()
			};

			IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
				dtoPassport: dtoPassport,
				enumPassportVisaId: lstPassportVisaId.AsEnumerable());

			if (ppPassport is null)
				return;

			// Act
			bool bResult = ppPassport.TryRemoveVisa(ppVisa);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryDisable_ShouldBeTrue_WhenPassportIsEnabledAndIsNotExpired()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthorizedPassport = DataFaker.Passport.CreateDefault();

			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();
			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt;

			ppPassport.TryEnable(ppAuthority, dtEnabledAt);
			ppAuthorizedPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Act
			DateTimeOffset dtDisabledAt = DataFaker.Passport.LastCheckedAt.AddDays(5);

			bool bResult = ppPassport.TryDisable(ppAuthorizedPassport, dtDisabledAt);

			// Assert
			ppAuthorizedPassport.IsEnabled.Should().BeTrue();
			bResult.Should().BeTrue();
			ppPassport.IsEnabled.Should().BeFalse();
			ppPassport.LastCheckedAt.Should().Be(dtDisabledAt);
			ppPassport.LastCheckedBy.Should().Be(ppAuthorizedPassport.Id);
			ppPassport.IsExpired(dtDisabledAt.AddDays(1));
		}

		[Fact]
		public void TryDisable_ShouldBeFalse_WhenPassportIsDisabledAndIsNotExpired()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthorizedPassport = DataFaker.Passport.CreateDefault();

			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();
			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt;

			ppPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Act
			DateTimeOffset dtDisabledAt = DataFaker.Passport.LastCheckedAt.AddDays(5);

			bool bResult = ppPassport.TryDisable(ppAuthorizedPassport, dtDisabledAt);

			// Assert
			ppAuthorizedPassport.IsEnabled.Should().BeFalse();
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryDisable_ShouldBeFalse_WhenPassportIsEnabledAndIsExpired()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthorizedPassport = DataFaker.Passport.CreateDefault();

			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();
			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt;

			ppPassport.TryEnable(ppAuthority, dtEnabledAt);
			ppAuthorizedPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Act
			DateTimeOffset dtDisabledAt = ppPassport.ExpiredAt.AddDays(1);

			bool bResult = ppPassport.TryDisable(ppAuthorizedPassport, dtDisabledAt);

			// Assert
			ppAuthorizedPassport.IsEnabled.Should().BeTrue();
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryEnable_ShouldBeTrue_WhenPassportIsEnabledAndAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			// Act
			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt.AddDays(1);

			bool bResult = ppPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Assert
			bResult.Should().BeTrue();
		}

		[Fact]
		public void TryEnable_ShouldBeFalse_WhenPassportIsDisabledAndAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			DateTimeOffset dtDisabledAt = ppAuthority.LastCheckedAt.AddDays(-1);
			ppAuthority.TryDisable(ppAuthority, dtDisabledAt);

			// Act
			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryEnable_ShouldBeFalse_WhenPassportIsEnabledAndIsNotAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateDefault();

			// Act
			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryExtendTerm_ShouldBeTrue_WhenDateIsLater()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			// Act
			DateTimeOffset dtExtendedAt = DataFaker.Passport.LastCheckedAt;
			DateTimeOffset dtDate = ppPassport.ExpiredAt.AddDays(1);

			bool bResult = ppPassport.TryExtendTerm(dtDate, dtExtendedAt, ppPassport.Id);

			// Assert
			bResult.Should().BeTrue();
			ppPassport.LastCheckedAt.Should().Be(dtExtendedAt);
			ppPassport.LastCheckedBy.Should().Be(ppPassport.Id);
			ppPassport.IsExpired(dtDate).Should().BeTrue();
		}

		[Fact]
		public void TryExtendTerm_ShouldBeFalse_WhenDateIsGone()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();

			// Act
			DateTimeOffset dtExtendedAt = DataFaker.Passport.LastCheckedAt;
			DateTimeOffset dtDate = ppPassport.ExpiredAt.AddDays(-1);

			bool bResult = ppPassport.TryExtendTerm(dtDate, dtExtendedAt, ppPassport.Id);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryJoinToAuthority_ShouldBeTrue_WhenPassportIsEnabledAndAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt.AddDays(1);
			ppPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Act
			DateTimeOffset dtJoinedAt = DataFaker.Passport.LastCheckedAt.AddDays(1);

			bool bResult = ppPassport.TryJoinToAuthority(ppAuthority, dtJoinedAt);

			// Assert
			bResult.Should().BeTrue();
			ppPassport.IsAuthority.Should().BeTrue();
			ppPassport.IsEnabled.Should().BeTrue();
			ppPassport.LastCheckedAt.Should().Be(dtJoinedAt);
			ppPassport.LastCheckedBy.Should().Be(ppAuthority.Id);
		}

		[Fact]
		public void TryJoinToAuthority_ShouldBeFalse_WhenPassportIsDisabledAndAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			DateTimeOffset dtDisabledAt = ppAuthority.LastCheckedAt.AddDays(-1);
			ppAuthority.TryDisable(ppAuthority, dtDisabledAt);

			// Act
			DateTimeOffset dtJoinedAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryJoinToAuthority(ppAuthority, dtJoinedAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryJoinToAuthority_ShouldBeFalse_WhenPassportIsEnabledAndIsNotAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateDefault();

			// Act
			DateTimeOffset dtJoinedAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryJoinToAuthority(ppAuthority, dtJoinedAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryJoinToAuthority_ShouldBeFalse_WhenPassportIsDisabled()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			// Act
			DateTimeOffset dtJoinedAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryJoinToAuthority(ppAuthority, dtJoinedAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryJoinToAuthority_ShouldBeFalse_WhenPassportIsExpired()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateDefault();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			DateTimeOffset dtEnabledAt = DataFaker.Passport.LastCheckedAt.AddDays(1);
			ppPassport.TryEnable(ppAuthority, dtEnabledAt);

			// Act
			DateTimeOffset dtJoinedAt = ppPassport.ExpiredAt.AddDays(1);

			bool bResult = ppPassport.TryJoinToAuthority(ppAuthority, dtJoinedAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryReset_ShouldBeTrue_WhenPassportIsEnabledAndAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateAuthority();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			// Act
			DateTimeOffset dtResetAt = DataFaker.Passport.LastCheckedAt.AddDays(1);

			bool bResult = ppPassport.TryReset(ppAuthority, dtResetAt);

			// Assert
			bResult.Should().BeTrue();
			ppPassport.IsAuthority.Should().BeFalse();
			ppPassport.IsEnabled.Should().BeFalse();
			ppPassport.LastCheckedAt.Should().Be(dtResetAt);
			ppPassport.LastCheckedBy.Should().Be(ppAuthority.Id);
			ppPassport.IsExpired(dtResetAt).Should().BeTrue();
		}

		[Fact]
		public void TryReset_ShouldBeFalse_WhenPassportIsDisabledAndAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateAuthority();
			IPassport ppAuthority = DataFaker.Passport.CreateAuthority();

			DateTimeOffset dtDisabledAt = ppAuthority.LastCheckedAt.AddDays(-1);
			ppAuthority.TryDisable(ppAuthority, dtDisabledAt);

			// Act
			DateTimeOffset dtResetAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryReset(ppAuthority, dtResetAt);

			// Assert
			bResult.Should().BeFalse();
		}

		[Fact]
		public void TryReset_ShouldBeFalse_WhenPassportIsEnabledAndIsNotAuthorized()
		{
			// Arrange
			IPassport ppPassport = DataFaker.Passport.CreateAuthority();
			IPassport ppAuthority = DataFaker.Passport.CreateDefault();

			// Act
			DateTimeOffset dtResetAt = DataFaker.Passport.LastCheckedAt;

			bool bResult = ppPassport.TryReset(ppAuthority, dtResetAt);

			// Assert
			bResult.Should().BeFalse();
		}
	}
}