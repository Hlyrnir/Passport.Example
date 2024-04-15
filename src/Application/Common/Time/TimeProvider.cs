using Application.Interface.Time;

namespace Application.Common.Time
{
	public class TimeProvider : ITimeProvider
	{
		public TimeSpan GetElapsedTime(long lStartingTimestamp)
		{
			throw new NotImplementedException();
		}

		public DateTimeOffset GetLocalNow()
		{
			return DateTimeOffset.Now;
		}

		public long GetTimestamp()
		{
			throw new NotImplementedException();
		}

		public DateTimeOffset GetUtcNow()
		{
			return DateTimeOffset.UtcNow;
		}
	}
}
