using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    /// <summary>
    /// A general purpose Timer class, designed for to be inherited
    /// to extend its functionality. Performs basic operations like start/stop
    /// and tells how much time passed.
    /// </summary>
    public class GenericTimer
    {
        protected long m_ticksPerSecond;
        protected long m_currentTime;
        protected long m_lastTime;


        protected float m_runningTime;
        protected float m_timeElapsed;
        protected bool m_timerStopped;

        /// <summary>Creates a new Timer</summary>
        public GenericTimer()
        {
            // Find the frequency, or amount of ticks per second
            NativeMethods.QueryPerformanceFrequency(ref m_ticksPerSecond);

            m_timerStopped = true;
        }

        /// <summary>Starts the timer.</summary>
        public virtual void Start()
        {
            if (!Stopped)
            {
                return;
            }
            NativeMethods.QueryPerformanceCounter(ref m_lastTime);
            m_timerStopped = false;
        }

        /// <summary>Stops the timer.</summary>
        public virtual void Stop()
        {
            if (Stopped)
            {
                return;
            }
            long stopTime = 0;
            NativeMethods.QueryPerformanceCounter(ref stopTime);
            m_runningTime += (float)(stopTime - m_lastTime) / (float)m_ticksPerSecond;
            m_timerStopped = true;
        }
        /// <summary>Resets the timer</summary>
        public virtual void Reset()
        {
            m_runningTime = 0;
            m_timerStopped = true;
            m_timeElapsed = 0;

            NativeMethods.QueryPerformanceFrequency(ref m_ticksPerSecond);
        }
        /// <summary>Updates the timer.</summary>
        public virtual bool Update()
        {
            if (Stopped)
            {
                return false;
            }

            // Get the current time
            NativeMethods.QueryPerformanceCounter(ref m_currentTime);

            // Update time elapsed since last frame
            m_timeElapsed = (float)(m_currentTime - m_lastTime) / (float)m_ticksPerSecond;
            m_runningTime += m_timeElapsed;

            m_lastTime = m_currentTime;
            return true;
        }

        /// <summary>Is the timer stopped?</summary>
        public virtual bool Stopped
        {
            get { return m_timerStopped; }
        }

        /// <summary>Elapsed time since last update. If the timer is stopped, returns 0.</summary>
        public virtual float ElapsedTime
        {
            get
            {
                if (Stopped)
                {
                    return 0;
                }
                return m_timeElapsed;
            }
        }

        /// <summary>Total running time.</summary>
        public virtual float RunningTime
        {
            get { return m_runningTime; }
        }
    }

}
