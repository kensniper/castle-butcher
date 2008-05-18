using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Xml;
using System.Drawing;
using Framework.Fonts;

namespace Framework.GUI
{
    public class ControlDefaults
    {
        public float TextSize;
        public Color Color;
    }
    /// <summary>A ControlNode of the XML file</summary>
    public class ControlNode
    {
        public string Name;
        public List<ImageNode> Images = new List<ImageNode>();
        public ControlDefaults Defaults;
        public ImageNode GetNodeByName(string name)
        {
            foreach (ImageNode node in Images)
            {
                if (node.Name == name)
                    return node;
            }
            return null;
        }
    }

    /// <summary>An ImageNode of the XML file</summary>
    public class ImageNode
    {
        public string Name;
        public Color Color;
        public RectangleF Rectangle;
    }

    /// <summary>
    /// A class that represent's a GUI style.
    /// </summary>
    public class GUIStyle : IDisposable, IDeviceRelated
    {
        #region Fileds
        private string m_name;
        private string m_xmlFile;
        private string m_guiTexture;
        private Texture m_texture = null;
        private VertexBuffer m_vb = null;
        private const int MaxVertices = 1024;
        private ImageInformation m_imageInfo;
        private string m_fontName;

        private List<ControlNode> m_controlNodes = new List<ControlNode>();
        private ControlDefaults defaultValues = null;

        public ControlDefaults Defaults
        {
            get { return defaultValues; }
        }
        #endregion

        /// <summary>
        /// Creates GUIStyle object
        /// </summary>
        /// <param name="xmlFile">XML File containing style info.</param>
        public GUIStyle(string xmlFile)
        {
            FromXMLFile(xmlFile);
        }

        /// <summary>
        /// Converts a hex string into a Color
        /// </summary>
        /// <param name="hexString">Hex string of form 0x00000000 or 00000000</param>
        /// <returns>New Color</returns>
        private Color StringToColor(string hexString)
        {
            if (hexString.IndexOf("0x") >= 0)
            {
                hexString = hexString.Remove(0, 2);
            }
            System.Globalization.NumberStyles style =
                System.Globalization.NumberStyles.AllowHexSpecifier;
            int alpha = int.Parse(hexString.Substring(0, 2), style);
            int red = int.Parse(hexString.Substring(2, 2), style);
            int green = int.Parse(hexString.Substring(4, 2), style);
            int blue = int.Parse(hexString.Substring(6, 2), style);
            return Color.FromArgb(alpha, red, green, blue);
        }

