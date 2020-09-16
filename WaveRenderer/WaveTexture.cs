using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;

namespace WaveRenderer
{
    public class WaveTexture : ManagedTexture
    {
        public Texture texture { private set; get; }

        public override void Init(ManagedRenderDevice managedRenderDevice, uint width, uint height, uint numLevels, byte format, IntPtr[] data)
        {
            WaveRenderer waveRenderer = (WaveRenderer)managedRenderDevice;
            GraphicsContext graphicsContext = waveRenderer.graphicsContext;

            TextureDescription desc = TextureDescription.CreateTexture2DDescription(width, height, (format == (byte)Format.RGBA8) ? PixelFormat.R8G8B8A8_UInt : PixelFormat.R8_UInt);
            if (data != null)
            {
                DataBox textureData = new DataBox(data[0]);
                texture = graphicsContext.Factory.CreateTexture(new DataBox[] { textureData }, ref desc);
            }
            else
            {
                texture = graphicsContext.Factory.CreateTexture(ref desc);
            }
        }

        public override int GetHeight()
        {
            return (int)texture.Description.Height;
        }

        public override int GetWidth()
        {
            return (int)texture.Description.Width;
        }

        public override bool HasMipMaps()
        {
            return texture.Description.MipLevels > 1;
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
