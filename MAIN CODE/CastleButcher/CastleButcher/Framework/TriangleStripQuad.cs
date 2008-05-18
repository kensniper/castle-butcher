using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    class TriangleStripQuad
    {
        /// <summary>
        /// Builds an array of PositionOnly vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <returns>An array of PositionOnly vertices</returns>
        public static CustomVertex.PositionOnly[] BuildPositionOnly(PointF bottomLeft, PointF topRight,
            int nX, int nY)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float fx, fy;

            CustomVertex.PositionOnly[] verts = new CustomVertex.PositionOnly[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                    fy = topRight.Y;
                else
                    fy = bottomLeft.Y + (float)y * deltaY;
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                        fx = topRight.X;
                    else
                        fx = bottomLeft.X + (float)x * deltaX;
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                }
            }
            return verts;
        }

        /// <summary>
        /// Builds an array of PositionNormal vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <returns>An array of PositionNormal vertices</returns>
        public static CustomVertex.PositionNormal[] BuildPositionNormal(PointF bottomLeft, PointF topRight,
            int nX, int nY)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float fx, fy;

            CustomVertex.PositionNormal[] verts = new CustomVertex.PositionNormal[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                    fy = topRight.Y;
                else
                    fy = bottomLeft.Y + (float)y * deltaY;
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                        fx = topRight.X;
                    else
                        fx = bottomLeft.X + (float)x * deltaX;
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                    verts[y * nY + x].Normal = new Vector3(0, 0, -1);
                }
            }
            return verts;
        }

        /// <summary>
        /// Builds an array of PositionColored vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// /// <param name="color">The color of the quad</param>
        /// <returns>An array of PositionColored vertices</returns>        
        public static CustomVertex.PositionColored[] BuildPositionSingleColored(PointF bottomLeft, PointF topRight,
            int nX, int nY, Color color)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float fx, fy;
            CustomVertex.PositionColored[] verts = new CustomVertex.PositionColored[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                    fy = topRight.Y;
                else
                    fy = bottomLeft.Y + (float)y * deltaY;
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                        fx = topRight.X;
                    else
                        fx = bottomLeft.X + (float)x * deltaX;
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                    verts[y * nY + x].Color = color.ToArgb();
                }
            }
            return verts;
        }
        /// <summary>
        /// Builds an array of PositionNormalColored vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// /// <param name="color">The color of the quad</param>
        /// <returns>An array of PositionNormalColored vertices</returns>        
        public static CustomVertex.PositionNormalColored[] BuildPositionNormalSingleColored(PointF bottomLeft, PointF topRight,
            int nX, int nY, Color color)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float fx, fy;
            CustomVertex.PositionNormalColored[] verts = new CustomVertex.PositionNormalColored[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                    fy = topRight.Y;
                else
                    fy = bottomLeft.Y + (float)y * deltaY;
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                        fx = topRight.X;
                    else
                        fx = bottomLeft.X + (float)x * deltaX;
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                    verts[y * nY + x].Color = color.ToArgb();
                    verts[y * nY + x].Normal = new Vector3(0, 0, -1f);
                }
            }
            return verts;
        }
        /// <summary>
        /// Builds an array of PositionColored vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <param name="bRight">The color of bottom right corner</param>
        /// <param name="bLeft">The color of bottom left corner</param>
        /// <param name="tRight">The color of top right corner</param>
        /// <param name="tLeft">The color of top left corner</param>
        /// <returns>An array of PositionColored vertices</returns> 
        public static CustomVertex.PositionColored[] BuildPositionSmoothColored(PointF bottomLeft,
            PointF topRight, int nX, int nY, Color bRight, Color bLeft, Color tRight, Color tLeft)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float fx, fy;
            CustomVertex.PositionColored[] verts = new CustomVertex.PositionColored[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                    fy = topRight.Y;
                else
                    fy = bottomLeft.Y + (float)y * deltaY;
                for (int x = 0; x < nX; x++)
                {

                    if (x == nX - 1)
                        fx = topRight.X;
                    else
                        fx = bottomLeft.X + (float)x * deltaX;

                    float tX = (fx - bottomLeft.X) / (topRight.X - bottomLeft.X);
                    float tY = (fy - bottomLeft.Y) / (topRight.Y - bottomLeft.Y);
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;

                    float col1r = (1f - tY) * (float)bLeft.R + tY * (float)tLeft.R;
                    float col2r = (1f - tY) * (float)bRight.R + tY * (float)tRight.R;
                    int col3r = (int)((1f - tX) * col1r + tX * col2r);


                    float col1g = (1f - tY) * (float)bLeft.G + tY * (float)tLeft.G;
                    float col2g = (1f - tY) * (float)bRight.G + tY * (float)tRight.G;
                    int col3g = (int)((1f - tX) * col1g + tX * col2g);

                    float col1b = (1f - tY) * (float)bLeft.B + tY * (float)tLeft.B;
                    float col2b = (1f - tY) * (float)bRight.B + tY * (float)tRight.B;
                    int col3b = (int)((1f - tX) * col1b + tX * col2b);

                    float col1a = (1f - tY) * (float)bLeft.A + tY * (float)tLeft.A;
                    float col2a = (1f - tY) * (float)bRight.A + tY * (float)tRight.A;
                    int col3a = (int)((1f - tX) * col1a + tX * col2a);

                    verts[y * nY + x].Color = (Color.FromArgb(col3a, col3r, col3g, col3b)).ToArgb();
                }
            }
            return verts;
        }

        /// <summary>
        /// Builds an array of PositionNormalColored vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <param name="bRight">The color of bottom right corner</param>
        /// <param name="bLeft">The color of bottom left corner</param>
        /// <param name="tRight">The color of top right corner</param>
        /// <param name="tLeft">The color of top left corner</param>
        /// <returns>An array of PositionNormalColored vertices</returns> 
        public static CustomVertex.PositionNormalColored[] BuildPositionNormalSmoothColored(PointF bottomLeft,
            PointF topRight, int nX, int nY, Color bRight, Color bLeft, Color tRight, Color tLeft)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float fx, fy;
            CustomVertex.PositionNormalColored[] verts = new CustomVertex.PositionNormalColored[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                    fy = topRight.Y;
                else
                    fy = bottomLeft.Y + (float)y * deltaY;
                for (int x = 0; x < nX; x++)
                {

                    if (x == nX - 1)
                        fx = topRight.X;
                    else
                        fx = bottomLeft.X + (float)x * deltaX;

                    float tX = (fx - bottomLeft.X) / (topRight.X - bottomLeft.X);
                    float tY = (fy - bottomLeft.Y) / (topRight.Y - bottomLeft.Y);
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                    float col1r = (1f - tY) * (float)bLeft.R + tY * (float)tLeft.R;
                    float col2r = (1f - tY) * (float)bRight.R + tY * (float)tRight.R;
                    int col3r = (int)((1f - tX) * col1r + tX * col2r);


                    float col1g = (1f - tY) * (float)bLeft.G + tY * (float)tLeft.G;
                    float col2g = (1f - tY) * (float)bRight.G + tY * (float)tRight.G;
                    int col3g = (int)((1f - tX) * col1g + tX * col2g);

                    float col1b = (1f - tY) * (float)bLeft.B + tY * (float)tLeft.B;
                    float col2b = (1f - tY) * (float)bRight.B + tY * (float)tRight.B;
                    int col3b = (int)((1f - tX) * col1b + tX * col2b);

                    float col1a = (1f - tY) * (float)bLeft.A + tY * (float)tLeft.A;
                    float col2a = (1f - tY) * (float)bRight.A + tY * (float)tRight.A;
                    int col3a = (int)((1f - tX) * col1a + tX * col2a);

                    verts[y * nY + x].Color = (Color.FromArgb(col3a, col3r, col3g, col3b)).ToArgb();
                    verts[y * nY + x].Normal = new Vector3(0, 0, -1f);
                }
            }
            return verts;
        }

        /// <returns>An array of PositionNormalTextured vertices</returns>  
        /// <summary>
        /// Builds an array of PositionNormalTextured vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <param name="texture_bLeft">Texture coordinates for bottom left corner</param>
        /// <param name="texture_tRight">Texture coordinates for top right corner</param>
        /// <returns>Array of CustomVertex.PositionNormalTextured vertices</returns>
        public static CustomVertex.PositionNormalTextured[] BuildPositionNormalTextured(PointF bottomLeft,
            PointF topRight, int nX, int nY, PointF texture_bLeft, PointF texture_tRight)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float tdeltaX = (texture_tRight.X - texture_bLeft.X) / (float)(nX - 1);
            float tdeltaY = (texture_tRight.Y - texture_bLeft.Y) / (float)(nY - 1);
            float fx, fy;
            float tx, ty;

            CustomVertex.PositionNormalTextured[] verts = new CustomVertex.PositionNormalTextured[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                {
                    fy = topRight.Y;
                    ty = texture_tRight.Y;
                }
                else
                {
                    fy = bottomLeft.Y + (float)y * deltaY;
                    ty = texture_bLeft.Y + (float)y * tdeltaY;
                }
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                    {
                        fx = topRight.X;
                        tx = texture_tRight.Y;
                    }
                    else
                    {
                        fx = bottomLeft.X + (float)x * deltaX;
                        tx = texture_bLeft.X + (float)x * tdeltaX;
                    }
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                    verts[y * nY + x].Normal = new Vector3(0, 0, -1f);
                    verts[y * nY + x].Tu = tx;
                    verts[y * nY + x].Tv = ty;
                }
            }
            return verts;
        }


        public static CustomVertex.PositionTextured[] BuildPositionTextured(PointF bottomLeft,
            PointF topRight, int nX, int nY, PointF texture_bLeft, PointF texture_tRight)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float tdeltaX = (texture_tRight.X - texture_bLeft.X) / (float)(nX - 1);
            float tdeltaY = (texture_tRight.Y - texture_bLeft.Y) / (float)(nY - 1);
            float fx, fy;
            float tx, ty;

            CustomVertex.PositionTextured[] verts = new CustomVertex.PositionTextured[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                {
                    fy = topRight.Y;
                    ty = texture_tRight.Y;
                }
                else
                {
                    fy = bottomLeft.Y + (float)y * deltaY;
                    ty = texture_bLeft.Y + (float)y * tdeltaY;
                }
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                    {
                        fx = topRight.X;
                        tx = texture_tRight.Y;
                    }
                    else
                    {
                        fx = bottomLeft.X + (float)x * deltaX;
                        tx = texture_bLeft.X + (float)x * tdeltaX;
                    }
                    verts[y * nY + x].X = fx;
                    verts[y * nY + x].Y = fy;
                    verts[y * nY + x].Z = 0;
                    verts[y * nY + x].Tu = tx;
                    verts[y * nY + x].Tv = ty;
                }
            }
            return verts;
        }

        /// <returns>An array of PositionNTBTextured vertices</returns>  
        /// <summary>
        /// Builds an array of PositionNTBTextured vertices that form a 
        /// nX by nY tesselation of a quad given by it's 2 corners
        /// </summary>
        /// <param name="bottomLeft">Bottom left corner of the quad</param>
        /// <param name="topRight">Top right corner of the quad</param>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <param name="texture_bLeft">Texture coordinates for bottom left corner</param>
        /// <param name="texture_tRight">Texture coordinates for top right corner</param>
        /// <returns>Array of Vertex.PositionNTBTextured vertices</returns>
        public static Vertex.PositionNTBTextured[] BuildPositionNTBTextured(PointF bottomLeft,
            PointF topRight, int nX, int nY, PointF texture_bLeft, PointF texture_tRight)
        {
            if (nX < 2 || nY < 2)
            {
                throw new Exception("Cannot tesselate with nX or nY below 2");
            }
            float deltaX = (topRight.X - bottomLeft.X) / (float)(nX - 1);
            float deltaY = (topRight.Y - bottomLeft.Y) / (float)(nY - 1);
            float tdeltaX = (texture_tRight.X - texture_bLeft.X) / (float)(nX - 1);
            float tdeltaY = (texture_tRight.Y - texture_bLeft.Y) / (float)(nY - 1);
            float fx, fy;
            float tx, ty;

            Vertex.PositionNTBTextured[] verts = new Vertex.PositionNTBTextured[nX * nY];

            for (int y = 0; y < nY; y++)
            {
                if (y == nY - 1)
                {
                    fy = topRight.Y;
                    ty = texture_tRight.Y;
                }
                else
                {
                    fy = bottomLeft.Y + (float)y * deltaY;
                    ty = texture_bLeft.Y + (float)y * tdeltaY;
                }
                for (int x = 0; x < nX; x++)
                {
                    if (x == nX - 1)
                    {
                        fx = topRight.X;
                        tx = texture_tRight.Y;
                    }
                    else
                    {
                        fx = bottomLeft.X + (float)x * deltaX;
                        tx = texture_bLeft.X + (float)x * tdeltaX;
                    }
                    verts[y * nY + x].Position = new Vector3(fx, fy, 0);
                    verts[y * nY + x].Normal = new Vector3(0, 0, 1f);
                    verts[y * nY + x].Binormal = new Vector3(0, 1, 0);
                    verts[y * nY + x].Tangent = new Vector3(1, 0, 0);
                    verts[y * nY + x].Coords = new Vector2(tx, ty);
                }
            }
            return verts;
        }
        /// <summary>
        /// Builds indices for nX by nY tesselation
        /// </summary>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <returns>Array of ushort indices</returns>
        public static ushort[] BuildIndices16(int nX, int nY)
        {
            int nIndices = 2 * nX * (nY - 1) + nY - 2;
            ushort[] indices = new ushort[nIndices];
            int index = 0;
            for (int y = 0; y < nY - 1; y++)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < nX; x++)
                    {
                        indices[index++] = (ushort)(x + y * nY);
                        indices[index++] = (ushort)(x + y * nY + nX);
                    }
                    if (y != nY - 2)
                    {
                        //insert degenerate triangle
                        indices[index++] = indices[index - 3];
                    }
                }
                else
                {
                    for (int x = nX - 1; x >= 0; x--)
                    {
                        indices[index++] = (ushort)(x + y * nY);
                        indices[index++] = (ushort)(x + y * nY + nX);
                    }
                    if (y != nY - 2)
                    {
                        //insert degenerate triangle
                        indices[index++] = indices[index - 3];
                    }

                }
            }
            return indices;
        }
        /// <summary>
        /// Builds indices for nX by nY tesselation
        /// </summary>
        /// <param name="nX">amount of X tesselation</param>
        /// <param name="nY">amount of Y tesselation</param>
        /// <returns>Array of ushort indices</returns>
        public static int[] BuildIndices32(int nX, int nY)
        {
            int nIndices = 2 * nX * (nY - 1) + nY - 2;
            int[] indices = new int[nIndices];
            int index = 0;
            for (int y = 0; y < nY - 1; y++)
            {
                if (y % 2 == 0)
                {
                    for (int x = 0; x < nX; x++)
                    {
                        indices[index++] = (x + y * nY);
                        indices[index++] = (x + y * nY + nX);
                    }
                    if (y != nY - 2)
                    {
                        //insert degenerate triangle
                        indices[index++] = indices[index - 3];
                    }
                }
                else
                {
                    for (int x = nX - 1; x >= 0; x--)
                    {
                        indices[index++] = (x + y * nY);
                        indices[index++] = (x + y * nY + nX);
                    }
                    if (y != nY - 2)
                    {
                        //insert degenerate triangle
                        indices[index++] = indices[index - 3];
                    }

                }
            }
            return indices;
        }
    }
}
