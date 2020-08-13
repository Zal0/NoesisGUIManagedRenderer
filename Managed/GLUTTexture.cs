using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GLUTTexture : ManagedTexture
{
    int width;
    int height;
    int numLevels;

    public override void Init(UInt32 width, UInt32 height, UInt32 numLevels, byte format, IntPtr data)
    {
        this.width = (int)width;
        this.height = (int)height;
        this.numLevels = (int)numLevels;
    }

    public override int GetHeight()
    {
        return height;
    }

    public override int GetWidth()
    {
        return width;
    }

    public override bool HasMipMaps()
    {
        return numLevels > 1;
    }

    public override bool IsInverted()
    {
        return false;
    }
}
