namespace Domain.Interface.Authorization
{
	public interface IPassportHasher
	{
		byte[] HashSignature(string sUnprotectedSignature);
	}
}