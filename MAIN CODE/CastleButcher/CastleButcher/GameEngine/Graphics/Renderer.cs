using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;
using Framework;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using CastleButcher.GameEngine.Resources;
using CastleButcher.GameEngine.Graphics.Particles;

namespace CastleButcher.GameEngine.Graphics
{
    public class Renderer : IUpdateable, IDeviceRelated, IDisposable
    {
        #region Parameters
        MyVector cameraPosition;
        MyVector cameraLookDir;
        MyVector cameraUp;
        MyVector cameraRight;

        MyVector cameraVelocity;
        Device device;

        public Device Device
        {
            get { return device; }
            set { device = value; }
        }

        ConstantsMap constantsMap = new ConstantsMap();

        public EffectData currentEffect = null;

        #endregion
        #region Rendering data
        Environment environment;
        List<IGameObject> asteroids = new List<IGameObject>();
        List<IGameObject> ships = new List<IGameObject>();
        List<IGameObject> powerups = new List<IGameObject>();
        List<IGameObject> otherObjects = new List<IGameObject>();

        List<Explosion> explosions = new List<Explosion>();


        ParticleSystem particleSystem = new ParticleSystem();


        public ParticleSystem ParticleSystem
        {
            get { return particleSystem; }
        }

        //Dictionary<Ship, EngineEmitter> shipEngineEmitters = new Dictionary<Ship, EngineEmitter>();


        #endregion

        public Renderer(Device device)
        {
            this.device = device;
        }

        #region Setting up parameters

        public void LoadData()
        {
            environment = World.Instance.Environment;
            environment.SkyBox.Create(device);

            ShaderConstants.LightDirection = new Vector4(environment.Stars[0].LightDirection.X,
                environment.Stars[0].LightDirection.Y,
                environment.Stars[0].LightDirection.Z,
                0);
            //environment = null;
            //Vector4 v=new Vector4(0, -0.5f, -1, 0);
            //v.Normalize();
            //ShaderConstants.LightDirection = v;
            ShaderConstants.Projection = device.Transform.Projection;

            ShaderConstants.LightColor = new ColorValue(1, 1, 1);


            //foreach (IGameObject obj in World.Instance.GameObjects)
            //{
            //    //if (obj is Ship)
            //    //{
            //    //    EngineEmitter em=new EngineEmitter(particleSystem);
            //    //    this.particleSystem.AddEmitter(em);
            //    //    shipEngineEmitters.Add(obj as Ship, em);
            //    //}
            //}

            World.Instance.OnObjectAdded += new ObjectEventHandler(World_OnObjectAdded);
            World.Instance.OnObjectRemoved += new ObjectEventHandler(World_OnObjectRemoved);

            World.Instance.OnCharacterAdded += new CharacterEventHandler(World_OnShipAdded);
            World.Instance.OnCharacterRemoved += new CharacterEventHandler(World_OnShipRemoved);
        }

        void World_OnShipRemoved(Character ship)
        {
            //EngineEmitter em = shipEngineEmitters[ship];
            //particleSystem.RemoveEmitter(em);
            //shipEngineEmitters.Remove(ship);
        }

        void World_OnShipAdded(Character ship)
        {

        }

        void World_OnObjectRemoved(IGameObject obj)
        {

        }

        void World_OnObjectAdded(IGameObject obj)
        {

        }




        public ConstantsMap ShaderConstants
        {
            get { return constantsMap; }
        }


        public void SetUp(MyVector pos, MyVector look, MyVector up, MyVector vel)
        {

            this.cameraPosition = pos;
            this.cameraLookDir = look;
            this.cameraUp = up;

            this.cameraRight = cameraLookDir.Cross(cameraUp);
            this.cameraVelocity = vel;

            this.ShaderConstants.World = Matrix.Identity;
            ShaderConstants.SetCamera(pos, up, look);
        }
        #endregion

        #region Helper Functions
        public BillboardObject MakeBillboardObject(MyVector position, float size)
        {
            BillboardObject obj = new BillboardObject(position, cameraUp * (size / 2), cameraRight * (size / 2));
            return obj;
        }
        public BillboardObject MakeBillboardObject(MyVector position, float width, float height)
        {
            BillboardObject obj = new BillboardObject(position, cameraUp * (height / 2), cameraRight * (width / 2));
            return obj;
        }

