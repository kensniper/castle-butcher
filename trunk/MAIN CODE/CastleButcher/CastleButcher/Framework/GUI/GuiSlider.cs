using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;

namespace Framework.GUI
{
    public delegate void SliderEventHandler(int value, float sliderPos);
    public class GuiSlider : GuiControl
    {
        #region Fields

        List<Quad> m_normalSliderQuads = new List<Quad>();
        List<Quad> m_overSliderQuads = new List<Quad>();
        List<Quad> m_downSliderQuads = new List<Quad>();
        List<Quad> m_disabledSliderQuads = new List<Quad>();
        List<Quad> m_normalMarkerQuad = new List<Quad>(1);
        List<Quad> m_overMarkerQuad = new List<Quad>(1);
        List<Quad> m_downMarkerQuad = new List<Quad>(1);
        List<Quad> m_disabledMarkerQuad = new List<Quad>(1);

        State m_markerState = State.Normal;
        float m_sliderPosition = 0;         //0-leftmost 1-rightmost
        float m_mouseSliderDelta;
        float m_initialSliderPosition;
        float m_arrowPressTime = 0;

        RectangleF m_sliderRect;
        RectangleF m_sliderMarkerRect;
        RectangleF m_leftRect;
        RectangleF m_rightRect;
        RectangleF m_markerRect;

        int m_numValues = 5;
        int m_value = 0;    //0-leftmost, m_numValues-1 - rightmost

        #endregion

        public GuiSlider(RectangleF rect)
            : base(rect, GM.GetUniqueName())
        {
            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());

        }

        public GuiSlider(string name, RectangleF rect)
            : base(rect, name)
        {
            ProcessStyle(GM.GUIStyleManager.GetCurrentStyle());
        }

