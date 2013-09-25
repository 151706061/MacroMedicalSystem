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
    #region DDiscFormat2RawCDEvents

    /// <summary>
    /// CD Disc-At-Once RAW Writer Events
    /// </summary>
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    [Guid("27354142-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface DDiscFormat2RawCDEvents
    {
        // Update to current progress
        [DispId(0x200)]     // DISPID_DDISCFORMAT2RAWCDEVENTS_UPDATE 
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);
    }

    [ComEventInterface(typeof(DDiscFormat2RawCDEvents), typeof(DiscFormat2RawCD_EventProvider))]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [ComVisible(false)]
    public interface DiscFormat2RawCD_Event
    {
        // Events
        event DiscFormat2RawCD_EventHandler Update;
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DiscFormat2RawCD_EventProvider : DiscFormat2RawCD_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;

        // Methods
        public DiscFormat2RawCD_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DDiscFormat2RawCDEvents).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DiscFormat2RawCD_EventHandler Update
        {
            add
            {
                lock (this)
                {
                    DiscFormat2RawCD_SinkHelper helper =
                        new DiscFormat2RawCD_SinkHelper(value);
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
                    DiscFormat2RawCD_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DiscFormat2RawCD_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DiscFormat2RawCD_EventProvider()
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
                foreach (DiscFormat2RawCD_SinkHelper helper in m_aEventSinkHelpers.Values)
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
    public sealed class DiscFormat2RawCD_SinkHelper : DDiscFormat2RawCDEvents
    {
        // Fields
        private int m_dwCookie;
        private DiscFormat2RawCD_EventHandler m_UpdateDelegate;

        public DiscFormat2RawCD_SinkHelper(DiscFormat2RawCD_EventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_UpdateDelegate = eventHandler;
        }

        public void Update(object sender, object progress)
        {
            m_UpdateDelegate(sender, progress);
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

        public DiscFormat2RawCD_EventHandler UpdateDelegate
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
    public delegate void DiscFormat2RawCD_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);

    #endregion  // DDiscFormat2RawCDEvents

    /// <summary>
    /// CD Disc-At-Once RAW Writer
    /// </summary>
    [ComImport, Guid("27354155-8F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IDiscFormat2RawCD
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
        // IDiscFormat2RawCD
        //

        // Locks the current media for use by this writer
        [DispId(0x200)]
        void PrepareMedia();

        // Writes a RAW image that starts at 95:00:00 (MSF) to the currently inserted blank CD media
        [DispId(0x201)]
        void WriteMedia(IStream data);

        // Writes a RAW image to the currently inserted blank CD media.  A stream starting at 95:00:00
        // (-5 minutes) would use 5*60*75 + 150 sectors pregap == 22,650 for the number of sectors
        [DispId(0x202)]
        void WriteMedia2(IStream data, int streamLeadInSectors);

        // Cancels the current write.
        [DispId(0x203)]
        void CancelWrite();

        // Finishes use of the locked media.
        [DispId(0x204)]
        void ReleaseMedia();

        // Sets the write speed (in sectors per second) of the attached disc recorder
        [DispId(0x205)]
        void SetWriteSpeed(int RequestedSectorsPerSecond, bool RotationTypeIsPureCAV);

        // The disc recorder to use
        [DispId(0x100)]
        IDiscRecorder2 Recorder { set; get; }

        // Buffer Underrun Free recording should be disabled
        [DispId(0x102)]
        bool BufferUnderrunFreeDisabled { set; get; }

        // The first sector of the next session.  May be negative for blank media
        [DispId(0x103)]
        int StartOfNextSession { get; }

        // The last possible start for the leadout area.  Can be used to 
        // calculate available space on media
        [DispId(260)]
        int LastPossibleStartOfLeadout { get; }

        // Get the current physical media type
        [DispId(0x105)]
        IMAPI_MEDIA_PHYSICAL_TYPE CurrentPhysicalMediaType { get; }

        // Supported data sector types for the current recorder
        [DispId(0x108)]
        object[] SupportedSectorTypes { get; }

        // Requested data sector to use during write of the stream
        [DispId(0x109)]
        IMAPI_FORMAT2_RAW_CD_DATA_SECTOR_TYPE RequestedSectorType { set; get; }

        // The friendly name of the client (used to determine recorder reservation conflicts).
        [DispId(0x10a)]
        string ClientName { set; get; }

        // The last requested write speed
        [DispId(0x10b)]
        int RequestedWriteSpeed { get; }

        // The last requested rotation type.
        [DispId(0x10c)]
        bool RequestedRotationTypeIsPureCAV { get; }

        // The drive's current write speed.
        [DispId(0x10d)]
        int CurrentWriteSpeed { get; }

        // The drive's current rotation type
        [DispId(270)]
        bool CurrentRotationTypeIsPureCAV { get; }

        // Gets an array of the write speeds supported for the attached disc 
        // recorder and current media
        [DispId(0x10f)]
        object[] SupportedWriteSpeeds { get; }

        // Gets an array of the detailed write configurations supported for the 
        // attached disc recorder and current media
        [DispId(0x110)]
        object[] SupportedWriteSpeedDescriptors { get; }
    }

    /// <summary>
    /// CD Disc-At-Once RAW Writer Event Arguments
    /// </summary>
    [ComImport, Guid("27354143-7F64-5B0F-8F00-5D77AFBE261E"), TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IDiscFormat2RawCDEventArgs
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
        // IDiscFormat2RawCDEventArgs
        //

        // The current write action
        [DispId(0x301)]
        IMAPI_FORMAT2_RAW_CD_WRITE_ACTION CurrentAction { get; }

        // The elapsed time for the current track write or media finishing operation
        [DispId(770)]
        int ElapsedTime { get; }

        // The estimated time remaining for the current track write or media finishing operation
        [DispId(0x303)]
        int RemainingTime { get; }
    }
    

    [ComImport]
    [CoClass(typeof(MsftDiscFormat2RawCDClass))]
    [Guid("27354155-8F64-5B0F-8F00-5D77AFBE261E")]
    public interface MsftDiscFormat2RawCD : IDiscFormat2RawCD, DiscFormat2RawCD_Event
    {
    }

    [ComImport]
    [Guid("27354128-7F64-5B0F-8F00-5D77AFBE261E")]
    [ComSourceInterfaces("DDiscFormat2RawCDEvents\0")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftDiscFormat2RawCDClass
    {
    }

}
