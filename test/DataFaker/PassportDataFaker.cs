using Domain.Interface.Authorization;
using Domain.Interface.Transfer;
using DomainFaker.Implementation;

namespace DomainFaker
{
	public static partial class DataFaker
	{
		private const string sProvider = "DEFAULT_PROVIDER";

		public static class Passport
		{
			public static readonly DateTimeOffset LastCheckedAt = new DateTimeOffset(2000, 1, 31, 0, 0, 0, 0, 0, TimeSpan.Zero);

			public static IPassport CreateDefault()
			{
				IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Create(
					dtExpiredAt: Passport.LastCheckedAt.AddDays(30),
					guHolderId: Guid.NewGuid(),
					guIssuedBy: Guid.NewGuid(),
					dtLastCheckedAt: Passport.LastCheckedAt);

				if (ppPassport is null)
					throw new NullReferenceException();

				return ppPassport;
			}

			public static IPassport CreateAuthority()
			{
				IPassportTransferObject dtoPassport = new PassportTransferObjectFaker()
				{
					ConcurrencyStamp = Guid.NewGuid().ToString(),
					ExpiredAt = Passport.LastCheckedAt.AddDays(30),
					HolderId = Guid.NewGuid(),
					Id = Guid.NewGuid(),
					IsAuthority = true,
					IsEnabled = true,
					IssuedBy = Guid.NewGuid(),
					LastCheckedAt = Passport.LastCheckedAt,
					LastCheckedBy = Guid.NewGuid()
				};

				IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
					dtoPassport: dtoPassport,
					enumPassportVisaId: Enumerable.Empty<Guid>());

				if (ppPassport is null)
					throw new NullReferenceException();

				return ppPassport;
			}

			public static IPassport Clone(IPassport ppPassportToClone, bool bResetConcurrencyStamp = false)
			{
				string sConcurrencyStamp = ppPassportToClone.ConcurrencyStamp;

				if (bResetConcurrencyStamp == true)
					sConcurrencyStamp = Guid.NewGuid().ToString();

				IPassportTransferObject dtoPassportToClone = ppPassportToClone.WriteTo<PassportTransferObjectFaker>();

				IPassportTransferObject dtoPassport = new PassportTransferObjectFaker()
				{
					ConcurrencyStamp = sConcurrencyStamp,
					ExpiredAt = dtoPassportToClone.ExpiredAt,
					HolderId = dtoPassportToClone.HolderId,
					Id = dtoPassportToClone.Id,
					IsAuthority = dtoPassportToClone.IsAuthority,
					IsEnabled = dtoPassportToClone.IsEnabled,
					IssuedBy = dtoPassportToClone.IssuedBy,
					LastCheckedAt = dtoPassportToClone.LastCheckedAt,
					LastCheckedBy = dtoPassportToClone.LastCheckedBy
				};

				IPassport? ppPassport = Domain.Aggregate.Authorization.Passport.Passport.Initialize(
					dtoPassport: dtoPassport,
					enumPassportVisaId: Enumerable.Empty<Guid>());

				if (ppPassport is null)
					throw new NullReferenceException();

				return ppPassport;
			}
		}

		public static class PassportTransferObject
		{
			public static readonly DateTimeOffset LastCheckedAt = new DateTimeOffset(2000, 1, 31, 0, 0, 0, 0, 0, TimeSpan.Zero);

			public static IPassportTransferObject Create()
			{
				return new PassportTransferObjectFaker()
				{
					ConcurrencyStamp = Guid.NewGuid().ToString(),
					ExpiredAt = PassportTransferObject.LastCheckedAt.AddDays(30),
					HolderId = Guid.NewGuid(),
					Id = Guid.NewGuid(),
					IsAuthority = false,
					IsEnabled = false,
					IssuedBy = Guid.NewGuid(),
					LastCheckedAt = PassportTransferObject.LastCheckedAt,
					LastCheckedBy = Guid.NewGuid(),
					RefreshToken = Guid.NewGuid().ToString()
				};
			}
		}

