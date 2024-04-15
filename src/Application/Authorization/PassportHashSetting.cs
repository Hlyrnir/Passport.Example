using Domain.Interface.Passport;

namespace Application.Authorization
{
	public sealed class PassportHashSetting : IPassportHashSetting
	{
		public static string SectionName = "SignatureHash";

		private readonly string sPublicKey;

		public PassportHashSetting()
		{
			sPublicKey = string.Empty;
		}

		public string PublicKey { get => sPublicKey; init => sPublicKey = value; }
	}
}
