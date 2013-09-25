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

    #region DDiscMaster2Events
    /// <summary>
    /// Provides notification of the arrival/removal of CD/DVD (optical) devices.
    /// </summary>
    [ComImport]
    [Guid("27354131-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    public interface DDiscMaster2Events
    {
        // A device was added to the system
        [DispId(0x100)]     // DISPID_DDISCMASTER2EVENTS_DEVICEADDED
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void NotifyDeviceAdded([In, MarshalAs(UnmanagedType.IDispatch)] object sender, string uniqueId);

        // A device was removed from the system
        [DispId(0x101)]     // DISPID_DDISCMASTER2EVENTS_DEVICEREMOVED
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void NotifyDeviceRemoved([In, MarshalAs(UnmanagedType.IDispatch)] object sender, string uniqueId);
    }
    
    [ComVisible(false)]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    [ComEventInterface(typeof(DDiscMaster2Events), typeof(DiscMaster2_EventProvider))]
    public interface DiscMaster2_Event
    {
        // Events
        event DiscMaster2_NotifyDeviceAddedEventHandler NotifyDeviceAdded;
        event DiscMaster2_NotifyDeviceRemovedEventHandler NotifyDeviceRemoved;
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DiscMaster2_EventProvider : DiscMaster2_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;

        // Methods
        public DiscMaster2_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DDiscMaster2Events).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DiscMaster2_NotifyDeviceAddedEventHandler NotifyDeviceAdded
        {
            add
            {
                lock (this)
                {
                    DiscMaster2_SinkHelper helper =
                        new DiscMaster2_SinkHelper(value);
                    int cookie;

                    m_connectionPoint.Advise(helper, out cookie);
                    helper.Cookie = cookie;
                    m_aEventSinkHelpers.Add(helper.NotifyDeviceAddedDelegate, helper);
                }
            }

            remove
            {
                lock (this)
                {
                    DiscMaster2_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DiscMaster2_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.NotifyDeviceAddedDelegate);
                    }
                }
            }
        }

        public event DiscMaster2_NotifyDeviceRemovedEventHandler NotifyDeviceRemoved
        {
            add
            {
                lock (this)
                {
                    DiscMaster2_SinkHelper helper =
                        new DiscMaster2_SinkHelper(value);
                    int cookie;

                    m_connectionPoint.Advise(helper, out cookie);
                    helper.Cookie = cookie;
                    m_aEventSinkHelpers.Add(helper.NotifyDeviceRemovedDelegate, helper);
                }
            }

            remove
            {
                lock (this)
                {
                    DiscMaster2_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DiscMaster2_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.NotifyDeviceRemovedDelegate);
                    }
                }
            }
        }

        ~DiscMaster2_EventProvider()
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
                foreach (DiscMaster2_SinkHelper helper in m_aEventSinkHelpers.Values)
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

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DiscMaster2_NotifyDeviceAddedEventHandler([In, MarshalAs(UnmanagedType.IDispatch)]object sender, string uniqueId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DiscMaster2_NotifyDeviceRemovedEventHandler([In, MarshalAs(UnmanagedType.IDispatch)]object sender, string uniqueId);

    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public sealed class DiscMaster2_SinkHelper : DDiscMaster2Events
    {
        // Fields
        private int m_dwCookie;
        private DiscMaster2_NotifyDeviceAddedEventHandler m_AddedDelegate = null;
        private DiscMaster2_NotifyDeviceRemovedEventHandler m_RemovedDelegate = null;

        public DiscMaster2_SinkHelper(DiscMaster2_NotifyDeviceAddedEventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_AddedDelegate = eventHandler;
        }

        public DiscMaster2_SinkHelper(DiscMaster2_NotifyDeviceRemovedEventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_RemovedDelegate = eventHandler;
        }

        public void NotifyDeviceAdded(object sender, string uniqueId)
        {
            m_AddedDelegate(sender, uniqueId);
        }

        public void NotifyDeviceRemoved(object sender, string uniqueId)
        {
            m_RemovedDelegate(sender, uniqueId);
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

        public DiscMaster2_NotifyDeviceAddedEventHandler NotifyDeviceAddedDelegate
        {
            get
            {
                return m_AddedDelegate;
            }
            set
            {
                m_AddedDelegate = value;
            }
        }

        public DiscMaster2_NotifyDeviceRemovedEventHandler NotifyDeviceRemovedDelegate
        {
            get
            {
                return m_RemovedDelegate;
            }
            set
            {
                m_RemovedDelegate = value;
            }
        }
    }

    #endregion DDiscMaster2Events

    /// <summary>
    /// IDiscMaster2 is used to get an enumerator for the set of CD/DVD (optical) devices on the system
    /// </summary>
    [ComImport]
    [Guid("27354130-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IDiscMaster2
    {
        // Enumerates the list of CD/DVD devices on the system (VT_BSTR)
        [DispId(-4), TypeLibFunc((short)0x41)]
        IEnumerator GetEnumerator();

        // Gets a single recorder's ID (ZERO BASED INDEX)
        [DispId(0)]
        string this[int index] { get; }

        // The current number of recorders in the system.
        [DispId(1)]
        int Count { get; }

        // Whether IMAPI is running in an environment with optical devices and permission to access them.
        [DispId(2)]
        bool IsSupportedEnvironment { get; }
    }

    /// <summary>
    /// Microsoft IMAPIv2 Disc Master
    /// </summary>
    [ComImport]
    [Guid("27354130-7F64-5B0F-8F00-5D77AFBE261E")]
    [CoClass(typeof(MsftDiscMaster2Class))]
    public interface MsftDiscMaster2 : IDiscMaster2, DiscMaster2_Event
    {
    }



    [ComImport, ComSourceInterfaces("DDiscMaster2Events\0")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid("2735412E-7F64-5B0F-8F00-5D77AFBE261E")]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftDiscMaster2Class
    {
    }

}
