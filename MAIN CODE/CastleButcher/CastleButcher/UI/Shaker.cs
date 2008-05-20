using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MyMath;

namespace CastleButcher.UI
{
    class Shaker
    {
        float angle;
        MyVector position;

        public float Angle
        {
            get { return angle; }
        }
        

        public MyVector Position
        {
            get { return position; }
        }
        public void Update(float dt);
        public void StartWalking();
        public void StopWalking();
        public void Jump();
        public void FireCrossbow();
    }
}
