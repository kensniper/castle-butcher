using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Xml;

namespace CastleButcher.GameEngine.Resources
{

    public enum MaterialType
    {
        Emissive, Diffuse, DiffuseNormal, DiffuseSpecular,
        DiffuseNormalSpecular, EmissiveDiffuse, EmissiveDiffuseNormalSpecular
    };
    public class MaterialData
    {
        protected IMapData emissiveMap;

        public IMapData EmissiveMap
        {
            get { return emissiveMap; }
        }
        protected IMapData diffuseMap;

        public IMapData DiffuseMap
        {
            get { return diffuseMap; }
        }
        protected IMapData normalMap;

        public IMapData NormalMap
        {
            get { return normalMap; }
        }
        protected IMapData specularMap;

        public IMapData SpecularMap
        {
            get { return specularMap; }
        }
        protected Material material;

        public Material DxMaterial
        {
            get { return material; }
        }

        protected MaterialType materialType;

        public MaterialType MaterialType
        {
            get
            {
                if (diffuseMap != null && normalMap != null && specularMap != null && emissiveMap != null)
                    materialType = MaterialType.EmissiveDiffuseNormalSpecular;
                else if (diffuseMap != null && normalMap != null && specularMap != null && emissiveMap == null)
                    materialType = MaterialType.DiffuseNormalSpecular;
                else if (diffuseMap != null && normalMap == null && specularMap == null && emissiveMap == null)
                    materialType = MaterialType.Diffuse;
                else if (diffuseMap != null && normalMap != null && specularMap == null && emissiveMap == null)
                    materialType = MaterialType.DiffuseNormal;
                else if (diffuseMap == null && normalMap == null && specularMap == null && emissiveMap != null)
                    materialType = MaterialType.Emissive;
                else if (diffuseMap != null)
                    materialType = MaterialType.Diffuse;
                else
                    materialType = MaterialType.Emissive;

                return materialType;
            }
        }

        protected MaterialData()
        {
        }

        public MaterialData(IMapData diffuse, IMapData normal, IMapData specular, Material mat)
        {
            diffuseMap = diffuse;
            normalMap = normal;
            specularMap = specular;

            material = mat;


        }


        public static MaterialData FromFile(string fileName)
        {
            //MaterialData data = new MaterialData();
            XmlTextReader reader = new XmlTextReader(fileName);
            string result;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "MaterialData")
                {

                    result = reader.GetAttribute("type");
                    if (result == "Animated")
                    {
                        MaterialData data = new MaterialData();
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if (reader.Name == "DiffuseMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu DiffuseMap w " + fileName);
                                data.diffuseMap = ResourceCache.Instance.GetTexture(result);

                            }
                            else if (reader.Name == "NormalMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu NormalMap w " + fileName);
                                data.normalMap = ResourceCache.Instance.GetTexture(result);

                            }
                            else if (reader.Name == "SpecularMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu SpeculareMap w " + fileName);
                                data.specularMap = ResourceCache.Instance.GetTexture(result);

                            }
                            else if (reader.Name == "EmissiveMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu EmissiveMap w " + fileName);
                                data.emissiveMap = ResourceCache.Instance.GetTexture(result);

                            }
                        }
                        return new AnimatedMaterialData(data.diffuseMap, data.NormalMap, data.SpecularMap, data.EmissiveMap,new Material());
                    }
                    else
                    {
                        MaterialData data = new MaterialData();
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if (reader.Name == "DiffuseMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu DiffuseMap w " + fileName);
                                data.diffuseMap = ResourceCache.Instance.GetTexture(result);

                            }
                            else if (reader.Name == "NormalMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu NormalMap w " + fileName);
                                data.normalMap = ResourceCache.Instance.GetTexture(result);

                            }
                            else if (reader.Name == "SpecularMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu SpeculareMap w " + fileName);
                                data.specularMap = ResourceCache.Instance.GetTexture(result);

                            }
                            else if (reader.Name == "EmissiveMap")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu EmissiveMap w " + fileName);
                                data.emissiveMap = ResourceCache.Instance.GetTexture(result);

                            }
                        }
                        return data;



                    }

                }

            }
            return null;
        }

        public void Reload()
        {

        }
    }

    public class AnimatedMaterialData : MaterialData
    {


        protected AnimatedMaterialData()
        {
        }

        public AnimatedMaterialData(IMapData diffuse, IMapData normal, IMapData specular, IMapData emissive, Material mat)
        {
            diffuseMap = diffuse;
            normalMap = normal;
            specularMap = specular;
            emissiveMap = emissive;

            material = mat;
        }
    }
}
