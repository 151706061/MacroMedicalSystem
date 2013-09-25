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

    #region  DDiscFormat2EraseEvents
    /// <summary>
    /// Provides notification of media erase progress.
    /// </summary>
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    [Guid("2735413A-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface DDiscFormat2EraseEvents
    {
        // Update to current progress
        [DispId(0x200)]     // DISPID_IDISCFORMAT2ERASEEVENTS_UPDATE 
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In] int elapsedSeconds, [In] int estimatedTotalSeconds);
    }

    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [ComVisible(false)]
    [ComEventInterface(typeof(DDiscFormat2EraseEvents), typeof(DiscFormat2Erase_EventProvider))]
    public interface DiscFormat2Erase_Event
    {
        // Events
        event DiscFormat2Erase_EventHandler Update;
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DiscFormat2Erase_EventProvider : DiscFormat2Erase_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;

        // Methods
        public DiscFormat2Erase_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DDiscFormat2EraseEvents).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DiscFormat2Erase_EventHandler Update
        {
            add
            {
                lock (this)
                {
                    DiscFormat2Erase_SinkHelper helper =
                        new DiscFormat2Erase_SinkHelper(value);
                    int cookie = -1;

                    m_connectionPoint.Advise(helper, out cookie);
                    helper.Cookie = cookie;
                    m_aEventSinkHelpers.Add(helper.UpdateDelegate, helper);
                }
            }

            remove
            {
                lock (this)
                {
                    DiscFormat2Erase_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DiscFormat2Erase_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DiscFormat2Erase_EventProvider()
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
                foreach (DiscFormat2Erase_SinkHelper helper in m_aEventSinkHelpers.Values)
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
    public sealed class DiscFormat2Erase_SinkHelper : DDiscFormat2EraseEvents
    {
        // Fields
        private int m_dwCookie;
        private DiscFormat2Erase_EventHandler m_UpdateDelegate;

        public DiscFormat2Erase_SinkHelper(DiscFormat2Erase_EventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_UpdateDelegate = eventHandler;
        }

        public void Update(object sender, int elapsedSeconds, int estimatedTotalSeconds)
        {
            m_UpdateDelegate(sender, elapsedSeconds, estimatedTotalSeconds);
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

        public DiscFormat2Erase_EventHandler UpdateDelegate
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
    public delegate void DiscFormat2Erase_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)]object sender, [In] int elapsedSeconds, [In] int estimatedTotalSeconds);
    #endregion  // DDiscFormat2EraseEvents


    [ComImport, Guid("27354156-8F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IDiscFormat2Erase
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
        // IDiscFormat2Erase
        //

        // Sets the recorder object to use for an erase operation
        [DispId(0x100)]
        IDiscRecorder2 Recorder { set; get; }

        // 
        [DispId(0x101)]
        bool FullErase { set; get; }

        // Get the current physical media type
        [DispId(0x102)]
        IMAPI_MEDIA_PHYSICAL_TYPE CurrentPhysicalMediaType { get; }

        // The friendly name of the client (used to determine recorder reservation conflicts)
        [DispId(0x103)]
        string ClientName { set; get; }

        // Erases the media in the active disc recorder
        [DispId(0x201)]
        void EraseMedia();
    }


    [ComImport]
    [Guid("27354156-8F64-5B0F-8F00-5D77AFBE261E")]
    [CoClass(typeof(MsftDiscFormat2EraseClass))]
    public interface MsftDiscFormat2Erase : IDiscFormat2Erase, DiscFormat2Erase_Event
    {
    }

    [ComImport]
    [Guid("2735412B-7F64-5B0F-8F00-5D77AFBE261E")]
    [ComSourceInterfaces("DDiscFormat2EraseEvents\0")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate), ClassInterface(ClassInterfaceType.None)]
    public class MsftDiscFormat2EraseClass
    {
    }

}
