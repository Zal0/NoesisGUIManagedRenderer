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
    public int format;

    public int gl_id;

    unsafe public override void Init(UInt32 width, UInt32 height, UInt32 numLevels, byte format, IntPtr[] data)
    {
        this.width = (int)width;
        this.height = (int)height;
        this.numLevels = (int)numLevels;
        this.format = ((Format)format == Format.RGBA8) ? GL.GL_RGBA : GL.GL_ALPHA;

        fixed (int* f_id = &gl_id)
        {
            IntPtr _ids = new IntPtr(f_id);
            GL.GenTextures(1, _ids);
        }

        GL.BindTexture(GL.GL_TEXTURE_2D, gl_id);

        GL.TexImage2D(GL.GL_TEXTURE_2D, 0, this.format, this.width, this.height, 0, this.format, GL.GL_UNSIGNED_BYTE, data == null ? new IntPtr() : data[0]);

        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_REPEAT);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_REPEAT);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
        GL.PixelStorei(GL.GL_UNPACK_ALIGNMENT, 1);
    }

    public override void UpdateTexture(UInt32 level, UInt32 x, UInt32 y, UInt32 width, UInt32 height, IntPtr data)
    {
        GL.BindTexture(GL.GL_TEXTURE_2D, gl_id);
        GL.TexSubImage2D(GL.GL_TEXTURE_2D, (int)level, (int)x, (int)y, (int)width, (int)height, this.format, GL.GL_UNSIGNED_BYTE, data);
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
