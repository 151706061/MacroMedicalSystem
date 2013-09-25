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

    #region DDiscFormat2TrackAtOnceEvents
    /// <summary>
    /// CD Track-at-Once Audio Writer Events
    /// </summary>
    [ComImport]
    [Guid("2735413F-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    public interface DDiscFormat2TrackAtOnceEvents
    {
        // Update to current progress
        [DispId(0x200)]     // DISPID_DDISCFORMAT2TAOEVENTS_UPDATE
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);
    }

    [ComVisible(false)]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [ComEventInterface(typeof(DDiscFormat2TrackAtOnceEvents), typeof(DiscFormat2TrackAtOnce_EventProvider))]
    public interface DiscFormat2TrackAtOnce_Event
    {
        // Events
        event DiscFormat2TrackAtOnce_EventHandler Update;
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DiscFormat2TrackAtOnce_EventProvider : DiscFormat2TrackAtOnce_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;

        // Methods
        public DiscFormat2TrackAtOnce_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DDiscFormat2TrackAtOnceEvents).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DiscFormat2TrackAtOnce_EventHandler Update
        {
            add
            {
                lock (this)
                {
                    DiscFormat2TrackAtOnce_SinkHelper helper =
                        new DiscFormat2TrackAtOnce_SinkHelper(value);
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
                    DiscFormat2TrackAtOnce_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DiscFormat2TrackAtOnce_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DiscFormat2TrackAtOnce_EventProvider()
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
                foreach (DiscFormat2TrackAtOnce_SinkHelper helper in m_aEventSinkHelpers.Values)
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

    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class DiscFormat2TrackAtOnce_SinkHelper : DDiscFormat2TrackAtOnceEvents
    {
        // Fields
        private int m_dwCookie;
        private DiscFormat2TrackAtOnce_EventHandler m_UpdateDelegate;

        public DiscFormat2TrackAtOnce_SinkHelper(DiscFormat2TrackAtOnce_EventHandler eventHandler)
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

        public DiscFormat2TrackAtOnce_EventHandler UpdateDelegate
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
    public delegate void DiscFormat2TrackAtOnce_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);

    #endregion  // DDiscFormat2TrackAtOnceEvents

    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [ComImport]
    [Guid("27354154-8F64-5B0F-8F00-5D77AFBE261E")]
    public interface IDiscFormat2TrackAtOnce
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
        // IDiscFormat2TrackAtOnce
        //

        // Locks the current media for use by this writer.
        [DispId(0x200)]
        void PrepareMedia();

        // Immediately writes a new audio track to the locked media
        [DispId(0x201)]
        void AddAudioTrack(IStream data);

        // Cancels the current addition of a track
        [DispId(0x202)]
        void CancelAddTrack();

        // Finishes use of the locked media
        [DispId(0x203)]
        void ReleaseMedia();

        // Sets the write speed (in sectors per second) of the attached disc recorder
        [DispId(0x204)]
        void SetWriteSpeed(int RequestedSectorsPerSecond, bool RotationTypeIsPureCAV);

        // The disc recorder to use
        [DispId(0x100)]
        IDiscRecorder2 Recorder { set; get; }

        // Buffer Underrun Free recording should be disabled
        [DispId(0x102)]
        bool BufferUnderrunFreeDisabled { set; get; }

        // Number of tracks already written to the locked media
        [DispId(0x103)]
        int NumberOfExistingTracks { get; }

        // Total sectors available on locked media if writing one continuous audio track
        [DispId(260)]
        int TotalSectorsOnMedia { get; }

        // Number of sectors available for adding a new track to the media
        [DispId(0x105)]
        int FreeSectorsOnMedia { get; }

        // Number of sectors used on the locked media, including overhead (space between tracks)
        [DispId(0x106)]
        int UsedSectorsOnMedia { get; }

        // Set the media to be left 'open' after writing, to allow multisession discs
        [DispId(0x107)]
        bool DoNotFinalizeMedia { set; get; }

        // Get the current physical media type
        [DispId(0x10a)]
        object[] ExpectedTableOfContents { get; }

        // Get the current physical media type
        [DispId(0x10b)]
        IMAPI_MEDIA_PHYSICAL_TYPE CurrentPhysicalMediaType { get; }

        // The friendly name of the client (used to determine recorder reservation conflicts)
        [DispId(270)]
        string ClientName { set; get; }

        // The last requested write speed
        [DispId(0x10f)]
        int RequestedWriteSpeed { get; }

        // The last requested rotation type.
        [DispId(0x110)]
        bool RequestedRotationTypeIsPureCAV { get; }

        // The drive's current write speed.
        [DispId(0x111)]
        int CurrentWriteSpeed { get; }

        // The drive's current rotation type.
        [DispId(0x112)]
        bool CurrentRotationTypeIsPureCAV { get; }

        // Gets an array of the write speeds supported for the attached disc recorder and current media
        [DispId(0x113)]
        object[] SupportedWriteSpeeds { get; }

        // Gets an array of the detailed write configurations supported for the attached disc recorder and current media
        [DispId(0x114)]
        object[] SupportedWriteSpeedDescriptors { get; }
    }


    /// <summary>
    /// CD Track-at-once Audio Writer Event Arguments
    /// </summary>
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("27354140-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface IDiscFormat2TrackAtOnceEventArgs
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
        // IDiscFormat2TrackAtOnceEventArgs
        //

        // The total elapsed time for the current write operation
        [DispId(0x300)]
        int CurrentTrackNumber { get; }

        // The current write action
        [DispId(0x301)]
        IMAPI_FORMAT2_TAO_WRITE_ACTION CurrentAction { get; }

        // The elapsed time for the current track write or media finishing operation
        [DispId(770)]
        int ElapsedTime { get; }

        // The estimated time remaining for the current track write or media finishing operation
        [DispId(0x303)]
        int RemainingTime { get; }
    }



    /// <summary>
    /// Microsoft IMAPIv2 Track-at-Once Audio CD Writer
    /// </summary>
    [ComImport]
    [Guid("27354154-8F64-5B0F-8F00-5D77AFBE261E")]
    [CoClass(typeof(MsftDiscFormat2TrackAtOnceClass))]
    public interface MsftDiscFormat2TrackAtOnce : IDiscFormat2TrackAtOnce, DiscFormat2TrackAtOnce_Event
    {
    }

    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComSourceInterfaces("DDiscFormat2TrackAtOnceEvents\0")]
    [Guid("27354129-7F64-5B0F-8F00-5D77AFBE261E")]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftDiscFormat2TrackAtOnceClass
    {
    }


}
