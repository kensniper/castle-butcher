using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Framework.GUI;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.UI.HUD
{
    public class SpeedometerControl : GuiControl
    {
        private Quad outlineQuad;
        private AdjustableQuad barQuad;
        private Quad markerQuad;

        private List<Quad> textQuads;

        private float barQuadWidth;
        private float barQuadStart;
        private float barQuadUWidth;

        private float maxSpeed;
        //private float setSpeed;

        

        public SpeedometerControl(float maxSpeed,PointF position)
            : base(new RectangleF(position, new SizeF(0, 0)), "speedometer")
        {
            this.maxSpeed = maxSpeed;
            ProcessStyle(GM.GUIStyleManager.GetStyleByName("PlayerInfo"));
        }

        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            ImageNode outline = style.GetNodeByName("Speedometer").GetNodeByName("Outline");
            ImageNode bar = style.GetNodeByName("Speedometer").GetNodeByName("Bar");
            ImageNode marker = style.GetNodeByName("Speedometer").GetNodeByName("Marker");

            this.m_position.X -= outline.Rectangle.Width / 2;
            this.m_position.Y -= outline.Rectangle.Height / 2;

            this.m_size = outline.Rectangle.Size;

            float tWidth = style.ImageInfo.Width;
            float tHeight = style.ImageInfo.Height;

            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;

            //topLeft=new CustomVertex.TransformedColoredTextured(m_position.X,m_position.Y+normal.Rectangle.Height,0,1,normal.Color.ToArgb(),
            ImageNode i = outline;
            RectangleF rect = outline.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            outlineQuad = new Quad(topleft, topright, bottomleft, bottomright);

            i = bar;
            rect = bar.Rectangle;
            barQuadWidth = rect.Width;

            float deltax = (outline.Rectangle.Width - bar.Rectangle.Width) / 2;
            float deltay = (outline.Rectangle.Height - bar.Rectangle.Height) / 2;
            barQuadStart = m_position.X + deltax;
            barQuadUWidth = (rect.Width) / tWidth;


            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + deltax, m_position.Y + deltay, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width + deltax, m_position.Y + deltay, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + deltax, m_position.Y + rect.Height + deltay, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width + deltax, m_position.Y + rect.Height + deltay, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            barQuad = new AdjustableQuad(topleft, topright, bottomleft, bottomright);

            i = marker;
            rect = marker.Rectangle;

            deltax = (outline.Rectangle.Width - bar.Rectangle.Width) / 2 - marker.Rectangle.Width / 2;
            deltay = -marker.Rectangle.Height;


            topleft = new CustomVertex.TransformedColoredTextured(m_position.X + deltax, m_position.Y + deltay, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width + deltax, m_position.Y + deltay, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X + deltax, m_position.Y + rect.Height + deltay, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width + deltax, m_position.Y + rect.Height + deltay, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            markerQuad = new Quad(topleft, topright, bottomleft, bottomright);
        }

        public override PointF Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                PointF delta = new PointF(value.X - m_position.X-m_size.Width/2, value.Y - m_position.Y);
                //foreach (Quad q in textQuads)
                //{
                //    q.X += delta.X;
                //    q.Y += delta.Y;
                //}
                outlineQuad.X += delta.X;
                outlineQuad.Y += delta.Y;

                barQuad.X += delta.X;
                barQuad.Y += delta.Y;

                markerQuad.X += delta.X;
                markerQuad.Y += delta.Y;

                base.Position = value;

            }
        }

        public override void Render(Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);
            GUIStyle style = GM.GUIStyleManager.GetStyleByName("PlayerInfo");

            List<Quad> quads = new List<Quad>();
            quads.Add(outlineQuad);
            quads.Add(barQuad);
            quads.Add(markerQuad);
            style.Render(device, quads);
        }

        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }

        public float SetSpeed
        {
            get
            {
                return MaxSpeed * (markerQuad.X+markerQuad.Width/2-barQuad.X) / barQuad.FullWidthX;
            }
            set
            {
                markerQuad.X = (value * barQuad.FullWidthX / MaxSpeed) + barQuad.X - markerQuad.Width / 2;
            }
        }

        public float CurrentSpeed
        {
            get
            {
                return MaxSpeed * barQuad.XWidth;
            }
            set
            {
                if (value > MaxSpeed)
                    value = MaxSpeed;

                value = (float)((int)value);
                barQuad.XWidth = value / MaxSpeed;
            }
        }

    }
}
