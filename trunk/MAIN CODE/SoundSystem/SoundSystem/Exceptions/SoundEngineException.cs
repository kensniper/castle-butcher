using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace SoundSystem.Exceptions
{
    class SoundEngineException : DirectXException
    {
        private String publicMessage;

        public String PublicMessage
        {
            get { return publicMessage; }
            set { publicMessage = value; }
        }

        public SoundEngineException(String msg)
        {
            //mes
        }
	
    }
}
