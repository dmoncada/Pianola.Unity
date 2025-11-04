using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

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

            var mins = seconds / 60;
            var secs = seconds % 60;

            var formatted = $"{mins}:{secs:D2}";
            return isNegative ? $"-{formatted}" : formatted;
        }
    }

    public class TimedRegion : IDisposable
    {
        private readonly Stopwatch _stopwatch = null;
        private readonly UnityEngine.Object _context = null;
        private readonly string _caller = null;

        public TimedRegion(
            UnityEngine.Object context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0
        )
        {
            _context = context;

            _caller = $"{memberName} ({Path.GetFileName(filePath)}:{lineNumber})";

            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            Debug.LogFormat(
                "[{0}] Elapsed: {1}",
                _caller,
                Utils.FormatTime(_stopwatch.Elapsed),
                _context
            );
        }
    }
}
