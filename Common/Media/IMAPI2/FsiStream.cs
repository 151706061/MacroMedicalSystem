using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Macro.Common.Media.IMAPI2
{
    [ComImport]
    [Guid("0000000C-0000-0000-C000-000000000046")]
    [CoClass(typeof(FsiStreamClass))]
    public interface FsiStream : IStream
    {
    }

    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("2C941FCD-975B-59BE-A960-9A2A262853A5")]
    public class FsiStreamClass
    {
    }
}