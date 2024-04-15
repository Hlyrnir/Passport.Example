using Domain.Interface.Authorization;
using System.Text;

namespace DomainFaker
{
	public class PassportCredentialFaker : IPassportCredential
	{
		private string sProvider;
		private string sCredential;
		private string sSignature;

        public PassportCredentialFaker()
        {
			this.sProvider = string.Empty;
			this.sCredential = string.Empty;
			this.sSignature = string.Empty;
        }

        public string Provider { get => sProvider; }

		public string Credential { get => sCredential; }

		public string Signature { get => sSignature; }

		public byte[] HashSignature(IPassportHasher ppHasher)
		{
			return Encoding.UTF8.GetBytes(sSignature);
		}

		public bool Initialize(string sProvider, string sCredential, string sSignature)
		{
			this.sProvider = sProvider;
			this.sCredential = sCredential;
			this.sSignature = sSignature;

			return true;
		}
	}
}
