using Application.Interface.Time;

namespace ApplicationTest.Common
{
	internal class TimeProviderFaker : ITimeProvider
	{
		private static readonly DateTimeOffset dtDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, 0, 0, TimeSpan.Zero);

		public TimeSpan GetElapsedTime(long lStartingTimestamp)
		{
			throw new NotImplementedException();
		}

		public DateTimeOffset GetLocalNow()
		{
			return dtDate;
		}

		public long GetTimestamp()
		{
			throw new NotImplementedException();
		}

		public DateTimeOffset GetUtcNow()
		{
			return dtDate;
		}
	}
}