using System;

namespace Infrastructure.Extensions
{
    public static class DateTimeExtension
    {
        public static string Timestamp(this DateTime source)
        {
            return (source - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString("F0");
        }
    }
}