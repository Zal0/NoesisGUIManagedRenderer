using NoesisManagedRenderer;
using System;
using WaveEngine.Common.Graphics;

namespace WaveRenderer.WaveRenderDevice
{
    public class WaveRenderTarget : ManagedRenderTarget
    {
        public override ManagedTexture Texture { get; }

        public WaveRenderTarget(GraphicsContext graphicsContext, uint width, uint height, uint sampleCount)
        {
            var desc = new TextureDescription()
            {
                Type = TextureType.Texture2D,
                Width = width,
                Height = height,
                Depth = 1,
                ArraySize = 1,
                Faces = 1,
                Usage = ResourceUsage.Default,
                CpuAccess = ResourceCpuAccess.Write,
                Flags = TextureFlags.ShaderResource | TextureFlags.RenderTarget,
                Format = PixelFormat.R8G8B8A8_UNorm,
                MipLevels = 1,
            };

            switch (sampleCount)
            {
                case 1:
                    desc.SampleCount = TextureSampleCount.None;
                    break;

                case 2:
                    desc.SampleCount = TextureSampleCount.Count2;
                    break;

                case 4:
                    desc.SampleCount = TextureSampleCount.Count4;
                    break;

                case 8:
                    desc.SampleCount = TextureSampleCount.Count8;
                    break;

                case 16:
                    desc.SampleCount = TextureSampleCount.Count16;
                    break;

                case 32:
                    desc.SampleCount = TextureSampleCount.Count32;
                    break;

                default:
                    throw new InvalidOperationException("Invalid sample count value");
            }

            this.Texture = new WaveTexture(graphicsContext, ref desc, null);
        }
    }
}
