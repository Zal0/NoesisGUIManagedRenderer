using System.Runtime.InteropServices;

public static class GLU
{
    private const string LIB_GLU = "../../libs/glu32.dll"; // "/usr/lib64/libGLU.so"

    [DllImport(LIB_GLU, EntryPoint = "gluOrtho2D", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Ortho2D(double left, double right, double bottom, double top);
}