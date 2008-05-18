using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Threading;


namespace Framework
{
    public interface IUpdateable
    {
        /// <summary>
        /// Updates object
        /// </summary>
        /// <param name="timeElapsed">time since last update</param>
        /// <returns>True on successfull update</returns>
        bool Update(float timeElapsed);
    }

    public interface IDeviceRelated
    {
        void OnCreateDevice(Device device);
        void OnResetDevice(Device device);
        void OnLostDevice(Device device);
        void OnDestroyDevice(Device device);
    }

    public sealed partial class FrameworkWindow : Form
    {
        //public enum AppUpdateMode { Continous, OnDemand };

        //AppUpdateMode m_updateMode;

        //public AppUpdateMode UpdateMode
        //{
        //    get { return m_updateMode; }
        //    set { m_updateMode = value; }
        //}
        //AutoResetEvent m_needsUpdate;

        //public void NeedsUpdate()
        //{
        //    m_needsUpdate.Set();
        //}

        #region  Window's fields
        Device m_device;
        bool m_windowed;
        Size m_windowSize;
        Point m_windowLocation;
        FormWindowState m_windowState;
        DisplayMode m_displayMode;
        Caps m_caps;
        PresentParameters m_windowedParameters;
        PresentParameters m_fullscreenParameters;
        CreateFlags m_deviceFlags;
        bool m_disableResize = true;
        bool m_deviceLost = false;
        bool m_isClosing = false;


        List<GameLayer> m_layerList;
        FPSTimer m_FPSTimer;

        List<IUpdateable> m_updateableItems;
        List<GenericTimer> m_timers;

        GraphicsParameters m_graphicsParams = null;
        #endregion

        /// <summary>
        /// Constructor that allows us to specify fullscreen or windowed mode
        /// </summary>
        /// <param name="windowed">Mode(windowed/fullscreen)</param>
        /// <param name="width">Window width</param>
        /// <param name="height">Window height</param>
        public FrameworkWindow(bool windowed, int width, int height)
        {
            GM.AppWindow = this;
            //InitializeComponent();
            m_layerList = new List<GameLayer>(1);
            m_windowed = windowed;
            this.ClientSize = new Size(width, height);
            this.Show();
            m_windowSize = new Size(width, height);

            m_windowLocation = new Point(0, 0);
            m_FPSTimer = new FPSTimer(0.1f);
            m_FPSTimer.Start();

            m_updateableItems = new List<IUpdateable>();
            m_timers = new List<GenericTimer>();
            m_mousePosition = new System.Drawing.Point(
                MousePosition.X - this.ClientRectangle.X, MousePosition.Y - this.ClientRectangle.Y);
            m_mousePosition = PointToClient(MousePosition);

            //m_needsUpdate = new AutoResetEvent(true);
            //m_needsUpdate = true;
            //m_updateMode = AppUpdateMode.Continous;


        }

        /// <summary>
        /// Current FPS value
        /// </summary>
        public float FPS
        {
            get
            {
                return m_FPSTimer.FPS;
            }
        }
        public float RunningTime
        {
            get
            {
                return m_FPSTimer.RunningTime;
            }
        }

        public void AddUpdateableItem(IUpdateable item)
        {
            m_updateableItems.Add(item);
        }
        public void RemoveUpdateableItem(IUpdateable item)
        {
            m_updateableItems.Remove(item);
        }
        public void AddTimer(GenericTimer item)
        {
            m_timers.Add(item);
        }
        public void RemoveTimer(GenericTimer item)
        {
            m_timers.Remove(item);
        }


