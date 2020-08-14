using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public unsafe class GLUTRenderer : ManagedRenderDevice
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

    const int Pos      = 1 << 0;
    const int Color    = 1 << 1;
    const int Tex0     = 1 << 2;
    const int Tex1     = 1 << 3;
    const int Tex2     = 1 << 4;
    const int Coverage = 1 << 5;

    enum ShaderName
    {
        RGBA,
        Mask,

        Path_Solid,
        Path_Linear,
        Path_Radial,
        Path_Pattern,

        PathAA_Solid,
        PathAA_Linear,
        PathAA_Radial,
        PathAA_Pattern,

        SDF_Solid,
        SDF_Linear,
        SDF_Radial,
        SDF_Pattern,

        SDF_LCD_Solid,
        SDF_LCD_Linear,
        SDF_LCD_Radial,
        SDF_LCD_Pattern,

        Image_Opacity_Solid,
        Image_Opacity_Linear,
        Image_Opacity_Radial,
        Image_Opacity_Pattern,

        Image_Shadow35V,
        Image_Shadow63V,
        Image_Shadow127V,

        Image_Shadow35H_Solid,
        Image_Shadow35H_Linear,
        Image_Shadow35H_Radial,
        Image_Shadow35H_Pattern,

        Image_Shadow63H_Solid,
        Image_Shadow63H_Linear,
        Image_Shadow63H_Radial,
        Image_Shadow63H_Pattern,

        Image_Shadow127H_Solid,
        Image_Shadow127H_Linear,
        Image_Shadow127H_Radial,
        Image_Shadow127H_Pattern,

        Image_Blur35V,
        Image_Blur63V,
        Image_Blur127V,

        Image_Blur35H_Solid,
        Image_Blur35H_Linear,
        Image_Blur35H_Radial,
        Image_Blur35H_Pattern,

        Image_Blur63H_Solid,
        Image_Blur63H_Linear,
        Image_Blur63H_Radial,
        Image_Blur63H_Pattern,

        Image_Blur127H_Solid,
        Image_Blur127H_Linear,
        Image_Blur127H_Radial,
        Image_Blur127H_Pattern,
    }

    int[] formats =
    {
        Pos,                                 //RGBA,                      
        Pos,                                 //Mask,                      

        Pos | Color,                         //Path_Solid,                
        Pos | Tex0,                          //Path_Linear,               
        Pos | Tex0,                          //Path_Radial,               
        Pos | Tex0,                          //Path_Pattern,              

        Pos | Color | Coverage,              //PathAA_Solid,              
        Pos | Tex0  | Coverage,              //PathAA_Linear,             
        Pos | Tex0  | Coverage,              //PathAA_Radial,             
        Pos | Tex0  | Coverage,              //PathAA_Pattern,            

        Pos | Color | Tex1,                  //SDF_Solid,                 
        Pos | Tex0  | Tex1,                  //SDF_Linear,                
        Pos | Tex0  | Tex1,                  //SDF_Radial,                
        Pos | Tex0  | Tex1,                  //SDF_Pattern,               

        Pos | Color | Tex1,                  //SDF_LCD_Solid,             
        Pos | Tex0  | Tex1,                  //SDF_LCD_Linear,            
        Pos | Tex0  | Tex1,                  //SDF_LCD_Radial,            
        Pos | Tex0  | Tex1,                  //SDF_LCD_Pattern,           

        Pos | Color | Tex1,                  //Image_Opacity_Solid,       
        Pos | Tex0  | Tex1,                  //Image_Opacity_Linear,      
        Pos | Tex0  | Tex1,                  //Image_Opacity_Radial,      
        Pos | Tex0  | Tex1,                  //Image_Opacity_Pattern,     

        Pos | Color | Tex1 | Tex2,           //Image_Shadow35V,           
        Pos | Color | Tex1 | Tex2,           //Image_Shadow63V,           
        Pos | Color | Tex1 | Tex2,           //Image_Shadow127V,          

        Pos | Color | Tex1 | Tex2,           //Image_Shadow35H_Solid,     
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Linear,    
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Radial,    
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Pattern,   

        Pos | Color | Tex1 | Tex2,           //Image_Shadow63H_Solid,     
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Linear,    
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Radial,    
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Pattern,   

        Pos | Color | Tex1 | Tex2,           //Image_Shadow127H_Solid,    
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Linear,   
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Radial,   
        Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Pattern,  

        Pos | Color | Tex1 | Tex2,           //Image_Blur35V,             
        Pos | Color | Tex1 | Tex2,           //Image_Blur63V,             
        Pos | Color | Tex1 | Tex2,           //Image_Blur127V,            

        Pos | Color | Tex1 | Tex2,           //Image_Blur35H_Solid,       
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Linear,      
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Radial,      
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Pattern,     

        Pos | Color | Tex1 | Tex2,           //Image_Blur63H_Solid,       
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Linear,      
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Radial,      
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Pattern,     

        Pos | Color | Tex1 | Tex2,           //Image_Blur127H_Solid,      
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Linear,     
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Radial,     
        Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Pattern,    
    };

    private byte[] vertices;
    private UInt16[] indices;

    int GetStride(ref Batch batch)
    {
        int format = formats[batch.shader];
        int ret = 0;

        if ((format & Pos) != 0)
            ret += 8;
        if ((format & Color) != 0)
            ret += 4;
        if ((format & Tex0) != 0)
            ret += 8;
        if ((format & Tex1) != 0)
            ret += 8;
        if ((format & Tex2) != 0)
            ret += 8;
        if ((format & Coverage) != 0)
            ret += 4;

        return ret;
    }

    public override void DrawBatch(ref Batch batch)
    {
        ShaderName shader = (ShaderName)batch.shader;
        int stride = GetStride(ref batch);

        int format = formats[batch.shader];
        
        bool hasColor = (format & Color) != 0;
        if (hasColor)
        {
            // Nothing special
        }
        else
        {
            GL.Color4ub(255, 255, 255, 255);
        }

        bool hasTexture = ((format & Tex0) | (format & Tex1) | (format & Tex2)) != 0;
        if (hasTexture)
        {
            IntPtr txtPtr = batch.ramps;
            if (batch.ramps == IntPtr.Zero) 
                txtPtr = batch.glyphs;
            GLUTTexture texture = (GLUTTexture)ManagedTexture.textures[ManagedTexture.GetTextureId(txtPtr)];
            GL.Enable(GL.GL_TEXTURE_2D);
            GL.BindTexture(GL.GL_TEXTURE_2D, texture.gl_id);
            GL.TexEnvi(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_MODULATE);
        }
        else 
        {
            GL.Disable(GL.GL_TEXTURE_2D);
        }
        
        GL.Enable(GL.GL_BLEND);
        GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

        GL.Begin(GL.GL_TRIANGLES);
        for (int i = 0; i < batch.numIndices; ++i)
        {
            int idx = indices[(int)(batch.startIndex) + i];
            int offset = 8; //8 first bytes are pos x and pos y

            if (hasColor)
            {
                byte r = vertices[(int)batch.vertexOffset + idx * stride + offset];
                byte g = vertices[(int)batch.vertexOffset + idx * stride + offset + 1];
                byte b = vertices[(int)batch.vertexOffset + idx * stride + offset + 2];
                byte a = vertices[(int)batch.vertexOffset + idx * stride + offset + 3];
                GL.Color4ub(r, g, b, a);
                
                offset += 4;
            }

            if(hasTexture)
            {
                float u = BitConverter.ToSingle(vertices, (int)batch.vertexOffset + idx * stride + offset);
                float v = BitConverter.ToSingle(vertices, (int)batch.vertexOffset + idx * stride + offset + 4);
                GL.TexCoord2f(u, v);

                offset += 8;
            }

            float x = BitConverter.ToSingle(vertices, (int)batch.vertexOffset + idx * stride);
            float y = BitConverter.ToSingle(vertices, (int)batch.vertexOffset + idx * stride + 4);
            GL.Vertex2f(x, y);
        }
        GL.End();
        GL.Flush();
    }

    
    unsafe public override IntPtr MapVertices(UInt32 bytes)
    {
        UInt32 size = bytes;
        if(vertices == null || size > vertices.Length)
            vertices = new byte[size];

        fixed (byte* pRetUpper = vertices)
        {
            return new IntPtr(pRetUpper);
        }
    }

    public override void UnmapVertices()
    {
        //vertices = null;
    }

    unsafe public override IntPtr MapIndices(uint bytes)
    {
        UInt32 size = bytes / sizeof(UInt16);
        if (indices == null || size > indices.Length)
            indices = new UInt16[size];

        fixed (UInt16* pRetUpper = indices)
        {
            return new IntPtr(pRetUpper);
        }
    }

    public override void UnmapIndices()
    {
        //indices = null;
    }
}

