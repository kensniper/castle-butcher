using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Framework;

namespace CastleButcher.GameEngine
{
    /// <summary>
    /// Zawiera konfiguracje gry
    /// </summary>
    static class AppConfig
    {
        static string mapPath;

        public static string MapPath
        {
            get { return mapPath; }
            //set { mapPath = value; }
        }
        static string meshPath;

        public static string MeshPath
        {
            get { return meshPath; }
            //set { meshPath = value; }
        }
        static string texturePath;

        public static string TexturePath
        {
            get { return texturePath; }
            //set { texturePath = value; }
        }
        static string effectPath;

        public static string EffectPath
        {
            get { return effectPath; }
            //set { effectPath = value; }
        }
        static string objectPath;

        public static string ObjectPath
        {
            get { return objectPath; }
            //set { objectPath = value; }
        }

        static int windowWidth;

        public static int WindowWidth
        {
            get { return AppConfig.windowWidth; }
            set { AppConfig.windowWidth = value; }
        }
        static int windowHeight;

        public static int WindowHeight
        {
            get { return AppConfig.windowHeight; }
            set { AppConfig.windowHeight = value; }
        }

        static AppConfig()
        {
            FromFile(DefaultValues.MediaPath + "config.xml");
        }

        private static void FromFile(string fileName)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Paths")
                    {
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Paths"))
                        {
                            reader.Read();
                            if (reader.Name == "MeshPath")
                                meshPath = DefaultValues.MediaPath + reader.ReadString();
                            else if (reader.Name == "TexturePath")
                                texturePath = DefaultValues.MediaPath + reader.ReadString();
                            else if (reader.Name == "EffectPath")
                                effectPath = DefaultValues.MediaPath + reader.ReadString();
                            else if (reader.Name == "MapPath")
                                mapPath = DefaultValues.MediaPath + reader.ReadString();
                            else if (reader.Name == "ObjectPath")
                                objectPath = DefaultValues.MediaPath + reader.ReadString();
                        }
                    }
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Graphics")
                    {
                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Graphics"))
                        {
                            reader.Read();
                            if (reader.Name == "Resolution")
                            {
                                reader.MoveToAttribute("width");
                                WindowWidth = reader.ReadContentAsInt();

                                reader.MoveToAttribute("height");
                                WindowHeight = reader.ReadContentAsInt();
                            }

                        }
                    }
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
