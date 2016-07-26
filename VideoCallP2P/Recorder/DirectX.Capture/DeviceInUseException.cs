using System;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  Exception thrown when the device cannot be rendered or started.
    /// </summary>
    public class DeviceInUseException : SystemException
    {
        // Initializes a new instance with the specified HRESULT
        public DeviceInUseException(string deviceName, int hResult)
            : base(string.Concat(new object[] { deviceName, " is in use or cannot be rendered. (", hResult, ")" }))
        {
        }
    }
}
