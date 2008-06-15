using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Xml;
using System.Globalization;
using Framework;

namespace CastleButcher.GameEngine.Resources
{
    
    public class AnimatedMapData
    {
        string fileName;
        Texture[] dxTextures;

        public Texture[] DxTextures
        {
            get { return dxTextures; }
            set { dxTextures = value; }
        }
        float duration;

        public float Duration
        {
            get { return duration; }
        }
        bool loop;

        public bool Loop
        {
            get { return loop; }
        }

        private AnimatedMapData()
        {
        }

        public static AnimatedMapData FromFile(string fileName)
        {
            AnimatedMapData data = new AnimatedMapData();
            data.fileName = fileName;
            XmlTextReader reader = new XmlTextReader(fileName);
            string result;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "AnimatedMapData")
                {
                    List<Texture> textures=new List<Texture>();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        reader.Read();
                        if (reader.Name == "AnimationData")
                        {
                            result = reader.GetAttribute("duration");

                            if (result == null)
                                throw new FileFormatException("Nie znaleziono atrybutu duration w " + fileName);
                            NumberFormatInfo nfi = new NumberFormatInfo();
                            nfi.NumberDecimalSeparator = ".";
                            data.duration = float.Parse(result, nfi);
                            
                            result = reader.GetAttribute("loop");

                            if (result == null)
                                throw new FileFormatException("Nie znaleziono atrybutu loop w " + fileName);
                            if (result=="true")
                            {
                                data.loop = true;
                            }
                            else if (result == "false")
                            {
                                data.loop = false;
                            }
                            else
                            {
                                throw new Exception("Atrybut loop nie ma poprawnego formatu " + fileName);
                            }
                        }
                        else if (reader.Name == "MapFile")
                        {
                            result = reader.GetAttribute("file");

                            if (result == null)
                                throw new FileFormatException("Nie znaleziono atrybutu MapFile w " + fileName);
                            textures.Add(ResourceCache.Instance.GetDxTexture(result));

                        }
                    }
                    data.DxTextures = textures.ToArray();
                    return data;
                }

            }
            return null;
        }

        public MapAnimation GetAnimationInstance()
        {
            MapAnimation anim = new MapAnimation(this,this.Duration,this.Loop);
            return anim;
        }
    }


    public class MapAnimation : AnimationInstance,IMapData,IUpdateable
    {
        AnimatedMapData map;

        int currentTexture;

        public AnimatedMapData AnimatedMap
        {
            get { return map; }
            set { map = value; }
        }
        

        public MapAnimation(AnimatedMapData map,float duration,bool loop)
        {
            this.map = map;
            this.duration = duration;
            this.Loop = loop;

        }

        public override void Reset()
        {
            base.Reset();
            currentTexture = 0;
        }
        public override void Rewind(float position)
        {
            base.Rewind(position);

            float timeStep = duration / map.DxTextures.Length;
            int n =(int)( position / timeStep);
            currentTexture = n;

        }
        public override void Advance(float dt)
        {
            base.Advance(dt);
            if (play)
            {
                float timeStep = duration / map.DxTextures.Length;
                if (Position - currentTexture * timeStep >= timeStep)
                {
                    currentTexture++;
                }
            }
        }
        public Texture CurrentTexture
        {
            get
            {
                if (currentTexture >= map.DxTextures.Length)
                    currentTexture %= map.DxTextures.Length;
                return map.DxTextures[currentTexture];
            }
        }



        #region IMapData Members

        public Texture DxTexture
        {
            get { return CurrentTexture; }
        }

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            Advance(timeElapsed);
            return true;
        }

        #endregion
    }

    
}
