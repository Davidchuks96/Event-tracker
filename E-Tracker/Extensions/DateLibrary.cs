using System;
using System.Globalization;

namespace E_Tracker.Extensions
{
    public static class DateLibrary
    {
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow.AddHours(1);
        }

        public static DateTime GetYesterdayDateTime()
        {
            return DateTime.UtcNow.AddDays(-1);
        }

        public static DateTime ValidateStartDate(string startdate)
        {
            if (startdate != null)
            {
                var convertedDate = startdate.ToInvariantDateTime("dd MMM yyyy");
                //string was wrongly formatted and could not be converted
                if (convertedDate.Date == DateTime.MinValue.Date)
                {
                    return GetFirstDayOfTheMonth();
                }
                //string was correctly formatted and was converted
                return convertedDate;
            }
            return GetFirstDayOfTheMonth();
        }

        public static DateTime ValidateEndDate(DateTime validatedStartdate, string enddate)
        {
            //if enddate is null, add 1 month to validatedStartdate
            if (enddate == null)
            {
                return validatedStartdate.AddMonths(1).AddSeconds(-1);
            }

            var convertedEndDate = enddate.ToInvariantDateTime("dd MMM yyyy");
            
            //If string was wrongly formatted and could not be converted
            //add 1 month to the validated Start Date
            if (convertedEndDate.Date == DateTime.MinValue.Date)
            {
                return validatedStartdate.AddMonths(1).AddSeconds(-1);
            }

            //string was correctly formatted and was converted
            //check for if start date is higher than end date
            if (convertedEndDate < validatedStartdate)
            {
                //add 1 month to the validated Start Date
                return validatedStartdate.AddMonths(1).AddSeconds(-1);
            }

            return convertedEndDate;
        }

        public static DateTime GetLastWeekDateTime()
        {
            return DateTime.UtcNow.AddDays(-7);
        }

        public static DateTime AddMinutesToCurrentTime(double x)
        {
            return DateTime.UtcNow.AddHours(1).AddMinutes(x);
        }

        public static DateTime AddDaysToCurrentTime(double x)
        {
            return DateTime.UtcNow.AddDays(x);
        }

        public static DateTime AddMonthsToCurrentTime(int x)
        {
            return DateTime.UtcNow.AddMonths(x);
        }

        public static DateTime AddYearsToCurrentTime(int x)
        {
            return DateTime.UtcNow.AddYears(x);
        }

        public static int GetWeekNumberOfMonth(DateTime date)
        {
            if (date != DateTime.MinValue)
            {
                date = date.Date;
                DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
                var val = (DayOfWeek.Sunday + 7 - firstMonthDay.DayOfWeek);
                DateTime firstMonthSunday = firstMonthDay.AddDays(val);
                if (firstMonthSunday > date)
                {
                    firstMonthDay = firstMonthDay.AddMonths(-1);
                    firstMonthSunday = firstMonthDay.AddDays((DayOfWeek.Sunday + 7 - firstMonthDay.DayOfWeek));
                }
                return (date - firstMonthSunday).Days / 7 + 1;
            }
            return 0;
        }
        public static DateTime ToInvariantDateTime(this string value, string format)
        {
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.InvariantInfo;
            var result = DateTime.TryParseExact(value, format, dtfi, DateTimeStyles.None, out DateTime newValue);
            return newValue;
        }

        public static string ToDateString(this DateTime dt, string format)
        {
            return dt.ToString(format, DateTimeFormatInfo.InvariantInfo);
        }

        public static DateTime GetFirstDayOfTheMonth()
        {
            var today = DateTime.UtcNow.Date;
            DateTime firstDayOfTheMonth = new DateTime(today.Year, today.Month, 1);
            return firstDayOfTheMonth;
        }

        public static DateTime GetLastDayOfTheMonth()
        {
            DateTime lastDayOfTheMonth = GetFirstDayOfTheMonth().AddMonths(1).AddSeconds(-1);
            return lastDayOfTheMonth;
        }
    }
}
