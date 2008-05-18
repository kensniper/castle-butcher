using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.GameEngine.Graphics.Particles
{
    public class ParticleSystem:IUpdateable
    {
        List<Particle> particles = new List<Particle>();
        List<ParticleEmitter> emitters = new List<ParticleEmitter>();
        List<IParticleClass> particleClasses = new List<IParticleClass>();

        int maxParticles = 1000;



        public void AddParticle(Particle particle)
        {
            if (particles.Count > maxParticles)
                particles.RemoveAt(0);
            particles.Add(particle);

            if (!particleClasses.Contains(particle.ParticleClass))
            {
                particleClasses.Add(particle.ParticleClass);
            }
        }

        public void RemoveParticle(Particle particle)
        {
            particles.Remove(particle);
        }

        public void AddEmitter(ParticleEmitter emitter)
        {
            emitters.Add(emitter);
        }

        public void RemoveEmitter(ParticleEmitter emitter)
        {
            emitters.Remove(emitter);
        }


        public void Render(Device device,Renderer renderer)
        {
            for (int i = 0; i < particleClasses.Count; i++)
            {
                int count = particleClasses[i].Render(this.particles, device, renderer);
                if (count == 0)
                {
                    particleClasses.RemoveAt(i);
                    i--;
                }
            }
        }
        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            foreach (ParticleEmitter emi in emitters)
            {
                emi.Update(timeElapsed);
            }


            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(timeElapsed);
                if (particles[i].RemainingTime <= 0)
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }

            return true;
        }

        #endregion
    }
}
