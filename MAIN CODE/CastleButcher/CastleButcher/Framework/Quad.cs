using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace Framework
{
    public class Quad : ICloneable
    {
        protected CustomVertex.TransformedColoredTextured[] m_vertices;

        public Quad()
        {
            // Empty
        }


        /// <summary>Creates a new Quad</summary>
        /// <param name="topLeft">Top left vertex.</param>
        /// <param name="topRight">Top right vertex.</param>
        /// <param name="bottomLeft">Bottom left vertex.</param>
        /// <param name="bottomRight">Bottom right vertex.</param>
        public Quad(CustomVertex.TransformedColoredTextured topLeft, CustomVertex.TransformedColoredTextured topRight,
        CustomVertex.TransformedColoredTextured bottomLeft, CustomVertex.TransformedColoredTextured bottomRight)
        {
            m_vertices = new CustomVertex.TransformedColoredTextured[6];
            m_vertices[0] = topLeft;
            m_vertices[1] = bottomRight;
            m_vertices[2] = bottomLeft;
            m_vertices[3] = topLeft;
            m_vertices[4] = topRight;
            m_vertices[5] = bottomRight;
        }
        


        /// <summary>Gets and sets the vertices.</summary>
        public CustomVertex.TransformedColoredTextured[] Vertices
        {
            get { return m_vertices; }
            set { value.CopyTo(m_vertices, 0); }
        }


        /// <summary>Gets the top left vertex.</summary>
        public CustomVertex.TransformedColoredTextured TopLeft
        {
            get { return m_vertices[0]; }
        }


        /// <summary>Gets the top right vertex.</summary>
        public CustomVertex.TransformedColoredTextured TopRight
        {
            get { return m_vertices[4]; }
        }


        /// <summary>Gets the bottom left vertex.</summary>
        public CustomVertex.TransformedColoredTextured BottomLeft
        {
            get { return m_vertices[2]; }
        }


        /// <summary>Gets the bottom right vertex.</summary>
        public CustomVertex.TransformedColoredTextured BottomRight
        {
            get { return m_vertices[5]; }
        }


        /// <summary>Gets and sets the X coordinate. Changing it moves the whole quad.</summary>
        public float X
        {
            get { return m_vertices[0].X; }
            set
            {
                float width = Width;
                m_vertices[0].X = value;
                m_vertices[1].X = value + width;
                m_vertices[2].X = value;
                m_vertices[3].X = value;
                m_vertices[4].X = value + width;
                m_vertices[5].X = value + width;
            }
        }


        /// <summary>Gets and sets the Y coordinate. Changing it moves the whole quad.</summary>
        public float Y
        {
            get { return m_vertices[0].Y; }
            set
            {
                float height = Height;
                m_vertices[0].Y = value;
                m_vertices[1].Y = value + height;
                m_vertices[2].Y = value + height;
                m_vertices[3].Y = value;
                m_vertices[4].Y = value;
                m_vertices[5].Y = value + height;
            }
        }


        /// <summary>Gets and sets the width.</summary>
        public float Width
        {
            get { return m_vertices[4].X - m_vertices[0].X; }
            set
            {
                m_vertices[1].X = m_vertices[0].X + value;
                m_vertices[4].X = m_vertices[0].X + value;
                m_vertices[5].X = m_vertices[0].X + value;
            }
        }


        /// <summary>Gets and sets the height.</summary>
        public float Height
        {
            get { return m_vertices[2].Y - m_vertices[0].Y; }
            set
            {
                m_vertices[1].Y = m_vertices[0].Y + value;
                m_vertices[2].Y = m_vertices[0].Y + value;
                m_vertices[5].Y = m_vertices[0].Y + value;
            }
        }

        /// <summary>Gets the texture width.</summary>
        public float TWidth
        {
            get { return m_vertices[4].Tu - m_vertices[0].Tu; }
        }


        /// <summary>Gets the texture height.</summary>
        public float THeight
        {
            get { return m_vertices[2].Tv - m_vertices[0].Tv; }
        }


        /// <summary>Gets the X coordinate of the right.</summary>
        public float Right
        {
            get { return X + Width; }
        }
        /// <summary>Gets the X coordinate of the left.</summary>
        public float Left
        {
            get { return X; }
        }
        /// <summary>Gets the Y coordinate of the top.</summary>
        public float Top
        {
            get { return Y; }
        }


        /// <summary>Gets the texture Y coordinate of the bottom.</summary>
        public float Bottom
        {
            get { return Y + Height; }
        }

        /// <summary>Gets the texture X coordinate of the right.</summary>
        public float TRight
        {
            get { return m_vertices[1].Tu; }
            set
            {
                m_vertices[1].Tu = value;
                m_vertices[4].Tu = value;
                m_vertices[5].Tu = value;
            }
        }
        /// <summary>Gets the texture X coordinate of the left.</summary>
        public float TLeft
        {
            get { return m_vertices[0].Tu; }
            set
            {
                m_vertices[0].Tu = value;
                m_vertices[2].Tu = value;
                m_vertices[3].Tu = value;
            }
        }
        /// <summary>Gets the texture Y coordinate of the top.</summary>
        public float TTop
        {
            get { return m_vertices[0].Tv; }
            set
            {
                m_vertices[0].Tv = value;
                m_vertices[3].Tv = value;
                m_vertices[4].Tv = value;
            }
        }


        /// <summary>Gets the texture Y coordinate of the bottom.</summary>
        public float TBottom
        {
            get { return m_vertices[1].Tv; }
            set
            {
                m_vertices[1].Tv = value;
                m_vertices[2].Tv = value;
                m_vertices[5].Tv = value;
            }
        }


        /// <summary>Gets and sets the Quad's color.</summary>
        public int Color
        {
            get { return m_vertices[0].Color; }
            set
            {
                for (int i = 0; i < 6; i++)
                {
                    m_vertices[i].Color = value;
                }
            }
        }


        /// <summary>Writes the Quad to a string</summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            string result = "X = " + X.ToString();
            result += "nY = " + Y.ToString();
            result += "nWidth = " + Width.ToString();
            result += "nHeight = " + Height.ToString();
            return result;
        }


        /// <summary>Clones the Quad.</summary>
        /// <returns>Cloned Quad</returns>
        public object Clone()
        {
            return new Quad(m_vertices[0], m_vertices[4], m_vertices[2], m_vertices[5]);
        }
    }
}
