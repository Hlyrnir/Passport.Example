using Domain.Interface.Authorization;
using Domain.Interface.Transfer;

namespace Domain.Aggregate.Authorization.PassportToken
{
    public sealed class PassportToken : IPassportToken
    {
        private Guid guId;
        private Guid guPassportId;
        private string sProvider;
        private string sRefreshToken;
        private bool bTwoFactorIsEnabled;

        private PassportToken(
            Guid guId,
            Guid guPassportId,
            string sProvider,
            string sRefreshToken,
            bool bTwoFactorIsEnabled)
        {
            this.guId = guId;
            this.guPassportId = guPassportId;
            this.sProvider = sProvider;
            this.sRefreshToken = sRefreshToken;
            this.bTwoFactorIsEnabled = bTwoFactorIsEnabled;
        }

        public Guid Id { get => guId; }
        public Guid PassportId { get => guPassportId; }
        public string Provider { get => sProvider; }
        public string RefreshToken { get => sRefreshToken; }
        public bool TwoFactorIsEnabled { get => bTwoFactorIsEnabled; }

		public bool TryEnableTwoFactorAuthentication(bool bEnable = false)
		{
			if (bTwoFactorIsEnabled == bEnable)
				return false;

			bTwoFactorIsEnabled = bEnable;

			return true;
		}

        private static IPassportToken? Initialize(
            Guid guId,
            Guid guPassportId,
            string sProvider,
            string sRefreshToken,
            bool bTwoFactorIsEnabled)
        {
			if (string.IsNullOrWhiteSpace(sProvider) == true)
				return null;

			return new PassportToken(
                guId: guId,
                guPassportId: guPassportId,
                sProvider: sProvider,
                sRefreshToken: sRefreshToken,
                bTwoFactorIsEnabled: bTwoFactorIsEnabled);
        }

		public static IPassportToken? Create(
            Guid guPassportId,
            string sProvider,
            bool bTwoFactorIsEnabled)
        {
            return Initialize(
                guId: Guid.NewGuid(),
                guPassportId: guPassportId,
                sProvider: sProvider,
                sRefreshToken: Guid.NewGuid().ToString(),
                bTwoFactorIsEnabled: bTwoFactorIsEnabled);
        }

        public static IPassportToken? Initialize(IPassportTokenTransferObject dtoPassportToken)
        {
            if (string.IsNullOrWhiteSpace(dtoPassportToken.Provider) == true)
                return null;

            return Initialize(
                guId: dtoPassportToken.Id,
                guPassportId: dtoPassportToken.PassportId,
                sProvider: dtoPassportToken.Provider,
                sRefreshToken: dtoPassportToken.RefreshToken,
                bTwoFactorIsEnabled: dtoPassportToken.TwoFactorIsEnabled);
        }

		public IPassportTokenTransferObject WriteTo<T>() where T : IPassportTokenTransferObject, new()
		{
            return new T()
            {
                Id = guId,
                PassportId = guPassportId,
                Provider = sProvider,
                RefreshToken = sRefreshToken,
                TwoFactorIsEnabled = bTwoFactorIsEnabled
            };
		}
	}
}