using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{

    [ComImport]
    [Guid("2C941FD6-975B-59BE-A960-9A2A262853A5")]
    [CoClass(typeof(EnumProgressItemsClass))]
    public interface EnumProgressItems : IEnumProgressItems
    {
    }

    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("2C941FCA-975B-59BE-A960-9A2A262853A5")]
    public class EnumProgressItemsClass
    {
    }



    /// <summary>
    /// FileSystemImageResult progress item enumerator
    /// </summary>
    [Guid("2C941FD6-975B-59BE-A960-9A2A262853A5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumProgressItems
    {
        // Not in COM, but is in Interop.cs
        void Next(uint celt, out IProgressItem rgelt, out uint pceltFetched);

        // Remoting support for Next (allow NULL pointer for item count when requesting single item)
        void RemoteNext(uint celt, out IProgressItem rgelt, out uint pceltFetched);

        // Skip items in the enumeration
        void Skip(uint celt);

        // Reset the enumerator
        void Reset();

        // Make a copy of the enumerator
        void Clone(out IEnumProgressItems ppEnum);
    }

}