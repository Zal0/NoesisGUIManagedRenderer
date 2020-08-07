using System;
using System.Runtime.InteropServices;
using System.Text;

public static class GLUT
{
    private const string LIB_GLUT = "../../../libs/glut32.dll"; // "/usr/lib64/libglut.so.3"

    public const int GLUT_RGB = 0;
    public const int GLUT_RGBA = GLUT_RGB;

    public const int GLUT_LEFT_BUTTON = 0;
    public const int GLUT_MIDDLE_BUTTON = 1;
    public const int GLUT_RIGHT_BUTTON = 2;

    public const int GLUT_DOWN = 0;
    public const int GLUT_UP = 1;

    public const int GLUT_ACTIVE_SHIFT = 1;
    public const int GLUT_ACTIVE_CTRL = 2;
    public const int GLUT_ACTIVE_ALT = 4;

    #region Initialization and cleanup

    [DllImport(LIB_GLUT, EntryPoint = "glutInit", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void glutInit(ref int argcp, string[] argv);

    public static void Init()
    {
        string[] args = Environment.GetCommandLineArgs();
        int argc = args.Length;
        glutInit(ref argc, args);
    }

    [DllImport(LIB_GLUT, EntryPoint = "glutInitDisplayMode", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void InitDisplayMode(int argcp);

    [DllImport(LIB_GLUT, EntryPoint = "glutInitWindowPosition",
                         CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void InitWindowPosition(int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutInitWindowSize", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void InitWindowSize(int widt, int height);

    #endregion Initialization and cleanup

    #region Window handling and main loop

    [DllImport(LIB_GLUT, EntryPoint = "glutMainLoop", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void MainLoop();

    [DllImport(LIB_GLUT, EntryPoint = "glutCreateWindow", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern int CreateWindow(string title);

    [DllImport(LIB_GLUT, EntryPoint = "glutDestroyWindow", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void DestroyWindow(int win);

    #endregion Window handling and main loop

    #region Callbacks (keyboard)

    // void glutKeyboardFunc(void (* callback)(unsigned char key, int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void KeyboardProcDelegate(byte key, int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutKeyboardFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void KeyboardFunc(KeyboardProcDelegate keyboardProc);

    // void glutKeyboardUpFunc(void (* callback)(unsigned char key, int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void KeyboardUpProcDelegate(byte key, int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutKeyboardUpFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void KeyboardUpFunc(KeyboardUpProcDelegate keyboardUpProc);

    // void glutSpecialFunc(void (* callback)(int key, int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void SpecialProcDelegate(int key, int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutSpecialFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void SpecialFunc(SpecialProcDelegate specialProc);

    // void glutSpecialUpFunc(void (* callback)(int key, int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void SpecialUpProcDelegate(int key, int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutSpecialUpFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void SpecialUpFunc(SpecialUpProcDelegate specialUpProc);

    #endregion Callbacks (keyboard)

    #region Callbacks

    // void glutDisplayFunc(void (* callback)(void));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void DisplayProcDelegate();

    [DllImport(LIB_GLUT, EntryPoint = "glutDisplayFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void DisplayFunc(DisplayProcDelegate displayProc);

    // void glutMouseFunc(void (* callback)(int button, int state, int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void MouseProcDelegate(int button, int state, int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutMouseFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void MouseFunc(MouseProcDelegate mouseProc);

    // void glutMotionFunc(void (* callback)(int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void MotionProcDelegate(int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutMotionFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void MotionFunc(MotionProcDelegate motionProc);

    // void glutPassiveMotionFunc(void (* callback)(int x, int y));
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void PassiveMotionProcDelegate(int x, int y);

    [DllImport(LIB_GLUT, EntryPoint = "glutPassiveMotionFunc", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern void PassiveMotionFunc(PassiveMotionProcDelegate passiveMotionProc);

    #endregion Callbacks

    [DllImport(LIB_GLUT, EntryPoint = "glutGetModifiers", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    public static extern int GetModifiers();
}