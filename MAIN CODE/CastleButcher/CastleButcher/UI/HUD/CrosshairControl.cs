using System;
using System.Collections.Generic;
using System.Text;
using Framework.GUI;
using System.Drawing;
using Framework;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.UI.HUD
{
    public class CrosshairControl:GuiControl
    {
        private Quad normalQuad;

        public CrosshairControl(PointF position)
            : base(new RectangleF(position, new SizeF(0, 0)), "crosshair")
        {
            ProcessStyle(GM.GUIStyleManager.GetStyleByName("PlayerInfo"));
        }

        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            ImageNode normal = style.GetNodeByName("Crosshair").GetNodeByName("Normal");

            this.m_position.X -= normal.Rectangle.Width / 2;
            this.m_position.Y -= normal.Rectangle.Height / 2;
            this.m_size = normal.Rectangle.Size;

            float tWidth=style.ImageInfo.Width;
            float tHeight=style.ImageInfo.Height;

            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;

            //topLeft=new CustomVertex.TransformedColoredTextured(m_position.X,m_position.Y+normal.Rectangle.Height,0,1,normal.Color.ToArgb(),
            ImageNode i = normal;
            RectangleF rect = normal.Rectangle;

            topleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y , 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_position.X, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_position.X + rect.Width, m_position.Y + rect.Height, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            normalQuad = new Quad(topleft, topright, bottomleft, bottomright);
        }

        public override PointF Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;

                normalQuad.X = value.X - this.m_size.Width / 2;
                normalQuad.Y = value.Y - this.m_size.Height / 2;

            }
        }

        public override void Render(Device device, float elapsedTime)
        {
            if (this.IsDisabled == false)
            {
                base.Render(device, elapsedTime);
                GUIStyle style = GM.GUIStyleManager.GetStyleByName("PlayerInfo");

                List<Quad> quads = new List<Quad>();
                quads.Add(normalQuad);
                style.Render(device, quads);
            }
        }
    }
}
