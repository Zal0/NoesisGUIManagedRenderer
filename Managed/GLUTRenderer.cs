using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public unsafe class GLUTRenderer : ManagedRenderDevice
{
    int[] stride =
    {
        8, //RGBA,                       // Pos
        8, //Mask,                       // Pos

        12, //Path_Solid,                 // Pos | Color
        16, //Path_Linear,                // Pos | Tex0
        16, //Path_Radial,                // Pos | Tex0
        16, //Path_Pattern,               // Pos | Tex0

        16, //PathAA_Solid,               // Pos | Color | Coverage
        20, //PathAA_Linear,              // Pos | Tex0  | Coverage
        20, //PathAA_Radial,              // Pos | Tex0  | Coverage
        20, //PathAA_Pattern,             // Pos | Tex0  | Coverage

        20, //SDF_Solid,                  // Pos | Color | Tex1
        24, //SDF_Linear,                 // Pos | Tex0  | Tex1
        24, //SDF_Radial,                 // Pos | Tex0  | Tex1
        24, //SDF_Pattern,                // Pos | Tex0  | Tex1

        20, //SDF_LCD_Solid,              // Pos | Color | Tex1
        24, //SDF_LCD_Linear,             // Pos | Tex0  | Tex1
        24, //SDF_LCD_Radial,             // Pos | Tex0  | Tex1
        24, //SDF_LCD_Pattern,            // Pos | Tex0  | Tex1

        20, //Image_Opacity_Solid,        // Pos | Color | Tex1
        24, //Image_Opacity_Linear,       // Pos | Tex0  | Tex1
        24, //Image_Opacity_Radial,       // Pos | Tex0  | Tex1
        24, //Image_Opacity_Pattern,      // Pos | Tex0  | Tex1

        28, //Image_Shadow35V,            // Pos | Color | Tex1 | Tex2
        28, //Image_Shadow63V,            // Pos | Color | Tex1 | Tex2
        28, //Image_Shadow127V,           // Pos | Color | Tex1 | Tex2

        28, //Image_Shadow35H_Solid,      // Pos | Color | Tex1 | Tex2
        32, //Image_Shadow35H_Linear,     // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Shadow35H_Radial,     // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Shadow35H_Pattern,    // Pos | Tex0  | Tex1 | Tex2

        28, //Image_Shadow63H_Solid,      // Pos | Color | Tex1 | Tex2
        32, //Image_Shadow63H_Linear,     // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Shadow63H_Radial,     // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Shadow63H_Pattern,    // Pos | Tex0  | Tex1 | Tex2

        28, //Image_Shadow127H_Solid,     // Pos | Color | Tex1 | Tex2
        32, //Image_Shadow127H_Linear,    // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Shadow127H_Radial,    // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Shadow127H_Pattern,   // Pos | Tex0  | Tex1 | Tex2

        28, //Image_Blur35V,              // Pos | Color | Tex1 | Tex2
        28, //Image_Blur63V,              // Pos | Color | Tex1 | Tex2
        28, //Image_Blur127V,             // Pos | Color | Tex1 | Tex2

        28, //Image_Blur35H_Solid,        // Pos | Color | Tex1 | Tex2
        32, //Image_Blur35H_Linear,       // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Blur35H_Radial,       // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Blur35H_Pattern,      // Pos | Tex0  | Tex1 | Tex2

        28, //Image_Blur63H_Solid,        // Pos | Color | Tex1 | Tex2
        32, //Image_Blur63H_Linear,       // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Blur63H_Radial,       // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Blur63H_Pattern,      // Pos | Tex0  | Tex1 | Tex2

        28, //Image_Blur127H_Solid,       // Pos | Color | Tex1 | Tex2
        32, //Image_Blur127H_Linear,      // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Blur127H_Radial,      // Pos | Tex0  | Tex1 | Tex2
        32, //Image_Blur127H_Pattern,     // Pos | Tex0  | Tex1 | Tex2
    };

    private byte[] vertices;
    private UInt16[] indices;


    int GetStride(ref Batch batch)
    {
        return stride[batch.shader];
    }

    public override void DrawBatch(ref Batch batch)
    {
        int stride = GetStride(ref batch);

        GL.Begin(GL.GL_TRIANGLES);
        for (int i = 0; i < batch.numIndices; ++i)
        {
            int idx = indices[(int)(batch.startIndex) + i];
            
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

