using System;
using System.Collections.Generic;
using System.Text;
using CastleButcher.GameEngine;
using CastleButcher.GameEngine.Graphics;
using CastleButcher.GameEngine.Resources;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Framework.MyMath;

namespace CastleButcher.GameEngine.Graphics.Particles
{
    public interface IParticleClass
    {
        EffectData Effect
        { get;}

        Particle MakeParticle();
        int Render(List<Particle> particles, Device dev, Renderer renderer);
    }


    public class SimpleParticleClass:IParticleClass
    {

        public SimpleParticleClass(string mapName)
        {
            particleMap = ResourceCache.Instance.GetTexture(mapName);
        }
        Resources.MapData particleMap;
        #region IParticleClass Members

        public EffectData Effect
        {
            get { return ResourceCache.Instance.GetEffect("emissive.fx"); }
        }

        public int Render(List<Particle> particles,Device device,Renderer renderer)
        {
            int count = 0;
            renderer.ShaderConstants.World = Matrix.Identity;
            EffectData effect = this.Effect;
            
            effect.DxEffect.SetValue("EmissiveTexture", particleMap.DxTexture);

            BillboardObject baseBillboard = renderer.MakeBillboardObject(new MyVector(0, 0, 0), 1);

            effect.DxEffect.Begin(FX.None);
            

            //effect.
            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i].ParticleClass == this)
                {
                    renderer.ShaderConstants.World = Matrix.Translation((Vector3)particles[i].Position);
                    effect.DxEffect.SetValue("WorldViewProjection", renderer.ShaderConstants.WorldViewProjection);
                    baseBillboard.Scale = 100f;
                    effect.DxEffect.BeginPass(0);
                    baseBillboard.Render(device);
                    effect.DxEffect.EndPass();
                    count++;
                }
            }

            
            effect.DxEffect.End();

            return count;
        }

        #endregion

        #region IParticleClass Members


        public Particle MakeParticle()
        {
            return new Particle(this);
        }

        #endregion
    }

    
}
