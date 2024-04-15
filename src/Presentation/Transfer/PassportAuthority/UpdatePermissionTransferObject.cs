namespace Presentation.Transfer.Passport
{
	public struct UpdatePermissionTransferObject
	{
		public UpdatePermissionTransferObject()
		{

		}

		public string Provider { get; init; } = "DEFAULT_PROVIDER";
		public string Credential { get; init; } = "DEFAULT_CREDENTIAL";
		public string Signature { get; init; } = "DEFAULT_SIGNATURE";

		public string SecurityStamp { get; init; } = "NO_SECURITY_STAMP";

		public string PassportId { get; init; } = "DEFAULT_ID";
		public bool PermissionToCommand { get; init; } = false;
		public bool PermissionToQuery { get; init; } = false;
	}
}
