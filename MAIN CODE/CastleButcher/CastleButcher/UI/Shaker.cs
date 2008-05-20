using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.MyMath;
using System.Drawing;

namespace CastleButcher.UI
{
    class Shaker
    {
        private class Lissajous
        {
            float A, B;
            float a, b;
            public Lissajous(float A_new,float B_new,float a_new,float b_new)
            {
                A=A_new;
                B=B_new;
                a = a_new;
                b = b_new;
            }
            public float ComputeX(float t)
            {
                return  A*(float)Math.Sin(a * t);
                
            }
            public float ComputeY(float t)
            {
                return B * (float)Math.Sin(b * t);
            }
        }
        const float Max_X=2.5f;
        const float Max_Y=2.5f;
        const float Max_Z=2.5f;
        float t;
        Lissajous CrossBowMove=new Lissajous(0.15f,0.05f,3,4);
        int direction;
        float angle;
        MyVector position;
        const float stop_speed=0.00035f;
        const float run_speed = 0.0035f;
        const float acc_mullti = 0.00001f;
        float speed;
        float acceleration;
        bool walking;
        bool destination_chaged;

        bool enabled;

        public void Power()
        {
            if(enabled) enabled=false;
            else enabled=true;
        }

        public Shaker()
        {
            direction = 1;
            speed = 0;
            acceleration=0;
            t = 0;
        }
        public float Angle
        {
            get { return angle; }
        }
        

        public MyVector Position
        {
            get { return position; }
        }

        private void Reset()
        {
            
            direction = -direction;
           
            
        }



        

        private void NextPosition(float dt) //znajduje kolejną pozycję w kierunku destination
        {

            t+= 0.00035f*direction;

            position.X =0.15f*(float)Math.Sin(3*t);// p*t*t+h;
            position.Y =0.05f*(float)Math.Sin(4 * t);// 2 * p * t + k;
            
        }

        public void Update(float dt)
        {
            
            if (position.X >= Max_X || position.Y >= Max_Y || position.Z >= Max_Z) Reset();
            NextPosition(dt);
            if(!enabled) position=position.Subtract(position);
        }
        public void StartWalking() { walking = true; acceleration = acc_mullti*10; }
        public void StopWalking() { walking = false; acceleration = acc_mullti * (-10); }
        public void Jump()
        {
            speed = 1.4f;
            destination_chaged = true;
        }
        public void FireCrossbow()
        {
            speed = 2;
            destination_chaged = true;
        }
    }
}
