using System;
using System.Collections.Generic;
using System.Text;
using Framework.MyMath;
using System.Xml;
using CastleButcher.GameEngine.Resources;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.GameEngine.Graphics
{
    public class Environment
    {
        public class Star
        {
            public Star(string mapName, MyVector direction,ColorValue color)
            {
                starTexture = ResourceCache.Instance.GetTexture(mapName);
                lightDirection = direction;
                lightColor = color;
            }
            MyVector lightDirection;

            public MyVector LightDirection
            {
                get { return lightDirection; }
                set { lightDirection = value; }
            }

            Resources.MapData starTexture;

            public Resources.MapData StarTexture
            {
                get { return starTexture; }
                set { starTexture = value; }
            }

            private ColorValue lightColor;

            public ColorValue LightColor
            {
                get { return lightColor; }
                set { lightColor = value; }
            }
        }

        SkyBox skyBox;

        public SkyBox SkyBox
        {
            get { return skyBox; }
        }
        List<Star> stars = new List<Star>();

        public List<Star> Stars
        {
            get { return stars; }
            set { stars = value; }
        }

        public static Environment FromFile(string fileName)
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            //string result;
            Environment data = null;
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Map")
                    {

                        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Map"))
                        {
                            reader.Read();
                            if (reader.Name == "EnvironmentData")
                            {
                                data = new Environment();
                                string result = reader.GetAttribute("background");
                                if (result == null)
                                    throw new Exception("brak t³a w pliku " + fileName);

                                string result2 = reader.GetAttribute("tiling");
                                if (result2 == null)
                                    throw new Exception("brak t³a w pliku " + fileName);

                                data.skyBox = new SkyBox(result, int.Parse(result2));

                                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == "EnvironmentData"))
                                {
                                    reader.Read();
                                    

                                    if (reader.Name == "LightSource")
                                    {

                                        result = reader.GetAttribute("image");
                                        result2 = reader.GetAttribute("direction");
                                        MyVector v;
                                        if (!MyVector.FromString(result2, out v))
                                            throw new Exception("brak t³a w pliku " + fileName);
                                        
                                        result2 = reader.GetAttribute("color");
                                        MyVector color;
                                        if (!MyVector.FromString(result2, out color))
                                            throw new Exception("brak t³a w pliku " + fileName);
                                        Star star = new Star(result, v.Normalize(), new ColorValue(color.X, color.Y, color.Z));
                                        data.Stars.Add(star);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return data;
        }

        
    }
}
