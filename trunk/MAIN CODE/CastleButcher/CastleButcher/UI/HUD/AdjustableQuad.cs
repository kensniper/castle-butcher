using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Microsoft.DirectX.Direct3D;
//using 

namespace CastleButcher.UI.HUD
{
    class AdjustableQuad:Quad
    {

        float fullwidthV;
        float fullwidthX;
        
        public AdjustableQuad(CustomVertex.TransformedColoredTextured topLeft, CustomVertex.TransformedColoredTextured topRight,
        CustomVertex.TransformedColoredTextured bottomLeft, CustomVertex.TransformedColoredTextured bottomRight)
            : base(topLeft, topRight, bottomLeft, bottomRight)
        {
            fullwidthV = this.TWidth;
            fullwidthX = this.Width;
        }

        public float XWidth
        {
            get
            {
                return this.TWidth / fullwidthX;
            }
            set
            {
                if (value > 1)
                    value = 1;

                this.Width = (int)(fullwidthX * value);
                this.TRight = TLeft + value * fullwidthV;
            }

        }

        public float FullWidthX
        {
            get
            {
                return fullwidthX;
            }
        }
        public float FullWidthV
        {
            get
            {
                return fullwidthV;
            }
        }
    }
}
