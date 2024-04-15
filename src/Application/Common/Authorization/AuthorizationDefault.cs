namespace Application.Common.Authorization
{
	internal static class AuthorizationDefault
	{
		public static class Name
		{
			public const string Passport = "PASSPORT";

			public const string PhysicalDimension = "PHYSICAL_DIMENSION";
			public const string TimePeriod = "TIME_PERIOD";
		}

		public static class Level
		{
			public const int Read = 0;
			public const int Update = 1;
			public const int Create = 2;
			public const int Delete = 3;
		}
	}
}
