using System;
using System.Collections.Generic;
using System.Text;
using CastleButcher.GameEngine.Resources;
using Framework.MyMath;
using Framework;
namespace CastleButcher.GameEngine.Graphics
{
    public class Explosion:IUpdateable
    {
        MyVector position;

        public MyVector Position
        {
            get { return position; }
            set { position = value; }
        }
        float size=10f;

        public float Size
        {
            get { return size; }
            set { size = value; }
        }
        MapAnimation explosionMap;

        public MapAnimation ExplosionMap
        {
            get { return explosionMap; }
            set { explosionMap = value; }
        }



        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            explosionMap.Advance(timeElapsed);
            return true;
        }

        #endregion
    }


}
