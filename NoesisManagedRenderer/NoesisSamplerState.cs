namespace NoesisManagedRenderer
{
    public struct NoesisSamplerState
    {
        public enum WrapModes
        {
            // Clamp between 0.0 and 1.0
            ClampToEdge,
            // Out of range texture coordinates return transparent zero (0,0,0,0)
            ClampToZero,
            // Wrap to the other side of the texture
            Repeat,
            // The same as repeat but flipping horizontally
            MirrorU,
            // The same as repeat but flipping vertically
            MirrorV,
            // The combination of MirrorU and MirrorV
            Mirror,

            Count
        }

        public enum MinMagFilters
        {
            // Select the single pixel nearest to the sample point
            Nearest,
            // Select two pixels in each dimension and interpolate linearly between them
            Linear,

            Count
        }

        public enum MipFilters
        {
            // Texture sampled from mipmap level 0
            Disabled,
            // The nearest mipmap level is selected
            Nearest,
            // Both nearest levels are sampled and linearly interpolated
            Linear,

            Count
        }

        private readonly byte v;

        public WrapModes WrapMode => (WrapModes)((this.v >> 0) & 7);

        public MinMagFilters MinMagFilter => (MinMagFilters)((this.v >> 3) & 1);

        public MipFilters MipFilter => (MipFilters)((this.v >> 4) & 3);

        public override bool Equals(object obj)
        {
            return obj is NoesisSamplerState state &&
                   v == state.v;
        }

        public override int GetHashCode()
        {
            return 238427441 + v.GetHashCode();
        }
    }
}
