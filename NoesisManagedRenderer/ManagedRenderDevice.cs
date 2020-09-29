using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    public abstract class ManagedRenderDevice
    {
        public const string LIB_NOESIS = "ManagedRendererNative";

        public delegate void SetDrawBatchCallbackDelegate(ref NoesisBatch batch);
        public delegate IntPtr SetMapVerticesCallbackDelegate(UInt32 size);
        public delegate void SetUnmapVerticesCallbackDelegate();
        public delegate IntPtr SetMapIndicesCallbackDelegate(UInt32 size);
        public delegate void SetUnmapIndicesCallbackDelegate();
        public delegate void SetBeginRenderCallbackDelegate();
        public delegate void SetEndRenderCallbackDelegate();
        public delegate void CreateTextureDelegate(IntPtr ptr, UInt32 width, UInt32 height, UInt32 numLevels, NoesisTextureFormat format);
        public delegate void UpdateTextureDelegate(IntPtr ptr, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data);

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

        public abstract void DrawBatch(ref NoesisBatch batch);
        public abstract IntPtr MapVertices(UInt32 size);
        public abstract void UnmapVertices();
        public abstract IntPtr MapIndices(UInt32 size);
        public abstract void UnmapIndices();
        public abstract void BeginRender();
        public abstract void EndRender();

        public abstract void CreateTexture(IntPtr ptr, UInt32 width, UInt32 height, UInt32 numLevels, NoesisTextureFormat format);

        public void UpdateTexture(IntPtr ptr, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data)
        {
            var texture = ManagedTexture.GetTexture(ptr);
            texture.UpdateTexture(level, x, y, width, height, data);
        }

        public static void SetMamanagedRenderDevice(ManagedRenderDevice renderDevice)
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

