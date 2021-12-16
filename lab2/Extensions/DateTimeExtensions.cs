using System.Globalization;

namespace TRS.Extensions;

public static class DateTimeExtensions
{
    public const string DateFormat = "yyyy-MM-dd";
    public const string MonthFormat = "yyyy-MM";

    public static DateTime TrimToMonth(this DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month, 1);

    public static string ToDateString(this DateTime dateTime) =>
        dateTime.ToString(DateFormat, CultureInfo.InvariantCulture);

    public static string ToMonthString(this DateTime dateTime) =>
        dateTime.ToString(MonthFormat, CultureInfo.InvariantCulture);
}
