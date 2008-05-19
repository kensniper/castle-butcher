using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using CastleButcher.GameEngine;
using Framework.MyMath;
using Framework.Fonts;
using System.Drawing;
using CastleButcher.GameEngine.Resources;
using CastleButcher.GameEngine.Graphics;
using CastleButcher.GameEngine.Graphics.Particles;
using CastleButcher.GameEngine.AI;
using Framework.Vertex;
using CastleButcher.GameEngine.Weapons;
namespace CastleButcher.UI
{
    class MainView : GameLayer
    {
        //Ship ship;
        UIPlayer player;

        SteeringLayer slay;
        ProgressReporter reporter;
        bool worldLoaded = false;


        Renderer renderer;
        PlayerControl playerInfoLayer;


        bool normalMapping = false;
        public MainView(ProgressReporter reporter, UIPlayer player)
            : base()
        {
            this.isTransparent = true;
            this.RecievesEmptyMouse = true;
            RecievesEmptyMouse = true;

            this.reporter = reporter;
            this.player = player;


            this.player.CurrentCharacter = new SpectatingCharacter(player, null, new MyVector(0, 24, -107), MyQuaternion.FromEulerAngles(0, 0/*(float)Math.PI*/, 0));


        }

        public void Close()
        {
            hasFinished = true;
            //GM.AppWindow.RemoveLayer(playerInfoLayer);
            renderer.Dispose();
        }

        private void InitView(Device device)
        {

            worldLoaded = true;

            slay = new SteeringLayer(player);

            playerInfoLayer = new PlayerControl(player);
            playerInfoLayer.RenderCrosshair = false;
            player.PlayerControl = playerInfoLayer;
            GM.AppWindow.PushLayer(playerInfoLayer);
            GM.AppWindow.PushLayer(new BeginGameLayer(player));

            this.renderer = new Renderer(device);
            renderer.LoadData();

            SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.round1Music);

            //ParticleEmitter em = new EngineEmitter(renderer.ParticleSystem);
            //em.Position = new MyVector(0, 0, 100);
            //renderer.ParticleSystem.AddEmitter(em);


            GM.AppWindow.AddKeyLock(System.Windows.Forms.Keys.Space);

        }

        private void InitRound()
        {

        }

        public override void OnUpdateFrame(Device device, float elapsedTime)
        {
            base.OnUpdateFrame(device, elapsedTime);

            if (player.CurrentCharacter != null)
            {
                SoundSystem.SoundEngine.Update((Vector3)player.CurrentCharacter.Position, (Vector3)player.CurrentCharacter.LookDirection);

            }
            if (worldLoaded == true)
            {

                World.Instance.Update(elapsedTime);
                renderer.Update(elapsedTime);


            }
            else
            {

            }
        }

        private void RenderLoadingInfo(Device device)
        {
            SizeF windowSize = GM.AppWindow.GraphicsParameters.WindowSize;
            StringBlock b = new StringBlock(reporter.Info,
                new System.Drawing.RectangleF(windowSize.Width / 2 - 200, windowSize.Height / 2 - 20, 400, 35),
                Align.Center, 35, ColorValue.FromColor(Color.Blue), true);
            List<Quad> quads = GM.FontManager.GetDefaultFamily().GetFont(35).GetProcessedQuads(b);
            GM.FontManager.GetDefaultFamily().GetFont(35).Render(device, quads);
        }

