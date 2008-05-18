using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CastleButcher.GameEngine
{
    public class ProgressReporter
    {
        bool complete=false;
        object privateLock = new object();

        public bool Complete
        {
            get 
            {
                return complete; 
            }
            set
            {
                complete = value;
            }
        }

        string info;
        public string Info
        {
            get
            {
                if (complete)
                    return "Gotowe";
                else
                    return info;
            }
            set
            {
                info = value;
            }
        }

        object data;
        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }

        }
    }
}
