using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine
{
    public class SkyBox
    {
        string skyTextureName;
        int tiling;

        Texture skyTexture;

        public Texture SkyTexture
        {
            get { return skyTexture; }
            set { skyTexture = value; }
        }

        VertexBuffer vb;
        IndexBuffer ib;

        CustomVertex.PositionTextured[] vertices;
        int[] indices;

        public SkyBox(string skyTexture,int tiling)
        {
            this.skyTextureName = skyTexture;
            this.tiling = tiling;

            Build(10);
        }

        public void Build(float radius)
        {
            vertices = new CustomVertex.PositionTextured[24];
            //
            indices = new int[] { 0,1,2,1,3,2,
            4,5,6,5,7,6,
            8,9,10,9,11,10,
            12,13,14,13,15,14,
            16,17,18,17,19,18,
            20,21,22,21,23,22};

            vertices[0].Position = new Microsoft.DirectX.Vector3(radius, -radius, radius);            
            vertices[1].Position = new Microsoft.DirectX.Vector3(-radius, -radius, radius);            
            vertices[2].Position = new Microsoft.DirectX.Vector3(radius, radius, radius);            
            vertices[3].Position = new Microsoft.DirectX.Vector3(-radius, radius, radius);
            vertices[0].Tu = 0; vertices[0].Tv = tiling;
            vertices[1].Tu = tiling; vertices[1].Tv = tiling;
            vertices[2].Tu = 0; vertices[2].Tv = 0;
            vertices[3].Tu = tiling; vertices[3].Tv = 0;

            vertices[4].Position = new Microsoft.DirectX.Vector3(radius, -radius, -radius);
            vertices[5].Position = new Microsoft.DirectX.Vector3(radius, -radius, radius);
            vertices[6].Position = new Microsoft.DirectX.Vector3(radius, radius, -radius);
            vertices[7].Position = new Microsoft.DirectX.Vector3(radius, radius, radius);
            vertices[4].Tu = 0; vertices[4].Tv = 0;
            vertices[5].Tu = tiling; vertices[5].Tv = 0;
            vertices[6].Tu = 0; vertices[6].Tv = tiling;
            vertices[7].Tu = tiling; vertices[7].Tv = tiling;

            vertices[8].Position = new Microsoft.DirectX.Vector3(-radius, -radius, -radius);
            vertices[9].Position = new Microsoft.DirectX.Vector3(radius, -radius, -radius);
            vertices[10].Position = new Microsoft.DirectX.Vector3(-radius, radius, -radius);
            vertices[11].Position = new Microsoft.DirectX.Vector3(radius, radius, -radius);
            vertices[8].Tu = 0; vertices[8].Tv = 0;
            vertices[9].Tu = tiling; vertices[9].Tv = 0;
            vertices[10].Tu = 0; vertices[10].Tv = tiling;
            vertices[11].Tu = tiling; vertices[11].Tv = tiling;

            vertices[12].Position = new Microsoft.DirectX.Vector3(-radius, -radius, radius);
            vertices[13].Position = new Microsoft.DirectX.Vector3(-radius, -radius, -radius);
            vertices[14].Position = new Microsoft.DirectX.Vector3(-radius, radius, radius);
            vertices[15].Position = new Microsoft.DirectX.Vector3(-radius, radius, -radius);
            vertices[12].Tu = 0; vertices[12].Tv = 0;
            vertices[13].Tu = tiling; vertices[13].Tv = 0;
            vertices[14].Tu = 0; vertices[14].Tv = tiling;
            vertices[15].Tu = tiling; vertices[15].Tv = tiling;



            vertices[16].Position = new Microsoft.DirectX.Vector3(-radius,-radius, radius);
            vertices[17].Position = new Microsoft.DirectX.Vector3(radius, -radius, radius);
            vertices[18].Position = new Microsoft.DirectX.Vector3(-radius, -radius, -radius);
            vertices[19].Position = new Microsoft.DirectX.Vector3(radius, -radius, -radius);
            vertices[16].Tu = 0; vertices[16].Tv = 0;
            vertices[17].Tu = tiling; vertices[17].Tv = 0;
            vertices[18].Tu = 0; vertices[18].Tv = tiling;
            vertices[19].Tu = tiling; vertices[19].Tv = tiling;

            vertices[20].Position = new Microsoft.DirectX.Vector3(-radius, radius, -radius);
            vertices[21].Position = new Microsoft.DirectX.Vector3(radius, radius, -radius);
            vertices[22].Position = new Microsoft.DirectX.Vector3(-radius, radius, radius);
            vertices[23].Position = new Microsoft.DirectX.Vector3(radius, radius, radius);
            vertices[20].Tu = 0; vertices[20].Tv = 0;
            vertices[21].Tu = tiling; vertices[21].Tv = 0;
            vertices[22].Tu = 0; vertices[22].Tv = tiling;
            vertices[23].Tu = tiling; vertices[23].Tv = tiling;

            
        }

        public void Create(Device dev)
        {
            vb = new VertexBuffer(dev, vertices.Length * CustomVertex.PositionTextured.StrideSize, Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Managed);
            GraphicsStream str = vb.Lock(0, 0, LockFlags.None);
            str.Write(vertices);
            vb.Unlock();

            ib = new IndexBuffer(dev, indices.Length * sizeof(int), Usage.WriteOnly, Pool.Managed, false);
            ib.SetData(indices, 0, LockFlags.None);

            //Mesh mesh = new Mesh(vertices.Length * 3, numberVerts, MeshFlags.Managed,
            //         CustomVertex.PositionTextured.Format, device);

            //using (VertexBuffer vb = mesh.VertexBuffer)
            //{
            //    GraphicsStream data = vb.Lock(0, 0, LockFlags.None);

            //    data.Write(new CustomVertex.PositionColored(-1.0f, 1.0f, 1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(-1.0f, -1.0f, 1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(1.0f, 1.0f, 1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(1.0f, -1.0f, 1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(-1.0f, 1.0f, -1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(1.0f, 1.0f, -1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(-1.0f, -1.0f, -1.0f, 0x00ff00ff));
            //    data.Write(new CustomVertex.PositionColored(1.0f, -1.0f, -1.0f, 0x00ff00ff));

            //    vb.Unlock();
            //}

            //using (IndexBuffer ib = mesh.IndexBuffer)
            //{
            //    ib.SetData(indices, 0, LockFlags.None);
            //}


            skyTexture = ResourceCache.Instance.GetDxTexture(skyTextureName);
        }

        public void Render(Device dev)
        {
            dev.SetStreamSource(0, vb, 0,CustomVertex.PositionTextured.StrideSize);
            dev.Indices = ib;
            dev.VertexFormat = CustomVertex.PositionTextured.Format;            
            dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, 12);
        }



    }
}
