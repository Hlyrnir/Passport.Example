using System.Data;

namespace Infrastructure.Extension
{
	internal static class DateTimeOffsetExtension
	{
		internal static DateTimeOffset GetDateTimeOffset(this IDataReader sqlReader, int i)
		{
			return DateTimeOffset.Parse(sqlReader.GetString(i));
		}
	}
}
