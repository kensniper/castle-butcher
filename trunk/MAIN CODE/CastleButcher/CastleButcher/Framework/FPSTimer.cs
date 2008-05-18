using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    public class FPSTimer : GenericTimer
    {
        protected long m_lastFPSUpdate;
        protected long m_FPSUpdateInterval;
        protected float m_fps;
        protected long m_numFrames;


        /// <summary>Creates a new Timer</summary>
        public FPSTimer(float updateTime)
            : base()
        {
            //
            m_FPSUpdateInterval = (int)(updateTime * (float)m_ticksPerSecond);

        }

        /// <summary>Updates the timer.</summary>
        public override bool Update()
        {
            //perform standard Update()
            bool res=base.Update();

            // Update FPS
            m_numFrames++;
            if (m_currentTime - m_lastFPSUpdate >= m_FPSUpdateInterval)
            {
                float currentTime = (float)m_currentTime / (float)m_ticksPerSecond;
                float lastTime = (float)m_lastFPSUpdate / (float)m_ticksPerSecond;
                m_fps = (float)m_numFrames / (currentTime - lastTime);

                m_lastFPSUpdate = m_currentTime;
                m_numFrames = 0;
            }
            return res;
        }

        /// <summary>Frames per second</summary>
        public float FPS
        {
            get { return m_fps; }
        }
    }
}
