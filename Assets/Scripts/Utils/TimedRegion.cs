using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Pianola
{
    public sealed class TimedRegion : IDisposable
    {
        private readonly UnityEngine.Object _context = null;
        private readonly Stopwatch _stopwatch = null;
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

    public sealed class UnityTimedRegion : IDisposable
    {
        private readonly UnityEngine.Object _context;
        private readonly float _startTime;
        private readonly string _caller;

        public UnityTimedRegion(
            UnityEngine.Object context = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0
        )
        {
            _context = context;

            _caller = $"{memberName} ({Path.GetFileName(filePath)}:{lineNumber})";

            _startTime = Time.realtimeSinceStartup;
        }

        public void Dispose()
        {
            float elapsed = Time.realtimeSinceStartup - _startTime;

            Debug.LogFormat(
                _context,
                "[{0}] Elapsed: {1}",
                _caller,
                Utils.FormatTime(TimeSpan.FromSeconds(elapsed))
            );
        }
    }
}
