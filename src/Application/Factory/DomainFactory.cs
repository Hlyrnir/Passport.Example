using Domain.Interface.Authorization;

namespace Application.Factory
{
	public static class DomainFactory
	{
        public static class Authorization
		{
			public static class Passport
			{
				public static IPassport? Create(DateTimeOffset dtExpiredAt, Guid guHolderId, Guid guIssuedBy, DateTimeOffset dtLastCheckedAt)
				{
					return Domain.Aggregate.Authorization.Passport.Passport.Create(
						dtExpiredAt: dtExpiredAt,
						guHolderId: guHolderId,
						guIssuedBy: guIssuedBy,
						dtLastCheckedAt: dtLastCheckedAt);
				}
			}

			public static class PassportHolder
			{
				public static IPassportHolder? Create(string sCultureName, string sEmailAddress, string sFirstName, string sLastName, string sPhoneNumber, IPassportSetting ppSetting)
				{
					return Domain.Aggregate.Authorization.PassportHolder.PassportHolder.Create(
						sCultureName: sCultureName,
						sEmailAddress: sEmailAddress,
						sFirstName: sFirstName,
						sLastName: sLastName,
						sPhoneNumber: sPhoneNumber,
						ppSetting: ppSetting);
				}
			}

			public static class PassportToken
			{
				public static IPassportToken? Create(Guid guPassportId,string sProvider, bool bTwoFactorIsEnabled)
				{
					return Domain.Aggregate.Authorization.PassportToken.PassportToken.Create(
						guPassportId: guPassportId,
						sProvider: sProvider,
						bTwoFactorIsEnabled: bTwoFactorIsEnabled);
				}
			}

			public static class PassportVisa
			{
				public static IPassportVisa? Create(string sName, int iLevel)
				{
					return Domain.Aggregate.Authorization.PassportVisa.PassportVisa.Create(
						sName: sName, 
						iLevel: iLevel);
				}
			}
		}
	}
}