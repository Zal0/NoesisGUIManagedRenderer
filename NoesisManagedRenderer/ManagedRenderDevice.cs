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
            DrawBatchCallback drawBatchCallback,
            MapVerticesCallback mapVerticesCallback,
            UnmapVerticesCallback unmapVerticesCallback,
            MapIndicesCallback mapIndicesCallback,
            UnmapIndicesCallback unmapIndicesCallback,
            BeginRenderCallback beginRenderCallback,
            EndRenderCallback endRenderCallback,
            CreateTextureCallback createTextureCallback,
            UpdateTextureCallback updateTextureCallback,
            CreateRenderTargetCallback createRenderTargetCallback,
            CloneRenderTargetCallback cloneRenderTargetCallback,
            SetRenderTargetCallback setRenderTargetCallback,
            BeginTileCallback beginTileCallback,
            EndTileCallback endTileCallback,
            ResolveRenderTargetCallback resolveRenderTargetCallback);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DrawBatchCallback(ref NoesisBatch batch);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr MapVerticesCallback(uint size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnmapVerticesCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr MapIndicesCallback(uint size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UnmapIndicesCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void BeginRenderCallback(bool offscreen);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EndRenderCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CreateTextureCallback(IntPtr pTexture, string label, uint width, uint height, uint numLevels, NoesisTextureFormat format, IntPtr[] data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void UpdateTextureCallback(IntPtr pTexture, uint level, uint x, uint y, uint width, uint height, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CreateRenderTargetCallback(IntPtr pSurface, IntPtr pSurfaceTexture, string label, uint width, uint height, uint sampleCount);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void CloneRenderTargetCallback(IntPtr pClonedSurface, IntPtr pClonedSurfaceTexture, string label, IntPtr pSurface);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SetRenderTargetCallback(IntPtr pSurface);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void BeginTileCallback(ref NoesisTile tile, uint surfaceWidth, uint surfaceHeight);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void EndTileCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ResolveRenderTargetCallback(IntPtr pSurface, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] NoesisTile[] tiles, uint numTiles);

        private static DrawBatchCallback drawBatchCallback;
        private static MapVerticesCallback mapVerticesCallback;
        private static UnmapVerticesCallback unmapVerticesCallback;
        private static MapIndicesCallback mapIndicesCallback;
        private static UnmapIndicesCallback unmapIndicesCallback;
        private static BeginRenderCallback beginRenderCallback;
        private static EndRenderCallback endRenderCallback;
        private static CreateTextureCallback createTextureCallback;
        private static UpdateTextureCallback updateTextureCallback;
        private static CreateRenderTargetCallback createRenderTargetCallback;
        private static CloneRenderTargetCallback cloneRenderTargetCallback;
        private static SetRenderTargetCallback setRenderTargetCallback;
        private static BeginTileCallback beginTileCallback;
        private static EndTileCallback endTileCallback;
        private static ResolveRenderTargetCallback resolveRenderTargetCallback;

        protected const uint DYNAMIC_VB_SIZE = 512 * 1024;
        protected const uint DYNAMIC_IB_SIZE = 128 * 1024;
        protected const uint DYNAMIC_TEX_SIZE = 128 * 1024;

        internal IntPtr NativePointer;

        private static ManagedRenderDevice instance;

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
            this.NativePointer = CreateManagedRenderDevice(deviceCaps, flippedTextures);
            RegisterCallbacks(this);
        }

        private static void RegisterCallbacks(ManagedRenderDevice renderDevice)
        {
            drawBatchCallback = renderDevice.DrawBatch;
            mapVerticesCallback = renderDevice.MapVertices;
            unmapVerticesCallback = renderDevice.UnmapVertices;
            mapIndicesCallback = renderDevice.MapIndices;
            unmapIndicesCallback = renderDevice.UnmapIndices;
            beginRenderCallback = renderDevice.BeginRender;
            endRenderCallback = renderDevice.EndRender;
            createTextureCallback = renderDevice.CreateTexture;
            updateTextureCallback = renderDevice.UpdateTexture;
            createRenderTargetCallback = renderDevice.CreateRenderTarget;
            cloneRenderTargetCallback = renderDevice.CloneRenderTarget;
            setRenderTargetCallback = renderDevice.SetRenderTarget;
            beginTileCallback = renderDevice.BeginTile;
            endTileCallback = renderDevice.EndTile;
            resolveRenderTargetCallback = renderDevice.ResolveRenderTarget;

            SetManagedRenderDeviceCallbacks(
                renderDevice.NativePointer,
                drawBatchCallback,
                mapVerticesCallback,
                unmapVerticesCallback,
                mapIndicesCallback,
                unmapIndicesCallback,
                beginRenderCallback,
                endRenderCallback,
                createTextureCallback,
                updateTextureCallback,
                createRenderTargetCallback,
                cloneRenderTargetCallback,
                setRenderTargetCallback,
                beginTileCallback,
                endTileCallback,
                resolveRenderTargetCallback);
        }

        protected abstract void DrawBatch(ref NoesisBatch batch);

        protected abstract IntPtr MapVertices(uint size);

        protected abstract void UnmapVertices();

        protected abstract IntPtr MapIndices(uint size);

        protected abstract void UnmapIndices();

        protected abstract void BeginRender(bool offscreen);

        protected abstract void EndRender();

        protected abstract ManagedTexture CreateTexture(string label, uint width, uint height, uint numLevels, NoesisTextureFormat format, IntPtr[] data);

        private void CreateTexture(IntPtr pTexture, string label, uint width, uint height, uint numLevels, NoesisTextureFormat format, IntPtr[] data)
        {
            var texture = this.CreateTexture(label, width, height, numLevels, format, data);
            texture.Register(pTexture);
        }

        private void UpdateTexture(IntPtr pTexture, uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
            var texture = ManagedTexture.GetTexture(pTexture);
            texture.UpdateTexture(level, x, y, width, height, data);
        }

        protected abstract ManagedRenderTarget CreateRenderTarget(string label, uint width, uint height, uint sampleCount);

        private void CreateRenderTarget(IntPtr pSurface, IntPtr pSurfaceTexture, string label, uint width, uint height, uint sampleCount)
        {
            var renderTarget = this.CreateRenderTarget(label, width, height, sampleCount);
            renderTarget.Register(pSurface, pSurfaceTexture);
        }

        protected abstract ManagedRenderTarget CloneRenderTarget(string label, ManagedRenderTarget surface);

        private void CloneRenderTarget(IntPtr pClonedSurface, IntPtr pClonedSurfaceTexture, string label, IntPtr pSurface)
        {
            var surface = ManagedRenderTarget.GetRenderTarget(pSurface);
            var clonedRenderTarget = this.CloneRenderTarget(label, surface);
            clonedRenderTarget.Register(pClonedSurface, pClonedSurfaceTexture);
        }

        protected abstract void SetRenderTarget(ManagedRenderTarget surface);

        private void SetRenderTarget(IntPtr pSurface)
        {
            var surface = ManagedRenderTarget.GetRenderTarget(pSurface);
            this.SetRenderTarget(surface);
        }

        protected abstract void BeginTile(ref NoesisTile tile, uint surfaceWidth, uint surfaceHeight);

        protected abstract void EndTile();

        protected abstract void ResolveRenderTarget(ManagedRenderTarget surface, NoesisTile[] tiles);

        private void ResolveRenderTarget(IntPtr pSurface, NoesisTile[] tiles, uint numTiles)
        {
            var surface = ManagedRenderTarget.GetRenderTarget(pSurface);
            this.ResolveRenderTarget(surface, tiles);
        }
    }
}

