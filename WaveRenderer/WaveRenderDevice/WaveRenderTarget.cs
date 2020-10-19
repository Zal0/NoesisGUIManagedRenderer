using NoesisManagedRenderer;
using System;
using WaveEngine.Common.Graphics;

namespace WaveRenderer.WaveRenderDevice
{
    public class WaveRenderTarget : ManagedRenderTarget
    {
        private Texture depthTexture;

        private Viewport[] viewports;

        private WaveTexture color;

        public override string Label { get; }

        public override uint Width => this.FrameBuffer?.Width ?? 0;

        public override uint Height => this.FrameBuffer?.Height ?? 0;

        public override uint SampleCount { get; }

        public override ManagedTexture Texture => this.color;

        public FrameBuffer FrameBuffer { get; }

        private WaveRenderTarget(GraphicsContext graphicsContext, string name, uint width, uint height, uint sampleCount, WaveTexture colorAA, Texture depthTexture)
        {
            this.SampleCount = sampleCount;
            this.Label = name;

            var desc = new TextureDescription()
            {
                Type = TextureType.Texture2D,
                Width = width,
                Height = height,
                Depth = 1,
                ArraySize = 1,
                Faces = 1,
                Usage = ResourceUsage.Default,
                CpuAccess = ResourceCpuAccess.None,
                Flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                Format = PixelFormat.R8G8B8A8_UNorm,
                MipLevels = 1,
            };

            switch (this.SampleCount)
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

            this.color = colorAA ?? new WaveTexture(graphicsContext, $"{name}_Color", ref desc, null);

            this.depthTexture = depthTexture;
            if (this.depthTexture == null)
            {
                desc.Format = PixelFormat.D24_UNorm_S8_UInt;
                desc.Flags = TextureFlags.DepthStencil;
                desc.SampleCount = TextureSampleCount.None;
                this.depthTexture = graphicsContext.Factory.CreateTexture(ref desc);
                this.depthTexture.Name = $"{name}_Depth";
            }

            var depthAttachment = new FrameBufferAttachment(this.depthTexture, 0, 1);
            var colorsAttachment = new[] { new FrameBufferAttachment(this.color.Texture, 0, 1) };
            this.FrameBuffer = graphicsContext.Factory.CreateFrameBuffer(depthAttachment, colorsAttachment);

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, width, height);
        }

        public WaveRenderTarget(GraphicsContext graphicsContext, string name, uint width, uint height, uint sampleCount)
            : this(graphicsContext, name, width, height, sampleCount, null, null)
        {
        }

        public WaveRenderTarget Clone(GraphicsContext graphicsContext, string name)
        {
            var colorAA = this.SampleCount > 1 ? (WaveTexture)this.Texture : null;
            var depth = this.depthTexture;
            return new WaveRenderTarget(graphicsContext, name, this.Width, this.Height, this.SampleCount, colorAA, depth);
        }

        public void SetRenderTarget(CommandBuffer commandBuffer)
        {
            commandBuffer.SetViewports(this.viewports);
        }
    }
}