        protected override void ProcessStyle(GUIStyle style)
        {
            base.ProcessStyle(style);

            ImageNode left, middle, right, marker;
            left = style.GetNodeByName("Slider").GetNodeByName("NormalLeftCap");
            middle = style.GetNodeByName("Slider").GetNodeByName("NormalMiddle");
            right = style.GetNodeByName("Slider").GetNodeByName("NormalRightCap");
            marker = style.GetNodeByName("Slider").GetNodeByName("NormalMarker");




            float tWidth = (float)style.ImageInfo.Width;
            float tHeight = (float)style.ImageInfo.Height;

            //Positioning Quads
            ImageNode i;
            RectangleF rect;
            CustomVertex.TransformedColoredTextured topleft, topright, bottomleft, bottomright;

            #region Normal
            //left
            i = left;
            rect = i.Rectangle;
            m_leftRect = new RectangleF(m_position.X, m_position.Y + m_size.Height / 2 - rect.Height / 2, rect.Width, rect.Height);

            topleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //right
            i = right;
            rect = i.Rectangle;
            m_rightRect = new RectangleF(m_position.X + m_size.Width - rect.Width, m_position.Y + m_size.Height / 2 - rect.Height / 2, rect.Width, rect.Height);

            topleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Left, m_rightRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Left, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //middle
            i = middle;
            rect = i.Rectangle;
            m_sliderRect = new RectangleF(m_position.X + m_leftRect.Width, m_position.Y + m_size.Height / 2 - rect.Height / 2, m_size.Width - m_leftRect.Width - m_rightRect.Width, rect.Height);

            topleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.Left, m_sliderRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.Left, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //marker
            i = marker;
            rect = i.Rectangle;
            m_markerRect = new RectangleF(m_sliderRect.X + m_sliderPosition * m_sliderRect.Width, m_position.Y + m_size.Height / 2 - rect.Height / 2, rect.Width, rect.Height);
            m_sliderMarkerRect = new RectangleF(m_position.X + m_leftRect.Width, m_position.Y + m_size.Height / 2 - rect.Height / 2,
                m_size.Width - m_leftRect.Width - m_rightRect.Width - m_markerRect.Width, rect.Height);

            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_normalMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));

            #endregion

            left = style.GetNodeByName("Slider").GetNodeByName("OverLeftCap");
            middle = style.GetNodeByName("Slider").GetNodeByName("OverMiddle");
            right = style.GetNodeByName("Slider").GetNodeByName("OverRightCap");
            marker = style.GetNodeByName("Slider").GetNodeByName("OverMarker");

            #region Over
            //left
            i = left;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //right
            i = right;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Left, m_rightRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Left, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //middle
            i = middle;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.Left, m_sliderRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.Left, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //marker
            i = marker;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Left, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_overMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            left = style.GetNodeByName("Slider").GetNodeByName("DownLeftCap");
            middle = style.GetNodeByName("Slider").GetNodeByName("DownMiddle");
            right = style.GetNodeByName("Slider").GetNodeByName("DownRightCap");
            marker = style.GetNodeByName("Slider").GetNodeByName("DownMarker");

            #region Down
            //left
            i = left;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //right
            i = right;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Left, m_rightRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Left, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //middle
            i = middle;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.X, m_sliderRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.Width, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //marker
            i = marker;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.X, m_markerRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Width, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_downMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            left = style.GetNodeByName("Slider").GetNodeByName("DisabledLeftCap");
            middle = style.GetNodeByName("Slider").GetNodeByName("DisabledMiddle");
            right = style.GetNodeByName("Slider").GetNodeByName("DisabledRightCap");
            marker = style.GetNodeByName("Slider").GetNodeByName("DisabledMarker");

            #region Disabled
            //left
            i = left;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Top, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_leftRect.Left, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_leftRect.Right, m_leftRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //right
            i = right;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_rightRect.X, m_rightRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_rightRect.Width, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_rightRect.Right, m_rightRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //middle
            i = middle;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.X, m_sliderRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_sliderRect.Width, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_sliderRect.Right, m_sliderRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledSliderQuads.Add(new Quad(topleft, topright, bottomleft, bottomright));

            //marker
            i = marker;
            rect = i.Rectangle;


            topleft = new CustomVertex.TransformedColoredTextured(m_markerRect.X, m_markerRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Top / tHeight);
            topright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Y, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Top / tHeight);
            bottomleft = new CustomVertex.TransformedColoredTextured(m_markerRect.Width, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Left / tWidth, rect.Bottom / tHeight);
            bottomright = new CustomVertex.TransformedColoredTextured(m_markerRect.Right, m_markerRect.Bottom, 0, 1,
                i.Color.ToArgb(), rect.Right / tWidth, rect.Bottom / tHeight);
            m_disabledMarkerQuad.Add(new Quad(topleft, topright, bottomleft, bottomright));
            #endregion

            foreach (Quad q in m_normalSliderQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_overSliderQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_downSliderQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_disabledSliderQuads)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_normalMarkerQuad)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_overMarkerQuad)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_downMarkerQuad)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }
            foreach (Quad q in m_disabledMarkerQuad)
            {
                q.X += m_positionDelta.X;
                q.Y += m_positionDelta.Y;
            }


        }

        /// <summary>
        /// Gets/sets control's position delta
        /// Position delta is the offset from a coordinate system in which
        /// control was created, to a coordinate system of the screen.
        /// <example>If you have a window, all controls in it are placed 
        /// relative to window's position.</example>
        /// </summary>
        public override PointF PositionDelta
        {
            get
            {
                return base.PositionDelta;
            }
            set
            {
                PointF delta = new PointF(value.X - m_positionDelta.X, value.Y - m_positionDelta.Y);
                foreach (Quad q in m_normalSliderQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_overSliderQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downSliderQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledSliderQuads)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_normalMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_overMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_downMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                foreach (Quad q in m_disabledMarkerQuad)
                {
                    q.X += delta.X;
                    q.Y += delta.Y;
                }
                base.PositionDelta = value;
            }
        }

        /// <summary>
        /// Changes controls PositionDelta
        /// </summary>
        /// <param name="xDelta">x delta</param>
        /// <param name="yDelta">y delta</param>
        public override void ChangePositionDelta(float xDelta, float yDelta)
        {
            base.ChangePositionDelta(xDelta, yDelta);
            foreach (Quad q in m_normalSliderQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_overSliderQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downSliderQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledSliderQuads)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_normalMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_overMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_downMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            foreach (Quad q in m_disabledMarkerQuad)
            {
                q.X += xDelta;
                q.Y += yDelta;
            }
            m_positionDelta.X += xDelta;
            m_positionDelta.Y += yDelta;
        }

        /// <summary>
        /// Fires when slider's value has changed.
        /// </summary>
        public event SliderEventHandler OnChangeValue;

        /// <summary>
        /// Number of values slider can have (from 0).
        /// </summary>
        public int NumValues
        {
            get
            {
                return m_numValues;
            }
            set
            {
                int old = m_numValues;
                m_numValues = value;
                if (m_numValues < 2)
                    m_state = State.Disabled;
                else
                {
                    m_sliderPosition = (float)m_value / (m_numValues - 1);
                    SetMarker();
                }
                if (old != m_numValues && OnChangeValue != null)
                    OnChangeValue(m_value, m_sliderPosition);

            }
        }

        /// <summary>
        /// Slider value
        /// </summary>
        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                int old = m_value;
                m_value = value;
                if (m_value < 0)
                    m_value = 0;
                if (m_value >= m_numValues)
                    m_value = m_numValues - 1;

                m_sliderPosition = (float)m_value / (m_numValues - 1);
                SetMarker();

                if (m_value != old && OnChangeValue != null)
                    OnChangeValue(m_value, m_sliderPosition);
            }
        }

        /// <summary>
        /// Slider posiion
        /// </summary>
        public float SliderPosition
        {
            get
            {
                return m_sliderPosition;
            }
            set
            {
                float old = m_sliderPosition;
                m_sliderPosition = value;
                if (m_sliderPosition < 0)
                    m_sliderPosition = 0;
                else if (m_sliderPosition > 1)
                    m_sliderPosition = 1;
                else
                {
                    float d = 0;
                    int di = 0;
                    //int m_value=0;
                    for (int i = 1; i < m_numValues; i++)
                    {
                        if (m_sliderPosition >= (float)i / (m_numValues - 1))
                        {
                            d = (float)i / (m_numValues - 1);
                            di = i;

                        }
                        else
                        {
                            if ((float)i / (m_numValues - 1) - m_sliderPosition < m_sliderPosition - d)
                            {
                                d = (float)i / (m_numValues - 1);
                                di = i;
                            }
                            break;
                        }
                    }
                    m_sliderPosition = d;
                    m_value = di;

                    SetMarker();

                }
                if (m_sliderPosition != old && OnChangeValue != null)
                    OnChangeValue(m_value, m_sliderPosition);
            }
        }


        private void SetMarker()
        {
            m_markerRect.X = m_sliderMarkerRect.X + m_sliderPosition * m_sliderMarkerRect.Width;
            foreach (Quad q in m_normalMarkerQuad)
            {
                q.X = m_markerRect.X + m_positionDelta.X;
            }
            foreach (Quad q in m_overMarkerQuad)
            {
                q.X = m_markerRect.X + m_positionDelta.X;
            }
            foreach (Quad q in m_downMarkerQuad)
            {
                q.X = m_markerRect.X + m_positionDelta.X;
            }
            foreach (Quad q in m_disabledMarkerQuad)
            {
                q.X = m_markerRect.X + m_positionDelta.X;
            }
        }


        /// <summary>
        /// This method is called when mouse leaves control's space.
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            m_state = State.Normal;
            m_markerState = State.Normal;

        }

        /// <summary>
        /// Processes mouse data.
        /// </summary>
        /// <param name="position">Mouse position</param>
        /// <param name="xDelta">x delta (in pixels)</param>
        /// <param name="yDelta">y delta (in pixels)</param>
        /// <param name="zDelta">Scroll delta (in ??)</param>
        /// <param name="pressedButtons">Pressed buttons (0-left button)</param>
        /// <param name="releasedButtons">Released buttons (0-left button)</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void OnMouse(PointF position, float xDelta, float yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
            base.OnMouse(position, xDelta, yDelta, zDelta, pressedButtons, releasedButtons, elapsedTime);

            if (this.LocksMouse)
            {
                //user is dragging the slider by mouse
                if (releasedButtons[0])
                {
                    this.LocksMouse = false;
                    GM.GeneralLog.Write("Mouse released on slider");
                    //slider was released
                }
                else
                {
                    m_mouseSliderDelta += xDelta;
                    this.SliderPosition = m_initialSliderPosition + m_mouseSliderDelta / m_sliderMarkerRect.Width;
                }
            }
            else
            {
                if (m_leftRect.Contains(position) && pressedButtons[0] && (GM.RunningTime - m_arrowPressTime) > 0.2)
                {
                    this.Value--;
                    m_arrowPressTime = GM.RunningTime;
                }
                else if (m_rightRect.Contains(position) && pressedButtons[0] && (GM.RunningTime - m_arrowPressTime) > 0.2)
                {
                    this.Value++;
                    m_arrowPressTime = GM.RunningTime;
                }
                else if (zDelta != 0)
                {
                    m_markerState = State.Normal;
                    m_state = State.Normal;
                    this.Value += (int)zDelta / 100;
                }
                else if (m_markerRect.Contains(position))
                {
                    m_state = State.Normal;
                    m_markerState = State.Over;
                    if (pressedButtons[0])
                    {
                        m_markerState = State.Down;
                        GM.GeneralLog.Write("Mouse locked on slider");
                        this.LocksMouse = true;
                        m_mouseSliderDelta = 0;
                        m_initialSliderPosition = this.SliderPosition;
                    }

                }
                else if (m_sliderRect.Contains(position))
                {
                    m_markerState = State.Normal;
                    m_state = State.Over;
                    //if(pressedButtons[0])
                    //    this.SliderPosition = (position.X-m_sliderRect.Left) / m_sliderMarkerRect.Width;
                }
                else
                {
                    m_markerState = State.Normal;
                    m_state = State.Normal;
                }
            }
        }

        /// <summary>
        /// Processes keyboard data.
        /// </summary>
        /// <param name="pressedKeys">List of pressed keys</param>
        /// <param name="releasedKeys">List of released keys (since last frame)</param>
        /// <param name="pressedChar">Last pressed key as a character</param>
        /// <param name="pressedKey">Last pressed key as int</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void OnKeyboard(List<System.Windows.Forms.Keys> pressedKeys, List<System.Windows.Forms.Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
            base.OnKeyboard(pressedKeys, releasedKeys, pressedChar, pressedKey, elapsedTime);
            if (pressedKey == (int)Keys.Left)
                this.Value--;
            if (pressedKey == (int)Keys.Right)
                this.Value++;
        }

        /// <summary>
        /// Renders control
        /// </summary>
        /// <param name="device">A Device object</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public override void Render(Microsoft.DirectX.Direct3D.Device device, float elapsedTime)
        {
            base.Render(device, elapsedTime);

            GUIStyle style = GM.GUIStyleManager.GetStyleByName(m_styleName);
            switch (m_state)
            {
                case State.Normal:
                    style.Render(device, m_normalSliderQuads);

                    break;
                case State.Over:
                    style.Render(device, m_overSliderQuads);

                    break;
                case State.Down:
                    style.Render(device, m_downSliderQuads);

                    break;
                case State.Disabled:
                    style.Render(device, m_disabledSliderQuads);
                    style.Render(device, m_disabledMarkerQuad);
                    break;
            }
            if (m_state != State.Disabled)
            {
                switch (m_markerState)
                {
                    case State.Normal:
                        style.Render(device, m_normalMarkerQuad);
                        break;
                    case State.Over:
                        style.Render(device, m_overMarkerQuad);
                        break;
                    case State.Down:
                        style.Render(device, m_downMarkerQuad);
                        break;
                }
            }


        }
    }
}
