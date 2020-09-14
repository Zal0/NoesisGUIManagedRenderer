using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public abstract class ManagedRenderDevice
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Shader
    {
        // List of shaders to be implemented by the device with expected vertex format
        //
        //  Name       Format                   Size (bytes)      Semantic
        //  ---------------------------------------------------------------------------------
        //  Pos        R32G32_FLOAT             8                 Position (x,y)
        //  Color      R8G8B8A8_UNORM           4                 Color (rgba)
        //  Tex0       R32G32_FLOAT             8                 Texture (u,v)
        //  Tex1       R32G32_FLOAT             8                 Texture (u,v)
        //  Tex2       R16G16B16A16_UNORM       8                 Rect (x0,y0, x1,y1)
        //  Coverage   R32_FLOAT                4                 Coverage (x)
        //
        public enum Enum
        {
            RGBA,                       // Pos
            Mask,                       // Pos

            Path_Solid,                 // Pos | Color
            Path_Linear,                // Pos | Tex0
            Path_Radial,                // Pos | Tex0
            Path_Pattern,               // Pos | Tex0

            PathAA_Solid,               // Pos | Color | Coverage
            PathAA_Linear,              // Pos | Tex0  | Coverage
            PathAA_Radial,              // Pos | Tex0  | Coverage
            PathAA_Pattern,             // Pos | Tex0  | Coverage

            SDF_Solid,                  // Pos | Color | Tex1
            SDF_Linear,                 // Pos | Tex0  | Tex1
            SDF_Radial,                 // Pos | Tex0  | Tex1
            SDF_Pattern,                // Pos | Tex0  | Tex1

            SDF_LCD_Solid,              // Pos | Color | Tex1
            SDF_LCD_Linear,             // Pos | Tex0  | Tex1
            SDF_LCD_Radial,             // Pos | Tex0  | Tex1
            SDF_LCD_Pattern,            // Pos | Tex0  | Tex1

            Image_Opacity_Solid,        // Pos | Color | Tex1
            Image_Opacity_Linear,       // Pos | Tex0  | Tex1
            Image_Opacity_Radial,       // Pos | Tex0  | Tex1
            Image_Opacity_Pattern,      // Pos | Tex0  | Tex1

            Image_Shadow35V,            // Pos | Color | Tex1 | Tex2
            Image_Shadow63V,            // Pos | Color | Tex1 | Tex2
            Image_Shadow127V,           // Pos | Color | Tex1 | Tex2

            Image_Shadow35H_Solid,      // Pos | Color | Tex1 | Tex2
            Image_Shadow35H_Linear,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow35H_Radial,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow35H_Pattern,    // Pos | Tex0  | Tex1 | Tex2

            Image_Shadow63H_Solid,      // Pos | Color | Tex1 | Tex2
            Image_Shadow63H_Linear,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow63H_Radial,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow63H_Pattern,    // Pos | Tex0  | Tex1 | Tex2

            Image_Shadow127H_Solid,     // Pos | Color | Tex1 | Tex2
            Image_Shadow127H_Linear,    // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow127H_Radial,    // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow127H_Pattern,   // Pos | Tex0  | Tex1 | Tex2

            Image_Blur35V,              // Pos | Color | Tex1 | Tex2
            Image_Blur63V,              // Pos | Color | Tex1 | Tex2
            Image_Blur127V,             // Pos | Color | Tex1 | Tex2

            Image_Blur35H_Solid,        // Pos | Color | Tex1 | Tex2
            Image_Blur35H_Linear,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur35H_Radial,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur35H_Pattern,      // Pos | Tex0  | Tex1 | Tex2

            Image_Blur63H_Solid,        // Pos | Color | Tex1 | Tex2
            Image_Blur63H_Linear,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur63H_Radial,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur63H_Pattern,      // Pos | Tex0  | Tex1 | Tex2

            Image_Blur127H_Solid,       // Pos | Color | Tex1 | Tex2
            Image_Blur127H_Linear,      // Pos | Tex0  | Tex1 | Tex2
            Image_Blur127H_Radial,      // Pos | Tex0  | Tex1 | Tex2
            Image_Blur127H_Pattern,     // Pos | Tex0  | Tex1 | Tex2

            Count
        };

        byte v;
    }

    // Render batch information
    [StructLayout(LayoutKind.Explicit, Size = 184)]
    public struct Batch
    {
        // Render state
        [FieldOffset(0)] public byte shader;
        [FieldOffset(1)] public byte renderState;
        [FieldOffset(2)] public byte stencilRef;

        // Draw parameters
        [FieldOffset(4)] public UInt32 vertexOffset;
        [FieldOffset(8)] public UInt32 numVertices;
        [FieldOffset(12)] public UInt32 startIndex;
        [FieldOffset(16)] public UInt32 numIndices;

        // Textures (Unused textures are set to null)
        [FieldOffset(20)] public IntPtr pattern;
        [FieldOffset(24)] public byte patternSampler;

        [FieldOffset(28)] public IntPtr ramps;
        [FieldOffset(32)] public byte rampsSampler;

        [FieldOffset(36)] public IntPtr image;
        [FieldOffset(40)] public byte imageSampler;

        [FieldOffset(44)] public IntPtr glyphs;
        [FieldOffset(48)] public byte glyphsSampler;

        [FieldOffset(52)] public IntPtr shadow;
        [FieldOffset(56)] public byte shadowSampler;

        // Effect parameters
        /*IntPtr effectParams;
        UInt32 effectParamsSize;
        UInt32 effectParamsHash;

        // Shader constants (Unused constants are set to null)
        fixed IntPtr projMtx[16];
        UInt32 projMtxHash;

        const float* opacity;
        UInt32 opacityHash;

        const float (*rgba)[4];
        UInt32 rgbaHash;

        const float (*radialGrad)[8];
        UInt32 radialGradHash;*/
    };

    public const string LIB_NOESIS = "../../../../../ManagedRendererNative/Projects/windows_x86/Win32/Debug/ManagedRendererNative.dll";

    [DllImport(ManagedRenderDevice.LIB_NOESIS)]
    [System.Security.SuppressUnmanagedCodeSecurity()]
    private static extern void NoesisInit();

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

    public delegate void SetDrawBatchCallbackDelegate(ref Batch batch);
    public delegate IntPtr SetMapVerticesCallbackDelegate(UInt32 size);
    public delegate void SetUnmapVerticesCallbackDelegate();
    public delegate IntPtr SetMapIndicesCallbackDelegate(UInt32 size);
    public delegate void SetUnmapIndicesCallbackDelegate();
    public delegate void SetBeginRenderCallbackDelegate();
    public delegate void SetEndRenderCallbackDelegate();

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetDrawBatchCallback(SetDrawBatchCallbackDelegate callback);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetMapVerticesCallback(SetMapVerticesCallbackDelegate callback);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetUnmapVerticesCallback(SetUnmapVerticesCallbackDelegate callback);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetMapIndicesCallback(SetMapIndicesCallbackDelegate callback);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetUnmapIndicesCallback(SetUnmapIndicesCallbackDelegate callback);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetBeginRenderCallback(SetBeginRenderCallbackDelegate callback);

    [DllImport(LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetEndRenderCallback(SetEndRenderCallbackDelegate callback);

    public delegate int CreateTextureDelegate(UInt32 width, UInt32 height, UInt32 numLevels, byte format, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)][In, Out] IntPtr[] data);
    public delegate int GetWidthDelegate(int id);
    public delegate int GetHeightDelegate(int id);
    public delegate bool HasMipMapsDelegate(int id);
    public delegate bool IsInvertedDelegate(int id);
    public delegate void UpdateTextureDelegate(IntPtr texture, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetCreateTextureCallback(CreateTextureDelegate callback);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetGetWidthCallback(GetWidthDelegate callback);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetGetHeightCallback(GetHeightDelegate callback);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetHasMipMapsCallback(HasMipMapsDelegate callback);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetIsInvertedCallback(IsInvertedDelegate callback);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SetUpdateTextureCallback(UpdateTextureDelegate callback);

    [DllImport(ManagedRenderDevice.LIB_NOESIS, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetTextureId(IntPtr texture);

    public abstract void DrawBatch(ref Batch batch);
    public abstract IntPtr MapVertices(UInt32 size);
    public abstract void UnmapVertices();
    public abstract IntPtr MapIndices(UInt32 size);
    public abstract void UnmapIndices();
    public abstract void BeginRender();
    public abstract void EndRender();

    //Texture stuff
    public static Dictionary<int, ManagedTexture> textures = new Dictionary<int, ManagedTexture>();
    public static int nextId = 0;

    protected int RegisterTexture(ManagedTexture texture)
    {
        int id = nextId++;
        textures[id] = texture;
        return id;
    }

    public int GetWidth(int id)
    {
        return textures[id].GetWidth();
    }

    public int GetHeight(int id)
    {
        return textures[id].GetHeight();
    }

    public bool HasMipMaps(int id)
    {
        return textures[id].HasMipMaps();
    }

    public bool IsInverted(int id)
    {
        return textures[id].IsInverted();
    }

    public void UpdateTexture(IntPtr texture, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data)
    {
        ManagedTexture t = textures[GetTextureId(texture)];
        t.UpdateTexture(level, x, y, width, height, data);
    }

    public int CreateTexture(UInt32 width, UInt32 height, UInt32 numLevels, byte format, IntPtr[] data)
    {
        ManagedTexture texture = CreateTexture();
        int id = RegisterTexture(texture);
        texture.Init(this, width, height, numLevels, format, data);

        return id;
    }
    public abstract ManagedTexture CreateTexture();

    public static void SetMamanagedRenderDevice(ManagedRenderDevice renderDevice)
    {
        SetDrawBatchCallback(renderDevice.DrawBatch);
        SetMapVerticesCallback(renderDevice.MapVertices);
        SetUnmapVerticesCallback(renderDevice.UnmapVertices);
        SetMapIndicesCallback(renderDevice.MapIndices);
        SetUnmapIndicesCallback(renderDevice.UnmapIndices);
        SetBeginRenderCallback(renderDevice.BeginRender);
        SetEndRenderCallback(renderDevice.EndRender);

        SetCreateTextureCallback(renderDevice.CreateTexture);
        SetGetWidthCallback(renderDevice.GetWidth);
        SetGetHeightCallback(renderDevice.GetHeight);
        SetHasMipMapsCallback(renderDevice.HasMipMaps);
        SetIsInvertedCallback(renderDevice.IsInverted);
        SetUpdateTextureCallback(renderDevice.UpdateTexture);
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string filename);

    public static void Init(ManagedRenderDevice renderer)
    {
        //Manually load lib dependencies before calling any function
        IntPtr error;
        string NoesisPath = Environment.GetEnvironmentVariable("NOESIS_SDK_PATH");
        error = LoadLibrary(NoesisPath + @"\Bin\windows_x86\Noesis.dll");
        error = LoadLibrary(NoesisPath + @"\Bin\windows_x86\NoesisApp.dll");
        error = LoadLibrary(ManagedRenderDevice.LIB_NOESIS);

        ManagedRenderDevice.SetMamanagedRenderDevice(renderer);
        ManagedRenderDevice.NoesisInit();
    }
}

