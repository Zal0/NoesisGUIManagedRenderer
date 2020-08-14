using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public abstract class ManagedTexture
{
    public enum Format
    {
        RGBA8,
        R8,

        Count
    };

    public static Dictionary<int, ManagedTexture > textures = new Dictionary<int, ManagedTexture>();
    public static int nextId = 0;

    protected static int RegisterTexture(ManagedTexture texture)
    {
        int id = nextId ++;
        textures[id] = texture;
        return id;
    }

    public static int GetWidth(int id)
    {
        return textures[id].GetWidth();
    }

    public static int GetHeight(int id)
    {
        return textures[id].GetHeight();
    }

    public static bool HasMipMaps(int id)
    {
        return textures[id].HasMipMaps();
    }

    public static bool IsInverted(int id)
    {
        return textures[id].IsInverted();
    }

    public static void UpdateTexture(IntPtr texture, UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data)
    {
        ManagedTexture t = textures[GetTextureId(texture)];
        t.UpdateTexture(level, x, y, width, height, data);
    }

    public static int CreateTexture<T>(UInt32 width, UInt32 height, UInt32 numLevels, byte format, IntPtr[] data) where T : ManagedTexture, new()
    {
        T ret = new T();
        ret.Init(width, height, numLevels, format, data);
        return ret.id;
    }

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

    public static void SetMamanagedTexture<T>() where T : ManagedTexture, new()
    {
        SetCreateTextureCallback(CreateTexture<T>);
        SetGetWidthCallback(GetWidth);
        SetGetHeightCallback(GetHeight);
        SetHasMipMapsCallback(HasMipMaps);
        SetIsInvertedCallback(IsInverted);
        SetUpdateTextureCallback(UpdateTexture);
    }

    int id;
    public ManagedTexture()
    {
        id = RegisterTexture(this);
    }

    public abstract void Init(UInt32 width, UInt32 height, UInt32 numLevels, byte format, IntPtr[] data);
    public abstract int GetWidth();
    public abstract int GetHeight();
    public abstract bool HasMipMaps();
    public abstract bool IsInverted();
    public abstract void UpdateTexture(UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data);
}
