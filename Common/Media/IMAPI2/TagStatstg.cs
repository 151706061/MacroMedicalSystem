using System;
using System.Runtime.InteropServices;

namespace Macro.Common.Media.IMAPI2
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct TagStatstg
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwcsName;
        public uint type;
        public ULargeInteger cbSize;
        public FileTime mtime;
        public FileTime ctime;
        public FileTime atime;
        public uint grfMode;
        public uint grfLocksSupported;
        public Guid clsid;
        public uint grfStateBits;
        public uint reserved;
    }
}