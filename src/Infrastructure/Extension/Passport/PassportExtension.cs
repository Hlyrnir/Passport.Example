namespace Infrastructure.Extension.Passport
{
	public enum PassportColumn
	{
		ConcurrencyStamp,
		CreatedAt,
		EditedAt,
		ExpiredAt,
		HasPermissionToCommand,
		HasPermissionToQuery,
		HolderId,
		Id,
		IsAuthority,
		IsEnabled,
		IssuedBy,
		LastCheckedAt,
		LastCheckedBy
	}

	public enum PassportVisaRegisterColumn
	{
		Id,
		PassportId,
		PassportVisaId,
		RegisteredAt
	}

	public enum PassportTable
	{
		Passport
	}

	public enum PassportVisaRegisterTable
	{
		PassportVisaRegister
	}
}
