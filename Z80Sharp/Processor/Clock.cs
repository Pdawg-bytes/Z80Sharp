using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Processor
{
    internal class Clock
    {
        private readonly Stopwatch _stopwatch;
        private readonly long _ticksPerTState;
        private long _nextTickTarget;

        internal long TotalTStates;

        internal Clock(double clockSpeedMHz)
        {
            _ticksPerTState = (long)(Stopwatch.Frequency / (clockSpeedMHz * 1_000_000));
            _stopwatch = Stopwatch.StartNew();
            _nextTickTarget = 0;

            if (clockSpeedMHz == 0) { _ticksPerTState = 0; }
        }

        internal void Add([ConstantExpected] long tStates)
        {
            if (_ticksPerTState == 0) return;

            _nextTickTarget += tStates * _ticksPerTState;
            TotalTStates += tStates;
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