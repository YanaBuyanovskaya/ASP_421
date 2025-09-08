namespace ASP_421.Services.Random
{
    public class TimeStampService
    {
        public long TimeStampSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public long TimeStampMilliSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public long EpochTime()
        {
            return DateTimeOffset.UtcNow.Ticks;
        }

    }
}
