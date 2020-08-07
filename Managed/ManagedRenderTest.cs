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

    private const string LIB_NOESIS = "../../../../IntegrationGLUT/Projects/windows_x86_64/x64/Debug/IntegrationGLUT.dll";

    [DllImport(LIB_NOESIS, EntryPoint = "main", CallingConvention = CallingConvention.Winapi)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void main(int argcp, string[] argv);

    public static void Main(string[] _args)
    {
        //Manually load lib dependencies before calling any function
        string NoesisPath = Environment.GetEnvironmentVariable("NOESIS_SDK_PATH");
        LoadLibrary(NoesisPath + @"\Bin\windows_x86_64\Noesis.dll");
        LoadLibrary(NoesisPath + @"\Bin\windows_x86_64\NoesisApp.dll");

        string[] args = Environment.GetCommandLineArgs();
        int argc = args.Length;
        main(argc, args);
    }
}

