using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NoesisTile
    {
        public uint X;
        public uint Y;
        public uint Width;
        public uint Height;
    }
}
