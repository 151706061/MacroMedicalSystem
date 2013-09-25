using System;
using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{
    /// <summary>
    ///  Represents a single CD/DVD type device, and enables many common operations via a simplified API.
    /// </summary>
    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("27354133-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface IDiscRecorder2
    {
        // Ejects the media (if any) and opens the tray
        [DispId(0x100)]
        void EjectMedia();

        // Close the media tray and load any media in the tray.
        [DispId(0x101)]
        void CloseTray();

        // Acquires exclusive access to device.  May be called multiple times.
        [DispId(0x102)]
        void AcquireExclusiveAccess(bool force, string clientName);

        // Releases exclusive access to device.  Call once per AcquireExclusiveAccess().
        [DispId(0x103)]
        void ReleaseExclusiveAccess();

        // Disables Media Change Notification (MCN).
        [DispId(260)]
        void DisableMcn();

        // Re-enables Media Change Notification after a call to DisableMcn()
        [DispId(0x105)]
        void EnableMcn();

        // Initialize the recorder, opening a handle to the specified recorder.
        [DispId(0x106)]
        void InitializeDiscRecorder(string recorderUniqueId);

        // The unique ID used to initialize the recorder.
        [DispId(0)]
        string ActiveDiscRecorder { get; }

        // The vendor ID in the device's INQUIRY data.
        [DispId(0x201)]
        string VendorId { get; }

        // The Product ID in the device's INQUIRY data.
        [DispId(0x202)]
        string ProductId { get; }

        // The Product Revision in the device's INQUIRY data.
        [DispId(0x203)]
        string ProductRevision { get; }

        // Get the unique volume name (this is not a drive letter).
        [DispId(0x204)]
        string VolumeName { get; }

        // Drive letters and NTFS mount points to access the recorder.
        [DispId(0x205)]
        object[] VolumePathNames { [DispId(0x205)] get; }

        // One of the volume names associated with the recorder.
        [DispId(0x206)]
        bool DeviceCanLoadMedia { [DispId(0x206)] get; }

        // Gets the legacy 'device number' associated with the recorder.  This number is not guaranteed to be static.
        [DispId(0x207)]
        int LegacyDeviceNumber { [DispId(0x207)] get; }

        // Gets a list of all feature pages supported by the device
        [DispId(520)]
        object[] SupportedFeaturePages { [DispId(520)] get; }

        // Gets a list of all feature pages with 'current' bit set to true
        [DispId(0x209)]
        object[] CurrentFeaturePages { [DispId(0x209)] get; }

        // Gets a list of all profiles supported by the device
        [DispId(0x20a)]
        object[] SupportedProfiles { [DispId(0x20a)] get; }

        // Gets a list of all profiles with 'currentP' bit set to true
        [DispId(0x20b)]
        object[] CurrentProfiles { [DispId(0x20b)] get; }

        // Gets a list of all MODE PAGES supported by the device
        [DispId(0x20c)]
        object[] SupportedModePages { [DispId(0x20c)] get; }

        // Queries the device to determine who, if anyone, has acquired exclusive access
        [DispId(0x20d)]
        string ExclusiveAccessOwner { [DispId(0x20d)] get; }
    }


    /// <summary>
    /// Represents a single CD/DVD type device, enabling additional commands requiring advanced marshalling code
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("27354132-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface IDiscRecorder2Ex
    {
        //
        // Send a command to the device that does not transfer any data
        //
        void SendCommandNoData(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)]
            byte[] Cdb,
            uint CdbSize,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 18)] 
            byte[] SenseBuffer,
            uint Timeout);

        // Send a command to the device that requires data sent to the device
        void SendCommandSendDataToDevice(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)]
            byte[] Cdb,
            uint CdbSize,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 18)] 
            byte[] SenseBuffer,
            uint Timeout,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 5)] 
            byte[] Buffer,
            uint BufferSize);

        // Send a command to the device that requests data from the device
        void SendCommandGetDataFromDevice(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] 
            byte[] Cdb,
            uint CdbSize,
            [MarshalAs(UnmanagedType.LPArray, SizeConst = 18)] 
            byte[] SenseBuffer,
            uint Timeout,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 5)] 
            byte[] Buffer,
            uint BufferSize,
            out uint BufferFetched);

        // Read a DVD Structure from the media
        void ReadDvdStructure(uint format, uint address, uint layer, uint agid, out IntPtr data, out uint Count);

        // Sends a DVD structure to the media
        void SendDvdStructure(uint format,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 2)]
            byte[] data,
            uint Count);

        // Get the full adapter descriptor (via IOCTL_STORAGE_QUERY_PROPERTY).
        void GetAdapterDescriptor(out IntPtr data, ref uint byteSize);

        // Get the full device descriptor (via IOCTL_STORAGE_QUERY_PROPERTY).
        void GetDeviceDescriptor(out IntPtr data, ref uint byteSize);

        // Gets data from a READ_DISC_INFORMATION command
        void GetDiscInformation(out IntPtr discInformation, ref uint byteSize);

        // Gets data from a READ_TRACK_INFORMATION command
        void GetTrackInformation(uint address, IMAPI_READ_TRACK_ADDRESS_TYPE addressType, out IntPtr trackInformation, ref uint byteSize);

        // Gets a feature's data from a GET_CONFIGURATION command
        void GetFeaturePage(IMAPI_FEATURE_PAGE_TYPE requestedFeature, sbyte currentFeatureOnly, out IntPtr featureData, ref uint byteSize);

        // Gets data from a MODE_SENSE10 command
        void GetModePage(IMAPI_MODE_PAGE_TYPE requestedModePage, IMAPI_MODE_PAGE_REQUEST_TYPE requestType, out IntPtr modePageData, ref uint byteSize);

        // Sets mode page data using MODE_SELECT10 command
        void SetModePage(IMAPI_MODE_PAGE_REQUEST_TYPE requestType,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 2)]
            byte[] data,
            uint byteSize);

        // Gets a list of all feature pages supported by the device
        void GetSupportedFeaturePages(sbyte currentFeatureOnly, out IntPtr featureData, ref uint byteSize);

        // Gets a list of all PROFILES supported by the device
        void GetSupportedProfiles(sbyte currentOnly, out IntPtr profileTypes, out uint validProfiles);

        // Gets a list of all MODE PAGES supported by the device
        void GetSupportedModePages(IMAPI_MODE_PAGE_REQUEST_TYPE requestType, out IntPtr modePageTypes, out uint validPages);

        // The byte alignment requirement mask for this device.
        uint GetByteAlignmentMask();

        // The maximum non-page-aligned transfer size for this device.
        uint GetMaximumNonPageAlignedTransferSize();

        // The maximum non-page-aligned transfer size for this device.
        uint GetMaximumPageAlignedTransferSize();
    }



    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CoClass(typeof(MsftDiscRecorder2Class)), ComImport]
    [Guid("27354132-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface MsftDiscRecorder2Ex : IDiscRecorder2Ex
    {
    }

    [ComImport]
    [CoClass(typeof(MsftDiscRecorder2Class))]
    [Guid("27354133-7F64-5B0F-8F00-5D77AFBE261E")]
    public interface MsftDiscRecorder2 : IDiscRecorder2
    {
    }


    [ComImport]
    [Guid("2735412D-7F64-5B0F-8F00-5D77AFBE261E")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftDiscRecorder2Class
    {
    }

}