        /// <summary>
        /// Reads style data from xml file
        /// </summary>
        /// <param name="xmlFile">XML file</param>
        public void FromXMLFile(string xmlFile)
        {
            m_xmlFile = xmlFile;
            XmlTextReader reader = new XmlTextReader(xmlFile);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "GUI")
                    {
                        // Read in the image file name
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "ImageFile")
                            {
                                m_guiTexture = DefaultValues.MediaPath + reader.Value;
                                m_imageInfo = TextureLoader.ImageInformationFromFile(m_guiTexture);
                            }
                            else if (reader.Name == "StyleName")
                            {
                                m_name = reader.Value;
                            }
                            else if (reader.Name == "FntFile")
                            {
                                char[] separator = new char[] { ';' };
                                string[] tokens = reader.Value.Split(separator);
                                foreach (string token in tokens)
                                {
                                    string fntFile = DefaultValues.FontPath + token;
                                    BitmapFont font = GM.FontManager.GetFont(fntFile);
                                    if (font == null)
                                        GM.GeneralLog.Write("Wczytywanie GUIStyle, nie znaleziono czcionki:" + fntFile);
                                    else
                                        m_fontName = font.Name;
                                }
                                

                            }
                        }
                    }
                    else if (reader.Name == "Control")
                    {
                        ControlNode controlNode = new ControlNode();
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "Name")
                            {
                                controlNode.Name = reader.Value;
                            }
                        }
                        // Read the Image elements of this Control
                        while (reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                            if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Image"))
                            {
                                ImageNode imageNode = new ImageNode();
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);
                                    if (reader.Name == "Name")
                                    {
                                        imageNode.Name = reader.Value;
                                    }
                                    else if (reader.Name == "X")
                                    {
                                        imageNode.Rectangle.X = reader.ReadContentAsFloat();
                                    }
                                    else if (reader.Name == "Y")
                                    {
                                        imageNode.Rectangle.Y = reader.ReadContentAsFloat();
                                    }
                                    else if (reader.Name == "Width")
                                    {
                                        imageNode.Rectangle.Width = reader.ReadContentAsFloat();
                                    }
                                    else if (reader.Name == "Height")
                                    {
                                        imageNode.Rectangle.Height = reader.ReadContentAsFloat();
                                    }
                                    else if (reader.Name == "Color")
                                    {
                                        imageNode.Color = StringToColor(reader.Value);
                                    }
                                }
                                controlNode.Images.Add(imageNode);
                            }
                            else if (reader.Name == "DefaultValues")
                            {
                                controlNode.Defaults= new ControlDefaults();
                                controlNode.Defaults.TextSize = float.Parse(reader.GetAttribute("TextSize"));
                                controlNode.Defaults.Color = this.StringToColor(reader.GetAttribute("Color"));
                            }
                        }
                        m_controlNodes.Add(controlNode);
                    }
                    else if (reader.Name == "DefaultValues")
                    {
                        this.defaultValues = new ControlDefaults();
                        defaultValues.TextSize = float.Parse(reader.GetAttribute("TextSize"));
                        defaultValues.Color = this.StringToColor(reader.GetAttribute("Color"));
                    }
                }
            }
            if (this.Defaults == null)
            {
                this.defaultValues = new ControlDefaults();
                this.defaultValues.TextSize = DefaultValues.TextSize;
                this.defaultValues.Color = Color.Black;
            }
            foreach (ControlNode node in this.m_controlNodes)
            {
                if (node.Defaults == null)
                    node.Defaults = this.defaultValues;
            }
            reader.Close();
            
        }

        #region Properties

        /// <summary>
        /// Style's name
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Style's texture object
        /// </summary>
        public Texture Texture
        {
            get
            {
                return m_texture;
            }
        }
        /// <summary>
        /// Style's xml file name
        /// </summary>
        public string XmlFile
        {
            get
            {
                return m_xmlFile;
            }
        }

        /// <summary>
        /// Style's texture image info.
        /// </summary>
        public ImageInformation ImageInfo
        {
            get
            {
                return m_imageInfo;
            }
        }

        /// <summary>
        /// Style's font name
        /// </summary>
        public string FontName
        {
            get
            {
                return m_fontName;
            }

        }
        #endregion

        /// <summary>
        /// Returns ControlNode object for a specified control name.
        /// </summary>
        /// <param name="name">Control's name</param>
        /// <returns>ControlNode object if successful, null otherwise.</returns>
        public ControlNode GetNodeByName(string name)
        {
            foreach (ControlNode node in m_controlNodes)
            {
                if (node.Name == name)
                    return node;
            }
            return null;
        }

        /// <summary>Renders the quads with this style.</summary>
        /// <param name="device">D3D Device</param>
        /// <param name="quads">Quads to render</param>
        public void Render(Device device, List<Quad> quads)
        {
            if (quads.Count == 0)
                return;
            // Add vertices to the buffer
            GraphicsStream gb =
                m_vb.Lock(0, 6 * quads.Count * CustomVertex.TransformedColoredTextured.StrideSize, LockFlags.Discard);

            Quad q;
            for (int i = 0; i < quads.Count;i++ )
            {
                q = (Quad)quads[i].Clone();
                q.X -= 0.5f;
                q.Y -= 0.5f;
                gb.Write(q.Vertices);
            }

            m_vb.Unlock();

            // Set render states
            device.SetRenderState(RenderStates.Lighting, false);
            device.SetRenderState(RenderStates.ZBufferWriteEnable, false);
            device.SetRenderState(RenderStates.ZEnable, false);
            device.SetRenderState(RenderStates.AlphaBlendEnable, true);
            device.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            device.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            device.SetStreamSource(0, m_vb, 0, CustomVertex.TransformedColoredTextured.StrideSize);
            device.SetTexture(0, m_texture);
            device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2 * quads.Count);
        }

        #region On*Device functions
        /// <summary>Call when the device is created.</summary>
        /// <param name="device">D3D device.</param>
        public void OnCreateDevice(Device device)
        {
            m_texture = TextureLoader.FromFile(device, m_guiTexture, 0, 0, 0, Usage.None, Format.Dxt3, Pool.Managed, Filter.Linear, Filter.Linear, 0);

        }

        /// <summary>Call when the device is destroyed.</summary>
        public void OnDestroyDevice(Device device)
        {
            if (m_vb != null)
            {
                m_vb.Dispose();
                m_vb = null;
            }
            if (m_texture != null)
            {
                m_texture.Dispose();
                m_texture = null;
            }
        }

        /// <summary>Call when the device is reset.</summary>
        /// <param name="device">D3D device.</param>
        public void OnResetDevice(Device device)
        {
            m_vb = new VertexBuffer(device, MaxVertices * CustomVertex.TransformedColoredTextured.StrideSize,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.TransformedColoredTextured.Format, Pool.Default);
        }

        /// <summary>Call when the device is lost.</summary>
        public void OnLostDevice(Device device)
        {
            if (m_vb != null)
            {
                m_vb.Dispose();
                m_vb = null;
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_texture != null)
            {
                m_texture.Dispose();
                m_texture = null;
            }
        }

        #endregion
    }
}
