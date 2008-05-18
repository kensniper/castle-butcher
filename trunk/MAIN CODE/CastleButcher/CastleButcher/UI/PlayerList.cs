using System;
using System.Collections.Generic;
using System.Text;
using Framework.GUI;
using Framework.Fonts;
using System.Drawing;
using Framework;
using CastleButcher.GameEngine;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace CastleButcher.UI
{
    class PlayerList : GuiControl
    {
        List<Player> players=new List<Player>();
        int nameX, shipX, fragsX, deathsX;
        int headerY, playerY;
        List<Quad> headerQuads=new List<Quad>();
        List<Quad> playerQuads=new List<Quad>();

        RectangleF controlRect;

        public PlayerList(RectangleF rect)
            : base(rect, GM.GetUniqueName())
        {
            ProcessRect(rect);
        }

        public void ProcessRect(RectangleF rect)
        {
            this.controlRect = rect;
            nameX = (int)rect.X;
            headerY = (int)rect.Y;
            playerY = headerY + 30;

            shipX = nameX + 100;
            fragsX = shipX + 100;
            deathsX = fragsX + 80;

            BuildHeaderQuads();
            BuildPlayerQuads();
        }

        private void BuildHeaderQuads()
        {
            headerQuads.Clear();
            BitmapFont font = GM.FontManager.GetFamily("Arial").GetFont(22);
            StringBlock b=new StringBlock("PLAYER",new RectangleF(nameX,headerY,100,22),Align.Left,22,
                ColorValue.FromColor(Color.Yellow),true);
            headerQuads.AddRange(font.GetProcessedQuads(b));

            //b = new StringBlock("SHIP", new RectangleF(shipX, headerY, 100, 22), Align.Left, 22,
            //    ColorValue.FromColor(Color.Yellow), true);
            //headerQuads.AddRange(font.GetProcessedQuads(b));

            b = new StringBlock("FRAGS", new RectangleF(fragsX, headerY, 80, 22), Align.Left, 22,
                ColorValue.FromColor(Color.Yellow), true);
            headerQuads.AddRange(font.GetProcessedQuads(b));

            b = new StringBlock("DEATHS", new RectangleF(deathsX, headerY, 80, 22), Align.Left, 22,
                ColorValue.FromColor(Color.Yellow), true);
            headerQuads.AddRange(font.GetProcessedQuads(b));
        }

        private void BuildPlayerQuads()
        {
            playerQuads.Clear();
            BitmapFont font = GM.FontManager.GetFamily("Arial").GetFont(18);
            StringBlock b;
            int currentY = playerY;
            foreach (Player p in players)
            {
                b = new StringBlock(p.Name, new RectangleF(nameX, currentY, 100, 20), Align.Left, 18,
                ColorValue.FromColor(Color.Yellow), true);
                playerQuads.AddRange(font.GetProcessedQuads(b));

                //b = new StringBlock(p.ShipClass.Name, new RectangleF(shipX, currentY, 100, 20), Align.Left, 18,
                //ColorValue.FromColor(Color.Yellow), true);
                //playerQuads.AddRange(font.GetProcessedQuads(b));

                b = new StringBlock("FRAGS", new RectangleF(fragsX, currentY, 80, 20), Align.Left, 18,
                    ColorValue.FromColor(Color.Yellow), true);
                playerQuads.AddRange(font.GetProcessedQuads(b));

                b = new StringBlock("DEATHS", new RectangleF(deathsX, currentY, 80, 20), Align.Left, 18,
                    ColorValue.FromColor(Color.Yellow), true);
                playerQuads.AddRange(font.GetProcessedQuads(b));

                currentY += 20;
            }
        }

        public override void Render(Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);
            device.RenderState.ZBufferEnable = false;

            //Quad bkg = new Quad(new CustomVertex.TransformedColoredTextured(controlRect.X, controlRect.Y, 0, 1,Color.FromArgb(127,200,200,200).ToArgb(),0,0),
            //    new CustomVertex.TransformedColoredTextured(controlRect.X+controlRect.Width, controlRect.Y, 0, 1,Color.FromArgb(127,200,200,200).ToArgb(),0,0),
            //    new CustomVertex.TransformedColoredTextured(controlRect.X, controlRect.Y + controlRect.Height, 0, 1, Color.FromArgb(127, 200, 200, 200).ToArgb(), 0, 0),
            //    new CustomVertex.TransformedColoredTextured(controlRect.X + controlRect.Width, controlRect.Y + controlRect.Height, 0, 1, Color.FromArgb(127, 200, 200, 200).ToArgb(), 0, 0));

            //device.DrawUserPrimitives(PrimitiveType.TriangleList
            BitmapFont font = GM.FontManager.GetFamily("Arial").GetFont(22);
            font.Render(device, headerQuads);

            font = GM.FontManager.GetFamily("Arial").GetFont(18);
            font.Render(device, playerQuads);
        }
        public void AddPlayer(Player player)
        {
            players.Add(player);
            BuildPlayerQuads();
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
            BuildPlayerQuads();
        }
    }
}
