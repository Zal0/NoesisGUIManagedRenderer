using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NoesisDeviceCaps
    {
        public float CenterPixelOffset;

        [MarshalAs(UnmanagedType.U1)]
        public bool LinearRendering;

        [MarshalAs(UnmanagedType.U1)]
        public bool SubpixelRendering;
    };
}
