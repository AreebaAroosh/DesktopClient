using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace VideoCallP2P
{
    public class CustomizeTimer
    {
        private const int UNIT_MILISECONDS = 500;
        private const int SECOND_IN_MILISECONDS = 1000;

        private Timer _MyTimer;
        private bool _IsAlive;
        private bool _IsResumed;
        private int _Counter;

        private long _ElipsedMiliseconds;
        private int _InitIntervalInSecond;
        private int _IntervalInSecond;
        private object _Tag;

        public delegate void TickHandler(int counter, bool initTick = false, object state = null);
        public event TickHandler Tick;

        public delegate void CompeteHandler(int counter);
        public event CompeteHandler Compete;

        public CustomizeTimer()
        {
            _IntervalInSecond = SECOND_IN_MILISECONDS;
            _InitIntervalInSecond = 0;

            _MyTimer = new Timer();
            _MyTimer.Interval = UNIT_MILISECONDS;
            _MyTimer.AutoReset = true;
            _MyTimer.Elapsed += (source, e) =>
            {
                if (_IsAlive)
                {
                    _ElipsedMiliseconds += UNIT_MILISECONDS;
                    if (_IsResumed && _ElipsedMiliseconds >= _InitIntervalInSecond && _ElipsedMiliseconds % _IntervalInSecond == 0)
                    {
                        _Counter++;
                        if (Tick != null)
                        {
                            Tick(_Counter, false, _Tag);
                        }
                    }
                }
                else
                {
                    _MyTimer.Enabled = false;
                    if (Compete != null)
                    {
                        Compete(_Counter);
                    }
                }
            };
        }

        public void Start()
        {
            _IsAlive = true;
            _IsResumed = true;
            _ElipsedMiliseconds = 0;
            _Counter = 0;
            _MyTimer.Enabled = true;

            if (Tick != null)
            {
                Tick(_Counter, true, _Tag);
            }
        }

        public void Stop()
        {
            _IsAlive = false;
            _IsResumed = false;
        }

        public void Pause()
        {
            if (_IsAlive && _IsResumed)
            {
                _IsResumed = false;
            }
        }

        public void Resume()
        {
            if (_IsAlive && _IsResumed == false)
            {
                _IsResumed = true;
            }
            else if (_IsAlive == false)
            {
                throw new Exception("Timer is not alive!");
            }
        }

        public int IntervalInSecond
        {
            get { return _IntervalInSecond; }
            set { _IntervalInSecond = value * 1000; }
        }

        public int InitIntervalInSecond
        {
            get { return _InitIntervalInSecond; }
            set { _InitIntervalInSecond = value * 1000; }
        }

        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        public int Counter
        {
            get { return _Counter; }
        }

        public bool IsAlive
        {
            get { return _IsAlive; }
            set { _IsAlive = value; }
        }

    }
}
