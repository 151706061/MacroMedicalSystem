using System.Runtime.InteropServices;
using System.Collections;

namespace Macro.Common.Media.IMAPI2
{
    [ComImport]
    [Guid("2C941FD5-975B-59BE-A960-9A2A262853A5")]
    [CoClass(typeof(ProgressItemClass))]
    public interface ProgressItem : IProgressItem
    {
    }

    [ComImport]
    [Guid("2C941FCB-975B-59BE-A960-9A2A262853A5")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ProgressItemClass
    {
    }

    [ComImport]
    [Guid("2C941FD7-975B-59BE-A960-9A2A262853A5")]
    [CoClass(typeof(ProgressItemsClass))]
    public interface ProgressItems : IProgressItems
    {
    }

    [ComImport]
    [Guid("2C941FC9-975B-59BE-A960-9A2A262853A5")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ProgressItemsClass
    {
    }

    /// <summary>
    /// FileSystemImageResult progress item
    /// </summary>
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    [Guid("2C941FD5-975B-59BE-A960-9A2A262853A5")]
    public interface IProgressItem
    {
        // Progress item description
        [DispId(1)]
        string Description { get; }

        // First block in the range of blocks used by the progress item
        [DispId(2)]
        uint FirstBlock { get; }

        // Last block in the range of blocks used by the progress item
        [DispId(3)]
        uint LastBlock { get; }

        // Number of blocks used by the progress item
        [DispId(4)]
        uint BlockCount { get; }
    }

    /// <summary>
    /// FileSystemImageResult progress item
    /// </summary>
    [Guid("2C941FD7-975B-59BE-A960-9A2A262853A5")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IProgressItems
    {
        // Get an enumerator for the collection
        [DispId(-4), TypeLibFunc((short)0x41)]
        IEnumerator GetEnumerator();

        // Find the block mapping from the specified index
        [DispId(0)]
        IProgressItem this[int Index] { get; }

        // Number of items in the collection
        [DispId(1)]
        int Count { get; }

        // Find the block mapping from the specified block
        [DispId(2)]
        IProgressItem ProgressItemFromBlock(uint block);

        // Find the block mapping from the specified item description
        [DispId(3)]
        IProgressItem ProgressItemFromDescription(string Description);

        // Get a non-variant enumerator
        [DispId(4)]
        IEnumProgressItems EnumProgressItems { get; }
    }

}