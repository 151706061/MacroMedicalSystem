using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TagConnectData
    {
        [MarshalAs(UnmanagedType.IUnknown)]
        public object pUnk;
        public uint dwCookie;
    }

}