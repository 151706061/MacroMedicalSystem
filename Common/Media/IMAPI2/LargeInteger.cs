using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct LargeInteger
    {
        public long QuadPart;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ULargeInteger
    {
        public ulong QuadPart;
    }

}