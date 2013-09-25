using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

namespace Macro.Common.Media.IMAPI2
{

    #region DDiscFormat2DataEvents
    /// <summary>
    /// Data Writer
    /// </summary>
    [ComImport]
    [Guid("2735413C-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    public interface DDiscFormat2DataEvents
    {
        // Update to current progress
        [DispId(0x200)]     // DISPID_DDISCFORMAT2DATAEVENTS_UPDATE
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);
    }



    [ComVisible(false)]
    [ComEventInterface(typeof(DDiscFormat2DataEvents), typeof(DiscFormat2Data_EventProvider))]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DiscFormat2Data_Event
    {
        // Events
        event DiscFormat2Data_EventHandler Update;
    }


    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DiscFormat2Data_EventProvider : DiscFormat2Data_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;


        // Methods
        public DiscFormat2Data_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DDiscFormat2DataEvents).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DiscFormat2Data_EventHandler Update
        {
            add
            {
                lock (this)
                {
                    DiscFormat2Data_SinkHelper helper =
                        new DiscFormat2Data_SinkHelper(value);
                    int cookie;

                    m_connectionPoint.Advise(helper, out cookie);
                    helper.Cookie = cookie;
                    m_aEventSinkHelpers.Add(helper.UpdateDelegate, helper);
                }
            }

            remove
            {
                lock (this)
                {
                    DiscFormat2Data_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DiscFormat2Data_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DiscFormat2Data_EventProvider()
        {
            Cleanup();
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        private void Cleanup()
        {
            Monitor.Enter(this);
            try
            {
                foreach (DiscFormat2Data_SinkHelper helper in m_aEventSinkHelpers.Values)
                {
                    m_connectionPoint.Unadvise(helper.Cookie);
                }

                m_aEventSinkHelpers.Clear();
                Marshal.ReleaseComObject(m_connectionPoint);
            }
            catch (SynchronizationLockException)
            {
                return;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }



    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public sealed class DiscFormat2Data_SinkHelper : DDiscFormat2DataEvents
    {
        // Fields
        private int m_dwCookie;
        private DiscFormat2Data_EventHandler m_UpdateDelegate;

        // Methods
        internal DiscFormat2Data_SinkHelper(DiscFormat2Data_EventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_UpdateDelegate = eventHandler;
        }

        public void Update(object sender, object args)
        {
            m_UpdateDelegate(sender, args);
        }

        public int Cookie
        {
            get
            {
                return m_dwCookie;
            }
            set
            {
                m_dwCookie = value;
            }
        }

        public DiscFormat2Data_EventHandler UpdateDelegate
        {
            get
            {
                return m_UpdateDelegate;
            }
            set
            {
                m_UpdateDelegate = value;
            }
        }
    }



    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DiscFormat2Data_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);

    #endregion // DDiscFormat2DataEvents


    [ComImport]
    [CoClass(typeof(MsftDiscFormat2DataClass))]
    [Guid("27354153-9F64-5B0F-8F00-5D77AFBE261E")]
    public interface MsftDiscFormat2Data : IDiscFormat2Data, DiscFormat2Data_Event
    {
    }

    [ComImport]
    [ComSourceInterfaces("DDiscFormat2DataEvents\0")]
    [Guid("2735412A-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate), ClassInterface(ClassInterfaceType.None)]
    public class MsftDiscFormat2DataClass
    {
    }

    /// <summary>
    /// Data Writer
    /// </summary>
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("27354153-9F64-5B0F-8F00-5D77AFBE261E")]
    public interface IDiscFormat2Data
    {
        //
        // IDiscFormat2
        //

        // Determines if the recorder object supports the given format
        [DispId(0x800)]
        bool IsRecorderSupported(IDiscRecorder2 Recorder);

        // Determines if the current media in a supported recorder object supports the given format
        [DispId(0x801)]
        bool IsCurrentMediaSupported(IDiscRecorder2 Recorder);

        // Determines if the current media is reported as physically blank by the drive
        [DispId(0x700)]
        bool MediaPhysicallyBlank { get; }

        // Attempts to determine if the media is blank using heuristics (mainly for DVD+RW and DVD-RAM media)
        [DispId(0x701)]
        bool MediaHeuristicallyBlank { get; }

        // Supported media types
        [DispId(0x702)]
        object[] SupportedMediaTypes { get; }

        //
        // IDiscFormat2Data
        //

        // The disc recorder to use
        [DispId(0x100)]
        IDiscRecorder2 Recorder { set; get; }

        // Buffer Underrun Free recording should be disabled
        [DispId(0x101)]
        bool BufferUnderrunFreeDisabled { set; get; }

        // Postgap is included in image
        [DispId(260)]
        bool PostgapAlreadyInImage { set; get; }

        // The state (usability) of the current media
        [DispId(0x106)]
        IMAPI_FORMAT2_DATA_MEDIA_STATE CurrentMediaStatus { get; }

        // The write protection state of the current media
        [DispId(0x107)]
        IMAPI_MEDIA_WRITE_PROTECT_STATE WriteProtectStatus { get; }

        // Total sectors available on the media (used + free).
        [DispId(0x108)]
        int TotalSectorsOnMedia { get; }

        // Free sectors available on the media.
        [DispId(0x109)]
        int FreeSectorsOnMedia { get; }

        // Next writable address on the media (also used sectors)
        [DispId(0x10a)]
        int NextWritableAddress { get; }

        // The first sector in the previous session on the media
        [DispId(0x10b)]
        int StartAddressOfPreviousSession { get; }

        // The last sector in the previous session on the media
        [DispId(0x10c)]
        int LastWrittenAddressOfPreviousSession { get; }

        // Prevent further additions to the file system
        [DispId(0x10d)]
        bool ForceMediaToBeClosed { set; get; }

        // Default is to maximize compatibility with DVD-ROM.  May be disabled to 
        // reduce time to finish writing the disc or increase usable space on the 
        // media for later writing.
        [DispId(270)]
        bool DisableConsumerDvdCompatibilityMode { set; get; }

        // Get the current physical media type
        [DispId(0x10f)]
        IMAPI_MEDIA_PHYSICAL_TYPE CurrentPhysicalMediaType { get; }

        // The friendly name of the client (used to determine recorder reservation conflicts)
        [DispId(0x110)]
        string ClientName { set; get; }

        // The last requested write speed
        [DispId(0x111)]
        int RequestedWriteSpeed { get; }

        // The last requested rotation type
        [DispId(0x112)]
        bool RequestedRotationTypeIsPureCAV { get; }

        // The drive's current write speed
        [DispId(0x113)]
        int CurrentWriteSpeed { get; }

        // The drive's current rotation type.
        [DispId(0x114)]
        bool CurrentRotationTypeIsPureCAV { get; }

        // Gets an array of the write speeds supported for the attached disc recorder and current media
        [DispId(0x115)]
        object[] SupportedWriteSpeeds { get; }

        // Gets an array of the detailed write configurations supported for the attached disc recorder 
        // and current media
        [DispId(0x116)]
        object[] SupportedWriteSpeedDescriptors { get; }

        // Forces the Datawriter to overwrite the disc on overwritable media types
        [DispId(0x117)]
        bool ForceOverwrite { set; get; }

        // Returns the array of available multi-session interfaces. The array shall not be empty
        [DispId(280)]
        object[] MultisessionInterfaces { get; }

        // Writes all the data provided in the IStream to the device
        [DispId(0x200)]
        void Write(IStream data);

        // Cancels the current write operation
        [DispId(0x201)]
        void CancelWrite();

        // Sets the write speed (in sectors per second) of the attached disc recorder
        [DispId(0x202)]
        void SetWriteSpeed(int RequestedSectorsPerSecond, bool RotationTypeIsPureCAV);
    }


    /// <summary>
    /// Track-at-once Data Writer
    /// </summary>
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("2735413D-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface IDiscFormat2DataEventArgs
    {
        //
        // IWriteEngine2EventArgs
        //

        // The starting logical block address for the current write operation.
        [DispId(0x100)]
        int StartLba { get; }

        // The number of sectors being written for the current write operation.
        [DispId(0x101)]
        int SectorCount { get; }

        // The last logical block address of data read for the current write operation.
        [DispId(0x102)]
        int LastReadLba { get; }

        // The last logical block address of data written for the current write operation
        [DispId(0x103)]
        int LastWrittenLba { get; }

        // The total bytes available in the system's cache buffer
        [DispId(0x106)]
        int TotalSystemBuffer { get; }

        // The used bytes in the system's cache buffer
        [DispId(0x107)]
        int UsedSystemBuffer { get; }

        // The free bytes in the system's cache buffer
        [DispId(0x108)]
        int FreeSystemBuffer { get; }

        //
        // IDiscFormat2DataEventArgs
        //

        // The total elapsed time for the current write operation
        [DispId(0x300)]
        int ElapsedTime { get; }

        // The estimated time remaining for the write operation.
        [DispId(0x301)]
        int RemainingTime { get; }

        // The estimated total time for the write operation.
        [DispId(770)]
        int TotalTime { get; }

        // The current write action.
        [DispId(0x303)]
        IMAPI_FORMAT2_DATA_WRITE_ACTION CurrentAction { get; }
    }
}
