using System;
using System.Collections.Generic;

namespace NoesisManagedRenderer
{
    public abstract class ManagedRenderTarget : IDisposable
    {
        private static Dictionary<IntPtr, ManagedRenderTarget> renderTargets = new Dictionary<IntPtr, ManagedRenderTarget>();

        internal static ManagedRenderTarget GetRenderTarget(IntPtr nativePointer) => nativePointer != IntPtr.Zero ? renderTargets[nativePointer] : null;

        private IntPtr nativePointer;

        public abstract string Label { get; }

        public abstract uint Width { get; }

        public abstract uint Height { get; }

        public abstract uint SampleCount { get; }

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

        public override string ToString() => this.Label;

        /// <inheritdoc />
        public void Dispose()
        {
            renderTargets.Remove(this.nativePointer);
            this.nativePointer = IntPtr.Zero;
        }
    }
}