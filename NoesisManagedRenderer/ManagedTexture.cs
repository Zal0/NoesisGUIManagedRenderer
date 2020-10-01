using System;
using System.Collections.Generic;

namespace NoesisManagedRenderer
{
    public abstract class ManagedTexture : IDisposable
    {
        private static Dictionary<IntPtr, ManagedTexture> textures = new Dictionary<IntPtr, ManagedTexture>();

        internal static ManagedTexture GetTexture(IntPtr nativePointer) => nativePointer != IntPtr.Zero ? textures[nativePointer] : null;

        private IntPtr nativePointer;

        public abstract uint Width { get; }

        public abstract uint Height { get; }

        internal void Register(IntPtr nativePointer)
        {
            if (this.nativePointer != IntPtr.Zero)
            {
                throw new InvalidOperationException("Texture is already registered");
            }

            this.nativePointer = nativePointer;
            textures[nativePointer] = this;
        }

        public abstract void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data);

        /// <inheritdoc />
        public void Dispose()
        {
            textures.Remove(this.nativePointer);
            this.nativePointer = IntPtr.Zero;
        }
    }
}