using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CreateTextureParams
    {
        public uint width;
        public uint height;
        public uint numLevels;
        public NoesisTextureFormat format;
    }
}
