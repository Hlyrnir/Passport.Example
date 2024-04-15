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
		public void Initialize_ShouldReturnPassport_WhenTransferObjectExists()
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
		public void Write_ShouldContainPassportVisa_WhenPassportVisaWereInitialized()
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

		//[Fact]
		//public void Initialize_ShouldReturnPassport_WhenBusinessLogicIsOk()
		//{
		//	// Arrange
		//	string sConcurrencyStamp = Guid.NewGuid().ToString();
		//	DateTimeOffset dtExpiredAt = prvTime.GetUtcNow();
		//	bool bHasPermissionToCommand = false;
		//	bool bHasPermissionToQuery = false;
		//	Guid guHolderId = Guid.NewGuid();
		//	Guid guId = Guid.NewGuid();
		//	bool bIsAuthority = false;
		//	bool bIsEnabled = false;
		//	Guid guIssuedBy = Guid.NewGuid();
		//	DateTimeOffset dtLastCheckedAt = prvTime.GetUtcNow().AddHours(-1.0);
		//	Guid guLastCheckedBy = Guid.NewGuid();
		//	string sRefreshToken = Guid.NewGuid().ToString();
		//	IEnumerable<IPassportVisa> enumPassportVisa = Enumerable.Empty<IPassportVisa>();

		//	// Act
		//	IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
		//		sConcurrencyStamp: sConcurrencyStamp,
		//		dtExpiredAt: dtExpiredAt,
		//		bHasPermissionToCommand: bHasPermissionToCommand,
		//		bHasPermissionToQuery: bHasPermissionToQuery,
		//		guHolderId: guHolderId,
		//		guId: guId,
		//		bIsAuthority: bIsAuthority,
		//		bIsEnabled: bIsEnabled,
		//		guIssuedBy: guIssuedBy,
		//		dtLastCheckedAt: dtLastCheckedAt,
		//		guLastCheckedBy: guLastCheckedBy,
		//		sRefreshToken: sRefreshToken,
		//		enumVisa: enumPassportVisa);

		//	if (ppPassport is null)
		//		return;

		//	// Assert
		//	ppPassport.Should().NotBeNull();
		//	ppPassport.ConcurrencyStamp.Should().Be(sConcurrencyStamp);
		//	ppPassport.ExpiredAt.Should().Be(dtExpiredAt);
		//	ppPassport.HolderId.Should().Be(guHolderId);
		//	ppPassport.Id.Should().Be(guId);
		//	ppPassport.IsAuthority.Should().Be(bIsAuthority);
		//	ppPassport.IsEnabled.Should().Be(bIsEnabled);
		//	ppPassport.IssuedBy.Should().Be(guIssuedBy);
		//	ppPassport.LastCheckedAt.Should().Be(dtLastCheckedAt);
		//	ppPassport.LastCheckedBy.Should().Be(guLastCheckedBy);
		//}

		//[Fact]
		//public void Initialize_ShouldReturnNull_WhenPassportVisaIsNotUnique()
		//{
		//	// Arrange
		//	string sConcurrencyStamp = Guid.NewGuid().ToString();
		//	DateTimeOffset dtExpiredAt = prvTime.GetUtcNow();
		//	bool bHasPermissionToCommand = false;
		//	bool bHasPermissionToQuery = false;
		//	Guid guHolderId = Guid.NewGuid();
		//	Guid guId = Guid.NewGuid();
		//	bool bIsAuthority = false;
		//	bool bIsEnabled = false;
		//	Guid guIssuedBy = Guid.NewGuid();
		//	DateTimeOffset dtLastCheckedAt = prvTime.GetUtcNow().AddHours(-1.0);
		//	Guid guLastCheckedBy = Guid.NewGuid();
		//	string sRefreshToken = Guid.NewGuid().ToString();

		//	IPassportVisa ppVisa = DataFaker.PassportVisa.CreateDefault();

		//	// Act
		//	IEnumerable<IPassportVisa> enumPassportVisa = new List<IPassportVisa>()
		//	{
		//		DataFaker.PassportVisa.CreateDefault(),
		//		DataFaker.PassportVisa.CreateDefault(),
		//		ppVisa,
		//		ppVisa
		//	};

		//	IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
		//		sConcurrencyStamp: sConcurrencyStamp,
		//		dtExpiredAt: dtExpiredAt,
		//		bHasPermissionToCommand: bHasPermissionToCommand,
		//		bHasPermissionToQuery: bHasPermissionToQuery,
		//		guHolderId: guHolderId,
		//		guId: guId,
		//		bIsAuthority: bIsAuthority,
		//		bIsEnabled: bIsEnabled,
		//		guIssuedBy: guIssuedBy,
		//		dtLastCheckedAt: dtLastCheckedAt,
		//		guLastCheckedBy: guLastCheckedBy,
		//		sRefreshToken: sRefreshToken,
		//		enumVisa: enumPassportVisa);

		//	if (ppPassport is null)
		//		return;

		//	// Assert
		//	ppPassport.Should().BeNull();
		//}

		//[Fact]
		//public void HasVisaWithUnknownVisaShouldFail()
		//{
		//    // Arrange
		//    bool bResult = false;

		//    PassportVisa ppVisa = new PassportVisa();
		//    ppVisa.Initialize(Guid.NewGuid());

		//    // Act
		//    bResult = ppPassport.HasVisa(ppVisa);

		//    // Assert
		//    Assert.False(bResult);
		//}

		//      [Fact]
		//      public void InvalidVisaShouldBeIgnored()
		//      {
		//          // Arrange
		//          bool bIsAdded = true;
		//          bool bIsRemoved = true;

		//          PassportVisa? ppVisa = null;

		//          // Act
		//          bIsAdded = ppPassport.TryAddVisa(ppVisa!);
		//          bIsRemoved = ppPassport.TryRemoveVisa(ppVisa!);

		//          // Assert
		//          Assert.False(bIsAdded);
		//          Assert.False(bIsRemoved);
		//      }

		//      [Fact]
		//      public void PassportShouldContainUniqueVisa()
		//{
		//	// Arrange
		//	bool bIsNotAdded = true;
		//	PassportVisa ppVisa = CreateVisa();

		//	// Act
		//	bIsNotAdded = ppPassport.TryAddVisa(ppVisa);

		//	// Assert
		//	Assert.False(bIsNotAdded);
		//}

		//[Fact]
		//      public void PassportShouldContainAddedVisa()
		//      {
		//          // Arrange
		//          bool bIsAdded = false;
		//          int iInitialVisaCount = ppPassport.Visa.Count;

		//          PassportVisa ppVisa = new PassportVisa();
		//          ppVisa.Initialize(Guid.NewGuid());

		//          // Act
		//          bIsAdded = ppPassport.TryAddVisa(ppVisa);

		//          // Assert
		//          Assert.True(bIsAdded);
		//          Assert.True(ppPassport.Visa.Count == iInitialVisaCount + 1);
		//          Assert.Contains<IPassportVisa>(ppVisa, ppPassport.Visa);
		//      }

		//      [Fact]
		//      public void PassportShouldNotContainRemovedVisa()
		//      {
		//          // Arrange
		//          bool bIsRemoved = false;
		//          int iInitialVisaCount = ppPassport.Visa.Count;

		//          PassportVisa ppVisa = CreateVisa();

		//          // Act
		//          bIsRemoved = ppPassport.TryRemoveVisa(ppVisa);

		//          // Assert
		//          Assert.True(bIsRemoved);
		//          Assert.True(ppPassport.Visa.Count == iInitialVisaCount + (-1));
		//          Assert.DoesNotContain<IPassportVisa>(ppVisa, ppPassport.Visa);
		//      }

		//private PassportVisa CreateVisa()
		//{
		//	return new PassportVisa()
		//	{
		//		ConcurrencyStamp = ppPassport.Visa[0].ConcurrencyStamp,
		//		Id = ppPassport.Visa[0].Id,
		//		Level = ppPassport.Visa[0].Level,
		//		Name = ppPassport.Visa[0].Name
		//	};
		//}
	}
}