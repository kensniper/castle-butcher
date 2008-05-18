using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;

namespace CastleButcher.GameEngine.Resources
{
    public class EffectData
    {
        private EffectData()
        {
        }
        string fileName;

        public string FileName
        {
            get { return fileName; }
            //set { fileName = value; }
        }
        Effect dxEffect;

        public Effect DxEffect
        {
            get { return dxEffect; }
            //set { dxEffect = value; }
        }


        EffectHandle[] paramHandles;

        public EffectHandle[] ParamHandles
        {
            get { return paramHandles; }
            set { paramHandles = value; }
        }
        public void Reload()
        {
            if (fileName != null)
            {
                this.dxEffect = ResourceCache.Instance.GetDxEffect(fileName);
            }
        }

        public static EffectData FromFile(string fileName)
        {
            EffectData data = new EffectData();
            data.fileName = fileName;
            data.dxEffect = ResourceCache.Instance.GetDxEffect(fileName);
            return data;
        }
    }
}
