using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.GameEngine.Graphics.Particles
{
    public class Particle:ITemporaryObject
    {
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

        IParticleClass particleClass;

        public IParticleClass ParticleClass
        {
            get { return particleClass; }
        }

        float angularVelocity;

        public float AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }
        float rotation;

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        float currentTime;

        public float CurrentTime
        {
            get { return currentTime/maxLifetime; }
        }
        float maxLifetime;

        public float MaxLifetime
        {
            get { return maxLifetime; }
            set { maxLifetime = value; }
        }


        object customData;

        public object CustomData
        {
            get { return customData; }
            set { customData = value; }
        }

        public Particle(IParticleClass particleClass)
        {
            this.particleClass = particleClass;
        }
        public Particle(MyVector velocity, MyVector position, IParticleClass particleClass, float maxLifeTime)
        {
            this.velocity = velocity;
            this.position = position;
            this.particleClass = particleClass;
            this.maxLifetime = maxLifeTime;

            angularVelocity = 0;
            rotation = 0;
        }
        public Particle(MyVector velocity, MyVector position,float angularVelocity, IParticleClass particleClass, float maxLifeTime)
        {
            this.velocity = velocity;
            this.position = position;
            this.particleClass = particleClass;
            this.maxLifetime = maxLifeTime;

            this.angularVelocity = angularVelocity;
            rotation = 0;
        }

        #region ITemporaryObject Members

        public float RemainingTime
        {
            get { return maxLifetime-currentTime; }
        }

        #endregion

       

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            currentTime += timeElapsed;

            position += velocity * timeElapsed;
            rotation += timeElapsed * angularVelocity;
            return false;
        }

        #endregion
    }
}
