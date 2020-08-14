using System;
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

    public const int GL_TEXTURE_2D = 0x0DE1;

    public const int GL_ALPHA = 0x1906;
    public const int GL_RGB = 0x1907;
    public const int GL_RGBA = 0x1908;
    
    public const int GL_R8 = 0x8229;
    public const int GL_RED = 0x1903;

    public const int GL_BYTE = 0x1400;
    public const int GL_UNSIGNED_BYTE = 0x1401;
    public const int GL_SHORT = 0x1402;
    public const int GL_UNSIGNED_SHORT = 0x1403;
    public const int GL_FLOAT = 0x1406;
    public const int GL_FIXED = 0x140C;

    public const int GL_TEXTURE_ENV = 0x2300;
    public const int GL_TEXTURE_ENV_MODE = 0x2200;

    public const int GL_MODULATE = 0x2100;
    public const int GL_DECAL = 0x2101;
    public const int GL_REPLACE = 0x1E01;

    public const int GL_TEXTURE_MAG_FILTER = 0x2800;
    public const int GL_TEXTURE_MIN_FILTER = 0x2801;
    public const int GL_TEXTURE_WRAP_S = 0x2802;
    public const int GL_TEXTURE_WRAP_T = 0x2803;
    
    public const int GL_REPEAT = 0x2901;
    public const int GL_CLAMP_TO_EDGE = 0x812F;
    
    public const int GL_NEAREST = 0x2600;
    public const int GL_LINEAR = 0x2601;

    public const int GL_BLEND = 0x0BE2;

    public const int GL_ZERO                = 0;
    public const int GL_ONE                 = 1;
    public const int GL_SRC_COLOR           = 0x0300;
    public const int GL_ONE_MINUS_SRC_COLOR = 0x0301;
    public const int GL_SRC_ALPHA           = 0x0302;
    public const int GL_ONE_MINUS_SRC_ALPHA = 0x0303;
    public const int GL_DST_ALPHA           = 0x0304;
    public const int GL_ONE_MINUS_DST_ALPHA = 0x0305;
    public const int GL_DST_COLOR           = 0x0306;
    public const int GL_ONE_MINUS_DST_COLOR = 0x0307;
    public const int GL_SRC_ALPHA_SATURATE  = 0x0308;

    public const int GL_UNPACK_ALIGNMENT = 0x0CF5;
    public const int GL_PACK_ALIGNMENT = 0x0D05;


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

    [DllImport(LIB_GL, EntryPoint = "glColor4ub", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Color4ub(byte r, byte g, byte b, byte a);

    [DllImport(LIB_GL, EntryPoint = "glTexCoord2f", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void TexCoord2f(float u, float v);

    [DllImport(LIB_GL, EntryPoint = "glGenTextures", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void GenTextures(int n, IntPtr ids);

    [DllImport(LIB_GL, EntryPoint = "glBindTexture", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void BindTexture(int target, int texture);

    [DllImport(LIB_GL, EntryPoint = "glTexImage2D", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void TexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, IntPtr pixels);

    [DllImport(LIB_GL, EntryPoint = "glTexSubImage2D", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void TexSubImage2D(int target, int level, int x, int y, int width, int height, int format, int type, IntPtr pixels);

    [DllImport(LIB_GL, EntryPoint = "glEnable", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Enable(int cap);

    [DllImport(LIB_GL, EntryPoint = "glDisable", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void Disable(int cap);

    [DllImport(LIB_GL, EntryPoint = "glTexEnvi", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void TexEnvi(int target, int pname, int param);

    [DllImport(LIB_GL, EntryPoint = "glTexParameteri", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void TexParameteri(int target, int pname, int param);

    [DllImport(LIB_GL, EntryPoint = "glBlendFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void BlendFunc(int sfactor, int dfactor);


    [DllImport(LIB_GL, EntryPoint = "glPixelStorei", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void PixelStorei(int pname, int param);
}