using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Framework.GUI
{
    class GuiNumberBox:GuiEditBox
    {
        public GuiNumberBox(int number, RectangleF rect)
            :base(number.ToString(), rect,rect.Height-4)
        {

        }
        public GuiNumberBox(int number, RectangleF rect,float font)
            : base(number.ToString(), rect, font)
        {

        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                int i;
                if (value!=null && (int.TryParse(value, out i) || value.Length==0))
                    base.Text = value;
                
            }
        }
        public int Number
        {
            get
            {
                int i;
                if (int.TryParse(Text, out i))
                    return i;
                else
                    return 0;
            }
            set
            {
                base.Text = value.ToString();

            }
        }
    }
}
