using NoesisManagedRenderer;
using System;


public class GLUTTexture : ManagedTexture
{
    string label;
    uint width;
    uint height;
    uint numLevels;
    public int format;

    public int gl_id;

    public override string Label => this.label;

    public override uint Width => this.width;

    public override uint Height => this.width;

    public override uint LevelCount => this.numLevels;

    public override NoesisTextureFormat Format => throw new NotImplementedException();

    public unsafe GLUTTexture(string label, uint width, uint height, uint numLevels, NoesisTextureFormat format, IntPtr[] data)
    {
        this.label = label;
        this.width = width;
        this.height = height;
        this.numLevels = numLevels;
        this.format = (format == NoesisTextureFormat.RGBA8) ? GL.GL_RGBA : GL.GL_ALPHA;

        fixed (int* f_id = &gl_id)
        {
            IntPtr _ids = new IntPtr(f_id);
            GL.GenTextures(1, _ids);
        }

        GL.BindTexture(GL.GL_TEXTURE_2D, gl_id);

        GL.TexImage2D(GL.GL_TEXTURE_2D, 0, this.format, (int)this.width, (int)this.height, 0, this.format, GL.GL_UNSIGNED_BYTE, data == null ? new IntPtr() : data[0]);

        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_REPEAT);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_REPEAT);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);
        GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
        GL.PixelStorei(GL.GL_UNPACK_ALIGNMENT, 1);
    }

    public override void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data)
    {
        GL.BindTexture(GL.GL_TEXTURE_2D, gl_id);
        GL.TexSubImage2D(GL.GL_TEXTURE_2D, (int)level, (int)x, (int)y, (int)width, (int)height, this.format, GL.GL_UNSIGNED_BYTE, data);
    }
}
