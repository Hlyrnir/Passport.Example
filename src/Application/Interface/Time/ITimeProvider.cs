namespace Application.Interface.Time
{
    public interface ITimeProvider
    {
        public TimeSpan GetElapsedTime(long lStartingTimestamp);
        public DateTimeOffset GetLocalNow();
        public long GetTimestamp();
        public DateTimeOffset GetUtcNow();
    }
}