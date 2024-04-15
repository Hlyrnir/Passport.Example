using Application.Authorization;
using Microsoft.Extensions.Options;

namespace Application.Common.Validation.Passport
{
	public sealed class PassportHashSettingValidation : IValidateOptions<PassportHashSetting>
	{
		public ValidateOptionsResult Validate(string? sName, PassportHashSetting ppHashSetting)
		{
			if (ppHashSetting == null)
				throw new ArgumentNullException(nameof(ppHashSetting));

			if (string.IsNullOrWhiteSpace(ppHashSetting.PublicKey) == true)
				return ValidateOptionsResult.Fail("Public key is an empty string.");

			return ValidateOptionsResult.Success;
		}
	}
}