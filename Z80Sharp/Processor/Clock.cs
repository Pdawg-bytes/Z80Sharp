using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Z80Sharp.Processor
{
    internal class Clock
    {
        private readonly Stopwatch _stopwatch;
        private readonly long _ticksPerTState;
        private long _nextTickTarget;

        private readonly long[] _precomputedTimes;

        internal long TotalTStates;
        internal bool LastOperationStatus;

        internal Clock(double clockSpeedMHz)
        {
            _ticksPerTState = (long)(Stopwatch.Frequency / (clockSpeedMHz * 1_000_000));
            _stopwatch = Stopwatch.StartNew();
            _nextTickTarget = 0;

            if (clockSpeedMHz == 0) { _ticksPerTState = 0; }

            _precomputedTimes = new long[256];
            for (int i = 0; i < 256; i++)
            {
                _precomputedTimes[i] = i * _ticksPerTState;
            }
        }

        internal void Add([ConstantExpected] int tStates)
        {
            if (_ticksPerTState == 0) return;

            unchecked
            {
                _nextTickTarget += _precomputedTimes[tStates];
                TotalTStates += tStates;
            }
        }
        internal void Add([ConstantExpected] int tStatesIfConditionMet, [ConstantExpected] int tStatesIfConditionNotMet)
        {
            if (_ticksPerTState == 0) return;
            int tStates = LastOperationStatus ? tStatesIfConditionMet : tStatesIfConditionNotMet;
            unchecked
            {
                _nextTickTarget += _precomputedTimes[tStates];
                TotalTStates += tStates;
            }
        }

        internal void Wait()
        {
            if (_ticksPerTState == 0) return;
            while (_stopwatch.ElapsedTicks < _nextTickTarget) { }
        }

        public void Reset()
        {
            _stopwatch.Restart();
            _nextTickTarget = 0;
            TotalTStates = 0;
        }
    }
}