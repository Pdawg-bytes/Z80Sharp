using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Z80Sharp.Processor
{
    internal class Clock
    {
        private readonly Stopwatch _stopwatch;
        private readonly ulong _ticksPerTState;
        private ulong _nextTickTarget;

        private readonly ulong[] _precomputedTimes;

        internal ulong TotalTStates;
        internal bool LastOperationStatus;

        internal Clock(double clockSpeedMHz)
        {
            _ticksPerTState = (ulong)(Stopwatch.Frequency / (clockSpeedMHz * 1_000_000));
            _stopwatch = Stopwatch.StartNew();
            _nextTickTarget = 0;

            if (clockSpeedMHz == 0) { _ticksPerTState = 0; }

            _precomputedTimes = new ulong[256];
            for (uint i = 0; i < 256; i++)
            {
                _precomputedTimes[i] = i * _ticksPerTState;
            }
        }

        internal void Add([ConstantExpected] uint tStates)
        {
            unchecked
            {
                TotalTStates += tStates;
                if (_ticksPerTState == 0) return;
                _nextTickTarget += _precomputedTimes[tStates];
            }
        }
        internal void Add([ConstantExpected] uint tStatesIfConditionMet, [ConstantExpected] uint tStatesIfConditionNotMet)
        {
            unchecked
            {
                uint tStates = LastOperationStatus ? tStatesIfConditionMet : tStatesIfConditionNotMet;
                TotalTStates += tStates;
                if (_ticksPerTState == 0) return;
                _nextTickTarget += _precomputedTimes[tStates];
            }
        }

        internal void Wait()
        {
            if (_ticksPerTState == 0) return;
            while ((ulong)_stopwatch.ElapsedTicks < _nextTickTarget) { }
        }

        public void Reset()
        {
            _stopwatch.Restart();
            _nextTickTarget = 0;
            TotalTStates = 0;
        }
    }
}