		public static class PassportCredential
		{
			public static IPassportCredential CreateDefault()
			{
				IPassportCredential ppCredential = new PassportCredentialFaker();

				ppCredential.Initialize(
					sProvider: sProvider,
					sCredential: $"{Guid.NewGuid()}@passport.org",
					sSignature: $"S!gnature_{Guid.NewGuid()}");

				return ppCredential;
			}

			public static IPassportCredential Create(string sCredential, string sSignature)
			{
				IPassportCredential ppCredential = new PassportCredentialFaker();

				ppCredential.Initialize(
					sProvider: sProvider,
					sCredential: sCredential,
					sSignature: sSignature);

				return ppCredential;
			}

			public static IPassportCredential CreateAtProvider(string sCredential, string sProvider, string sSignature)
			{
				IPassportCredential ppCredential = new PassportCredentialFaker();

				ppCredential.Initialize(
					sProvider: sProvider,
					sCredential: sCredential,
					sSignature: sSignature);

				return ppCredential;
			}
		}

		public static class PassportHolder
		{
			public static IPassportHolder CreateDefault(IPassportSetting ppSetting)
			{
				IPassportHolder? ppPassport = Domain.Aggregate.Authorization.PassportHolder.PassportHolder.Create(
					sCultureName: "en-GB",
					sEmailAddress: $"{Guid.NewGuid()}@passport.org",
					sFirstName: "Jane",
					sLastName: "Doe",
					sPhoneNumber: "000",
					ppSetting: ppSetting);

				if (ppPassport is null)
					throw new NullReferenceException();

				return ppPassport;
			}

			public static IPassportHolder Clone(IPassportHolder ppHolderToClone, IPassportSetting ppSetting, bool bResetId = false, bool bResetConcurrencyStamp = false)
			{
				IPassportHolderTransferObject dtoPassportHolderToClone = ppHolderToClone.WriteTo<PassportHolderTransferObjectFaker>();

				Guid guId = ppHolderToClone.Id;
				string sConcurrencyStamp = ppHolderToClone.ConcurrencyStamp;

				if (bResetId == true)
					guId = Guid.NewGuid();

				if (bResetConcurrencyStamp == true)
					sConcurrencyStamp = Guid.NewGuid().ToString();

				IPassportHolderTransferObject dtoPassportHolder = new PassportHolderTransferObjectFaker()
				{
					EmailAddress = dtoPassportHolderToClone.EmailAddress,
					EmailAddressIsConfirmed = dtoPassportHolderToClone.EmailAddressIsConfirmed,
					ConcurrencyStamp = sConcurrencyStamp,
					CultureName = dtoPassportHolderToClone.CultureName,
					FirstName = dtoPassportHolderToClone.FirstName,
					Id = guId,
					LastName = dtoPassportHolderToClone.LastName,
					PhoneNumber = dtoPassportHolderToClone.PhoneNumber,
					PhoneNumberIsConfirmed = dtoPassportHolderToClone.PhoneNumberIsConfirmed,
					SecurityStamp = dtoPassportHolderToClone.SecurityStamp
				};

				IPassportHolder? ppHolder = Domain.Aggregate.Authorization.PassportHolder.PassportHolder.Initialize(
					dtoPassportHolder: dtoPassportHolder,
					ppSetting: ppSetting);

				if (ppHolder is null)
					throw new NullReferenceException();

				return ppHolder;
			}
		}

		public static class PassportHolderTransferObject
		{
			public static IPassportHolderTransferObject Create()
			{
				return new PassportHolderTransferObjectFaker()
				{
					EmailAddress = $"{Guid.NewGuid()}@passport.org",
					EmailAddressIsConfirmed = false,
					ConcurrencyStamp = Guid.NewGuid().ToString(),
					CultureName = "en-GB",
					FirstName = "Jane",
					Id = Guid.NewGuid(),
					LastName = "Doe",
					PhoneNumber = "000",
					PhoneNumberIsConfirmed = false,
					SecurityStamp = Guid.NewGuid().ToString()
				};
			}
		}

		public static class PassportToken
		{
			public static IPassportToken CreateDefault(Guid guPassportId)
			{
				IPassportToken? ppToken = Domain.Aggregate.Authorization.PassportToken.PassportToken.Create(
					guPassportId: guPassportId,
					sProvider: sProvider,
					bTwoFactorIsEnabled: false);

				if (ppToken is null)
					throw new NullReferenceException();

				return ppToken;
			}