        #region Device initialization and (setting/changing) device parameters
        /// <summary>
        /// Returns whether the Application is idle or not
        /// </summary>
        private bool AppStillIdle
        {
            get
            {
                NativeMethods.Message msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        /// <summary>
        /// Initializes DirectX graphics
        /// </summary>
        /// <returns>true on success, false on failure</returns> 
        public bool InitializeGraphics()
        {
            GM.GeneralLog.BeginBlock("InitializeGraphics() called");
            m_displayMode = Manager.Adapters[0].CurrentDisplayMode;
            m_caps = Manager.GetDeviceCaps(0, DeviceType.Hardware);

            //Check for hardware T&L
            if (m_caps.DeviceCaps.SupportsHardwareTransformAndLight)
            {
                m_deviceFlags = CreateFlags.HardwareVertexProcessing;
                //Check for pure device
                if (m_caps.DeviceCaps.SupportsPureDevice)
                {
                    //m_deviceFlags |= CreateFlags.PureDevice;
                }
            }
            else
            {
                m_deviceFlags = CreateFlags.SoftwareVertexProcessing;
            }
            //m_deviceFlags = CreateFlags.SoftwareVertexProcessing;
            //Prepare 2 sets of parameters
            //(for windowed and fullscreen mode)
            bool status = BuildPresentParameters(ref m_windowedParameters, true);
            status &= BuildPresentParameters(ref m_fullscreenParameters, false);
            if (!status)
            {
                return false;
            }
            try
            {
                if (m_windowed)
                    m_graphicsParams = new GraphicsParameters(true, new RectangleF((float)Location.X, (float)Location.Y, (float)ClientSize.Width, (float)ClientSize.Height),
                        m_displayMode.Format, m_windowedParameters, m_deviceFlags, DeviceType.Hardware, m_caps);
                else
                    m_graphicsParams = new GraphicsParameters(false, new RectangleF(0, 0, (float)ClientSize.Width, (float)ClientSize.Height),
                        Format.X8R8G8B8, m_fullscreenParameters, m_deviceFlags, DeviceType.Hardware, m_caps);

                ChangeDevice(m_windowed ? m_windowedParameters : m_fullscreenParameters,
                    m_deviceFlags);

                GM.GeneralLog.EndBlock("InitializeGraphics() finished");
                m_disableResize = false;

                
                return true;
            }
            catch (DirectXException e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }


        }

        /// <summary>
        /// Builds the PresentParameters class.
        /// </summary>
        /// <returns>true if the PresentParameters were successfully created, false otherwise.</returns> 
        private bool BuildPresentParameters(ref PresentParameters parameters, bool windowed)
        {
            GM.GeneralLog.Write("BuildPresentParameters() called");
            parameters = new PresentParameters();
            Format adapterFormat = windowed ? m_displayMode.Format : Format.X8R8G8B8;
            if (Manager.CheckDeviceFormat(0, DeviceType.Hardware, adapterFormat, Usage.DepthStencil,
                ResourceType.Surface, DepthFormat.D24X8))
            {
                parameters.AutoDepthStencilFormat = DepthFormat.D24X8;
            }
            else if (Manager.CheckDeviceFormat(0, DeviceType.Hardware, adapterFormat, Usage.DepthStencil,
                ResourceType.Surface, DepthFormat.D24S8))
            {
                parameters.AutoDepthStencilFormat = DepthFormat.D24S8;
            }
            else if (Manager.CheckDeviceFormat(0, DeviceType.Hardware, adapterFormat, Usage.DepthStencil,
           ResourceType.Surface, DepthFormat.D16))
            {
                parameters.AutoDepthStencilFormat = DepthFormat.D16;
            }
            else
            {
                return false;
            }
            parameters.BackBufferFormat = adapterFormat;
            //parameters.BackBufferWidth = windowed ? 0 : m_displayMode.Width;
            //parameters.BackBufferHeight = windowed ? 0 : m_displayMode.Height;
            parameters.BackBufferWidth = m_windowSize.Width;
            parameters.BackBufferHeight = m_windowSize.Height;
            parameters.BackBufferCount = 1;
            parameters.MultiSample = MultiSampleType.None;
            parameters.MultiSampleQuality = 0;
            parameters.SwapEffect = SwapEffect.Discard;
            parameters.DeviceWindowHandle = this.Handle;
            parameters.Windowed = windowed;
            parameters.PresentationInterval = PresentInterval.Immediate;
            parameters.EnableAutoDepthStencil = true;
            parameters.FullScreenRefreshRateInHz = windowed ? 0 : m_displayMode.RefreshRate;
            parameters.PresentFlag = PresentFlag.DiscardDepthStencil;
            return true;
        }

        /// <summary>
        /// Tries to regain a lost device.
        /// </summary>
        private void RegainLostDevice()
        {
            GM.GeneralLog.Write("RegainLostDevice() called");
            // Check for lost device
            int result;
            if (!m_device.CheckCooperativeLevel(out result))
            {
                ResultCode code = (ResultCode)result;//m_device.CheckCooperativeLevelResult();
                if (code == ResultCode.DeviceLost)
                {
                    // The device has been lost but cannot be reset at this time.  
                    // So wait until it can be reset.
                    System.Threading.Thread.Sleep(50);
                    GM.GeneralLog.Write("RegainLostDevice() finished(still lost)");
                    return;
                }
                try
                {
                    m_disableResize = true;
                    if (m_windowed)
                        m_device.Reset(m_windowedParameters);
                    else
                    {
                        //m_device.Reset(m_fullscreenParameters);
                        // Going to fullscreen mode
                        //Cursor.Hide();
                        //m_windowState = this.WindowState;
                        if (m_windowState == FormWindowState.Maximized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        //m_windowLocation = this.Location;
                        //m_windowSize = this.Size;
                        this.FormBorderStyle = FormBorderStyle.None;
                        this.TopMost = true;


                        m_graphicsParams.windowRect = new RectangleF(0, 0,
                            (float)m_fullscreenParameters.BackBufferWidth, (float)m_fullscreenParameters.BackBufferHeight);
                        m_graphicsParams.presentParams = (PresentParameters)m_fullscreenParameters.Clone();
                        m_graphicsParams.windowed = false;

                        m_device.Reset(m_fullscreenParameters);
                    }

                    m_deviceLost = false;
                    m_disableResize = false;
                    GM.GeneralLog.Write("RegainLostDevice() finished successfully");
                    return;
                }
                catch (DeviceLostException)
                {
                    GM.GeneralLog.Write("DeviceLostException recieved");
                    // The device was lost again, so continue waiting until it can be reset.
                    System.Threading.Thread.Sleep(50);
                }
            }
            GM.GeneralLog.Write("RegainLostDevice() finished(unneccessary call)");
        }

        /// <summary>
        /// Creates a new device with new parameters
        /// </summary>
        /// <param name="pp">New PresentParameters</param>
        /// <param name="flags">New CreateFlags</param>
        private void ChangeDevice(PresentParameters pp, CreateFlags flags)
        {
            GM.GeneralLog.BeginBlock("OnChangeDevice() called");
            try
            {
                if (m_device != null)
                {
                    m_device.Dispose();
                    m_device = null;
                }
                m_device = new Device(0, DeviceType.Hardware, this.Handle,
                            flags, pp);
                OnCreateDevice((object)m_device, null);
                OnResetDevice((object)m_device, null);

                m_device.DeviceLost += new EventHandler(this.OnLostDevice);
                m_device.DeviceReset += new EventHandler(this.OnResetDevice);
                m_device.Disposing += new EventHandler(this.OnDestroyDevice);
            }
            catch (DirectXException e)
            {
                MessageBox.Show(e.Message);
                //exit
            }

            GM.GeneralLog.EndBlock("OnChangeDevice() finished");
        }

        /// <summary>
        /// Switches between windowed and fullscreen mode with 
        /// the same adapter/backbuffer/stencil/depth format.
        /// </summary>
        public void ToggleFullscreen()
        {
            GM.GeneralLog.BeginBlock("ToggleFullscreen() called");

            bool timerStopped = m_FPSTimer.Stopped;
            m_FPSTimer.Stop();

            m_windowed = !m_windowed;
            this.m_disableResize = true;

            if (m_windowed)
            {
                // Going to window mode                
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                m_disableResize = false;

                this.Location = m_windowLocation;
                this.Size = m_windowSize;
                this.WindowState = m_windowState;
                

                m_graphicsParams.windowRect = new RectangleF((float)Location.X, (float)Location.Y,
                    (float)ClientSize.Width, (float)ClientSize.Height);
                m_graphicsParams.presentParams = (PresentParameters)m_windowedParameters.Clone();
                m_graphicsParams.windowed = false;

                m_device.Reset(m_windowedParameters);
                //Cursor.Show();
            }
            else
            {
                // Going to fullscreen mode
                //Cursor.Hide();
                m_windowState = this.WindowState;
                if (m_windowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                m_windowLocation = this.Location;
                m_windowSize = this.Size;
                this.FormBorderStyle = FormBorderStyle.None;
                this.TopMost = true;

                
                m_graphicsParams.windowRect = new RectangleF(0, 0,
                    (float)m_fullscreenParameters.BackBufferWidth, (float)m_fullscreenParameters.BackBufferHeight);
                m_graphicsParams.presentParams = (PresentParameters)m_fullscreenParameters.Clone();
                m_graphicsParams.windowed = false;

                m_device.Reset(m_fullscreenParameters);
            }
            GC.Collect();

            this.m_disableResize = false;
            CenterCursor();

            if (!timerStopped)
                m_FPSTimer.Start();
            GM.GeneralLog.EndBlock("ToggleFullscreen() finished");
        }

        /// <summary>
        /// Returns current device parameters
        /// </summary>
        public PresentParameters CurrentParameters
        {
            get
            {
                if (m_windowed)
                {
                    return m_windowedParameters;
                }
                else
                {
                    return m_fullscreenParameters;
                }
            }
        }

        public GraphicsParameters GraphicsParameters
        {
            get
            {
                return m_graphicsParams;
            }
        }
        #endregion

        #region Layer stack
        /// <summary>
        /// Size of the Window's layer stack
        /// </summary>
        public int LayerStackCount
        {
            get
            {
                return m_layerList.Count;
            }
        }

        /// <summary>
        /// Pushes a new layer onto the Window's layer stack
        /// </summary>
        /// <param name="layer">The layer to be added</param>
        public void PushLayer(GameLayer layer)
        {
            GM.GeneralLog.BeginBlock("PushLayer() called");
            if (layer != null)
            {
                if (m_device != null)
                {
                    layer.OnCreateDevice(m_device);
                    layer.OnResetDevice(m_device);
                }

                m_layerList.Add(layer);
            }
            GM.GeneralLog.EndBlock("PushLayer() finished");
        }

        /// <summary>
        /// Removes the topmost layer from the Window's layer stack
        /// </summary>
        /// <returns>Layer popped from the stack</returns>
        public GameLayer PopLayer()
        {
            //first we retrieve last element of the list(top of the stack)
            GameLayer layer = m_layerList[m_layerList.Count - 1];
            m_layerList.RemoveAt(m_layerList.Count - 1);
            return layer;
        }

        /// <summary>
        /// Inserts a new layer into the Window's layer stack
        /// (not necessairly on the top of it)
        /// </summary>
        /// <param name="layer">The layer to be inserted</param>
        /// <param name="index">Position at which it is to be inserted(0 for the top of the stack)</param>
        public void InsertLayer(GameLayer layer, int index)
        {
            if (layer != null)
            {
                m_layerList.Insert(index, layer);
            }
        }
        /// <summary>
        /// Removes a layer from the Window's layer stack
        /// </summary>
        /// <param name="layer">The layer to be removed</param>
        /// <returns>True if success, false otherwise (no such layer)</returns>
        public bool RemoveLayer(GameLayer layer)
        {
            if (layer != null)
            {
                m_layerList.Remove(layer);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Removes a layer from the Window's layer stack
        /// </summary>
        /// <param name="index">Position at which to remove(0 for the top of the stack)</param> 
        /// <returns>True if success, false otherwise (no such layer)</returns>
        public bool RemoveLayer(int index)
        {
            if (index >= 0 && index < m_layerList.Count)
            {
                m_layerList.RemoveAt(index);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Returns the layer at a specified position on the stack (counting from the top of the stack)
        /// </summary>
        /// <param name="index">Position of th layer</param>
        /// <returns></returns>
        public GameLayer GetLayer(int index)
        {
            if (index >= 0 && index < m_layerList.Count)
            {
                return m_layerList[m_layerList.Count - index - 1];
            }
            else
                return null;
        }

        #endregion

        #region Device callback methods

        /// <summary>
        /// Application idle event. Updates and renders frames.
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        public void OnApplicationIdle(object sender, EventArgs e)
        {

            if (m_device == null || m_device.Disposed)
            {
                return;
            }
            while (AppStillIdle)
            {
                //m_needsUpdate = false;
                //if (m_updateMode == AppUpdateMode.OnDemand)
                //{
                //    if (m_needsUpdate.WaitOne(0, false) == false)
                //        break;  //no update needed
                //}
                if (this.WindowState == FormWindowState.Minimized || m_device.Disposed)
                {
                    //When the window is minimized the application shouldn't
                    //do anything, so we stop the timer
                    if (!m_FPSTimer.Stopped)
                        m_FPSTimer.Stop();
                }
                else if (m_deviceLost)
                {
                    RegainLostDevice();
                }
                else
                {
                    if (m_FPSTimer.Stopped)
                        m_FPSTimer.Start();
                    m_FPSTimer.Update();

                    //if (m_FPSTimer.ElapsedTime < 0.04)
                    //    Thread.Sleep((int)(1000*(0.04 - m_FPSTimer.ElapsedTime)));

                    ProcessKeyboard();
                    ProcessMouse();
                    
                    UpdateFrame();
                    RenderFrame();                   
                }


            }
        }

        /// <summary>
        /// Responds to DeviceCreate event. Takes place each time a new device has been created.
        /// </summary>
        /// <param name="sender">Curent Device</param>
        /// <param name="args">Event EventArgs structure.
        /// Unused-for compatibility with EventHandler delegate only.</param>
        private void OnCreateDevice(object sender, EventArgs args)
        {
            GM.GeneralLog.BeginBlock("Window.OnCreateDevice() called");

            GM.OnCreateDevice((Device)sender);
            foreach (GameLayer layer in m_layerList)
            {
                layer.OnCreateDevice((Device)sender);
            }
            //NeedsUpdate();
            GM.GeneralLog.EndBlock("Window.OnCreateDevice() finished");

        }

        /// <summary>
        /// Responds to  DeviceLost event. Takes place each time device has been lost and needs to be reset.
        /// </summary>
        /// <param name="sender">Curent Device</param>
        /// <param name="args">Event EventArgs structure.
        /// Unused-for compatibility with EventHandler delegate only.</param>
        private void OnLostDevice(object sender, EventArgs args)
        {
            GM.GeneralLog.BeginBlock("Window.OnLostDevice() called");

            GM.OnLostDevice((Device)sender);
            foreach (GameLayer layer in m_layerList)
            {
                layer.OnLostDevice((Device)sender);
            }
            //NeedsUpdate();
            GM.GeneralLog.EndBlock("Window.OnLostDevice() finished");
        }

        /// <summary>
        /// Responds to  DeviceReset event. 
        /// </summary>
        /// <param name="sender">Curent Device</param>
        /// <param name="args">Event EventArgs structure.
        /// Unused-for compatibility with EventHandler delegate only.</param>
        private void OnResetDevice(object sender, EventArgs args)
        {
            GM.GeneralLog.BeginBlock("Window.OnResetDevice() called");
            m_device.Transform.World = Matrix.Identity;
            m_device.Transform.View = Matrix.Identity;
            float aspect;
            if (m_graphicsParams != null)
                aspect = m_graphicsParams.WindowSize.Width / m_graphicsParams.WindowSize.Height;
            else
            {
                if (m_windowed)
                {
                    aspect = (float)m_windowSize.Width / (float)m_windowSize.Height;
                }
                else
                {
                    aspect = (float)m_fullscreenParameters.BackBufferWidth / (float)m_fullscreenParameters.BackBufferHeight;
                }
            }

            m_device.Transform.Projection = Matrix.PerspectiveFovRH(
                (float)Math.PI / 3f, aspect, 0.1f, 100000f);

            GM.OnResetDevice((Device)sender);
            foreach (GameLayer layer in m_layerList)
            {
                layer.OnResetDevice((Device)sender);
            }
            //NeedsUpdate();
            GM.GeneralLog.EndBlock("Window.OnResetDevice() finished");
        }
        /// <summary>
        /// Responds to Device Disposing event. Takes place each time device is disposing.
        /// </summary>
        /// <param name="sender">Curent Device</param>
        /// <param name="args">Event EventArgs structure.
        /// Unused-for compatibility with EventHandler delegate only.</param>
        private void OnDestroyDevice(object sender, EventArgs args)
        {
            GM.GeneralLog.BeginBlock("Window.OnDestroyDevice() called");

            GM.OnDestroyDevice((Device)sender);
            foreach (GameLayer layer in m_layerList)
            {
                layer.OnDestroyDevice((Device)sender);
            }

            //NeedsUpdate();
            GM.GeneralLog.EndBlock("Window.OnDestroyDevice() finished");
        }

        /// <summary>
        /// Responds to OnResize event
        /// </summary>
        /// <param name="e">Unused</param>
        protected override void OnResize(EventArgs e)
        {
            GM.GeneralLog.BeginBlock("OnResize() called");

            bool timerStopped = m_FPSTimer != null ? m_FPSTimer.Stopped : true;
            if (!timerStopped)
                m_FPSTimer.Stop();

            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                GM.GeneralLog.EndBlock("OnResize() finished(minimized)");
                return;
            }
            if (this.m_disableResize)
            {
                GM.GeneralLog.EndBlock("OnResize() finished(m_disableResize)");
                return;
            }

            GM.GeneralLog.Write(this.Size.Width.ToString() + " " + this.Size.Height.ToString());
            if (m_windowed)
            {
                if (!BuildPresentParameters(ref m_windowedParameters, true))
                {
                    return;
                }
                try
                {
                    m_windowSize = this.Size;
                    m_device.Reset(m_windowedParameters);

                    m_graphicsParams.presentParams = (PresentParameters)m_windowedParameters.Clone();
                    m_graphicsParams.windowRect = new RectangleF((float)Location.X, (float)Location.Y,
                        (float)ClientSize.Width, (float)ClientSize.Height);


                }
                catch (DirectXException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            if (!timerStopped)
                m_FPSTimer.Start();

            GM.GeneralLog.EndBlock("OnResize() finished");
        }
        /// <summary>
        /// Responds to OnMove event
        /// </summary>
        /// <param name="e">Unused</param>
        protected override void OnMove(EventArgs e)
        {

            GM.GeneralLog.Write("OnMove() called");
            bool timerStopped = m_FPSTimer != null ? m_FPSTimer.Stopped : true;
            if (!timerStopped)
                m_FPSTimer.Stop();

            base.OnMove(e);
            if (m_disableResize)
                GM.GeneralLog.Write("OnMove exitted(m_disableResize)");
            else
            {
                m_windowLocation = this.Location;
                m_graphicsParams.windowRect = new RectangleF((float)Location.X, (float)Location.Y,
                        (float)ClientSize.Width, (float)ClientSize.Height);
            }

            if (!timerStopped)
                m_FPSTimer.Start();

        }
        #endregion

        /// <summary>
        /// Renders current frame
        /// </summary>
        private void RenderFrame()
        {
            if (m_device == null || m_device.Disposed ||
                this.WindowState == FormWindowState.Minimized || m_deviceLost)
            {
                return;
            }
            try
            {
                m_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                m_device.BeginScene();

                //First we find topmost non-transparent layer
                int i = m_layerList.Count - 1;
                while (i >= 0 && m_layerList[i].isTransparent)
                    i--;

                if (i < 0)
                    i = 0;
                //and then we render it and those that are above it
                for (; i < m_layerList.Count; i++)
                {
                    m_layerList[i].OnRenderFrame(m_device, m_FPSTimer.ElapsedTime);
                }

                m_device.EndScene();
                m_device.Present();
            }
            catch (DeviceLostException)
            {
                GM.GeneralLog.Write("DeviceLostException recieved");
                // The device is lost
                System.Threading.Thread.Sleep(50);
                if (!m_deviceLost)
                {
                    if (!m_FPSTimer.Stopped)
                        m_FPSTimer.Stop();
                    m_deviceLost = true;
                }
            }
        }
        /// <summary>
        /// Updates Frame. Takes place each frame before RenderFrame(). 
        /// </summary>
        private void UpdateFrame()
        {
            foreach (IUpdateable item in m_updateableItems)
            {
                item.Update(m_FPSTimer.ElapsedTime);
            }
            for (int i = m_layerList.Count - 1; i >= 0; i--)
            {

                bool cont = m_layerList[i].isTransparent;
                m_layerList[i].OnUpdateFrame(m_device, m_FPSTimer.ElapsedTime);
                
                if (m_layerList[i].hasFinished)
                {
                    m_layerList.RemoveAt(i);
                    if (m_layerList.Count == 0)
                    {
                        this.Close();
                    }
                }
                //if this layer isn't transparent, we don't update 
                //layers that are below this one on stack
                if (!cont)
                    break;
            }
            if (m_isClosing)
                this.Close();
        }

        /// <summary>
        /// Closes Framework window and ends application.
        /// </summary>
        public void CloseWindow()
        {
            m_FPSTimer.Stop();
            m_isClosing = true;
        }
        private new void Close()
        {
            m_layerList.Clear();
            GC.Collect();
            base.Close();
        }

        /// <summary>
        /// Manages keyboard input. 
        /// </summary>
        private void ProcessKeyboard()
        {
            if (this.IsDisposed == true)
                return;
            if (m_pressedKeys.Count > 0 || m_releasedKeys.Count > 0)
            {
                //NeedsUpdate();
                //if(m_pressedKey!=0 || m_pressedChar!=0)
                //{
                //GM.GeneralLog.BeginBlock("ProcessKeyboard() called");
                for (int i = m_layerList.Count - 1; i >= 0; i--)
                {
                    m_layerList[i].OnKeyboard(m_pressedKeys, m_releasedKeys, m_pressedChar, m_pressedKey, m_FPSTimer.ElapsedTime);
                    if (i<m_layerList.Count && m_layerList[i].locksKeyboard)
                        break;
                }

                foreach (Keys k in m_lockedKeys)
                {
                    m_pressedKeys.Remove(k);
                }
                m_releasedKeys.RemoveRange(0, m_releasedKeys.Count);

                m_pressedKey = 0;
                m_pressedChar = char.MinValue;
                //GM.GeneralLog.EndBlock("ProcessKeyboard() finished");
                if (m_isClosing)
                    Close();
            }
        }
        /// <summary>
        /// Manages mouse input
        /// </summary>
        private void ProcessMouse()
        {
            if (this.IsDisposed == true)
                return;

            if (!NoButtonsDown || !NoButtonsReleased || m_xDelta != 0 || m_yDelta != 0 || m_zDelta != 0)
            {
                //NeedsUpdate();
                for (int i = m_layerList.Count - 1; i >= 0; i--)
                {
                    m_layerList[i].OnMouse(m_mousePosition, m_xDelta, m_yDelta, m_zDelta,
                        m_pressedButtons, m_releasedButtons, m_FPSTimer.ElapsedTime);
                    if (i < m_layerList.Count && m_layerList[i].locksMouse)
                        break;
                }
                m_releasedButtons[0] = m_releasedButtons[1] = m_releasedButtons[2] = false;
                m_xDelta = m_yDelta = m_zDelta = 0;
            }
            else
            {
                for (int i = m_layerList.Count - 1; i >= 0; i--)
                {
                    if (m_layerList[i].RecievesEmptyMouse)
                    {
                        m_layerList[i].OnMouse(m_mousePosition, m_xDelta, m_yDelta, m_zDelta,
                            m_pressedButtons, m_releasedButtons, m_FPSTimer.ElapsedTime);
                        
                    }
                    if (i < m_layerList.Count && m_layerList[i].locksMouse)
                        break;
                }
            }

            if (m_isClosing)
                Close();

        }
    }
}