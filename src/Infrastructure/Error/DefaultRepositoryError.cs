using Application.Common.Result.Repository;

namespace Infrastructure.Error
{
	internal static class DefaultRepositoryError
	{
		public static RepositoryError TaskAborted => new RepositoryError() { Code = "METHOD_REPOSITORY", Description = "Task has been cancelled." };
	}

	internal static class PassportError
	{
		internal static class Code
		{
			public static string Method = "METHOD_REPOSITORY_PASSPORT";
			public static string Exception = "EXCEPTION_REPOSITORY_PASSPORT";
		}
	}

	internal static class PhysicalDimensionError
	{
		internal static class Code
		{
			public static string Method = "METHOD_REPOSITORY_PHYSICAL_DIMENSION";
			public static string Exception = "EXCEPTION_REPOSITORY_PHYSICAL_DIMENSION";
		}
	}

	internal static class TimePeriodError
	{
		internal static class Code
		{
			public static string Method = "METHOD_REPOSITORY_TIME_PERIOD";
			public static string Exception = "EXCEPTION_REPOSITORY_TIME_PERIOD";
		}
	}
}
