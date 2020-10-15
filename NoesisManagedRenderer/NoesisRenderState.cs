namespace NoesisManagedRenderer
{
    public struct NoesisRenderState
    {
        public enum BlendModes
        {
            Src,
            SrcOver,
            SrcOver_Dual,

            Count
        };

        public enum StencilModes
        {
            Disabled,
            Equal_Keep,
            Equal_Incr,
            Equal_Decr,

            Count
        };

        private readonly byte v;

        public bool ScissorEnable => (this.v & 1 << 0) != 0;

        public bool ColorEnable => (this.v & 1 << 1) != 0;

        public BlendModes BlendMode => (BlendModes)((this.v >> 2) & 3);

        public StencilModes StencilMode => (StencilModes)((this.v >> 4) & 3);

        public bool Wireframe => (this.v & 1 << 6) != 0;

        public override bool Equals(object obj)
        {
            return obj is NoesisRenderState state &&
                   v == state.v;
        }

        public override int GetHashCode()
        {
            return 238427441 + v.GetHashCode();
        }
    }
}
