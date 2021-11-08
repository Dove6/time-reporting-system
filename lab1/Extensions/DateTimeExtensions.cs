using System;

namespace TRS.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime TrimToMonth(this DateTime datetime) =>
            new DateTime(datetime.Year, datetime.Month, 1);
    }
}
