using System;
using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{
    [ComImport]
    [Guid("2C941FDA-975B-59BE-A960-9A2A262853A5")]
    [CoClass(typeof(EnumFsiItemsClass))]
    public interface EnumFsiItems : IEnumFsiItems
    {
    }

    [ComImport]
    [Guid("2C941FC6-975B-59BE-A960-9A2A262853A5")]
    [ClassInterface(ClassInterfaceType.None)]
    public class EnumFsiItemsClass
    {
    }

    /// <summary>
    /// FileSystemImage item enumerator
    /// </summary>
    [Guid("2C941FDA-975B-59BE-A960-9A2A262853A5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFsiItems
    {
        // Note: not listed in COM Interface, but it is in Interop.cs
        void Next(uint celt, out IFsiItem rgelt, out uint pceltFetched);

        // Remoting support for Next (allow NULL pointer for item count when requesting single item)
        void RemoteNext(uint celt, out IFsiItem rgelt, out uint pceltFetched);

        // Skip items in the enumeration
        void Skip(uint celt);

        // Reset the enumerator
        void Reset();

        // Make a copy of the enumerator
        void Clone(out IEnumFsiItems ppEnum);
    }
    
    /// <summary>
    /// FileSystemImage item
    /// </summary>
    [Guid("2C941FD9-975B-59BE-A960-9A2A262853A5")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IFsiItem
    {
        // Item name
        [DispId(11)]
        string Name { get; }

        // Full path
        [DispId(12)]
        string FullPath { get; }

        // Date and time of creation
        [DispId(13)]
        DateTime CreationTime { get; set; }

        // Date and time of last access
        [DispId(14)]
        DateTime LastAccessedTime { get; set; }

        // Date and time of last modification
        [DispId(15)]
        DateTime LastModifiedTime { get; set; }

        // Flag indicating if item is hidden
        [DispId(0x10)]
        bool IsHidden { get; set; }

        // Name of item in the specified file system
        [DispId(0x11)]
        string FileSystemName(FsiFileSystems fileSystem);

        // Name of item in the specified file system
        [DispId(0x12)]
        string FileSystemPath(FsiFileSystems fileSystem);
    }
}