using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FileTime
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    }

}