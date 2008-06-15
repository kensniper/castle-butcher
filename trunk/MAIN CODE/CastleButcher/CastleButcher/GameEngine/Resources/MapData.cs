using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Xml;

namespace CastleButcher.GameEngine.Resources
{
    public interface IMapData
    {
        Texture DxTexture
        {
            get;
        }
    }
    public class MapData:IMapData
    {
        string fileName;

        public string FileName
        {
            get { return fileName; }
        }

        Texture dxTexture;

        public Texture DxTexture
        {
            get { return dxTexture; }
        }

        private MapData()
        {
        }
        public MapData(Texture dxTexture,string fileName)
        {
            this.dxTexture = dxTexture;
            this.fileName = fileName;
        }

        public static MapData FromFile(string fileName)
        {
            MapData data = new MapData();
            data.fileName = fileName;
            XmlTextReader reader = new XmlTextReader(fileName);
            string result;
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "MapData")
                    {
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if (reader.Name == "MapFile")
                            {
                                result = reader.GetAttribute("file");

                                if (result == null)
                                    throw new FileFormatException("Nie znaleziono atrybutu MapFile w " + fileName);
                                data.dxTexture = ResourceCache.Instance.GetDxTexture(result);

                            }

                        }
                        return data;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
    }


}
