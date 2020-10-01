using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    public abstract class ManagedRenderDevice
    {
        public const string LIB_NOESIS = "ManagedRenderDevice.Native";

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string filename);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateManagedRenderDevice(NoesisDeviceCaps deviceCaps, bool flippedTextures);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetManagedRenderDeviceCallbacks(
            IntPtr pDevice,
            DrawBatchCallbackDelegate drawBatchCallback,
            MapVerticesCallbackDelegate mapVerticesCallback,
            UnmapVerticesCallbackDelegate unmapVerticesCallback,
            MapIndicesCallbackDelegate mapIndicesCallback,
            UnmapIndicesCallbackDelegate unmapIndicesCallback,
            BeginRenderCallbackDelegate beginRenderCallback,
            EndRenderCallbackDelegate endRenderCallback,
            CreateTextureDelegate createTextureCallback,
            UpdateTextureDelegate updateTextureCallback,
            CreateRenderTargetDelegate createRenderTargetCallback,
            CloneRenderTargetDelegate cloneRenderTargetCallback,
            SetRenderTargetDelegate setRenderTargetCallback,
            BeginTileDelegate beginTileCallback,
            EndTileDelegate endTileCallback,
            ResolveRenderTargetDelegate resolveRenderTargetCallback);

        private delegate void DrawBatchCallbackDelegate(ref NoesisBatch batch);
        private delegate IntPtr MapVerticesCallbackDelegate(uint size);
        private delegate void UnmapVerticesCallbackDelegate();
        private delegate IntPtr MapIndicesCallbackDelegate(uint size);
        private delegate void UnmapIndicesCallbackDelegate();
        private delegate void BeginRenderCallbackDelegate(bool offscreen);
        private delegate void EndRenderCallbackDelegate();
        private delegate void CreateTextureDelegate(IntPtr pTexture, ref CreateTextureParams args);
        private delegate void UpdateTextureDelegate(IntPtr pTexture, uint level, uint x, uint y, uint width, uint height, IntPtr data);
        private delegate void CreateRenderTargetDelegate(IntPtr pSurface, IntPtr pSurfaceTexture, ref CreateRenderTargetParams args);
        private delegate void CloneRenderTargetDelegate(IntPtr pClonedSurface, IntPtr pClonedSurfaceTexture, IntPtr pSurface);
        private delegate void SetRenderTargetDelegate(IntPtr pSurface);
        private delegate void BeginTileDelegate(ref NoesisTile tile, uint surfaceWidth, uint surfaceHeight);
        private delegate void EndTileDelegate();
        private delegate void ResolveRenderTargetDelegate(IntPtr pSurface, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] NoesisTile[] tiles, uint numTiles);

        protected const uint DYNAMIC_VB_SIZE = 512 * 1024;
        protected const uint DYNAMIC_IB_SIZE = 128 * 1024;
        protected const uint DYNAMIC_TEX_SIZE = 128 * 1024;

        internal IntPtr cPtr;

        static ManagedRenderDevice()
        {
            // Manually load lib dependencies before calling any function
            var noesisSDKPath = Environment.GetEnvironmentVariable("NOESIS_SDK_PATH");
            if (string.IsNullOrEmpty(noesisSDKPath))
            {
                throw new InvalidOperationException(
                    "'NOESIS_SDK_PATH' environment variable not defined. " +
                    "Add this variable pointing to 'NoesisGUI-NativeSDK-win-3.0.6' base directory");
            }

            var platform = Environment.Is64BitProcess ? "windows_x86_64" : "windows_x86";
            LoadLibrary(noesisSDKPath + $@"\Bin\{platform}\Noesis.dll");
            LoadLibrary(noesisSDKPath + $@"\Bin\{platform}\NoesisApp.dll");
            LoadLibrary(LIB_NOESIS);
        }

        public ManagedRenderDevice(NoesisDeviceCaps deviceCaps, bool flippedTextures)
        {
            this.cPtr = CreateManagedRenderDevice(deviceCaps, flippedTextures);

            SetManagedRenderDeviceCallbacks(
                this.cPtr,
                this.DrawBatch,
                this.MapVertices,
                this.UnmapVertices,
                this.MapIndices,
                this.UnmapIndices,
                this.BeginRender,
                this.EndRender,
                this.CreateTexture,
                this.UpdateTexture,
                this.CreateRenderTarget,
                this.CloneRenderTarget,
                this.SetRenderTarget,
                this.BeginTile,
                this.EndTile,
                this.ResolveRenderTarget);
        }

        protected abstract void DrawBatch(ref NoesisBatch batch);

        protected abstract IntPtr MapVertices(uint size);

        protected abstract void UnmapVertices();

        protected abstract IntPtr MapIndices(uint size);

        protected abstract void UnmapIndices();

        protected abstract void BeginRender(bool offscreen);

        protected abstract void EndRender();

        protected abstract void CreateTexture(IntPtr ptr, ref CreateTextureParams args);

        private void UpdateTexture(IntPtr ptr, uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
            var texture = ManagedTexture.GetTexture(ptr);
            texture.UpdateTexture(level, x, y, width, height, data);
        }

        protected abstract void CreateRenderTarget(IntPtr pSurface, IntPtr pSurfaceTexture, ref CreateRenderTargetParams args);

        protected abstract void CloneRenderTarget(IntPtr pClonedSurface, IntPtr pClonedSurfaceTexture, IntPtr pSurface);

        protected abstract void SetRenderTarget(IntPtr pSurface);

        protected abstract void BeginTile(ref NoesisTile tile, uint surfaceWidth, uint surfaceHeight);

        protected abstract void EndTile();

        protected abstract void ResolveRenderTarget(IntPtr pSurface, NoesisTile[] tiles, uint numTiles);
    }
}

