using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Macro.Common.Media.IMAPI2
{

    #region DWriteEngine2Events
    /// <summary>
    /// Provides notification of the progress of the WriteEngine2 writing.
    /// </summary>
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    [Guid("27354137-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface DWriteEngine2Events
    {
        // Update to current progress
        [DispId(0x100)]     // DISPID_DWRITEENGINE2EVENTS_UPDATE 
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);
    }

    [ComVisible(false)]
    [ComEventInterface(typeof(DWriteEngine2Events), typeof(DWriteEngine2_EventProvider))]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DWriteEngine2_Event
    {
        // Events
        event DWriteEngine2_EventHandler Update;
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DWriteEngine2_EventProvider : DWriteEngine2_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;

        // Methods
        public DWriteEngine2_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DWriteEngine2Events).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DWriteEngine2_EventHandler Update
        {
            add
            {
                lock (this)
                {
                    DWriteEngine2_SinkHelper helper =
                        new DWriteEngine2_SinkHelper(value);
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
                    DWriteEngine2_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DWriteEngine2_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DWriteEngine2_EventProvider()
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
                foreach (DWriteEngine2_SinkHelper helper in m_aEventSinkHelpers.Values)
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
    public sealed class DWriteEngine2_SinkHelper : DWriteEngine2Events
    {
        // Fields
        private int m_dwCookie;
        private DWriteEngine2_EventHandler m_UpdateDelegate;

        public DWriteEngine2_SinkHelper(DWriteEngine2_EventHandler eventHandler)
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

        public DWriteEngine2_EventHandler UpdateDelegate
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
    public delegate void DWriteEngine2_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress);
    #endregion // DWriteEngine2Events



    /// <summary>
    /// Write Engine
    /// </summary>
    [ComImport]
    [Guid("27354135-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IWriteEngine2
    {
        // Writes data provided in the IStream to the device
        [DispId(0x200)]
        void WriteSection(IStream data, int startingBlockAddress, int numberOfBlocks);

        // Cancels the current write operation
        [DispId(0x201)]
        void CancelWrite();

        // The disc recorder to use
        [DispId(0x100)]
        IDiscRecorder2Ex Recorder { set; get; }

        // If true, uses WRITE12 with the AV bit set to one; else uses WRITE10
        [DispId(0x101)]
        bool UseStreamingWrite12 { set; get; }

        // The approximate number of sectors per second the device can write at
        // the start of the write process.  This is used to optimize sleep time
        // in the write engine.
        [DispId(0x102)]
        int StartingSectorsPerSecond { set; get; }

        // The approximate number of sectors per second the device can write at
        // the end of the write process.  This is used to optimize sleep time 
        // in the write engine.
        [DispId(0x103)]
        int EndingSectorsPerSecond { set; get; }

        // The number of bytes to use for each sector during writing.
        [DispId(260)]
        int BytesPerSector { set; get; }

        // Simple check to see if the object is currently writing to media.
        [DispId(0x105)]
        bool WriteInProgress { get; }
    }

    /// <summary>
    /// CD Write Engine
    /// </summary>
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("27354136-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface IWriteEngine2EventArgs
    {
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
    }


    [ComImport]
    [Guid("27354135-7F64-5B0F-8F00-5D77AFBE261E")]
    [CoClass(typeof(MsftWriteEngine2Class))]
    public interface MsftWriteEngine2 : IWriteEngine2, DWriteEngine2_Event
    {
    }

    [ComImport]
    [Guid("2735412C-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComSourceInterfaces("DWriteEngine2Events\0")]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftWriteEngine2Class
    {
    }
}