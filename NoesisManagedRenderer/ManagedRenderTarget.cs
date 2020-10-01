using System;
using System.Collections.Generic;

namespace NoesisManagedRenderer
{
    public abstract class ManagedRenderTarget : IDisposable
    {
        private static Dictionary<IntPtr, ManagedRenderTarget> renderTargets = new Dictionary<IntPtr, ManagedRenderTarget>();

        internal static ManagedRenderTarget GetRenderTarget(IntPtr nativePointer) => nativePointer != IntPtr.Zero ? renderTargets[nativePointer] : null;

        private IntPtr nativePointer;

        public uint Width => this.Texture?.Width ?? 0;

        public uint Height => this.Texture?.Height ?? 0;

        public abstract ManagedTexture Texture { get; }

        internal void Register(IntPtr pSurface, IntPtr pSurfaceTexture)
        {
            if (this.nativePointer != IntPtr.Zero)
            {
                throw new InvalidOperationException("RenderTarget is already registered");
            }

            this.nativePointer = pSurface;
            renderTargets[nativePointer] = this;

            if (this.Texture == null)
            {
                throw new NullReferenceException($"{nameof(this.Texture)} cannot be null");
            }

            this.Texture.Register(pSurfaceTexture);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            renderTargets.Remove(this.nativePointer);
            this.nativePointer = IntPtr.Zero;
        }
    }
}