using System;
using System.Collections.Generic;
using System.Text;

namespace WaveRenderer
{
    public class WaveTexture : ManagedTexture
    {
        int width;
        int height;
        int numLevels;
        byte format;

        public override void Init(uint width, uint height, uint numLevels, byte format, IntPtr[] data)
        {
            this.width = (int)width;
            this.height = (int)height;
            this.numLevels = (int)numLevels;
            this.format = format;
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

        public override void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
        }
    }
}
