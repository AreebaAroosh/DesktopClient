using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  A collection of available PropertyPages in a DirectShow
    ///  filter graph. It is up to the driver manufacturer to implement
    ///  a property pages on their drivers. The list of supported 
    ///  property pages will vary from driver to driver.
    /// </summary>
    public class PropertyPageCollection : CollectionBase, IDisposable
    {

        internal PropertyPageCollection()
        {
            InnerList.Capacity = 1;
        }

        internal PropertyPageCollection(ICaptureGraphBuilder2 graphBuilder, IBaseFilter videoDeviceFilter, IBaseFilter audioDeviceFilter, IBaseFilter videoCompressorFilter, IBaseFilter audioCompressorFilter, SourceCollection videoSources, SourceCollection audioSources)
        {
            this.AddFromGraph(graphBuilder, videoDeviceFilter, audioDeviceFilter, videoCompressorFilter, audioCompressorFilter, videoSources, audioSources);
        }


        /// <summary> Populate the collection by looking for commonly implemented property pages. </summary>
        protected void AddFromGraph(ICaptureGraphBuilder2 graphBuilder, IBaseFilter videoDeviceFilter, IBaseFilter audioDeviceFilter, IBaseFilter videoCompressorFilter, IBaseFilter audioCompressorFilter, SourceCollection videoSources, SourceCollection audioSources)
        {
            object filter = null;
            Guid capture;
            Guid interleaved;
            Guid gUID;
            int hr;

            Trace.Assert(graphBuilder != null);
            // 1. the video capture filter
            AddIfSupported(videoDeviceFilter, "Video Capture Device");

            // 2. the video capture pin
            capture = PinCategory.Capture;
            interleaved = MediaType.Interleaved;
            gUID = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(ref capture, ref interleaved, videoDeviceFilter, ref gUID, out filter);
            if (hr != 0)
            {
                interleaved = MediaType.Video;
                hr = graphBuilder.FindInterface(ref capture, ref interleaved, videoDeviceFilter, ref gUID, out filter);
                if (hr != 0)
                {
                    filter = null;
                }
            }
            AddIfSupported(filter, "Video Capture Pin");

            // 3. the video preview pin
            capture = PinCategory.Preview;
            interleaved = MediaType.Interleaved;
            gUID = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(ref capture, ref interleaved, videoDeviceFilter, ref gUID, out filter);
            if (hr != 0)
            {
                interleaved = MediaType.Video;
                hr = graphBuilder.FindInterface(ref capture, ref interleaved, videoDeviceFilter, ref gUID, out filter);
                if (hr != 0)
                {
                    filter = null;
                }
            }
            AddIfSupported(filter, "Video Preview Pin");

            // 4. the video crossbar(s)
            ArrayList crossbars = new ArrayList();
            int num = 1;
            for (int c = 0; c < videoSources.Count; c++)
            {
                CrossbarSource source = videoSources[c] as CrossbarSource;
                if (source != null && crossbars.IndexOf(source.Crossbar) < 0)
                {
                    crossbars.Add(source.Crossbar);
                    if (AddIfSupported(source.Crossbar, "Video Crossbar " + (num == 1 ? "" : num.ToString())))
                    {
                        num++;
                    }
                }
            }
            crossbars.Clear();

            // 5. the video compressor
            AddIfSupported(videoCompressorFilter, "Video Compressor");

            // 6. the video TV tuner
            capture = PinCategory.Capture;
            interleaved = MediaType.Interleaved;
            gUID = typeof(IAMTVTuner).GUID;
            hr = graphBuilder.FindInterface(ref capture, ref interleaved, videoDeviceFilter, ref gUID, out filter);
            if (hr != 0)
            {
                interleaved = MediaType.Video;
                hr = graphBuilder.FindInterface(ref capture, ref interleaved, videoDeviceFilter, ref gUID, out filter);
                if (hr != 0)
                {
                    filter = null;
                }
            }
            AddIfSupported(filter, "TV Tuner");

            // 7. the video compressor (VFW)
            IAMVfwCompressDialogs compressDialog = videoCompressorFilter as IAMVfwCompressDialogs;
            if (compressDialog != null)
            {
                VfwCompressorPropertyPage page = new VfwCompressorPropertyPage("Video Compressor", compressDialog);
                base.InnerList.Add(page);
            }

            // 8. the audio capture filter
            AddIfSupported(audioDeviceFilter, "Audio Capture Device");

            // 9. the audio capture pin
            capture = PinCategory.Capture;
            interleaved = MediaType.Audio;
            gUID = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(ref capture, ref interleaved, audioDeviceFilter, ref gUID, out filter);
            if (hr != 0)
            {
                filter = null;
            }
            AddIfSupported(filter, "Audio Capture Pin");

            // 9. the audio preview pin
            capture = PinCategory.Preview;
            interleaved = MediaType.Audio;
            gUID = typeof(IAMStreamConfig).GUID;
            hr = graphBuilder.FindInterface(ref capture, ref interleaved, audioDeviceFilter, ref gUID, out filter);
            if (hr != 0)
            {
                filter = null;
            }
            AddIfSupported(filter, "Audio Preview Pin");

            // 10. the audio crossbar(s)
            num = 1;
            for (int c = 0; c < audioSources.Count; c++)
            {
                CrossbarSource source = audioSources[c] as CrossbarSource;
                if (source != null && crossbars.IndexOf(source.Crossbar) < 0)
                {
                    crossbars.Add(source.Crossbar);
                    if (AddIfSupported(source.Crossbar, "Audio Crossbar " + (num == 1 ? "" : num.ToString())))
                    {
                        num++;
                    }
                }
            }
            crossbars.Clear();

            // 11. the audio compressor
            AddIfSupported(audioCompressorFilter, "Audio Compressor");
        }

        /// <summary> 
        ///  Returns the object as an ISpecificPropertyPage
        ///  if the object supports the ISpecificPropertyPage
        ///  interface and has at least one property page.
        /// </summary>
        protected bool AddIfSupported(object o, string name)
        {
            ISpecifyPropertyPages specifyPropertyPages = null;
            DsCAUUID pPages = new DsCAUUID();
            bool wasAdded = false;

            // Determine if the object supports the interface
            // and has at least 1 property page
            try
            {
                specifyPropertyPages = o as ISpecifyPropertyPages;
                if (specifyPropertyPages != null)
                {
                    int hr = specifyPropertyPages.GetPages(out pPages);
                    if ((hr != 0) || (pPages.cElems <= 0))
                    {
                        specifyPropertyPages = null;
                    }
                }
            }
            finally
            {
                if (pPages.pElems != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pPages.pElems);
                }
            }

            // Add the page to the internal collection
            if (specifyPropertyPages != null)
            {
                DirectShowPropertyPage p = new DirectShowPropertyPage(name, specifyPropertyPages);
                base.InnerList.Add(p);
                wasAdded = true;
            }
            return wasAdded;
        }

        /// <summary> Get the filter at the specified index. </summary>
        public PropertyPage this[int index]
        {
            get { return ((PropertyPage)InnerList[index]); }
        }

        public new void Clear()
        {
            for (int c = 0; c < base.InnerList.Count; c++)
            {
                this[c].Dispose();
            }
            base.InnerList.Clear();
        }

        public void Dispose()
        {
            Clear();
            base.InnerList.Capacity = 1;
        }

        ~PropertyPageCollection()
        {
            Dispose();
        }

    }

}
