using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


public unsafe class GLUTRenderer : ManagedRenderDevice
{
    public override void DrawBatch(ref Batch batch)
    {
        
    }

    byte[] vertices;
    unsafe public override IntPtr MapVertices(UInt32 size)
    {
        vertices = new byte[size];

        fixed (byte* pRetUpper = &vertices[0])
        {
            return new IntPtr(pRetUpper);
        }
    }

    public override void UnmapVertices()
    {
        vertices = null;
    }

    byte[] indices;
    public override IntPtr MapIndices(uint size)
    {
        indices = new byte[size];

        fixed (byte* pRetUpper = indices)
        {
            return new IntPtr(pRetUpper);
        }
    }

    public override void UnmapIndices()
    {
        indices = null;
    }
}

