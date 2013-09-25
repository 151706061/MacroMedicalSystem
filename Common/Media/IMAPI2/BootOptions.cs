using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Macro.Common.Media.IMAPI2
{

    /// <summary>
    /// Boot Options
    /// </summary>
    [Guid("2C941FD4-975B-59BE-A960-9A2A262853A5")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FNonExtensible)]
    public interface IBootOptions
    {
        // Get boot image data stream
        [DispId(1)]
        IStream BootImage { get; }

        // Get boot manufacturer
        [DispId(2)]
        string Manufacturer { get; set; }

        // Get boot platform identifier
        [DispId(3)]
        PlatformId PlatformId { get; set; }

        // Get boot emulation type
        [DispId(4)]
        EmulationType Emulation { get; set; }

        // Get boot image size
        [DispId(5)]
        uint ImageSize { get; }

        // Set the boot image data stream, emulation type, and image size
        [DispId(20)]
        void AssignBootImage(IStream newVal);
    }


    [ComImport]
    [CoClass(typeof(BootOptionsClass))]
    [Guid("2C941FD4-975B-59BE-A960-9A2A262853A5")]
    public interface BootOptions : IBootOptions
    {
    }



    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid("2C941FCE-975B-59BE-A960-9A2A262853A5")]
    public class BootOptionsClass
    {
    }

}