        public Explosion MakeExplosion(MyVector position)
        {
            return MakeExplosion(position, 10f);
        }
        public Explosion MakeExplosion(MyVector position, float size)
        {
            Explosion exp = new Explosion();
            exp.Position = position;
            exp.Size = size;
            exp.ExplosionMap = ResourceCache.Instance.GetAnimatedTexture("explosion1.amd").GetAnimationInstance();
            exp.ExplosionMap.Start();
            exp.ExplosionMap.Loop = false;

            explosions.Add(exp);
            return exp;
        }



        public void SetMaterial(MaterialData material, Material dxMaterial)
        {
            if (material != null)
            {
                SetEffect("lighting.fx");
                string handle = material.MaterialType.ToString();
                currentEffect.DxEffect.Technique = handle;

                if (material.DiffuseMap != null)
                    currentEffect.DxEffect.SetValue("DiffuseTexture", material.DiffuseMap.DxTexture);
                if (material.NormalMap != null)
                    currentEffect.DxEffect.SetValue("NormalTexture", material.NormalMap.DxTexture);
                if (material.SpecularMap != null)
                    currentEffect.DxEffect.SetValue("SpecularTexture", material.SpecularMap.DxTexture);
                if (material.EmissiveMap != null)
                    currentEffect.DxEffect.SetValue("EmissiveTexture", material.EmissiveMap.DxTexture);
                currentEffect.DxEffect.SetValue("SpecularPower", dxMaterial.SpecularSharpness * 10);
            }
            else
            {
                SetEffect("simple.fx");
                currentEffect.DxEffect.Technique = "Simple";
                currentEffect.DxEffect.SetValue("EmissiveColor", dxMaterial.EmissiveColor);
                currentEffect.DxEffect.SetValue("DiffuseColor", dxMaterial.DiffuseColor);
                currentEffect.DxEffect.SetValue("SpecularColor", dxMaterial.SpecularColor);
                currentEffect.DxEffect.SetValue("SpecularPower", dxMaterial.SpecularSharpness * 10);

            }
        }
        #endregion

        #region Rendering
        public void RenderRD(RenderingData rdata, Matrix world)
        {
            ShaderConstants.World = world;
            for (int i = 0; i < rdata.MeshMaterials.Length; i++)
            {

                if (i == 0 || (i > 0 && rdata.MeshMaterials[i - 1] != rdata.MeshMaterials[i]))
                {
                    if (i > 0)
                    {
                        currentEffect.DxEffect.EndPass();
                        currentEffect.DxEffect.End();
                    }
                    SetMaterial(rdata.MeshMaterials[i], rdata.MeshDxMaterials[i]);
                    ShaderConstants.SetEffectParameters(currentEffect.DxEffect, currentEffect.ParamHandles);
                    currentEffect.DxEffect.Begin(FX.None);
                    currentEffect.DxEffect.BeginPass(0);

                }
                rdata.DxMesh.DrawSubset(i);

            }
            currentEffect.DxEffect.EndPass();
            currentEffect.DxEffect.End();
        }


        public void SetEffect(EffectData effect)
        {
            if (currentEffect != effect)
            {
                if (effect.ParamHandles == null)
                {
                    effect.ParamHandles = this.ShaderConstants.GetParameters(effect.DxEffect);
                }
                this.ShaderConstants.SetEffectParameters(effect.DxEffect, effect.ParamHandles);
                currentEffect = effect;
            }
            else
            {
                this.ShaderConstants.SetEffectParameters(effect.DxEffect, effect.ParamHandles);
            }
        }

        public void SetEffect(string effectName)
        {
            if (currentEffect == null || currentEffect.FileName != effectName)
            {
                EffectData effect = ResourceCache.Instance.GetEffect(effectName);
                if (effect.ParamHandles == null)
                {
                    effect.ParamHandles = this.ShaderConstants.GetParameters(effect.DxEffect);
                }
                this.ShaderConstants.SetEffectParameters(effect.DxEffect, effect.ParamHandles);
                currentEffect = effect;
            }
            else
            {
                this.ShaderConstants.SetEffectParameters(currentEffect.DxEffect, currentEffect.ParamHandles);
            }
        }

