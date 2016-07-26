using System.Windows.Input;

namespace VideoCallP2P.lib2005
{
    public static class CameraCommands
    {
        #region Properties

        public static RoutedCommand TakePhoto { get; private set; }

        #endregion

        #region Ctors

        static CameraCommands()
        {
            TakePhoto = new RoutedCommand("CaptureImage", typeof (CameraCommands));
        }

        #endregion
    }
}