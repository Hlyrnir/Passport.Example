namespace Domain.Interface.Authorization
{
	public interface IPassportCredential
	{
		string Provider { get; }
		string Credential { get; }
		string Signature { get; }

		byte[] HashSignature(IPassportHasher ppHasher);
		bool Initialize(string sProvider, string sCredential, string sSignature);
	}
}