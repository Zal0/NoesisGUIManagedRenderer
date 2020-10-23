using NoesisManagedRenderer;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WaveEngine.Common.Graphics;

namespace WaveRenderer.WaveRenderDevice
{
    public class WaveTexture : ManagedTexture
    {
        private GraphicsContext graphicsContext;

        private byte[] textureData;

        public Texture Texture { private set; get; }

        public ResourceSet ResourceSet { private set; get; }

        public override string Label => this.Texture?.Name;

        public override uint Width => this.Texture?.Description.Width ?? 0;

        public override uint Height => this.Texture?.Description.Height ?? 0;

        public override uint LevelCount => this.Texture?.Description.MipLevels ?? 1;

        public override NoesisTextureFormat Format => this.Texture?.Description.Format == PixelFormat.R8_UNorm ? NoesisTextureFormat.R8 : NoesisTextureFormat.RGBA8;

        internal WaveTexture(GraphicsContext graphicsContext, string name, ref TextureDescription desc, DataBox[] data)
        {
            this.graphicsContext = graphicsContext;

            this.Texture = this.graphicsContext.Factory.CreateTexture(data, ref desc);
            this.Texture.Name = name;

            if ((desc.Flags & TextureFlags.RenderTarget) == 0)
            {
                this.textureData = new byte[desc.Width * desc.Height * ((desc.Format == PixelFormat.R8G8B8A8_UNorm) ? 4 : 1)];
            }
        }

        public static WaveTexture Create(GraphicsContext graphicsContext, string name, uint width, uint height, uint numLevels, ref NoesisTextureFormat format, IntPtr[] data)
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
                Usage = ResourceUsage.Default,
                CpuAccess = data != null ? ResourceCpuAccess.None : ResourceCpuAccess.Write,
                Flags = TextureFlags.ShaderResource,
                Format = (format == NoesisTextureFormat.RGBA8) ? PixelFormat.R8G8B8A8_UNorm : PixelFormat.R8_UNorm,
                MipLevels = numLevels,
                SampleCount = TextureSampleCount.None,
            };

            return new WaveTexture(graphicsContext, name, ref desc, null);
        }

        unsafe public override void UpdateTexture(uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine($"{nameof(UpdateTexture)} -> {this.Label} Level:{level} Rect:[{x}, {y}, {width}, {height}]");
#endif

            byte* dataByte = (byte*)data;

            int bpp = Texture.Description.Format == PixelFormat.R8G8B8A8_UNorm ? 4 : 1;
            for (uint j = 0; j < height; ++j)
            {
                for (uint i = 0; i < width; ++i)
                {
                    for (int b = 0; b < bpp; ++b)
                    {
                        textureData[(j + y) * Texture.Description.Width * bpp + (i + x) * bpp + b] = dataByte[j * width * bpp + i * bpp + b];
                    }
                }
            }

            this.graphicsContext.UpdateTextureData(Texture, textureData);
        }
    }
}