			public static IPassportToken Clone(IPassportToken ppTokenToClone, bool bResetRefreshToken = false)
			{
				IPassportTokenTransferObject dtoPassportTokenToClone = ppTokenToClone.WriteTo<PassportTokenTransferObjectFaker>();

				string sRefreshToken = ppTokenToClone.RefreshToken;

				if (bResetRefreshToken == true)
					sRefreshToken = Guid.NewGuid().ToString();

				IPassportTokenTransferObject dtoPassportToken = new PassportTokenTransferObjectFaker()
				{
					Id = dtoPassportTokenToClone.Id,
					PassportId = dtoPassportTokenToClone.PassportId,
					Provider = dtoPassportTokenToClone.Provider,
					RefreshToken = sRefreshToken,
					TwoFactorIsEnabled = dtoPassportTokenToClone.TwoFactorIsEnabled
				};

				IPassportToken? ppToken = Domain.Aggregate.Authorization.PassportToken.PassportToken.Initialize(dtoPassportToken: dtoPassportToken);

				if (ppToken is null)
					throw new NullReferenceException();

				return ppToken;
			}
		}

		public static class PassportTokenTransferObject
		{
			public static IPassportTokenTransferObject Create(Guid guPassportId)
			{
				return new PassportTokenTransferObjectFaker()
				{
					Id = Guid.NewGuid(),
					PassportId = guPassportId,
					Provider = sProvider,
					RefreshToken = Guid.NewGuid().ToString(),
					TwoFactorIsEnabled = false
				};
			}
		}

		public static class PassportVisa
		{
			public static IPassportVisa CreateDefault()
			{
				IPassportVisa? ppVisa = Domain.Aggregate.Authorization.PassportVisa.PassportVisa.Create(
					sName: Guid.NewGuid().ToString(),
					iLevel: 0);

				if (ppVisa is null)
					throw new NullReferenceException();

				return ppVisa;
			}

			public static IPassportVisa CreateDefault(string sName, int iLevel)
			{
				IPassportVisa? ppVisa = Domain.Aggregate.Authorization.PassportVisa.PassportVisa.Create(
					sName: sName,
					iLevel: iLevel);

				if (ppVisa is null)
					throw new NullReferenceException();

				return ppVisa;
			}

			public static IPassportVisa Clone(IPassportVisa ppVisaToClone, bool bResetConcurrencyStamp = false)
			{
				IPassportVisaTransferObject dtoPassportVisaToClone = ppVisaToClone.WriteTo<PassportVisaTransferObjectFaker>();

				string sConcurrencyStamp = dtoPassportVisaToClone.ConcurrencyStamp;

				if (bResetConcurrencyStamp == true)
					sConcurrencyStamp = Guid.NewGuid().ToString();

				IPassportVisaTransferObject dtoPassportVisa = new PassportVisaTransferObjectFaker()
				{
					ConcurrencyStamp = sConcurrencyStamp,
					Id = dtoPassportVisaToClone.Id,
					Level = dtoPassportVisaToClone.Level,
					Name = dtoPassportVisaToClone.Name
				};

				IPassportVisa? ppVisa = Domain.Aggregate.Authorization.PassportVisa.PassportVisa.Initialize(dtoPassportVisa: dtoPassportVisa);

				if (ppVisa is null)
					throw new NullReferenceException();

				return ppVisa;
			}
		}

		public static class PassportVisaTransferObject
		{
			public static IPassportVisaTransferObject Create()
			{
				return new PassportVisaTransferObjectFaker()
				{
					ConcurrencyStamp = Guid.NewGuid().ToString(),
					Id = Guid.NewGuid(),
					Level = 0,
					Name = Guid.NewGuid().ToString()
				};
			}
		}

		public static class PassportSetting
		{
			public static IPassportSetting Create()
			{
				return new PassportSettingFaker()
				{
					MaximalAllowedAccessAttempt = 2,
					ValidProviderName = new List<string>() { sProvider, "DEFAULT_UNDEFINED" },
					MaximalCredentialLength = 64,
					MaximalSignatureLength = 64
				};
			}
		}
	}
}