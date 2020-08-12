using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


class ManagedRenderTest
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string filename);

    private const string LIB_NOESIS = "../../../../IntegrationGLUT/Projects/windows_x86/Win32/Debug/IntegrationGLUT.dll";

    [DllImport(LIB_NOESIS)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void NoesisInit();

    [DllImport(LIB_NOESIS)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void UpdateView();

    [DllImport(LIB_NOESIS)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void RenderView();

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void SetViewSize(int w, int h);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void ViewMouseMove(int x, int y);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void ViewMouseButtonDown(int x, int y);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void ViewMouseButtonUp(int x, int y);

    private int _windowHandle;
    private int w, h;

    public static void Main(string[] args)
    {
        ManagedRenderTest instance = new ManagedRenderTest();
        instance.Run(args);
    }

    public void Run(string[] _args)
    {
        //Manually load lib dependencies before calling any function
        IntPtr error;
        string NoesisPath = Environment.GetEnvironmentVariable("NOESIS_SDK_PATH");
        error = LoadLibrary(NoesisPath + @"\Bin\windows_x86\Noesis.dll");
        error = LoadLibrary(NoesisPath + @"\Bin\windows_x86\NoesisApp.dll");
        error = LoadLibrary(LIB_NOESIS);

        ManagedRenderDevice.SetMamanagedRenderDevice(new GLUTRenderer());

        GLUT.Init();
        GLUT.InitDisplayMode(GLUT.GLUT_RGB | GLUT.GLUT_DOUBLE | GLUT.GLUT_STENCIL);
        w = 1000;
        h = 600;
        GLUT.InitWindowSize(w, h);
        _windowHandle = GLUT.CreateWindow("NoesisGUI - Managed Renderer");

        NoesisInit();

        GLUT.DisplayFunc(DisplayFunc);
        GLUT.ReshapeFunc(ReshapeFunc);
        GLUT.PassiveMotionFunc(MouseMoveFunc);
        GLUT.MouseFunc(MouseFunc);

        GLUT.MainLoop();
    }

    private void DisplayFunc()
    {
        UpdateView();

        //GL.BindFramebuffer(GL.GL_FRAMEBUFFER, 0);
        GL.Viewport(0, 0, w, h);//todo: get w and h

        GL.ClearColor(0.0f, 0.0f, 0.25f, 0.0f);
        GL.ClearStencil(0);
        GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

        GL.PolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_LINE);

        RenderView();

        GLUT.SwapBuffers();
        GLUT.PostRedisplay();
    }

    private void ReshapeFunc(int w, int h)
    {
        this.w = w;
        this.h = h;
        SetViewSize(w, h);

        GL.MatrixMode(GL.GL_PROJECTION);
        GL.LoadIdentity();
        GL.Ortho(0.0f, w, 0.0f, h, 0.0f, 1.0f);
        GL.MatrixMode(GL.GL_MODELVIEW);
    }

    private void MouseFunc(int button, int state, int x, int y)
    {
        if(button == GLUT.GLUT_LEFT_BUTTON)
        {
            if(state == GLUT.GLUT_DOWN)
            {
                ViewMouseButtonDown(x, y);
            }
            else
            {
                ViewMouseButtonUp(x, y);
            }
        }
    }

    private void MouseMoveFunc(int x, int y)
    {
        ViewMouseMove(x, y);
    }
}

