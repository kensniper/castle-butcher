using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CastleButcher.GameEngine;
using Framework.GUI;
using Framework;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.UI.HUD
{
    class CastleMap : GuiControl
    {
        GameTeam playerTeam;


        Quad mapQuad;
        Quad assassinQuad;
        Quad knightQuad;

        List<Quad> mapQuads;

        float mapWidth = 700;
        float mapHeight = 700;

        public CastleMap(PointF position)
            : base(new RectangleF(position, new SizeF(0, 0)), "CastleMap")
        {
            mapQuads = new List<Quad>();
            ProcessStyle(GM.GUIStyleManager.GetStyleByName("PlayerInfo"));
        }


        public void Update()
        {
            mapQuads.Clear();
            mapQuads.Add(mapQuad);
            for (int i = 0; i < World.Instance.Players.Count; i++)
            {
                Player p = World.Instance.Players[i];
                Quad q;
                if (p.CharacterClass.GameTeam == GameTeam.Assassins)
                {
                    q = (Quad)assassinQuad.Clone();
                    q.X += (-p.CurrentCharacter.Position.X + mapWidth / 2) * (this.m_size.Width / mapWidth);
                    q.Y += (-p.CurrentCharacter.Position.Z + mapHeight / 2) * (this.m_size.Height / mapWidth);
                }
                else
                {
                    q = (Quad)knightQuad.Clone();
                    q.X += (-p.CurrentCharacter.Position.X + mapWidth / 2) * (this.m_size.Width / mapWidth);
                    q.Y += (-p.CurrentCharacter.Position.Z + mapHeight / 2) * (this.m_size.Height / mapHeight);
                }
                mapQuads.Add(q);
            }
        }
        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);
            ImageNode map = style.GetNodeByName("Map").GetNodeByName("Map");
            ImageNode ass = style.GetNodeByName("Map").GetNodeByName("Assassin");
            ImageNode kni = style.GetNodeByName("Map").GetNodeByName("Knight");

            this.m_size = new SizeF(map.Rectangle.Size.Width * 2, map.Rectangle.Size.Height * 2);
            this.m_position.X -= m_size.Width / 2;
            this.m_position.Y -= m_size.Height / 2;



            float tWidth = style.ImageInfo.Width;
            float tHeight = style.ImageInfo.Height;

            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;

            //topLeft=new CustomVertex.TransformedColoredTextured(m_position.X,m_position.Y+normal.Rectangle.Height,0,1,normal.Color.ToArgb(),
            ImageNode i = map;
            RectangleF rect = map.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + m_size.Width, m_position.Y + m_size.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            mapQuad = new Quad(topleft, topright, bottomleft, bottomright);

            i = ass;
            rect = ass.Rectangle;
            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            assassinQuad = new Quad(topleft, topright, bottomleft, bottomright);

            i = kni;
            rect = kni.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            knightQuad = new Quad(topleft, topright, bottomleft, bottomright);
        }

        public override PointF Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                PointF delta = new PointF(value.X - m_position.X - m_size.Width / 2, value.Y - m_position.Y - m_size.Height / 2);
                //foreach (Quad q in textQuads)
                //{
                //    q.X += delta.X;
                //    q.Y += delta.Y;
                //}
                mapQuad.X += delta.X;
                mapQuad.Y += delta.Y;

                assassinQuad.X += delta.X;
                assassinQuad.Y += delta.Y;

                knightQuad.X += delta.X;
                knightQuad.Y += delta.Y;

                base.Position = value;

            }
        }

        public override void Render(Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);
            GUIStyle style = GM.GUIStyleManager.GetStyleByName("PlayerInfo");


            style.Render(device, mapQuads);
        }
    }
}
