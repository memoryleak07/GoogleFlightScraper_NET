using System.Globalization;

namespace GFS_NET.Helpers
{
    public static partial class DateTimeExtensions
    {
        private readonly static CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var diff = dt.DayOfWeek - DefaultCulture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime NextWeekendDay(DateTime dt)
        {
            return dt.FirstDayOfWeek().AddDays(5);
        }
    }
}
