using System.Runtime.InteropServices;
using System.Collections;

namespace Macro.Common.Media.IMAPI2
{
    /// <summary>
    /// Named stream collection
    /// </summary>
    [ComImport]
    [Guid("ED79BA56-5294-4250-8D46-F9AECEE23459")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IFsiNamedStreams : IEnumerable
    {
        // Get a named stream from the collection
        [DispId(0)]
        FsiFileItem this[int Index] { get; }

        // Number of named streams in the collection
        [DispId(0x51)]
        int Count { [DispId(0x51)] get; }

        // Get a non-variant enumerator for the named stream collection
        [DispId(0x52)]
        EnumFsiItems EnumNamedStreams { get; }
    }


    [ComImport]
    [Guid("ED79BA56-5294-4250-8D46-F9AECEE23459")]
    [CoClass(typeof(FsiNamedStreamsClass))]
    public interface FsiNamedStreams : IFsiNamedStreams
    {
    }

    [ComImport]
    [Guid("C6B6F8ED-6D19-44B4-B539-B159B793A32D")]
    [ClassInterface(ClassInterfaceType.None)]
    public class FsiNamedStreamsClass
    {
    }
}