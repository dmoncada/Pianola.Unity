using UnityEngine;

namespace Pianola
{
    public static class Utils
    {
        private static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatBytes(long bytes)
        {
            var value = bytes;
            var index = 0;

            while (value >= 1024 && index < Sizes.Length - 1)
            {
                value >>= 10;
                index += 1;
            }

            var adjusted = (double)bytes / (1L << (10 * index));
            return $"{adjusted:0.##} {Sizes[index]}";
        }

        public static string FormatTime(float seconds)
        {
            if (-1f < seconds && seconds < 1f)
            {
                return "0:00";
            }

            var isNegative = seconds < 0;
            seconds = Mathf.Abs(seconds);

            var mins = (int)(seconds / 60);
            var secs = (int)(seconds % 60);

            var formatted = $"{mins}:{secs:D2}";
            return isNegative ? $"-{formatted}" : formatted;
        }
    }
}
