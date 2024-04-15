namespace Presentation.Endpoint
{
	internal static class EndpointRoute
	{
		private const string EndpointBase = "/api";

		internal static class Authentication
		{
			public const string Token = $"{EndpointBase}/token";
			public const string RefreshToken = $"{EndpointBase}/refresh";
		}

		internal static class Passport
		{
			public const string Base = $"{EndpointBase}/passport";
			public const string Create = Base;
			public const string Delete = Base;
			public const string GetById = $"{Base}/{{guId:Guid}}";
			public const string Update = Base;

			public const string Register = $"{Base}/register";
		}

		internal static class PassportHolder
		{
			private const string Base = $"{EndpointBase}/holder";
			public const string Create = Base;
			public const string Delete = Base;
			public const string GetById = $"{Base}/{{guId:Guid}}";
			public const string Update = Base;
		}

		internal static class PassportToken
		{
			public const string Base = $"{EndpointBase}/token";
			public const string Create = Base;
			public const string Delete = Base;
			public const string GetById = $"{Base}/{{guId:Guid}}";
			public const string GetUnspecific = Base;
			public const string Update = Base;
		}

		internal static class PassportVisa
		{
			public const string Base = $"{EndpointBase}/visa";
			public const string Create = Base;
			public const string Delete = Base;
			public const string GetById = $"{Base}/{{guId:Guid}}";
			public const string GetByPassportId = $"{Base}/passport";
			public const string GetUnspecific = Base;
			public const string Update = Base;
		}

		internal static class PhysicalDimension
		{
			public const string Base = $"{EndpointBase}/physical_dimension";
			public const string Create = Base;
			public const string Delete = Base;
			public const string GetById = $"{Base}/{{guId:Guid}}";
			public const string GetUnspecific = Base;
			public const string Update = Base;
		}

		internal static class TimePeriod
		{
			public const string Base = $"{EndpointBase}/time_period";
			public const string Create = Base;
			public const string Delete = Base;
			public const string GetById = $"{Base}/{{guId:Guid}}";
			public const string GetUnspecific = Base;
			public const string Update = Base;
		}
	}
}
