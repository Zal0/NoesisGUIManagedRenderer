using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    [StructLayout(LayoutKind.Explicit)]
    public struct NoesisDeviceCaps
    {
        [FieldOffset(0)]
        public float CenterPixelOffset;

        [FieldOffset(4)]
        public bool LinearRendering;

        [FieldOffset(5)]
        public bool SubpixelRendering;
    };
}
