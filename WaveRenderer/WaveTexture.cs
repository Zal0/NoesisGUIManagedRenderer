using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;

namespace WaveRenderer
{
    public class WaveTexture : ManagedTexture
    {
        public Texture texture { private set; get; }
        public ResourceSet resourceSet { private set; get; }

        private byte[] textureData;

        public void SetResourceSet(GraphicsContext graphicsContext, uint slot, byte sampler)
        {
            var resourceLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(slot, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(slot, ResourceType.Sampler, ShaderStages.Pixel)
            );

            var resourceLayout = graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);

            SamplerStateDescription samplerDescription = SamplerStates.LinearClamp;//TODO
            var samplerState = graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            var resourceSetDescription = new ResourceSetDescription(resourceLayout,
                texture, samplerState
            );

            resourceSet = graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);
        }

        public override void Init(ManagedRenderDevice managedRenderDevice, uint width, uint height, uint numLevels, byte format, IntPtr[] data)
        {
            WaveRenderer waveRenderer = (WaveRenderer)managedRenderDevice;
            GraphicsContext graphicsContext = waveRenderer.graphicsContext;

            //TextureDescription desc = TextureDescription.CreateTexture2DDescription(width, height, (format == (byte)Format.RGBA8) ? PixelFormat.R8G8B8A8_UInt : PixelFormat.R8_UInt);
            var desc = new TextureDescription()
            {
                Type = TextureType.Texture2D,
                Width = width,
                Height = height,
                Depth = 1,
                ArraySize = 1,
                Faces = 1,
                Usage = ResourceUsage.Dynamic,
                CpuAccess = ResourceCpuAccess.Write,
                Flags = TextureFlags.ShaderResource,
                Format = (format == (byte)Format.RGBA8) ? PixelFormat.R8G8B8A8_UInt : PixelFormat.R8_UInt,
                MipLevels = 1,
                SampleCount = TextureSampleCount.None,
            };

            if (data != null)
            {
                //TODO: copy data to texture Data
                //DataBox textureDataBox = new DataBox(data[0]);
                //texture = graphicsContext.Factory.CreateTexture(new DataBox[] { textureDataBox }, ref desc);
            }
            else
            {
                texture = graphicsContext.Factory.CreateTexture(ref desc);
                textureData = new byte[width * height * ((format == (byte)Format.RGBA8) ? 4 : 1)];
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

        unsafe public override void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
            byte* dataByte = (byte*)data;

            int bpp = texture.Description.Format == PixelFormat.R8G8B8A8_UInt ? 4 : 1;
            for (uint j = 0; j < height; ++ j)
            {
                for(uint i = 0; i < width; ++i)
                {
                    for (int b = 0; b < bpp; ++b)
                    {
                        textureData[(j + y) * texture.Description.Width * bpp + (i + x) * bpp + b] = dataByte[j * width * bpp + i * bpp + b];
                    }
                }
            }

            WaveRenderer.Instance.graphicsContext.UpdateTextureData(texture, textureData);
        }
    }
}
