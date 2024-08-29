namespace Contract.v01.Response.Authentication
{
	public class JwtTokenResponse
	{
		public required DateTimeOffset ExpiredAt { get; init; }
		public required string Provider { get; init; }
		public required string Token { get; init; }
		public required string RefreshToken { get; init; }
	}
}
