using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    
    public class GraphicsParameters
    {
        public bool windowed;
        public RectangleF windowRect;
        public Format adapterFormat;
        public PresentParameters presentParams;
        public CreateFlags createFlags;
        public DeviceType deviceType;
        public Caps deviceCaps;

        private float zNearPlane;

        public float ZNearPlane
        {
            get { return zNearPlane; }
            set { zNearPlane = value; }
        }
        private float zFarPlane;

        public float ZFarPlane
        {
            get { return zFarPlane; }
            set { zFarPlane = value; }
        }

        public GraphicsParameters()
        {
            presentParams = null;
        }

        public GraphicsParameters(bool windowed, RectangleF rect, Format format, PresentParameters pparams, CreateFlags cflags,
            DeviceType dtype, Caps dcaps)
        {
            this.windowed = windowed;
            windowRect = rect;
            adapterFormat = format;
            presentParams = (PresentParameters)pparams.Clone();
            createFlags = cflags;
            deviceType = dtype;
            deviceCaps = dcaps;
            zNearPlane = 0.1f;
            zFarPlane = 1000;
        }
        public GraphicsParameters(bool windowed, RectangleF rect, Format format, PresentParameters pparams, CreateFlags cflags,
            DeviceType dtype, Caps dcaps,float zNear,float zFar)
        {
            this.windowed = windowed;
            windowRect = rect;
            adapterFormat = format;
            presentParams = (PresentParameters)pparams.Clone();
            createFlags = cflags;
            deviceType = dtype;
            deviceCaps = dcaps;
            zNearPlane = zNear;
            zFar = zFarPlane;
        }
        public RectangleF WindowRect
        {
            get
            {
                return windowRect;
            }
        }

        public SizeF WindowSize
        {
            get
            {
                return windowRect.Size;
            }
        }
        public PointF WindowPos
        {
            get
            {
                return windowRect.Location;
            }
        }

        public bool Windowed
        {
            get
            {
                return windowed;
            }
        }

        public Format AdapterFormat
        {
            get
            {
                return adapterFormat;
            }
        }

        public Format BackbufferFormat
        {
            get
            {
                return presentParams.BackBufferFormat;
            }
        }

        public PresentParameters PresentParamers
        {
            get
            {
                return presentParams;
            }
        }

        public CreateFlags CreateFlags
        {
            get
            {
                return createFlags;
            }
        }

        public DeviceType DeviceType
        {
            get
            {
                return deviceType;
            }
        }

        public Caps DeviceCapabilities
        {
            get
            {
                return deviceCaps;
            }
        }

        public float AspectRatio
        {
            get
            {
                if (WindowSize.Height != 0)
                {
                    return WindowSize.Width / WindowSize.Height;
                }
                else
                    return 0;
            }
        }

    }
}
