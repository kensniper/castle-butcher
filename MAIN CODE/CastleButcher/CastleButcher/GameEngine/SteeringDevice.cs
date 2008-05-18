using System;
using System.Collections.Generic;
using System.Text;
using Framework.Physics;
using Framework.MyMath;
using Framework;

namespace CastleButcher.GameEngine
{


    class AverageQueueFloat 
    {
        Queue<float> data;


        public AverageQueueFloat(int size)
        {
            data = new Queue<float>(size);
            Size = size;
        }

        public int Size
        {
            get { return data.Count; }
            set
            {
                if (data.Count > value)
                {
                    while (data.Count > value)
                        data.Dequeue();
                }
                else
                {
                    while (data.Count < value)
                        data.Enqueue(this.Value);
                }

            }
        }

        public float Value
        {
            get
            {
                if (data.Count == 0)
                    return 0;
                float avg=0;
                foreach (float f in data)
                {
                    avg += f;
                }
                return avg / data.Count;
            }
        }

        public void Add(float f)
        {
            data.Dequeue();
            data.Enqueue(f);
        }
    }
}
