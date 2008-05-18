using System;
using System.Collections.Generic;
using System.Text;

namespace CastleButcher.GameEngine.Resources
{
    public abstract class AnimationInstance
    {
        protected float duration;
        private bool loop;

        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        private float position;

        public float Position
        {
            get { return position; }
        }
        protected bool play;

        public virtual bool Running
        {
            get
            {
                return play;
            }
        }

        public virtual void Reset()
        {
            position = 0;
        }

        public virtual float Duration
        {
            get { return duration; }
        }

        public virtual void Start()
        {
            play = true;
        }

        public virtual void Stop()
        {
            play = false;
        }

        public virtual void Rewind(float position)
        {
            this.position = position;
        }


        public virtual void Advance(float dt)
        {
            if (play)
            {
                position += dt;
                if (Position > duration)
                {
                    if (Loop)
                    {
                        Rewind(position - duration);
                    }
                    else
                    {
                        position = duration;
                        play = false;
                    }
                }
            }

        }
     
       
    
    }
}
