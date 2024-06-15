using Contract.v01.Response.Passport;
using Domain.Interface.Authorization;

namespace Presentation.Endpoint.Authorization.PassportVisa.MapToResponse
{
	public static class MapPassportVisaToResponse
	{
		public static IEnumerable<PassportVisaResponse> MapToResponse(this IEnumerable<IPassportVisa> enumPassportVisa)
		{
			foreach (IPassportVisa ppVisa in enumPassportVisa)
			{
				yield return ppVisa.MapToResponse();
			}
		}

		public static PassportVisaResponse MapToResponse(this IPassportVisa ppPassportVisa)
		{
			return new PassportVisaResponse()
			{
				ConcurrencyStamp = ppPassportVisa.ConcurrencyStamp,
				Id = ppPassportVisa.Id,
				Name = ppPassportVisa.Name,
				Level = ppPassportVisa.Level
			};
		}
	}
}