        private void OnObjectDestroyed(Player obj)
        {
            //renderer.MakeExplosion(obj.CurrentShip.PhysicalData.Position, 4*obj.CurrentShip.PhysicalData.BoundingSphereRadius);
        }
        private void RenderWorld(Device device)
        {
            CharacterController sdev = player.CurrentCharacter.CharacterController;
            MyVector pos = player.CurrentCharacter.Position;
            MyVector target = player.CurrentCharacter.Position + sdev.LookVector;
            MyVector up = sdev.UpVector;
            renderer.SetUp(pos, sdev.LookVector, sdev.UpVector, player.CurrentCharacter.Velocity);

            renderer.ShaderConstants.SetCamera(pos, sdev.UpVector, sdev.LookVector);



            device.RenderState.CullMode = Cull.Clockwise;
            device.RenderState.ZBufferEnable = false;
            device.RenderState.ZBufferWriteEnable = false;



            ////stars
            //renderer.RenderEnvironment(World.Instance.Environment);

            renderer.ShaderConstants.SetMatrices(Matrix.Identity, Matrix.LookAtRH((Vector3)pos, (Vector3)target, (Vector3)up),
                device.Transform.Projection);
            device.RenderState.ZBufferEnable = true;
            device.RenderState.ZBufferWriteEnable = true;








            ////device.RenderState.Lighting = true;
            //device.RenderState.FillMode = FillMode.Point;
            device.RenderState.CullMode = Cull.Clockwise;

            //****************************************************
            //test
            //renderer.ShaderConstants.World = Matrix.Identity;
            //for (int i = 0; i < dxMaterials.Length; i++)
            //{
            //    if (i==0 || (i > 0 && materials[i-1]!=materials[i]))
            //    {
            //        if (i > 0)
            //        {
            //            renderer.currentEffect.DxEffect.EndPass();
            //            renderer.currentEffect.DxEffect.End();
            //        }
            //        renderer.SetMaterial(materials[i], dxMaterials[i]);

            //        renderer.ShaderConstants.SetEffectParameters(renderer.currentEffect.DxEffect, renderer.currentEffect.ParamHandles);
            //        renderer.currentEffect.DxEffect.Begin(FX.None);
            //        renderer.currentEffect.DxEffect.BeginPass(0);

            //    }


            //    mesh.DrawSubset(i);

            //}
            //renderer.currentEffect.DxEffect.EndPass();
            //renderer.currentEffect.DxEffect.End();


            //*******************************************************
            foreach (IGameObject obj in World.Instance.GameObjects)
            {
                if (obj.RenderingData != null /*&& !(obj is Rocket)*/)
                {

                    if (obj != player.CurrentCharacter)
                    {
                        renderer.RenderRD(obj.RenderingData, obj.Transform);
                    }
                    else
                    {
                        renderer.RenderRD(obj.RenderingData, /*Matrix.Translation(0, 0, -10) */ obj.Transform);
                    }
                }
            }

            ////effects
            //renderer.RenderExplosions();
            //renderer.RenderParticles();



            //device.RenderState.AlphaBlendEnable = false;
            //BillboardObject bobj = renderer.MakeBillboardObject(new MyVector(0, 0, 50), 50, 50);
            //bobj.Color = Color.FromArgb(255,255, 255, 255);

            //BillboardObject bobj2 = bobj.Clone();
            //bobj2.Position = new Vector3(0, 2, 5);
            //BillboardObject bobj3 = bobj.Clone();
            //bobj3.Position = new Vector3(0, 4, 5);

            ////bobj2.Color = Color.FromArgb(255, 255, 0, 0);
            ////bobj3.Color = Color.FromArgb(255, 0, 0, 255);
            ////
            //bobj.Render(device);
            //bobj2.Render(device);
            //bobj3.Render(device);
            device.RenderState.CullMode = Cull.CounterClockwise;
            StringBlock b = new StringBlock(player.CurrentCharacter.Position.ToString() +
                "\n GroundContact:" + player.CurrentCharacter.HasGroundContact.ToString(), new RectangleF(10, 80, 300, 150), new RectangleF(10, 50, 300, 150), Align.Left, 18, ColorValue.FromColor(Color.White), true);
            List<Quad> quads = GM.FontManager.GetDefaultFamily().GetFont(DefaultValues.TextSize).GetProcessedQuads(b);
            GM.FontManager.GetDefaultFamily().GetFont(DefaultValues.TextSize).Render(device, quads);
        }

        public override void OnRenderFrame(Device device, float elapsedTime)
        {
            base.OnRenderFrame(device, elapsedTime);
            if (worldLoaded == true)
            {
                RenderWorld(device);
                //if (pilot.CurrentShip != null)
                //{

                //}
            }
            else
            {
                RenderLoadingInfo(device);
                if (reporter == null || reporter.Complete)
                {
                    InitView(device);
                }
            }

        }

        public override void OnResetDevice(Device device)
        {
            base.OnResetDevice(device);

            if (renderer != null)
                renderer.OnResetDevice(device);

            //ntbVertexDecl = new VertexDeclaration(device, PositionNTBTextured.Declarator);


        }

        public override void OnLostDevice(Device device)
        {
            base.OnLostDevice(device);
            //if (ntbVertexDecl != null)
            //{
            //    ntbVertexDecl.Dispose();
            //}

            if (renderer != null)
                renderer.OnLostDevice(device);
        }
        public override void OnCreateDevice(Device device)
        {
            base.OnCreateDevice(device);

            //anim = ResourceCache.Instance.GetAnimatedTexture("explosion1.amd").GetAnimationInstance();

            //PositionNTBTextured[] verts = TriangleStripQuad.BuildPositionNTBTextured(new PointF(20, 20),
            //    new PointF(-20, -20), 2, 2, new PointF(0, 0), new PointF(1, 1));
            //int[] indices = TriangleStripQuad.BuildIndices32(2, 2);


            //vb = new VertexBuffer(device, verts.Length * 56, Usage.WriteOnly,
            //    VertexFormats.Position | VertexFormats.Texture0, Pool.Managed);
            //GraphicsStream gs = vb.Lock(0, 0, LockFlags.None);
            //gs.Write(verts);
            //vb.Unlock();
            //ib = new IndexBuffer(device, sizeof(int) * indices.Length, Usage.WriteOnly, Pool.Managed, false);
            //ib.SetData(indices, 0, LockFlags.None);

            //diffTex = ResourceCache.Instance.GetDxTexture("NTB\\argon_diff.dds");
            //bumpTex = ResourceCache.Instance.GetDxTexture("NTB\\argon_bump.dds");
            //specTex = ResourceCache.Instance.GetDxTexture("NTB\\argon_spec.dds");
            //emisTex = ResourceCache.Instance.GetDxTexture("NTB\\argon_light.dds");
        }

        public override void OnDestroyDevice(Device device)
        {
            base.OnDestroyDevice(device);
            if (renderer != null)
                renderer.OnDestroyDevice(device);
            //if (vb != null)
            //{
            //    vb.Dispose();
            //}
            //if (ib != null)
            //{
            //    ib.Dispose();
            //}
        }

        public override void OnMouse(System.Drawing.Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
            if (worldLoaded)
            {

                if (true || player.IsAlive)
                {
                    slay.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
                    //sdev.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);
                }
                else
                {

                }
                GM.AppWindow.CenterCursor();
            }


        }
        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            if (worldLoaded)
            {
                slay.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
                //sdev.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            }

        }
    }
}
