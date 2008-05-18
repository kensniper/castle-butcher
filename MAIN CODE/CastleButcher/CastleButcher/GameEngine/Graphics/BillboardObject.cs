using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Framework.MyMath;
using System.Drawing;

namespace CastleButcher.GameEngine.Graphics
{
    public class BillboardObject
    {
        CustomVertex.PositionColoredTextured[] verts=new CustomVertex.PositionColoredTextured[4];

        Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        float scale = 1;

        public float Scale
        {
            get { return scale; }
            set 
            {
                float s = value / scale ;
                scale = value;

                Vector3 pos = (verts[0].Position + verts[1].Position + verts[2].Position + verts[3].Position)*0.25f;

                Vector3 right = (verts[2].Position + verts[3].Position) * 0.5f - pos;
                Vector3 up = (verts[0].Position + verts[2].Position) * 0.5f - pos;

                right *= s;
                up *= s;

                verts[0].Position = (Vector3)(-right + up);

                verts[1].Position = (Vector3)(-right - up);

                verts[2].Position = (Vector3)(right + up);

                verts[3].Position = (Vector3)(right - up);         


            }
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb(verts[0].Color);
            }
            set
            {
                verts[0].Color = verts[1].Color = verts[2].Color = verts[3].Color = value.ToArgb();
            }
        }
        //float rotation = 0;

        private BillboardObject()
        {
        }

        public BillboardObject(MyVector position, MyVector up, MyVector right)
        {
            this.position = (Vector3)position;
            verts[0].Position = (Vector3)(- right + up);
            verts[0].Tu = 0; verts[0].Tv = 1;

            verts[1].Position = (Vector3)(- right - up);
            verts[1].Tu = 0; verts[1].Tv = 0;

            verts[2].Position = (Vector3)(  right + up);
            verts[2].Tu = 1; verts[2].Tv = 1;

            verts[3].Position = (Vector3)(  right - up);
            verts[3].Tu = 1; verts[3].Tv = 0;

            verts[0].Color = verts[1].Color = verts[2].Color = verts[3].Color = Color.White.ToArgb();
        }

        public BillboardObject(MyVector position, MyVector up, MyVector right, Color color)
        {
            this.position = (Vector3)position;
            verts[0].Position = (Vector3)(-right + up);
            verts[0].Tu = 0; verts[0].Tv = 1;

            verts[1].Position = (Vector3)(-right - up);
            verts[1].Tu = 0; verts[1].Tv = 0;

            verts[2].Position = (Vector3)(right + up);
            verts[2].Tu = 1; verts[2].Tv = 1;

            verts[3].Position = (Vector3)(right - up);
            verts[3].Tu = 1; verts[3].Tv = 0;

            verts[0].Color = verts[1].Color = verts[2].Color = verts[3].Color = color.ToArgb();            
        }

        public void Render(Device dev)
        {
            dev.VertexFormat = CustomVertex.PositionColoredTextured.Format;
            //dev.Transform.World = Matrix.Translation(Position);


            dev.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, verts);
        }


        public BillboardObject Clone()
        {
            BillboardObject obj = new BillboardObject();

            this.verts.CopyTo(obj.verts, 0);
            obj.position = this.position;
            obj.scale = this.scale;
            return obj;
        }
    }
}