        public void RenderEnvironment(Environment env)
        {
            device.RenderState.ZBufferEnable = false;
            device.RenderState.ZBufferWriteEnable = false;
            SetEffect("emissive.fx");
            currentEffect.DxEffect.Technique = "emissive_map";
            currentEffect.DxEffect.SetValue("EmissiveTexture", env.SkyBox.SkyTexture);
            //ShaderConstants.World = Matrix.Identity;
            ShaderConstants.View = Matrix.LookAtRH(new Vector3(0, 0, 0), (Vector3)this.cameraLookDir, (Vector3)this.cameraUp);
            ShaderConstants.SetEffectParameters(currentEffect.DxEffect, currentEffect.ParamHandles);
            //skybox
            currentEffect.DxEffect.Begin(FX.None);
            currentEffect.DxEffect.BeginPass(0);
            env.SkyBox.Render(device);
            for (int i = 0; i < env.Stars.Count; i++)
            {
                BillboardObject star_bilb = MakeBillboardObject(-10 * env.Stars[i].LightDirection, 1);

                currentEffect.DxEffect.SetValue("EmissiveTexture", env.Stars[i].StarTexture.DxTexture);
                ShaderConstants.World = Matrix.Translation((Vector3)star_bilb.Position);
                currentEffect.DxEffect.SetValue("WorldViewProjection", ShaderConstants.WorldViewProjection);
                currentEffect.DxEffect.CommitChanges();
                star_bilb.Render(device);
            }
            currentEffect.DxEffect.EndPass();
            currentEffect.DxEffect.End();

        }

        public void RenderExplosions()
        {
            device.RenderState.ZBufferWriteEnable = false;

            device.SetRenderState(RenderStates.SourceBlend, (int)Blend.One);
            device.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);

            SetEffect("emissive.fx");
            currentEffect.DxEffect.Technique = "emissive_map";
            currentEffect.DxEffect.Begin(FX.None);
            currentEffect.DxEffect.BeginPass(0);
            for (int i = 0; i < explosions.Count; i++)
            {
                BillboardObject bilb = this.MakeBillboardObject(explosions[i].Position, explosions[i].Size);


                currentEffect.DxEffect.SetValue("EmissiveTexture", explosions[i].ExplosionMap.CurrentTexture);
                ShaderConstants.World = Matrix.Translation((Vector3)bilb.Position);
                currentEffect.DxEffect.SetValue("WorldViewProjection", ShaderConstants.WorldViewProjection);
                currentEffect.DxEffect.CommitChanges();

                bilb.Render(Device);


                if (explosions[i].ExplosionMap.Running == false)
                {
                    explosions.RemoveAt(i);
                    i--;
                }

            }
            currentEffect.DxEffect.EndPass();

            currentEffect.DxEffect.End();
        }


        public void RenderParticles()
        {
            device.RenderState.ZBufferWriteEnable = false;
            device.SetRenderState(RenderStates.SourceBlend, (int)Blend.One);
            device.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);

            particleSystem.Render(device, this);
        }
        #endregion


        #region IUpdateable Members
        public bool Update(float timeElapsed)
        {

            foreach (Explosion ex in explosions)
            {
                ex.ExplosionMap.Advance(timeElapsed);
            }

            //foreach(KeyValuePair<Ship,EngineEmitter> pair in shipEngineEmitters)
            //{
            //    pair.Value.Orientation = pair.Key.Orientation;
            //    pair.Value.Position = pair.Key.Position;
            //    pair.Value.Velocity = pair.Key.Velocity;
            //    pair.Value.Intensity = pair.Key.SteeringDevice.Velocity / pair.Key.EngineParameters.MaxVelocity;
            //}
            particleSystem.Update(timeElapsed);


            return true;
        }

        #endregion

        #region IDeviceRelated Members

        public void OnCreateDevice(Device device)
        {
            this.Device = device;
            this.environment.SkyBox.Create(device);
        }

        public void OnResetDevice(Device device)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void OnLostDevice(Device device)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void OnDestroyDevice(Device device)
        {
            this.Device = null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.environment = null;
            this.explosions.Clear();
            this.device = null;
            this.currentEffect = null;
            this.asteroids.Clear();
            //this.
        }

        #endregion
    }
}
