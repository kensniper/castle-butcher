using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Framework.GUI;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Drawing;
using CastleButcher.GameEngine;
using CastleButcher.GameEngine.AI;

namespace CastleButcher.UI
{
    public class BeginGameLayer:ControlContainer
    {
        GuiButton joinKnights;
        GuiButton joinAssassins;
        GuiButton startGame;
        Player player;
        CustomVertex.TransformedColored[] verts=new CustomVertex.TransformedColored[4];
        GameController controller;
        //device.DrawUserPrimitives(PrimitiveType.TriangleList

        public BeginGameLayer(Player player,GameController controller)
        {
            this.controller = controller;
            this.player = player;
            this.locksMouse = true;
            float x = GM.AppWindow.GraphicsParameters.WindowSize.Width / 2;
            float y = GM.AppWindow.GraphicsParameters.WindowSize.Height - 300;

            GM.GUIStyleManager.SetCurrentStyle("mystyle");
            joinKnights = new GuiButton("Rycerze", new System.Drawing.RectangleF(x - 50, y, 100, 34));
            joinAssassins = new GuiButton("Zabójcy", new System.Drawing.RectangleF(x - 50, y + 40, 100, 34));
            startGame = new GuiButton("Start", new System.Drawing.RectangleF(x - 80, y, 160, 70));
            AddControl(joinKnights);
            AddControl(joinAssassins);

            joinKnights.OnClick += new ButtonEventHandler(joinKnights_OnClick);
            joinAssassins.OnClick += new ButtonEventHandler(joinAssassins_OnClick);
            startGame.OnClick += new ButtonEventHandler(startGame_OnClick);


            verts[2]=new CustomVertex.TransformedColored(0, 0, 0, 1, Color.FromArgb(127, 200, 200, 200).ToArgb());
            verts[0]=new CustomVertex.TransformedColored(GM.AppWindow.GraphicsParameters.WindowSize.Width, 0, 0, 1, Color.FromArgb(127, 200, 200, 200).ToArgb());
            verts[3]=new CustomVertex.TransformedColored(0, GM.AppWindow.GraphicsParameters.WindowSize.Height, 0, 1, Color.FromArgb(127, 200, 200, 200).ToArgb());
            verts[1] = new CustomVertex.TransformedColored(GM.AppWindow.GraphicsParameters.WindowSize.Width, GM.AppWindow.GraphicsParameters.WindowSize.Height, 0, 1, Color.FromArgb(127, 200, 200, 200).ToArgb());
            
            System.Windows.Forms.Cursor.Show();
        }

        void startGame_OnClick()
        {
            RemoveControl(startGame);
            hasFinished = true;
            System.Windows.Forms.Cursor.Hide();
            
            //test
            if (player.CharacterClass.GameTeam == GameTeam.Assassins)
            {
                controller.AddPlayer(new AIPlayer("AIPlayer1", ObjectCache.Instance.GetKnightClass()));
                controller.AddPlayer(new AIPlayer("AIPlayer2", ObjectCache.Instance.GetKnightClass()));
                controller.AddPlayer(new AIPlayer("AIPlayer3", ObjectCache.Instance.GetAssassinClass()));
            }
            else
            {
                controller.AddPlayer(new AIPlayer("AIPlayer1", ObjectCache.Instance.GetKnightClass()));
                controller.AddPlayer(new AIPlayer("AIPlayer2", ObjectCache.Instance.GetAssassinClass()));
                controller.AddPlayer(new AIPlayer("AIPlayer3", ObjectCache.Instance.GetAssassinClass()));
            }
            controller.BeginRound();
        }

        void joinAssassins_OnClick()
        {
            RemoveControl(joinKnights);
            RemoveControl(joinAssassins);
            player.CharacterClass = ObjectCache.Instance.GetAssassinClass();
            controller.AddPlayer(player);
            if (controller.IsLocal == true)
            {
                AddControl(startGame);
            }
            else
            {
                hasFinished = true;
                System.Windows.Forms.Cursor.Hide();
            }
        }

        void joinKnights_OnClick()
        {
            RemoveControl(joinKnights);
            RemoveControl(joinAssassins);
            player.CharacterClass = ObjectCache.Instance.GetKnightClass();
            controller.AddPlayer(player);
            if (controller.IsLocal == true)
            {
                AddControl(startGame);
            }
            else
            {
                hasFinished = true;
                System.Windows.Forms.Cursor.Hide();
            }
        }


        public override void OnRenderFrame(Device device, float elapsedTime)
        {
            base.OnRenderFrame(device, elapsedTime);

            

            // Set render states
            device.SetRenderState(RenderStates.ZBufferWriteEnable, false);
            device.SetRenderState(RenderStates.ZEnable, false);
            device.SetRenderState(RenderStates.Lighting, false);
            device.SetRenderState(RenderStates.AlphaBlendEnable, true);
            
            device.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            device.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, verts);
        }
        
    }
}
