using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P
{
    class SampleGrabberCallback : ISampleGrabberCB
    {
        public SampleGrabberCallback()
        {
        }

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (BufferLen > 0)
            {
                byte[] buf = new byte[BufferLen];
                Marshal.Copy(pBuffer, buf, 0, BufferLen);
               //  Console.WriteLine("******************+" + buf.Length);
                MainWindow.mainWindow.ChangeImage(buf);
            }
            return 0;
        }

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
           //  Console.WriteLine("**********************55555555555555555555555**********************");
            if (pSample == null) return -1;
            int len = pSample.GetActualDataLength();
            IntPtr pbuf;
            if (pSample.GetPointer(out pbuf) == 0 && len > 0)
            {
                byte[] buf = new byte[len];
                Marshal.Copy(pbuf, buf, 0, len);
                for (int i = 0; i < len; i += 2)
                    buf[i] = (byte)(255 - buf[i]);
                Marshal.Copy(buf, 0, pbuf, len);
            }
            return 0;
        }
    }
}