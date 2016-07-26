using System;
using SlimDX.Direct3D9;
using Sonic;
using System.Runtime.InteropServices;
using SlimDX;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace EVRPlayback
{
    public class Scene: IDisposable 
    {
        #region Variables

        private object m_csLock = new object();
        private Surface m_RenderTarget = null;
        private Device m_Device = null;
        private Control m_Control = null;

        #endregion

        #region Properties

        public Device Direct3DDevice
        {
            get { return m_Device; }
        }

        #endregion

        #region Constructor

        public Scene(Control _control)
            : this(_control, null)
        {
            
        }

        public Scene(Control _control, Device _device)
        {
            m_Control = _control;
            m_Device = _device;
            if (m_Device == null)
            {
                Direct3DEx _d3d = new Direct3DEx();
                DisplayMode _mode = _d3d.GetAdapterDisplayMode(0);
                PresentParameters _parameters = new PresentParameters();
                _parameters.BackBufferFormat = _mode.Format;
                _parameters.BackBufferCount = 1;
                _parameters.BackBufferWidth = m_Control.Width;
                _parameters.BackBufferHeight = m_Control.Height;
                _parameters.Multisample = MultisampleType.None;
                _parameters.SwapEffect = SwapEffect.Discard;
                _parameters.PresentationInterval = PresentInterval.Default;
                _parameters.Windowed = true;
                _parameters.DeviceWindowHandle = m_Control.Handle;
                _parameters.PresentFlags = PresentFlags.DeviceClip | PresentFlags.Video;
                m_Device = new DeviceEx(_d3d, 0, DeviceType.Hardware, m_Control.Handle, CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing, _parameters);

                m_RenderTarget = m_Device.GetRenderTarget(0);
            }
        }

        ~Scene()
        {
            Dispose();
        }

        #endregion

        #region Scene Handling

        public void OnSurfaceReady(ref Surface _surface)
        {
            lock (m_csLock)
            {
                m_Device.SetRenderTarget(0, m_RenderTarget);
                m_Device.Clear(ClearFlags.Target, Color.Blue, 1.0f, 0);
                m_Device.BeginScene();
                Surface _backbufer = m_Device.GetBackBuffer(0, 0);
                m_Device.StretchRectangle(_surface, _backbufer, TextureFilter.Linear);
                m_Device.EndScene();
                m_Device.Present();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (m_csLock)
            {
                if (m_Device != null)
                {
                    m_Device.Dispose();
                    m_Device = null;
                }
            }
        }

        #endregion
    }
}