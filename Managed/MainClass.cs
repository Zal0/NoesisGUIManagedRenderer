using System;
using System.Runtime.InteropServices;
/// <summary>
/// The <see cref="MainClass"/> provides the application instance to test a X11 native window,
/// created with GLUT.
/// </summary>
class MainClass
{
    private const string LIB_NOESIS = "../../../../IntegrationGLUT/Projects/windows_x86_64/x64/Debug/IntegrationGLUT.dll";

    [DllImport(LIB_NOESIS, EntryPoint = "main", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void main(int argcp, string[] argv);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string filename);

    /// <summary>The X11 native window handle, used by GLUT.</summary>
    private int _windowHandle;

    /// <summary>
    /// The entry point of the program, where the program control starts and ends.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static void Main(string[] _args)
    {
        //MainClass instance = new MainClass();
        //instance.Run(args);

        //Manually load lib dependencies before calling any function
        string NoesisPath = Environment.GetEnvironmentVariable("NOESIS_SDK_PATH");
        LoadLibrary(NoesisPath + @"\Bin\windows_x86_64\Noesis.dll");
        LoadLibrary(NoesisPath + @"\Bin\windows_x86_64\NoesisApp.dll");

        string[] args = Environment.GetCommandLineArgs();
        int argc = args.Length;
        main(argc, args);
    }

    /*/// <summary>
    /// Run the program instance with the specified command-line args.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    private void Run(string[] args)
    {
        GLUT.Init();
        GLUT.InitDisplayMode(GLUT.GLUT_RGB);
        GLUT.InitWindowSize(400, 500);
        _windowHandle = GLUT.CreateWindow("Hallo");

        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        GLU.Ortho2D(0, 400, 0, 500);

        GLUT.DisplayFunc(new GLUT.DisplayProcDelegate(DisplayProc));
        GLUT.KeyboardFunc(new GLUT.KeyboardProcDelegate(KeyDownProc));
        GLUT.KeyboardUpFunc(new GLUT.KeyboardUpProcDelegate(KeyUpProc));
        GLUT.MouseFunc(new GLUT.MouseProcDelegate(MouseProcDelegate));

        GLUT.MainLoop();
    }

    /// <summary>
    /// The callback, called from GLUT on diaplay refresh request.
    /// </summary>
    private void DisplayProc()
    {
        GL.Clear(GL.GL_COLOR_BUFFER_BIT);
        GL.Color3f(1.0f, 1.0f, 1.0f);

        GL.Begin(GL.GL_POLYGON);
        GL.Vertex2i(200, 125);
        GL.Vertex2i(100, 375);
        GL.Vertex2i(300, 375);
        GL.End();
        GL.Flush();
    }

    /// <summary>
    /// The callback, called from GLUT on keyboard key down request.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private void KeyDownProc(byte key, int x, int y)
    {
        // int modifier = GLUT.GetModifiers();
        if (key == 'q')
            GLUT.DestroyWindow(_windowHandle);
    }

    /// <summary>
    /// The callback, called from GLUT on keyboard key up request.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private void KeyUpProc(byte key, int x, int y)
    {
        // int modifier = GLUT.GetModifiers();
        if (key == 'q')
            GLUT.DestroyWindow(_windowHandle);
    }

    /// <summary>
    /// The callback, called from GLUT on mouse button pressed or released request.
    /// </summary>
    /// <param name="button">The pressed mouse button.</param>
    /// <param name="state">The button state.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private void MouseProcDelegate(int button, int state, int x, int y)
    {
        if ((button == GLUT.GLUT_LEFT_BUTTON) && (state == GLUT.GLUT_DOWN))
        {
            ;
        }

        if ((button == GLUT.GLUT_LEFT_BUTTON) && (state == GLUT.GLUT_UP))
        {
            ;
        }
    }*/
}