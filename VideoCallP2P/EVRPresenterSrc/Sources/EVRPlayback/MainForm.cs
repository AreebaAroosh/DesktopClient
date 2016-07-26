using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SlimDX.Direct3D9;
using EVR;
using Sonic;
using SlimDX;
using System.Threading;

namespace EVRPlayback
{

    public partial class MainForm : Form
    {
        #region Variables

        private object m_csSceneLock = new object();

        private Scene m_Scene = null;

        private DSFilePlayback m_Playback = null;

        #endregion

        #region Constructor

        public MainForm()
        {
            InitializeComponent();
            lock (m_csSceneLock)
            {
                m_Scene = new Scene(this.pbView);
            }
        }

        #endregion

        #region Form Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            tbFileName.Text = @"d:\test\media\1.avi";
#endif
            btnStart.Enabled = tbFileName.Text != "" && File.Exists(tbFileName.Text);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_Playback != null)
            {
                m_Playback.Dispose();
                m_Playback = null;
            }
            if (m_Scene != null)
            {
                m_Scene.Dispose();
                m_Scene = null;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog _dlg = new OpenFileDialog();
            _dlg.Title = "Select File for playback...";
            if (tbFileName.Text != "")
            {
                _dlg.FileName = Path.GetFileName(tbFileName.Text);
                _dlg.InitialDirectory = Path.GetDirectoryName(tbFileName.Text);
            }
            _dlg.CheckPathExists = true;
            _dlg.DefaultExt = "*.avi";
            _dlg.Multiselect = false;
            _dlg.Filter = "Video Files |*.avi;*.mpg;*.mpeg;*.mov;*.wmv;*.asf;*.mp4;*.vob| All Files (*.*)|*.*";
            _dlg.FilterIndex = 0;
            if (_dlg.ShowDialog(this) == DialogResult.OK)
            {
                tbFileName.Text = _dlg.FileName;
                btnStart.Enabled = tbFileName.Text != "" && File.Exists(tbFileName.Text);
            }
            _dlg.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_Playback == null)
            {
                m_Playback = new DSFilePlaybackEVR(m_Scene.Direct3DDevice);
                m_Playback.OnPlaybackStop += new EventHandler(btnStart_Click);
                ((DSFilePlaybackEVR)m_Playback).OnSurfaceReady += new EVR.SurfaceReadyHandler(m_Scene.OnSurfaceReady);
                m_Playback.FileName = this.tbFileName.Text;
                if (m_Playback.Start().Succeeded)
                {
                    btnStart.Text = "Stop";
                    btnBrowse.Enabled = false;
                }
                else
                {
                    btnStart_Click(sender, e);
                }
            }
            else
            {
                m_Playback.Dispose();
                m_Playback = null;
                btnStart.Text = "Start";
                btnBrowse.Enabled = true;
                this.pbView.Invalidate();
            }
        }

        #endregion

    }
}
