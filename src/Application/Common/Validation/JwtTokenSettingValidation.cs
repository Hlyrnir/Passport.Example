using Application.Common.Authentication;
using Microsoft.Extensions.Options;

namespace Application.Common.Validation
{
	public class JwtTokenSettingValidation : IValidateOptions<JwtTokenSetting>
	{
		public ValidateOptionsResult Validate(string? name, JwtTokenSetting optJwtToken)
		{
			if (optJwtToken == null)
				throw new ArgumentNullException(nameof(optJwtToken));

			if (optJwtToken.SecretKey == string.Empty)
				return ValidateOptionsResult.Fail("Secret key is an empty string.");

			if (optJwtToken.Issuer == string.Empty)
				return ValidateOptionsResult.Fail("Issuer is an empty string.");

			if (optJwtToken.Audience == string.Empty)
				return ValidateOptionsResult.Fail("Audience is an empty string.");

			return ValidateOptionsResult.Success;
		}
	}
}
