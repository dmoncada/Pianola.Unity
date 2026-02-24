using System;
using UnityEngine;

namespace Pianola
{
    public static class Utils
    {
        private static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatBytes(long bytes)
        {
            var value = bytes;
            int index = 0;

            while (value >= 1024 && index < Sizes.Length - 1)
            {
                value >>= 10;
                index += 1;
            }

            var adjusted = (double)bytes / (1L << (10 * index));
            return $"{adjusted:0.##} {Sizes[index]}";
        }

        public static string FormatTime(TimeSpan span)
        {
            if (span.Minutes > 0) // X:YY minutes
            {
                return $"{FormatTime(span.TotalSeconds)} minutes";
            }
            if (span.Seconds > 0) // X.YY seconds
            {
                return $"{span.TotalMilliseconds / 1000:0.##} seconds";
            }
            else // X milliseconds
            {
                return $"{span.Milliseconds} milliseconds";
            }
        }

        public static string FormatTime(double seconds)
        {
            return FormatTime((int)seconds);
        }

        public static string FormatTime(float seconds)
        {
            return FormatTime((int)seconds);
        }

        public static string FormatTime(int seconds)
        {
            if (-1f < seconds && seconds < 1f)
            {
                return "0:00";
            }

            var isNegative = seconds < 0;
            seconds = Mathf.Abs(seconds);

            int mins = seconds / 60;
            int secs = seconds % 60;

            var formatted = $"{mins}:{secs:D2}";
            return isNegative ? $"-{formatted}" : formatted;
        }

        public static bool InRange(this int number, int start, int end, bool inclusive = false)
        {
            return start <= number && number < end || (inclusive && number == end);
        }
    }

    public static class Vector3Extensions
    {
        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
    }
}
