using Application.Common.DataProtection;
using Microsoft.Extensions.Options;

namespace Application.Common.Validation
{
	public class DataProtectionSettingValidation : IValidateOptions<DataProtectionSetting>
	{
		public ValidateOptionsResult Validate(string? sName, DataProtectionSetting dpSetting)
		{
			if (dpSetting == null)
				throw new ArgumentNullException(nameof(dpSetting));

			if (dpSetting.EncryptionKey == string.Empty)
				return ValidateOptionsResult.Fail("Encryption key is an empty string.");

			if (dpSetting.KeyStoragePath == string.Empty)
				return ValidateOptionsResult.Fail("Key storage path is an empty string.");

			if (Path.Exists(dpSetting.KeyStoragePath) == false)
				return ValidateOptionsResult.Fail("Key storage path does not exist.");

			return ValidateOptionsResult.Success;
		}
	}
}
