namespace safespace.Model
{
    public class Date
    {
        public static DateTime GetEgyptTime()
        {
            var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);
        }
    }
}
