using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CreateRenderTargetParams
    {
        public uint width;
        public uint height;
        public uint sampleCount;
    }
}
