using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  A collection of sources (or physical connectors) on an 
    ///  audio or video device. This is used by the <see cref="VideoCapture"/>
    ///  class to provide a list of available sources on the currently
    ///  selected audio and video devices. This class cannot be created
    ///  directly.  This class assumes there is only 1 video and 1 audio
    ///  crossbar and all input pins route to a single output pin on each 
    ///  crossbar.
    /// </summary>
    public class SourceCollection : CollectionBase, IDisposable
    {

        internal SourceCollection()
        {
            base.InnerList.Capacity = 1;
        }

        internal SourceCollection(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter, bool isVideoDevice)
        {
            this.AddFromGraph(graphBuilder, deviceFilter, isVideoDevice);
        }

        /// <summary>
        ///  Gets or sets the source/physical connector currently in use.
        ///  This is marked internal so that the Capture class can control
        ///  how and when the source is changed.
        /// </summary>
        internal Source CurrentSource
        {
            get
            {
                // Loop through each source and find the first
                // enabled source.
                foreach (Source source in base.InnerList)
                {
                    if (source.Enabled)
                    {
                        return source;
                    }
                }

                return (null);

            }
            set
            {
                if (value == null)
                {
                    // Disable all sources
                    foreach (Source source in base.InnerList)
                    {
                        source.Enabled = false;
                    }
                }
                else if (value is CrossbarSource)
                {
                    // Enable this source
                    // (this will automatically disable all other sources)
                    value.Enabled = true;
                }
                else
                {
                    // Disable all sources
                    // Enable selected source
                    foreach (Source source in base.InnerList)
                    {
                        source.Enabled = false;
                    }
                    value.Enabled = true;
                }
            }
        }

        /// <summary> Populate the collection from a filter graph. </summary>
        protected void AddFromGraph(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter, bool isVideoDevice)
        {
            Trace.Assert(graphBuilder != null);

            ArrayList crossbars = FindCrossbars(graphBuilder, deviceFilter);
            foreach (IAMCrossbar crossbar in crossbars)
            {
                ArrayList sources = FindCrossbarSources(graphBuilder, crossbar, isVideoDevice);
                base.InnerList.AddRange(sources);
            }

            if (!isVideoDevice && base.InnerList.Count == 0)
            {
                ArrayList sources = FindAudioSources(graphBuilder, deviceFilter);
                base.InnerList.AddRange(sources);
            }
        }

        /// <summary>
        ///	 Retrieve a list of crossbar filters in the graph.
        ///  Most hardware devices should have a maximum of 2 crossbars, 
        ///  one for video and another for audio.
        /// </summary>
        protected ArrayList FindCrossbars(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
        {
            ArrayList crossbars = new ArrayList();

            Guid upstreamOnly = FindDirection.UpstreamOnly;
            Guid pType = new Guid();
            Guid gUID = typeof(IAMCrossbar).GUID;
            int hr;

            object comObj = null;
            object comObjNext = null;

            // Find the first interface, look upstream from the selected device
            hr = graphBuilder.FindInterface(ref upstreamOnly, ref pType, deviceFilter, ref gUID, out comObj);
            while ((hr == 0) && (comObj != null))
            {
                // If found, add to the list
                if (comObj is IAMCrossbar)
                {
                    crossbars.Add(comObj as IAMCrossbar);

                    // Find the second interface, look upstream from the next found crossbar
                    hr = graphBuilder.FindInterface(ref upstreamOnly, ref pType, comObj as IBaseFilter, ref gUID, out comObjNext);
                    comObj = comObjNext;
                }
                else
                {
                    comObj = null;
                }
            }

            return crossbars;
        }

        /// <summary>
        ///  Populate the internal InnerList with sources/physical connectors
        ///  found on the crossbars. Each instance of this class is limited
        ///  to video only or audio only sources ( specified by the isVideoDevice
        ///  parameter on the constructor) so we check each source before adding
        ///  it to the list.
        /// </summary>
        protected ArrayList FindCrossbarSources(ICaptureGraphBuilder2 graphBuilder, IAMCrossbar crossbar, bool isVideoDevice)
        {
            ArrayList sources = new ArrayList();
            int errorCode;
            int numOutPins;
            int numInPins;
            errorCode = crossbar.get_PinCounts(out numOutPins, out numInPins);
            if (errorCode < 0)
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }

            // We loop through every combination of output and input pin
            // to see which combinations match.

            // Loop through output pins
            for (int cOut = 0; cOut < numOutPins; cOut++)
            {
                // Loop through input pins
                for (int cIn = 0; cIn < numInPins; cIn++)
                {
                    // Can this combination be routed?
                    errorCode = crossbar.CanRoute(cOut, cIn);
                    if (errorCode == 0)
                    {
                        // Yes, this can be routed
                        int relatedPin;
                        PhysicalConnectorType connectorType;
                        errorCode = crossbar.get_CrossbarPinInfo(true, cIn, out relatedPin, out connectorType);
                        if (errorCode < 0)
                        {
                            Marshal.ThrowExceptionForHR(errorCode);
                        }

                        // Is this the correct type?, If so add to the InnerList
                        CrossbarSource source = new CrossbarSource(crossbar, cOut, cIn, connectorType);
                        if (connectorType < PhysicalConnectorType.Audio_Tuner)
                        {
                            if (isVideoDevice)
                            {
                                sources.Add(source);
                            }
                            else
                            {
                                sources.Add(source);
                            }
                        }
                    }
                }
            }

            // Some silly drivers (*cough* Nvidia *cough*) add crossbars
            // with no real choices. Every input can only be routed to
            // one output. Loop through every Source and see if there
            // at least one other Source with the same output pin.
            int refIndex = 0;
            while (refIndex < sources.Count)
            {
                bool found = false;
                CrossbarSource refSource = (CrossbarSource)sources[refIndex];
                for (int c = 0; c < sources.Count; c++)
                {
                    CrossbarSource source = (CrossbarSource)sources[c];
                    if ((refSource.OutputPin == source.OutputPin) && (refIndex != c))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    refIndex++;
                }
                else
                {
                    sources.RemoveAt(refIndex);
                }
            }
            return sources;
        }

        protected ArrayList FindAudioSources(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
        {
            ArrayList sources = new ArrayList();

            if (deviceFilter is IAMAudioInputMixer)
            {
                // Get a pin enumerator off the filter
                IEnumPins pins;
                int hr = deviceFilter.EnumPins(out pins);
                pins.Reset();
                if ((hr == 0) && (pins != null))
                {
                    // Loop through each pin
                    IPin[] ppPins = new IPin[1];
                    int f;
                    do
                    {
                        // Get the next pin
                        hr = pins.Next(1, ppPins, out f);
                        if ((hr == 0) && (ppPins[0] != null))
                        {
                            // Is this an input pin?
                            PinDirection output = PinDirection.Output;
                            hr = ppPins[0].QueryDirection(out output);
                            if ((hr == 0) && (output == (PinDirection.Input)))
                            {
                                // Add the input pin to the sources list
                                AudioSource source = new AudioSource(ppPins[0]);
                                sources.Add(source);
                            }
                            ppPins[0] = null;
                        }
                    }
                    while (hr == 0);

                    Marshal.ReleaseComObject(ppPins); 
                    ppPins = null;
                }
            }

            // If there is only one source, don't return it
            // because there is nothing for the user to choose.
            // (Hopefully that single source is already enabled).
            if (sources.Count == 1)
            {
                sources.Clear();
            }

            return sources;
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

        public Source this[int index]
        {
            get { return ((Source)base.InnerList[index]); }
        }

        ~SourceCollection()
        {
            Dispose();
        }

    }
}
