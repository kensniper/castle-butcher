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
        PlayerControlLayer playerInfoLayer;

        RenderingData meshWithWeapon;
        GameController gameController;


        bool normalMapping = false;
        public MainView(ProgressReporter reporter, UIPlayer player, GameController controller)
            : base()
        {
            this.isTransparent = true;
            this.RecievesEmptyMouse = true;
            RecievesEmptyMouse = true;

            this.reporter = reporter;
            this.player = player;
            gameController = controller;

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

            meshWithWeapon = ResourceCache.Instance.GetRenderingData("handWithCrossbow.x");
            //slay = new SteeringLayer(player);

            gameController.Init();
            playerInfoLayer = new PlayerControlLayer(player, gameController);
            playerInfoLayer.RenderCrosshair = false;
            player.PlayerControl = playerInfoLayer;
            GM.AppWindow.PushLayer(playerInfoLayer);
            GM.AppWindow.PushLayer(new BeginGameLayer(player, gameController));

            this.renderer = new Renderer(device);
            renderer.LoadData();



            //ParticleEmitter em = new EngineEmitter(renderer.ParticleSystem);
            //em.Position = new MyVector(0, 0, 100);
            //renderer.ParticleSystem.AddEmitter(em);


            GM.AppWindow.AddKeyLock(KeyMapping.Default.Jump);
            GM.AppWindow.AddKeyLock(KeyMapping.Default.NextWeapon);
            GM.AppWindow.AddKeyLock(KeyMapping.Default.PreviousWeapon);

        }

        private void InitRound()
        {

        }

        public override void OnUpdateFrame(Device device, float elapsedTime)
        {
            base.OnUpdateFrame(device, elapsedTime);


            if (worldLoaded == true)
            {

                gameController.Update(elapsedTime);
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
            pos.X += player.PlayerControl.CameraShaker.Position.X;
            pos.Y += player.PlayerControl.CameraShaker.Position.Y;
            //pos.Y += 9;
            MyVector target = pos + sdev.LookVector;

            MyVector up = sdev.UpVector;
            device.RenderState.CullMode = Cull.Clockwise;


            renderer.SetUp(pos, sdev.LookVector, sdev.UpVector, player.CurrentCharacter.Velocity);
            ////stars
            renderer.RenderEnvironment(World.Instance.Environment);
            device.RenderState.ZBufferEnable = true;
            device.RenderState.ZBufferWriteEnable = true;

            if (player.IsAlive && player.CurrentCharacter.Weapons.CurrentWeapon != null)
            {
                renderer.ShaderConstants.SetCamera(new MyVector(0, 0, 0), new MyVector(0, 1, 0), new MyVector(0, 0, -1));
                renderer.ShaderConstants.SetMatrices(Matrix.Identity, Matrix.Identity,
                device.Transform.Projection);
                renderer.RenderRD(meshWithWeapon, Matrix.Translation(player.PlayerControl.CameraShaker.Position.X,
                                                    player.PlayerControl.CameraShaker.Position.Y,
                                                    player.PlayerControl.CameraShaker.Position.Z));
            }
            renderer.ShaderConstants.SetCamera(pos, sdev.UpVector, sdev.LookVector);




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
                    if (obj == player.CurrentCharacter) continue;
                    if (obj is Character)
                    {
                        renderer.RenderRD(obj.RenderingData, obj.RenderingData.CustomTransform * obj.Transform);
                    }
                    else
                    {
                        renderer.RenderRD(obj.RenderingData, obj.RenderingData.CustomTransform * obj.Transform);
                    }
                }
            }

            ////effects
            //renderer.RenderExplosions();
            //renderer.RenderParticles();



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

            if (renderer != null)
                renderer.OnLostDevice(device);
        }
        public override void OnCreateDevice(Device device)
        {
            base.OnCreateDevice(device);
        }

        public override void OnDestroyDevice(Device device)
        {
            base.OnDestroyDevice(device);
            if (renderer != null)
                renderer.OnDestroyDevice(device);
        }

        public override void OnMouse(System.Drawing.Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);


        }
        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);

        }
    }
}
