using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.DirectX.Direct3D;
namespace Framework
{
    /// <summary>
    /// Represents a visual layer of a game and provides basic functionality like
    /// keyboard/mouse input.
    /// </summary>
    public abstract class GameLayer : IDeviceRelated
    {

        private bool _isTransparent = false;
        private bool _isCreated = false;
        private bool _locksMouse = false;
        private bool _locksKeyboard = false;
        private bool _hasFinished = false;

        private bool _recievesEmptyMouse = false;

        public bool RecievesEmptyMouse
        {
            get { return _recievesEmptyMouse; }
            set { _recievesEmptyMouse = value; }
        }

        /// <summary>
        /// Tells framework's window that this state has finished
        /// </summary>
        public bool hasFinished
        {
            get
            {
                return _hasFinished;
            }
            protected set
            {
                _hasFinished = value;
            }
        }

        /// <summary>
        /// isTransparent property tells, if underlying
        /// GameLayers on Windw's GameLayer stack should be rendered
        /// (isTransparent=false => layers that are below this one on stack
        /// will not be rendered until this layer is removed or it's 
        /// isTransparent property is changed
        /// </summary>
        public bool isTransparent
        {
            get
            {
                return _isTransparent;
            }
            set
            {
                _isTransparent = value;
            }
        }

        /// <summary>
        /// isCreated property tells, if this layer has been already
        /// created, or is this just a new instance
        /// </summary>
        public bool isCreated
        {
            get
            {
                return _isCreated;
            }
            set
            {
                _isCreated = value;
            }
        }

        /// <summary>
        /// When locksMouse is true, all layers below this one will not recieve
        /// mouse events
        /// </summary>
        public bool locksMouse
        {
            get
            {
                return _locksMouse;
            }
            set
            {
                _locksMouse = value;
            }
        }

        /// <summary>
        /// When locksKeyboard is true, all layers below this one will not recieve
        /// keyboard events
        /// </summary>
        public bool locksKeyboard
        {
            get
            {
                return _locksKeyboard;
            }
            set
            {
                _locksKeyboard = value;
            }
        }

        /// <summary>
        /// Handles pressed keys
        /// </summary>
        /// <param name="pressedKeys">Pressed keys</param>
        /// <param name="pressedChar">Character value of pressed key</param>
        /// <param name="pressedKey">Integer value of pressed key</param>
        /// <param name="elapsedTime">T0 elapsed since last frame</param>
        public virtual void OnKeyboard(List<Keys> pressedKeys, List<Keys> releasedKeys, char pressedChar, int pressedKey, float elapsedTime)
        {
        }

        /// <summary>
        /// Mouse event handler
        /// </summary>
        /// <param name="position">Mouse position in client coordinates</param>
        /// <param name="xDelta">X-axis delta</param>
        /// <param name="yDelta">Y-axis delta</param>
        /// <param name="zDelta">Mouse wheel delta</param>
        /// <param name="buttons">Pressed buttons</param>
        public virtual void OnMouse(Point position, int xDelta, int yDelta, int zDelta, bool[] pressedButtons, bool[] releasedButtons, float elapsedTime)
        {
        }

        /// <summary>
        /// This is called by the main render loop every frame, to prepare 
        /// everything for rendering
        /// </summary>
        /// <param name="device">Window's device</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public virtual void OnUpdateFrame(Device device, float elapsedTime)
        {
        }
        /// <summary>
        /// This is called by the main render loop every frame after OnUpdateFrame.
        /// This is where all rendering should take place.
        /// </summary>
        /// <param name="device">Window's device</param>
        /// <param name="elapsedTime">T0 since last frame</param>
        public virtual void OnRenderFrame(Device device, float elapsedTime)
        {
        }

        /// <summary>
        /// Whenever a new device is created, this method is called
        /// </summary>
        /// <param name="device">New Window'a device</param>
        public virtual void OnCreateDevice(Device device)
        {
        }

        /// <summary>
        /// Whenever a new device is created, this method is called
        /// </summary>
        /// <param name="device">Window'a device</param>
        public virtual void OnResetDevice(Device device)
        {
        }

        /// <summary>
        /// Whenever the device is reset, this method is called
        /// </summary>
        /// <param name="device">Window'a device</param>
        public virtual void OnLostDevice(Device device)
        {
        }

        /// <summary>
        /// When the device is destroyed, this method is called
        /// </summary>
        /// <param name="device">Window'a device</param>
        public virtual void OnDestroyDevice(Device device)
        {
        }
    }
}
