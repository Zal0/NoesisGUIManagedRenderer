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

    public int id;

    public abstract void Init(ManagedRenderDevice managedRenderDevice, UInt32 width, UInt32 height, UInt32 numLevels, byte format, IntPtr[] data);
    public abstract int GetWidth();
    public abstract int GetHeight();
    public abstract bool HasMipMaps();
    public abstract bool IsInverted();
    public abstract void UpdateTexture(UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data);
}
