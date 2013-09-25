using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace Macro.Common.Media.IMAPI2
{

    #region DFileSystemImageEvents
    /// <summary>
    /// Provides notification of file system import progress
    /// </summary>
    [ComImport]
    [Guid("2C941FDF-975B-59BE-A960-9A2A262853A5")]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    public interface DFileSystemImageEvents
    {
        // Update to current progress
        [DispId(0x100)]     // DISPID_DFILESYSTEMIMAGEEVENTS_UPDATE 
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, string currentFile, [In] int copiedSectors, [In] int totalSectors);
    }

    [ComVisible(false)]
    [ComEventInterface(typeof(DFileSystemImageEvents), typeof(DFileSystemImage_EventProvider))]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DFileSystemImage_Event
    {
        // Events
        event DFileSystemImage_EventHandler Update;
    }

    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DFileSystemImage_EventProvider : DFileSystemImage_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;


        // Methods
        public DFileSystemImage_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DFileSystemImageEvents).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DFileSystemImage_EventHandler Update
        {
            add
            {
                lock (this)
                {
                    DFileSystemImage_SinkHelper helper = new DFileSystemImage_SinkHelper(value);
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
                    DFileSystemImage_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DFileSystemImage_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DFileSystemImage_EventProvider()
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
                foreach (DFileSystemImage_SinkHelper helper in m_aEventSinkHelpers.Values)
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

    [ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FHidden)]
    public sealed class DFileSystemImage_SinkHelper : DFileSystemImageEvents
    {
        // Fields
        private int m_dwCookie;
        private DFileSystemImage_EventHandler m_UpdateDelegate;

        public DFileSystemImage_SinkHelper(DFileSystemImage_EventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_UpdateDelegate = eventHandler;
        }

        public void Update(object sender, string currentFile, int copiedSectors, int totalSectors)
        {
            m_UpdateDelegate(sender, currentFile, copiedSectors, totalSectors);
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

        public DFileSystemImage_EventHandler UpdateDelegate
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
    public delegate void DFileSystemImage_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)]object sender, string currentFile, int copiedSectors, int totalSectors);
    #endregion  // DFileSystemImageEvents

    #region DFileSystemImageImportEvents
    //
    // DFileSystemImageImportEvents
    //
    [ComImport]
    [Guid("D25C30F9-4087-4366-9E24-E55BE286424B")]
    [TypeLibType(TypeLibTypeFlags.FNonExtensible | TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDispatchable)]
    public interface DFileSystemImageImportEvents
    {
        [DispId(0x101)]     // DISPID_DFILESYSTEMIMAGEIMPORTEVENTS_UPDATEIMPORT 
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void UpdateImport([In, MarshalAs(UnmanagedType.IDispatch)] object sender, FsiFileSystems fileSystem,
                string currentItem, int importedDirectoryItems, int totalDirectoryItems, int importedFileItems, int totalFileItems);
    }

    [ComVisible(false)]
    [ComEventInterface(typeof(DFileSystemImageImportEvents), typeof(DFileSystemImageImport_EventProvider))]
    [TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DFileSystemImageImport_Event
    {
        // Events
        event DFileSystemImageImport_EventHandler UpdateImport;
    }


    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class DFileSystemImageImport_EventProvider : DFileSystemImageImport_Event, IDisposable
    {
        // Fields
        private Hashtable m_aEventSinkHelpers = new Hashtable();
        private IConnectionPoint m_connectionPoint = null;


        // Methods
        public DFileSystemImageImport_EventProvider(object pointContainer)
        {
            lock (this)
            {
                Guid eventsGuid = typeof(DFileSystemImageImportEvents).GUID;
                IConnectionPointContainer connectionPointContainer = pointContainer as IConnectionPointContainer;

                connectionPointContainer.FindConnectionPoint(ref eventsGuid, out m_connectionPoint);
            }
        }

        public event DFileSystemImageImport_EventHandler UpdateImport
        {
            add
            {
                lock (this)
                {
                    DFileSystemImageImport_SinkHelper helper = new DFileSystemImageImport_SinkHelper(value);
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
                    DFileSystemImageImport_SinkHelper helper =
                        m_aEventSinkHelpers[value] as DFileSystemImageImport_SinkHelper;
                    if (helper != null)
                    {
                        m_connectionPoint.Unadvise(helper.Cookie);
                        m_aEventSinkHelpers.Remove(helper.UpdateDelegate);
                    }
                }
            }
        }

        ~DFileSystemImageImport_EventProvider()
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
                foreach (DFileSystemImageImport_SinkHelper helper in m_aEventSinkHelpers.Values)
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
    public sealed class DFileSystemImageImport_SinkHelper : DFileSystemImageImportEvents
    {
        // Fields
        private int m_dwCookie;
        private DFileSystemImageImport_EventHandler m_UpdateDelegate;

        public DFileSystemImageImport_SinkHelper(DFileSystemImageImport_EventHandler eventHandler)
        {
            if (eventHandler == null)
                throw new ArgumentNullException("Delegate (callback function) cannot be null");
            m_dwCookie = 0;
            m_UpdateDelegate = eventHandler;
        }

        public void UpdateImport(object sender, FsiFileSystems fileSystems, string currentItem,
            int importedDirectoryItems, int totalDirectoryItems, int importedFileItems, int totalFileItems)
        {
            m_UpdateDelegate(sender, fileSystems, currentItem, importedDirectoryItems, totalDirectoryItems,
                importedFileItems, totalFileItems);
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

        public DFileSystemImageImport_EventHandler UpdateDelegate
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
    public delegate void DFileSystemImageImport_EventHandler([In, MarshalAs(UnmanagedType.IDispatch)] object sender,
        FsiFileSystems fileSystem, string currentItem, int importedDirectoryItems, int totalDirectoryItems, int importedFileItems, int totalFileItems);

    #endregion  // DFileSystemImageImportEvents    


    [ComImport]
    [CoClass(typeof(MsftFileSystemImageClass))]
    [Guid("7CFF842C-7E97-4807-8304-910DD8F7C051")]
    public interface MsftFileSystemImage : IFileSystemImage3, DFileSystemImage_Event
    {
    }

    [ComImport]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid("2C941FC5-975B-59BE-A960-9A2A262853A5")]
    [ComSourceInterfaces("DFileSystemImageEvents\0DFileSystemImageImportEvents\0")]
    [ClassInterface(ClassInterfaceType.None)]
    public class MsftFileSystemImageClass
    {
    }
    
    /// <summary>
    /// File system image
    /// </summary>
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("2C941FE1-975B-59BE-A960-9A2A262853A5")]
    public interface IFileSystemImage
    {
        // Root directory item
        [DispId(0)]
        IFsiDirectoryItem Root { get; }

        // Disc start block for the image
        [DispId(1)]
        int SessionStartBlock { get; set; }

        // Maximum number of blocks available for the image
        [DispId(2)]
        int FreeMediaBlocks { get; set; }

        // Set maximum number of blocks available based on the recorder 
        // supported discs. 0 for unknown maximum may be set.
        [DispId(0x24)]
        void SetMaxMediaBlocksFromDevice(IDiscRecorder2 discRecorder);

        // Number of blocks in use
        [DispId(3)]
        int UsedBlocks { get; }

        // Volume name
        [DispId(4)]
        string VolumeName { get; set; }

        // Imported Volume name
        [DispId(5)]
        string ImportedVolumeName { get; }

        // Boot image and boot options
        [DispId(6)]
        IBootOptions BootImageOptions { get; set; }

        // Number of files in the image
        [DispId(7)]
        int FileCount { get; }

        // Number of directories in the image
        [DispId(8)]
        int DirectoryCount { get; }

        // Temp directory for stash files
        [DispId(9)]
        string WorkingDirectory { get; set; }

        // Change point identifier
        [DispId(10)]
        int ChangePoint { get; }

        // Strict file system compliance option
        [DispId(11)]
        bool StrictFileSystemCompliance { get; set; }

        // If true, indicates restricted character set is being used for file and directory names
        [DispId(12)]
        bool UseRestrictedCharacterSet { get; set; }

        // File systems to create
        [DispId(13)]
        FsiFileSystems FileSystemsToCreate { get; set; }

        // File systems supported
        [DispId(14)]
        FsiFileSystems FileSystemsSupported { get; }

        // UDF revision
        [DispId(0x25)]
        int UDFRevision { set; get; }

        // UDF revision(s) supported
        [DispId(0x1f)]
        object[] UDFRevisionsSupported { get; }

        // Select filesystem types and image size based on the current media
        [DispId(0x20)]
        void ChooseImageDefaults(IDiscRecorder2 discRecorder);

        // Select filesystem types and image size based on the media type
        [DispId(0x21)]
        void ChooseImageDefaultsForMediaType(IMAPI_MEDIA_PHYSICAL_TYPE value);

        // ISO compatibility level to create
        [DispId(0x22)]
        int ISO9660InterchangeLevel { set; get; }

        // ISO compatibility level(s) supported
        [DispId(0x26)]
        object[] ISO9660InterchangeLevelsSupported { get; }

        // Create result image stream
        [DispId(15)]
        IFileSystemImageResult CreateResultImage();

        // Check for existance an item in the file system
        [DispId(0x10)]
        FsiItemType Exists(string FullPath);

        // Return a string useful for identifying the current disc
        [DispId(0x12)]
        string CalculateDiscIdentifier();

        // Identify file systems on a given disc
        [DispId(0x13)]
        FsiFileSystems IdentifyFileSystemsOnDisc(IDiscRecorder2 discRecorder);

        // Identify which of the specified file systems would be imported by default
        [DispId(20)]
        FsiFileSystems GetDefaultFileSystemForImport(FsiFileSystems fileSystems);

        // Import the default file system on the current disc
        [DispId(0x15)]
        FsiFileSystems ImportFileSystem();

        // Import a specific file system on the current disc
        [DispId(0x16)]
        void ImportSpecificFileSystem(FsiFileSystems fileSystemToUse);

        // Roll back to the specified change point
        [DispId(0x17)]
        void RollbackToChangePoint(int ChangePoint);

        // Lock in changes
        [DispId(0x18)]
        void LockInChangePoint();

        // Create a directory item with the specified name
        [DispId(0x19)]
        IFsiDirectoryItem CreateDirectoryItem(string Name);

        // Create a file item with the specified name
        [DispId(0x1a)]
        IFsiFileItem CreateFileItem(string Name);

        // Volume Name UDF
        [DispId(0x1b)]
        string VolumeNameUDF { get; }

        // Volume name Joliet
        [DispId(0x1c)]
        string VolumeNameJoliet { get; }

        // Volume name ISO 9660
        [DispId(0x1d)]
        string VolumeNameISO9660 { get; }

        // Indicates whether or not IMAPI should stage the filesystem before the burn
        [DispId(30)]
        bool StageFiles { get; set; }

        // Get array of available multi-session interfaces.
        [DispId(40)]
        object[] MultisessionInterfaces { get; set; }
    }


    /// <summary>
    /// File system image (rev.2)
    /// </summary>
    [ComImport]
    [Guid("D7644B2C-1537-4767-B62F-F1387B02DDFD")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IFileSystemImage2
    {
        //
        // IFileSystemImage
        //

        // Root directory item
        [DispId(0)]
        IFsiDirectoryItem Root { get; }

        // Disc start block for the image
        [DispId(1)]
        int SessionStartBlock { get; set; }

        // Maximum number of blocks available for the image
        [DispId(2)]
        int FreeMediaBlocks { get; set; }

        // Set maximum number of blocks available based on the recorder 
        // supported discs. 0 for unknown maximum may be set.
        [DispId(0x24)]
        void SetMaxMediaBlocksFromDevice(IDiscRecorder2 discRecorder);

        // Number of blocks in use
        [DispId(3)]
        int UsedBlocks { get; }

        // Volume name
        [DispId(4)]
        string VolumeName { get; set; }

        // Imported Volume name
        [DispId(5)]
        string ImportedVolumeName { get; }

        // Boot image and boot options
        [DispId(6)]
        IBootOptions BootImageOptions { get; set; }

        // Number of files in the image
        [DispId(7)]
        int FileCount { get; }

        // Number of directories in the image
        [DispId(8)]
        int DirectoryCount { get; }

        // Temp directory for stash files
        [DispId(9)]
        string WorkingDirectory { get; set; }

        // Change point identifier
        [DispId(10)]
        int ChangePoint { get; }

        // Strict file system compliance option
        [DispId(11)]
        bool StrictFileSystemCompliance { get; set; }

        // If true, indicates restricted character set is being used for file and directory names
        [DispId(12)]
        bool UseRestrictedCharacterSet { get; set; }

        // File systems to create
        [DispId(13)]
        FsiFileSystems FileSystemsToCreate { get; set; }

        // File systems supported
        [DispId(14)]
        FsiFileSystems FileSystemsSupported { get; }

        // UDF revision
        [DispId(0x25)]
        int UDFRevision { set; get; }

        // UDF revision(s) supported
        [DispId(0x1f)]
        object[] UDFRevisionsSupported { get; }

        // Select filesystem types and image size based on the current media
        [DispId(0x20)]
        void ChooseImageDefaults(IDiscRecorder2 discRecorder);

        // Select filesystem types and image size based on the media type
        [DispId(0x21)]
        void ChooseImageDefaultsForMediaType(IMAPI_MEDIA_PHYSICAL_TYPE value);

        // ISO compatibility level to create
        [DispId(0x22)]
        int ISO9660InterchangeLevel { set; get; }

        // ISO compatibility level(s) supported
        [DispId(0x26)]
        object[] ISO9660InterchangeLevelsSupported { get; }

        // Create result image stream
        [DispId(15)]
        IFileSystemImageResult CreateResultImage();

        // Check for existance an item in the file system
        [DispId(0x10)]
        FsiItemType Exists(string FullPath);

        // Return a string useful for identifying the current disc
        [DispId(0x12)]
        string CalculateDiscIdentifier();

        // Identify file systems on a given disc
        [DispId(0x13)]
        FsiFileSystems IdentifyFileSystemsOnDisc(IDiscRecorder2 discRecorder);

        // Identify which of the specified file systems would be imported by default
        [DispId(20)]
        FsiFileSystems GetDefaultFileSystemForImport(FsiFileSystems fileSystems);

        // Import the default file system on the current disc
        [DispId(0x15)]
        FsiFileSystems ImportFileSystem();

        // Import a specific file system on the current disc
        [DispId(0x16)]
        void ImportSpecificFileSystem(FsiFileSystems fileSystemToUse);

        // Roll back to the specified change point
        [DispId(0x17)]
        void RollbackToChangePoint(int ChangePoint);

        // Lock in changes
        [DispId(0x18)]
        void LockInChangePoint();

        // Create a directory item with the specified name
        [DispId(0x19)]
        IFsiDirectoryItem CreateDirectoryItem(string Name);

        // Create a file item with the specified name
        [DispId(0x1a)]
        IFsiFileItem CreateFileItem(string Name);

        // Volume Name UDF
        [DispId(0x1b)]
        string VolumeNameUDF { get; }

        // Volume name Joliet
        [DispId(0x1c)]
        string VolumeNameJoliet { get; }

        // Volume name ISO 9660
        [DispId(0x1d)]
        string VolumeNameISO9660 { get; }

        // Indicates whether or not IMAPI should stage the filesystem before the burn
        [DispId(30)]
        bool StageFiles { get; set; }

        // Get array of available multi-session interfaces.
        [DispId(40)]
        object[] MultisessionInterfaces { get; set; }

        //
        // IFileSystemImage2
        //

        // Get boot options array for supporting multi-boot
        [DispId(60)]
        object[] BootImageOptionsArray { get; set; }
    }

    /// <summary>
    /// File system image (rev.3)"),
    /// </summary>
    [ComImport]
    [Guid("7CFF842C-7E97-4807-8304-910DD8F7C051")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IFileSystemImage3
    {
        //
        // IFileSystemImage
        //

        // Root directory item
        [DispId(0)]
        IFsiDirectoryItem Root { get; }

        // Disc start block for the image
        [DispId(1)]
        int SessionStartBlock { get; set; }

        // Maximum number of blocks available for the image
        [DispId(2)]
        int FreeMediaBlocks { get; set; }

        // Set maximum number of blocks available based on the recorder 
        // supported discs. 0 for unknown maximum may be set.
        [DispId(0x24)]
        void SetMaxMediaBlocksFromDevice(IDiscRecorder2 discRecorder);

        // Number of blocks in use
        [DispId(3)]
        int UsedBlocks { get; }

        // Volume name
        [DispId(4)]
        string VolumeName { get; set; }

        // Imported Volume name
        [DispId(5)]
        string ImportedVolumeName { get; }

        // Boot image and boot options
        [DispId(6)]
        IBootOptions BootImageOptions { get; set; }

        // Number of files in the image
        [DispId(7)]
        int FileCount { get; }

        // Number of directories in the image
        [DispId(8)]
        int DirectoryCount { get; }

        // Temp directory for stash files
        [DispId(9)]
        string WorkingDirectory { get; set; }

        // Change point identifier
        [DispId(10)]
        int ChangePoint { get; }

        // Strict file system compliance option
        [DispId(11)]
        bool StrictFileSystemCompliance { get; set; }

        // If true, indicates restricted character set is being used for file and directory names
        [DispId(12)]
        bool UseRestrictedCharacterSet { get; set; }

        // File systems to create
        [DispId(13)]
        FsiFileSystems FileSystemsToCreate { get; set; }

        // File systems supported
        [DispId(14)]
        FsiFileSystems FileSystemsSupported { get; }

        // UDF revision
        [DispId(0x25)]
        int UDFRevision { set; get; }

        // UDF revision(s) supported
        [DispId(0x1f)]
        object[] UDFRevisionsSupported { get; }

        // Select filesystem types and image size based on the current media
        [DispId(0x20)]
        void ChooseImageDefaults(IDiscRecorder2 discRecorder);

        // Select filesystem types and image size based on the media type
        [DispId(0x21)]
        void ChooseImageDefaultsForMediaType(IMAPI_MEDIA_PHYSICAL_TYPE value);

        // ISO compatibility level to create
        [DispId(0x22)]
        int ISO9660InterchangeLevel { set; get; }

        // ISO compatibility level(s) supported
        [DispId(0x26)]
        object[] ISO9660InterchangeLevelsSupported { get; }

        // Create result image stream
        [DispId(15)]
        IFileSystemImageResult CreateResultImage();

        // Check for existance an item in the file system
        [DispId(0x10)]
        FsiItemType Exists(string FullPath);

        // Return a string useful for identifying the current disc
        [DispId(0x12)]
        string CalculateDiscIdentifier();

        // Identify file systems on a given disc
        [DispId(0x13)]
        FsiFileSystems IdentifyFileSystemsOnDisc(IDiscRecorder2 discRecorder);

        // Identify which of the specified file systems would be imported by default
        [DispId(20)]
        FsiFileSystems GetDefaultFileSystemForImport(FsiFileSystems fileSystems);

        // Import the default file system on the current disc
        [DispId(0x15)]
        FsiFileSystems ImportFileSystem();

        // Import a specific file system on the current disc
        [DispId(0x16)]
        void ImportSpecificFileSystem(FsiFileSystems fileSystemToUse);

        // Roll back to the specified change point
        [DispId(0x17)]
        void RollbackToChangePoint(int ChangePoint);

        // Lock in changes
        [DispId(0x18)]
        void LockInChangePoint();

        // Create a directory item with the specified name
        [DispId(0x19)]
        IFsiDirectoryItem CreateDirectoryItem(string Name);

        // Create a file item with the specified name
        [DispId(0x1a)]
        IFsiFileItem CreateFileItem(string Name);

        // Volume Name UDF
        [DispId(0x1b)]
        string VolumeNameUDF { get; }

        // Volume name Joliet
        [DispId(0x1c)]
        string VolumeNameJoliet { get; }

        // Volume name ISO 9660
        [DispId(0x1d)]
        string VolumeNameISO9660 { get; }

        // Indicates whether or not IMAPI should stage the filesystem before the burn
        [DispId(30)]
        bool StageFiles { get; set; }

        // Get array of available multi-session interfaces.
        [DispId(40)]
        object[] MultisessionInterfaces { get; set; }

        //
        // IFileSystemImage2
        //

        // Get boot options array for supporting multi-boot
        [DispId(60)]
        object[] BootImageOptionsArray { get; set; }

        //
        // IFileSystemImage3
        //

        // If true, indicates that UDF Metadata and Metadata Mirror files are truly redundant,
        // i.e. reference different extents
        [DispId(0x3d)]
        bool CreateRedundantUdfMetadataFiles { get; set; }

        // Probe if a specific file system on the disc is appendable through IMAPI
        [DispId(70)]
        bool ProbeSpecificFileSystem(FsiFileSystems fileSystemToProbe);
    }

}