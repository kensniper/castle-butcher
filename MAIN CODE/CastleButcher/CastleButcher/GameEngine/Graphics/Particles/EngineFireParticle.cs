using System;
using System.Collections.Generic;
using System.Text;
using CastleButcher.GameEngine.Resources;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Framework.MyMath;

namespace CastleButcher.GameEngine.Graphics.Particles
{
    public class EngineFireParticleClass : IParticleClass
    {

        static Random randomGenerator = new Random();
        public EngineFireParticleClass()
        {
            fireMap = ResourceCache.Instance.GetTexture("fire.dds");
            smokeMap = ResourceCache.Instance.GetTexture("smoke.dds");
        }
        Resources.MapData fireMap;
        Resources.MapData smokeMap;
        #region IParticleClass Members

        public EffectData Effect
        {
            get { return ResourceCache.Instance.GetEffect("particle.fx"); }
        }

        private float Size(float t)
        {
            return 1 + 10 * t;
        }

        private float Alpha(float t)
        {
            if (t < 0.1f)
            {
                return 1 - t * t / 0.01f;
            }
            else if (t < 0.15)
            {
                return (t - 0.1f) * (t - 0.1f) / 0.0025f;
            }
            else if (t < 0.7f)
                return 1;
            else
            {
                return 1 - (t - 0.7f) * (t - 0.7f) / 0.09f;
            }
        }
        private Texture TextureMap(float t)
        {
            if (t < 0.1f)
                return fireMap.DxTexture;
            else
                return smokeMap.DxTexture;
        }

        public int Render(List<Particle> particles, Device device, Renderer renderer)
        {
            int count = 0;
            renderer.ShaderConstants.World = Matrix.Identity;
            EffectData effect = this.Effect;

            effect.DxEffect.Technique = "particle_map";


            BillboardObject baseBillboard = renderer.MakeBillboardObject(new MyVector(0, 0, 0), 1);

            effect.DxEffect.Begin(FX.None);


            //effect.
            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i].ParticleClass == this)
                {
                    renderer.ShaderConstants.World = Matrix.Translation((Vector3)particles[i].Position);
                    effect.DxEffect.SetValue("WorldViewProjection", renderer.ShaderConstants.WorldViewProjection);
                    baseBillboard.Scale = Size(particles[i].CurrentTime) * 10;
                    effect.DxEffect.SetValue("ParticleTexture", TextureMap(particles[i].CurrentTime));
                    float alpha = Alpha(particles[i].CurrentTime);
                    effect.DxEffect.SetValue("Alpha", alpha);
                    effect.DxEffect.BeginPass(0);
                    baseBillboard.Render(device);
                    effect.DxEffect.EndPass();
                    count++;
                }
            }


            effect.DxEffect.End();

            return count;
        }
        public Particle MakeParticle()
        {

            Particle p = new Particle(this);
            p.MaxLifetime = (float)(1 * (0.7 + 0.6 * randomGenerator.NextDouble()));
            return p;
        }

        #endregion
    }


    public class EngineEmitter : ParticleEmitter
    {
        public EngineEmitter(ParticleSystem system)
            : base(system)
        {
            this.emitterParameters = new EmitterParameters(500, new MyVector(0, 0, 30));
            this.particleClass = new EngineFireParticleClass();

            Position = new MyVector(0, 0, 0);
            Velocity = new MyVector(0, 0, 0);
            Orientation = MyQuaternion.FromEulerAngles(0, 0, 0);
        }

        EmitterParameters emitterParameters;
        IParticleClass particleClass;



        public override void Update(float dt)
        {
            if (Stopped) return;
            Random r = ParticleEmitter.RandomGenerator;
            if (Intensity <= 0) return;

            int n = Emit(4*dt);
            if (n > 3)
                n = 3;
            while (n > 0)
            {

                MyVector randomVelocity = new MyVector(0, 0, 0);
                randomVelocity.X = 0;// (float)(-0.2 + 0.4 * r.NextDouble()) * emitterParameters.InitialVelocity.Length;
                randomVelocity.Y = 0;// (float)(-0.2 + 0.4 * r.NextDouble()) * emitterParameters.InitialVelocity.Length;
                randomVelocity.Z = 0;// (float)(-0.2 + 0.4 * r.NextDouble()) * emitterParameters.InitialVelocity.Length;
                Particle p = particleClass.MakeParticle();
                p.Position = this.Position;
                p.Velocity=/*this.Velocity +*/ (randomVelocity + this.emitterParameters.InitialVelocity).Rotate(this.Orientation);
                
                this.ParticleSystem.AddParticle(p);
                n--;
            }



        }
    }
}
