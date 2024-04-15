using Contract.v01.Response.Passport;
using Domain.Interface.Authorization;

namespace Presentation.Endpoint.Authorization.Passport.MapToResponse
{
	public static class MapPassportToResponse
	{
		public static PassportResponse MapToResponse(this IPassport ppPassport)
		{
			return new PassportResponse()
			{
				ConcurrencyStamp = ppPassport.ConcurrencyStamp,
				ExpiredAt = ppPassport.ExpiredAt,
				HolderId = ppPassport.HolderId,
				Id = ppPassport.Id,
				IsAuthority = ppPassport.IsAuthority,
				IsEnabled = ppPassport.IsEnabled,
				IssuedBy = ppPassport.IssuedBy,
				LastCheckedAt = ppPassport.LastCheckedAt,
				LastCheckedBy = ppPassport.LastCheckedBy,
				VisaId = ppPassport.VisaId
			};
		}
	}
}
