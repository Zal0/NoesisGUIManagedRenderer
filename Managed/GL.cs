using System.Runtime.InteropServices;

public static class GL
{
    private const string LIB_GL = "opengl32.dll"; // "/usr/lib64/libGL.so"

    public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;
    public const uint GL_STENCIL_BUFFER_BIT = 0x00000400;
    public const uint GL_COLOR_BUFFER_BIT = 0x00004000;

    public const int GL_POINTS = 0x0000;
    public const int GL_LINES = 0x0001;
    public const int GL_LINE_LOOP = 0x0002;
    public const int GL_LINE_STRIP = 0x0003;
    public const int GL_TRIANGLES = 0x0004;
    public const int GL_TRIANGLE_STRIP = 0x0005;
    public const int GL_TRIANGLE_FAN = 0x0006;
    public const int GL_QUADS = 0x0007;
    public const int GL_QUAD_STRIP = 0x0008;
    public const int GL_POLYGON = 0x0009;

    public const int GL_FRAMEBUFFER = 0x8D40;

    public const int GL_MODELVIEW = 0x1700;
    public const int GL_PROJECTION = 0x1701;

    public const int GL_FRONT = 0x0404;
    public const int GL_BACK = 0x0405;
    public const int GL_FRONT_AND_BACK = 0x0408;

    public const int GL_POINT = 0x1B00;
    public const int GL_LINE = 0x1B01;
    public const int GL_FILL = 0x1B02;

    [DllImport(LIB_GL, EntryPoint = "glClearColor", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void ClearColor(float red, float green, float blue, float alpha);

    [DllImport(LIB_GL, EntryPoint = "glClear", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Clear(uint mask);

    [DllImport(LIB_GL, EntryPoint = "glColor3f", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Color3f(float red, float green, float blue);

    [DllImport(LIB_GL, EntryPoint = "glBegin", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Begin(int mode);

    [DllImport(LIB_GL, EntryPoint = "glVertex2i", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Vertex2i(int x, int y);

    [DllImport(LIB_GL, EntryPoint = "glVertex2f", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Vertex2f(float x, float y);

    [DllImport(LIB_GL, EntryPoint = "glEnd", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void End();

    [DllImport(LIB_GL, EntryPoint = "glFlush", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Flush();

    [DllImport(LIB_GL, EntryPoint = "glBindFramebuffer", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void BindFramebuffer(int target, int buffer);

    [DllImport(LIB_GL, EntryPoint = "glViewport", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Viewport(int x, int y, int w, int h);

    [DllImport(LIB_GL, EntryPoint = "glClearStencil", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void ClearStencil(int s);

    [DllImport(LIB_GL, EntryPoint = "glMatrixMode", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void MatrixMode(int mode);

    [DllImport(LIB_GL, EntryPoint = "glLoadIdentity", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void LoadIdentity();

    [DllImport(LIB_GL, EntryPoint = "glOrtho", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Ortho(double l, double r, double b, double t, double n, double f);

    [DllImport(LIB_GL, EntryPoint = "glPolygonMode", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void PolygonMode(int face, int mode);
}