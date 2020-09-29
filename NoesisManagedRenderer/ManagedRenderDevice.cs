using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    public abstract class ManagedRenderDevice
    {
        public const string LIB_NOESIS = "ManagedRendererNative";

        private delegate void SetDrawBatchCallbackDelegate(ref NoesisBatch batch);
        private delegate IntPtr SetMapVerticesCallbackDelegate(UInt32 size);
        private delegate void SetUnmapVerticesCallbackDelegate();
        private delegate IntPtr SetMapIndicesCallbackDelegate(UInt32 size);
        private delegate void SetUnmapIndicesCallbackDelegate();
        private delegate void SetBeginRenderCallbackDelegate();
        private delegate void SetEndRenderCallbackDelegate();
        private delegate bool CreateTextureDelegate(IntPtr ptr, ref CreateTextureParams args);
        private delegate void UpdateTextureDelegate(IntPtr ptr, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data);

        protected struct CreateTextureParams
        {
            public UInt32 width;
            public UInt32 height;
            public UInt32 numLevels;
            public NoesisTextureFormat format;
        }

        internal IntPtr cPtr;

        static ManagedRenderDevice()
        {
            //Manually load lib dependencies before calling any function
            IntPtr error;
            string NoesisPath = Environment.GetEnvironmentVariable("NOESIS_SDK_PATH");
            error = LoadLibrary(NoesisPath + @"\Bin\windows_x86\Noesis.dll");
            error = LoadLibrary(NoesisPath + @"\Bin\windows_x86\NoesisApp.dll");
            error = LoadLibrary(ManagedRenderDevice.LIB_NOESIS);
        }

        public ManagedRenderDevice()
        {
            this.cPtr = CreateManagedRenderDevice();
            SetMamanagedRenderDevice(this);
        }

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateManagedRenderDevice();

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetDrawBatchCallback(IntPtr pDevice, SetDrawBatchCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetMapVerticesCallback(IntPtr pDevice, SetMapVerticesCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetUnmapVerticesCallback(IntPtr pDevice, SetUnmapVerticesCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetMapIndicesCallback(IntPtr pDevice, SetMapIndicesCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetUnmapIndicesCallback(IntPtr pDevice, SetUnmapIndicesCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetBeginRenderCallback(IntPtr pDevice, SetBeginRenderCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetEndRenderCallback(IntPtr pDevice, SetEndRenderCallbackDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetCreateTextureCallback(IntPtr pDevice, CreateTextureDelegate callback);

        [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetUpdateTextureCallback(IntPtr pDevice, UpdateTextureDelegate callback);

        protected abstract void DrawBatch(ref NoesisBatch batch);
        protected abstract IntPtr MapVertices(UInt32 size);
        protected abstract void UnmapVertices();
        protected abstract IntPtr MapIndices(UInt32 size);
        protected abstract void UnmapIndices();
        protected abstract void BeginRender();
        protected abstract void EndRender();
        protected abstract bool CreateTexture(IntPtr ptr, ref CreateTextureParams args);

        private void UpdateTexture(IntPtr ptr, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data)
        {
            var texture = ManagedTexture.GetTexture(ptr);
            texture.UpdateTexture(level, x, y, width, height, data);
        }

        private static void SetMamanagedRenderDevice(ManagedRenderDevice renderDevice)
        {
            var cPtr = renderDevice.cPtr;
            SetDrawBatchCallback(cPtr, renderDevice.DrawBatch);
            SetMapVerticesCallback(cPtr, renderDevice.MapVertices);
            SetUnmapVerticesCallback(cPtr, renderDevice.UnmapVertices);
            SetMapIndicesCallback(cPtr, renderDevice.MapIndices);
            SetUnmapIndicesCallback(cPtr, renderDevice.UnmapIndices);
            SetBeginRenderCallback(cPtr, renderDevice.BeginRender);
            SetEndRenderCallback(cPtr, renderDevice.EndRender);
            SetCreateTextureCallback(cPtr, renderDevice.CreateTexture);
            SetUpdateTextureCallback(cPtr, renderDevice.UpdateTexture);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string filename);
    }
}

