namespace Application.Common.DataProtection
{
	public class DataProtectionSetting
	{
		public const string SectionName = "DataProtection";

		public const string ApplicationName = "CQRS_Prototype";
		public const string DataProtectorPurpose = "PASSPORT_PROTECTION";
		public const string KeyStoragePathName = "DataProtection:KeyStoragePath";

		public string KeyStoragePath { get; init; } = string.Empty;
		public string EncryptionKey { get; init; } = string.Empty;
	}
}
