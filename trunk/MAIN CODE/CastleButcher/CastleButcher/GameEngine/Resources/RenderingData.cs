using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Framework;
using System.Xml;
using Microsoft.DirectX;
namespace CastleButcher.GameEngine.Resources
{
    public enum RenderingDataType { Mesh };

    /// <summary>
    /// Klasa trzymaj¹ca dane do renderowania
    /// </summary>
    public class RenderingData
    {
        Mesh mesh;

        public Mesh DxMesh
        {
            get { return mesh; }
        }
        string meshFile;
        MaterialData[] meshMaterials;

        public MaterialData[] MeshMaterials
        {
            get { return meshMaterials; }
        }

        Material[] meshDxMaterials;

        public Material[] MeshDxMaterials
        {
            get { return meshDxMaterials; }
            set { meshDxMaterials = value; }
        }
        RenderingDataType type;

        float boundingSphereRadius = -1;

        public float BoundingSphereRadius
        {
            get
            {
                if (boundingSphereRadius < 0)
                {
                    Vector3 objectCenter = new Vector3();

                    using (VertexBuffer vb = mesh.VertexBuffer)
                    {
                        GraphicsStream vertexData = vb.Lock(0, 0, LockFlags.None);
                        
                        boundingSphereRadius = Geometry.ComputeBoundingSphere(vertexData,
                                                                      mesh.NumberVertices,
                                                                      mesh.VertexFormat,
                                                                      out objectCenter);

                        boundingSphereRadius += objectCenter.Length();
                        vb.Unlock();
                    }
                }
                return boundingSphereRadius;
            }
        }

        public RenderingDataType RenderingDataType
        {
            get { return type; }
            set { type = value; }
        }

        public static RenderingData FromFile(string fileName)
        {
            if (fileName.EndsWith("rd"))
            {
                XmlTextReader reader = new XmlTextReader(fileName);
                string result;
                RenderingData data;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "RenderingData")
                    {
                        data = new RenderingData();
                        result = reader.GetAttribute("type");
                        if (result == null)
                            throw new FileFormatException("Nie znaleziono typu danych w pliku " + fileName);

                        switch (result)
                        {
                            case "Mesh":
                                data.type = RenderingDataType.Mesh;
                                break;
                        }

                        if (data.type == RenderingDataType.Mesh)
                        {
                            while (reader.NodeType != XmlNodeType.EndElement)
                            {
                                reader.Read();
                                if (reader.Name == "Mesh")
                                {
                                    result = reader.GetAttribute("file");

                                    if (result == null)
                                        throw new FileFormatException("Nie znaleziono opisu broni w pliku " + fileName);
                                    data.meshFile = result;
                                    data.mesh = ResourceCache.Instance.GetMesh(result, out data.meshMaterials, out data.meshDxMaterials);

                                }

                            }
                        }
                        return data;
                    }
                }
                return null;
            }
            else
            {
                RenderingData data = new RenderingData();
                fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                data.mesh = ResourceCache.Instance.GetMesh(fileName, out data.meshMaterials, out data.meshDxMaterials);
                data.meshFile = fileName;
                data.type = RenderingDataType.Mesh;
                return data;

            }
        }


        

        public void Reload()
        {
            this.mesh = ResourceCache.Instance.GetMesh(meshFile, out meshMaterials, out meshDxMaterials);
        }

        
    }
}
