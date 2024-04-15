using Domain.Interface.Authorization;
using Domain.Interface.Passport;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Application.Authorization
{
	public class PassportHasher : IPassportHasher
	{
		IPassportHashSetting ppHashSetting;

		public PassportHasher(IOptions<PassportHashSetting> optAccessor)
		{
			ppHashSetting = optAccessor.Value;
		}

		public byte[] HashSignature(string sUnprotectedSignature)
		{
			using (HMACSHA256 hmacSHA256 = new HMACSHA256(Encoding.UTF8.GetBytes(ppHashSetting.PublicKey)))
			{
				return hmacSHA256.ComputeHash(Encoding.UTF8.GetBytes(sUnprotectedSignature));
			}
		}
	}
}