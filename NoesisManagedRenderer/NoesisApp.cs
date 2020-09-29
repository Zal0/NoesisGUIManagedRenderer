using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    public static class NoesisApp
    {
        public static void NoesisInit(ManagedRenderDevice renderDevice) => NoesisInit(renderDevice.cPtr);

        [DllImport(ManagedRenderDevice.LIB_NOESIS)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern void NoesisInit(IntPtr renderDevice);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void UpdateView(float t);

        [DllImport(ManagedRenderDevice.LIB_NOESIS)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void RenderView();

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void SetViewSize(int w, int h);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void ViewMouseMove(int x, int y);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void ViewMouseButtonDown(int x, int y);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void ViewMouseButtonUp(int x, int y);
    }
}
