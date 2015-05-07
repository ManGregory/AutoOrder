using System;

namespace AutoOrder.Misc
{
    public static class DateTimeExtension
    {
        public static int GetDecade(this DateTime date)
        {
            if (date.Day >= 1 && date.Day <= 10) return 1;
            if (date.Day >= 10 && date.Day <= 20) return 2;
            return 3;
        }
    }
}