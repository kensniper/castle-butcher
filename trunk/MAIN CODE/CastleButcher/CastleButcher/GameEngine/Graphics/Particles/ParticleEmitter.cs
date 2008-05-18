using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Framework.MyMath;

namespace CastleButcher.GameEngine.Graphics.Particles
{
    public class EmitterParameters
    {
        public float ParticlesPerSecond;
        public float ParticleReuse
        {
            get { return 1f / ParticlesPerSecond; }
        }

        public MyVector InitialVelocity;

        public EmitterParameters(float pps, MyVector vel)
        {
            this.ParticlesPerSecond = pps;
            this.InitialVelocity = vel;
        }
    }
    public abstract class ParticleEmitter
    {
        bool stopped = false;

        private static Random randomGenerator=new Random();

        protected static Random RandomGenerator
        {
            get { return ParticleEmitter.randomGenerator; }
            set { ParticleEmitter.randomGenerator = value; }
        }

        public bool Stopped
        {
            get { return stopped; }
            set { stopped = value; }
        }

        float intensity = 1;

        public float Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        MyVector position;

        public MyVector Position
        {
            get { return position; }
            set { position = value; }
        }
        MyVector velocity;

        public MyVector Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        MyQuaternion orientation;

        public MyQuaternion Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }
       

        ParticleSystem particleSystem;

        public ParticleSystem ParticleSystem
        {
            get { return particleSystem; }
            set { particleSystem = value; }
        }

        public ParticleEmitter(ParticleSystem system)
        {
            this.particleSystem = system;
        }

        public abstract void Update(float dt);



        protected virtual int Emit(float probability)
        {
            double l=randomGenerator.NextDouble();
            if (l < probability)
            {
//                return (int)Math.Log(l, probability);
                return (int)(probability/l);
            }
            else return 0;
        }
    }

    public class PointEmitter : ParticleEmitter
    {
        public PointEmitter(ParticleSystem system, EmitterParameters parameters,IParticleClass particleClass)
            : base(system)
        {
            this.emitterParameters = parameters;
            this.particleClass = particleClass;
            timeSinceLastEmit = parameters.ParticleReuse;

            Position = new MyVector(0, 0, 0);
            Velocity = new MyVector(0, 0, 0);
            Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
        }

        EmitterParameters emitterParameters;
        IParticleClass particleClass;
        float timeSinceLastEmit;

       

        public override void Update(float dt)
        {
            if (Stopped) return;
            timeSinceLastEmit += dt;


            if (Intensity <= 0) return;

            int n = Emit(dt);
            while (n > 0)
            {

                Particle p = new Particle(
                    this.Velocity + this.emitterParameters.InitialVelocity.Rotate(this.Orientation),
                    this.Position, particleClass,10);
                this.ParticleSystem.AddParticle(p);
                n--;
            }

            
            

        }
    }

    
}
