using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalPublishingPlatform.Helpers
{
    public static class NumberExtensions
    {
        public static string ToReadeableSize(this long size) {
            if (size <= 0) return "0";
            var units = new[] { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            var digitGroups = (int)(Math.Log10(size) / Math.Log10(1024));
            return String.Format("{0:#,##0.##} {1}", size / Math.Pow(1024, digitGroups), units[digitGroups + 1]);
        }

        public static string ToOrdinal(this long number)
        {
            if (number < 0) return number.ToString();
            var rem = number % 100;
            if (rem >= 11 && rem <= 13) return number + "th";
            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        public static string ToOrdinal(this int number)
        {
            return ((long)number).ToOrdinal();
        }
    }
}