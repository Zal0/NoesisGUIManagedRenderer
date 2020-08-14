using System;
using System.Collections.Generic;
using System.Text;

namespace WaveRenderer
{
    class WaveRenderer : ManagedRenderDevice
    {
        private byte[] vertices;
        private UInt16[] indices;

        public override void DrawBatch(ref Batch batch)
        {
            //throw new NotImplementedException();
        }

        unsafe public override IntPtr MapVertices(UInt32 bytes)
        {
            UInt32 size = bytes;
            if (vertices == null || size > vertices.Length)
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

        public override void SetManagedTexture()
        {
            ManagedTexture.SetMamanagedTexture<WaveTexture>();
        }
    }
}
