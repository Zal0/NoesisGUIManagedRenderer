using System;
using System.Collections.Generic;

public abstract class ManagedTexture
{
    private static Dictionary<IntPtr, ManagedTexture> textures = new Dictionary<IntPtr, ManagedTexture>();

    public static ManagedTexture GetTexture(IntPtr nativePointer) => textures[nativePointer];

    private IntPtr nativePointer;

    public abstract uint Width { get; }
    public abstract uint Height { get; }

    public ManagedTexture(IntPtr nativePointer)
    {
        this.nativePointer = nativePointer;
        textures[nativePointer] = this;
    }

    ~ManagedTexture()
    {
        textures.Remove(this.nativePointer);
    }

    public abstract void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data);
}
