using NoesisManagedRenderer;
using System;
using System.Diagnostics;
using WaveEngine.Common.Graphics;

namespace WaveRenderer.WaveRenderDevice
{
    public class WaveTexture : ManagedTexture
    {
        private GraphicsContext graphicsContext;

        public Texture texture { private set; get; }
        public ResourceSet resourceSet { private set; get; }

        public override uint Width => this.texture.Description.Width;

        public override uint Height => this.texture.Description.Height;

        private byte[] textureData;

        internal WaveTexture(GraphicsContext graphicsContext, ref TextureDescription desc, DataBox[] data)
        {
            this.graphicsContext = graphicsContext;

            this.texture = this.graphicsContext.Factory.CreateTexture(data, ref desc);
            this.textureData = new byte[desc.Width * desc.Height * ((desc.Format == PixelFormat.R8G8B8A8_UNorm) ? 4 : 1)];
        }

        public static WaveTexture Create(GraphicsContext graphicsContext, uint width, uint height, uint numLevels, ref NoesisTextureFormat format, IntPtr[] data)
        {
            if (numLevels > 1 || data != null)
                throw new NotImplementedException();

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
                Format = (format == NoesisTextureFormat.RGBA8) ? PixelFormat.R8G8B8A8_UNorm : PixelFormat.R8_UNorm,
                MipLevels = numLevels,
                SampleCount = TextureSampleCount.None,
            };

            return new WaveTexture(graphicsContext, ref desc, null);
        }

        public void SetResourceSet(uint slot, byte sampler)
        {
            var resourceLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(slot, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(slot, ResourceType.Sampler, ShaderStages.Pixel)
            );

            var resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);

            SamplerStateDescription samplerDescription = SamplerStates.LinearClamp;//TODO
            var samplerState = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            var resourceSetDescription = new ResourceSetDescription(resourceLayout,
                texture, samplerState
            );

            resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);
        }

        unsafe public override void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
            if (level > 1)
                throw new NotImplementedException();

            Debug.WriteLine("Updating texture");
            byte* dataByte = (byte*)data;

            int bpp = texture.Description.Format == PixelFormat.R8G8B8A8_UNorm ? 4 : 1;
            for (uint j = 0; j < height; ++j)
            {
                for (uint i = 0; i < width; ++i)
                {
                    for (int b = 0; b < bpp; ++b)
                    {
                        textureData[(j + y) * texture.Description.Width * bpp + (i + x) * bpp + b] = dataByte[j * width * bpp + i * bpp + b];
                    }
                }
            }

            this.graphicsContext.UpdateTextureData(texture, textureData);
        }
    }
}
