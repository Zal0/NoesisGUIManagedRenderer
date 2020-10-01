using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    public static class NoesisApp
    {
        public static IntPtr CreateView(ManagedRenderDevice renderDevice, string xamlString) => CreateView(renderDevice.NativePointer, xamlString);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl, CharSet= CharSet.Ansi)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void NoesisInit();

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern IntPtr CreateView(IntPtr pDevice, string xamlString);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void DeleteView(IntPtr pView);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void UpdateView(IntPtr pView, float t);

        [DllImport(ManagedRenderDevice.LIB_NOESIS)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void RenderView(IntPtr pView);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern void SetViewSize(IntPtr pView, int w, int h);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseButtonDown(IntPtr pView, int x, int y, int button);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseButtonUp(IntPtr pView, int x, int y, int button);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseDoubleClick(IntPtr pView, int x, int y, int button);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseMove(IntPtr pView, int x, int y);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseWheel(IntPtr pView, int x, int y, int wheelRotation);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseHWheel(IntPtr pView, int x, int y, int wheelRotation);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewScroll(IntPtr pView, float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewScrollPos(IntPtr pView, int x, int y, float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewHScroll(IntPtr pView, float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewHScrollPos(IntPtr pView, int x, int y, float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewTouchDown(IntPtr pView, int x, int y, ulong id);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewTouchMove(IntPtr pView, int x, int y, ulong id);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewTouchUp(IntPtr pView, int x, int y, ulong id);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewKeyDown(IntPtr pView, int key);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewKeyUp(IntPtr pView, int key);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewChar(IntPtr pView, uint ch);
    }
}
