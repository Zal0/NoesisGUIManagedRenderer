using System;
using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    public static class NoesisApp
    {
        public static void NoesisInit(ManagedRenderDevice renderDevice, string xamlString) => NoesisInit(renderDevice.cPtr, xamlString);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl, CharSet= CharSet.Ansi)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        private static extern void NoesisInit(IntPtr renderDevice, string xamlString);

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
        public static extern bool ViewMouseButtonDown(int x, int y, int button);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseButtonUp(int x, int y, int button);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseDoubleClick(int x, int y, int button);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseMove(int x, int y);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseWheel(int x, int y, int wheelRotation);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewMouseHWheel(int x, int y, int wheelRotation);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewScroll(float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewScrollPos(int x, int y, float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewHScroll(float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewHScrollPos(int x, int y, float value);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewTouchDown(int x, int y, ulong id);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewTouchMove(int x, int y, ulong id);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewTouchUp(int x, int y, ulong id);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewKeyDown(int key);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewKeyUp(int key);

        [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern bool ViewChar(uint ch);
    }
}
