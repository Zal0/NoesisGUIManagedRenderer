using WaveEngine.Common.Graphics;
using WaveEngine.Mathematics;
using WaveEngine.Platform;
using VisualTests.Runners.Common;
using Buffer = WaveEngine.Common.Graphics.Buffer;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Noesis;
using System;
using Rectangle = WaveEngine.Mathematics.Rectangle;
using Vector4 = WaveEngine.Mathematics.Vector4;

namespace WaveRenderer.WaveRenderDevice
{
    public class WaveRenderDevice : Noesis.RenderDevice
    {
        protected const uint DYNAMIC_VB_SIZE = 512 * 1024;
        protected const uint DYNAMIC_IB_SIZE = 128 * 1024;
        protected const uint DYNAMIC_TEX_SIZE = 128 * 1024;

        class DynamicBuffer
        {
            private uint pos;

            private IntPtr cpuBuffer;

            public Buffer InternalBuffer { get; }

            public uint DrawPos { get; private set; }

            public uint Size { get; }

            public DynamicBuffer(string name, GraphicsContext graphicsContext, uint size, BufferFlags flags)
            {
                this.Size = size;
                var resourceUsage = flags == BufferFlags.ConstantBuffer ? ResourceUsage.Default : ResourceUsage.Dynamic;
                var resourceCpuAccess = flags == BufferFlags.ConstantBuffer ? ResourceCpuAccess.None : ResourceCpuAccess.Write;
                var bufferDescription = new BufferDescription(size, flags, resourceUsage, resourceCpuAccess);
                this.InternalBuffer = graphicsContext.Factory.CreateBuffer(ref bufferDescription);
                this.InternalBuffer.Name = name;

                if (resourceCpuAccess == ResourceCpuAccess.Write)
                {
                    this.cpuBuffer = Marshal.AllocHGlobal((int)size);
                }
            }

            public IntPtr Map(uint size)
            {
                if (this.pos + size > this.Size)
                {
                    this.pos = 0;
                }

                this.DrawPos = this.pos;
                this.pos += size;

                return IntPtr.Add(this.cpuBuffer, (int)this.DrawPos);
            }

            public void Unmap(CommandBuffer commandBuffer)
            {
                commandBuffer.UpdateBufferData(this.InternalBuffer, this.cpuBuffer, this.Size);
            }
        }

        private GraphicsPipelineDescription[] graphicPipelineDescsByShader = new GraphicsPipelineDescription[Noesis.Shader.Formats.Length];
        private BlendStateDescription[] blendDescs;
        private RasterizerStateDescription[] rasterDescs;
        private DepthStencilStateDescription[] depthDescs;

        private Dictionary<int, GraphicsPipelineState> pipelineStateCache = new Dictionary<int, GraphicsPipelineState>();
        private Dictionary<int, ResourceSet> resourceSetsCache = new Dictionary<int, ResourceSet>();
        private Dictionary<Noesis.SamplerState, WaveEngine.Common.Graphics.SamplerState> samplerStateCache = new Dictionary<Noesis.SamplerState, WaveEngine.Common.Graphics.SamplerState>();

        private ResourceLayout resourceLayout;

        private GraphicsPipelineState renderTargetPipelineState;

        private WaveRenderTarget currentSurface;

        private FrameBuffer swapChainFrameBuffer;

        private CommandBuffer commandBuffer;

        private GraphicsContext graphicsContext { get; }

        //Buffers
        private DynamicBuffer vertexBuffer;
        private DynamicBuffer indexBuffer;
        private DynamicBuffer vertexCB;
        private DynamicBuffer pixelCB;
        private DynamicBuffer effectCB;
        private DynamicBuffer texDimensionsCB;

        private uint vertexCBHash;
        private uint pixelCBHash;
        private uint effectCBHash;
        private uint texDimensionsCBHash;
        private DeviceCaps caps;

        public override DeviceCaps Caps => this.caps;

        public WaveRenderDevice(GraphicsContext graphicsContext)
        {
            this.graphicsContext = graphicsContext;
            this.caps = new DeviceCaps() { SubpixelRendering = true };

            this.CreateBuffers();
            this.CreateStatesObjects();
        }

        private void CreateBuffers()
        {
            vertexCBHash = 0;
            pixelCBHash = 0;
            effectCBHash = 0;
            texDimensionsCBHash = 0;

            this.vertexBuffer = new DynamicBuffer("Noesis_VertexBuffer", this.graphicsContext, DYNAMIC_VB_SIZE, BufferFlags.VertexBuffer);
            this.indexBuffer = new DynamicBuffer("Noesis_IndexBuffer", this.graphicsContext, DYNAMIC_IB_SIZE, BufferFlags.IndexBuffer);
            this.vertexCB = new DynamicBuffer("Noesis_VertexCB", this.graphicsContext, 16 * sizeof(float), BufferFlags.ConstantBuffer);
            this.pixelCB = new DynamicBuffer("Noesis_PixelCB", this.graphicsContext, 12 * sizeof(float), BufferFlags.ConstantBuffer);
            this.effectCB = new DynamicBuffer("Noesis_EffectCB", this.graphicsContext, 16 * sizeof(float), BufferFlags.ConstantBuffer);
            this.texDimensionsCB = new DynamicBuffer("Noesis_TexDimensionsCB", this.graphicsContext, 4 * sizeof(float), BufferFlags.ConstantBuffer);
        }

        public async Task InitializeAsync(AssetsDirectory assetsDirectory)
        {

            var resourceLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                new LayoutElementDescription(1, ResourceType.ConstantBuffer, ShaderStages.Vertex | ShaderStages.Pixel),
                new LayoutElementDescription(2, ResourceType.ConstantBuffer, ShaderStages.Pixel),
                new LayoutElementDescription(3, ResourceType.ConstantBuffer, ShaderStages.Pixel),

                new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel),

                new LayoutElementDescription(1, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(1, ResourceType.Sampler, ShaderStages.Pixel),

                new LayoutElementDescription(2, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(2, ResourceType.Sampler, ShaderStages.Pixel),

                new LayoutElementDescription(3, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(3, ResourceType.Sampler, ShaderStages.Pixel),

                new LayoutElementDescription(4, ResourceType.Texture, ShaderStages.Pixel),
                new LayoutElementDescription(4, ResourceType.Sampler, ShaderStages.Pixel)
            );

            this.resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);

            for (int shaderIndex = 0; shaderIndex < Noesis.Shader.Formats.Length; ++shaderIndex)
            {
                await this.InitShaderResources(shaderIndex, assetsDirectory);
            }

            await this.InitRenderTargetResources(assetsDirectory);
        }

        private async Task InitShaderResources(int shaderIndex, AssetsDirectory assetsDirectory)
        {
            InputLayouts vertexLayouts = new InputLayouts();
            LayoutDescription layoutDescription = new LayoutDescription();
            vertexLayouts.Add(layoutDescription);

            int format = Noesis.Shader.Formats[shaderIndex];

            string vertexFilename = "";

            var vertexSB = new StringBuilder();
            if ((format & Noesis.Shader.Pos) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.Position));
                vertexFilename += "Pos";
            }
            if ((format & Noesis.Shader.Color) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.UByte4Normalized, ElementSemanticType.Color));
                vertexFilename += "Color";
                vertexSB.AppendLine("#define HAS_COLOR 1");
            }
            if ((format & Noesis.Shader.Tex0) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 0));
                vertexFilename += "Tex0";
                vertexSB.AppendLine("#define HAS_UV0 1");
            }
            if ((format & Noesis.Shader.Tex1) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 1));
                vertexFilename += "Tex1";
                vertexSB.AppendLine("#define HAS_UV1 1");
            }
            if ((format & Noesis.Shader.Tex2) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.UShort4Normalized, ElementSemanticType.TexCoord, 2));
                vertexFilename += "Tex2";
                vertexSB.AppendLine("#define HAS_UV2 1");
            }
            if ((format & Noesis.Shader.Coverage) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float, ElementSemanticType.TexCoord, 3));
                vertexFilename += "Coverage";
                vertexSB.AppendLine("#define HAS_COVERAGE 1");
            }
            if ((format & Noesis.Shader.SDF) != 0)
            {
                vertexFilename += "_SDF";
                vertexSB.AppendLine("#define GEN_ST1 1");
            }
            vertexFilename += "_VS";

            string pixelFilename = ((Noesis.Shader.Enum)shaderIndex).ToString() + "_FS";

            /*var pixelSB = new StringBuilder();
            var effectName = ((Noesis.Shader.Enum)shaderIndex).ToString().ToUpper()
                                                             .Replace("PATHAA", "PATH_AA")
                                                             .Replace("SHADOW", "SHADOW_")
                                                             .Replace("BLUR", "BLUR_");
            pixelSB.AppendLine($"#define EFFECT_{effectName} 1");

            var noesisShadersPath = "Shaders/HLSL/Noesis/";
            vertexSB.Append(await assetsDirectory.ReadAsStringAsync(noesisShadersPath + "ShaderVS.hlsl"));

            var source = await assetsDirectory.ReadAsStringAsync(noesisShadersPath + "ShaderPS.hlsl");
            pixelSB.Append(source
                //.Replace("o.color = (img + (1.0f - img.a) * (shadowColor * alpha)) * (opacity_ * paint.a);", "o.color = (img + (1.0f - img.a)) * (opacity_ * paint.a);")
                .Replace("register(b2)", "register(b3)")
                .Replace("register(b0)", "register(b2)"))
                .Replace("float1 blurSize;\r\n    float2 shadowOffset;", "float2 shadowOffset;\r\n    float1 blurSize;"); // Workaround for buffer block pack bug on OpenGL and ESSL

            var outputBasePath = "../../../../" + assetsDirectory.RootPath + "/" + noesisShadersPath;
            System.IO.File.WriteAllText(outputBasePath + vertexFilename + ".fx", vertexSB.ToString());
            System.IO.File.WriteAllText(outputBasePath + pixelFilename + ".fx", pixelSB.ToString());

            return;*/

            var vertexShaderDescription = await assetsDirectory.ReadAndCompileShader(this.graphicsContext, "Noesis/" + vertexFilename, ShaderStages.Vertex, "main");
            var pixelShaderDescription = await assetsDirectory.ReadAndCompileShader(this.graphicsContext, "Noesis/" + pixelFilename, ShaderStages.Pixel, "main");
            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

#if DEBUG
            vertexShader.Name = $"Noesis_{vertexFilename}";
            pixelShader.Name = $"Noesis_{pixelFilename}";
#endif

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                ResourceLayouts = new[] { this.resourceLayout },
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    SampleMask = -1,
                    StencilReference = 0,
                },
            };

            this.graphicPipelineDescsByShader[shaderIndex] = pipelineDescription;
        }

        private async Task InitRenderTargetResources(AssetsDirectory assetsDirectory)
        {
            var vertexFilename = "QuadVS";
            var pixelFilename = "ClearPS";

            var vertexShaderDescription = await assetsDirectory.ReadAndCompileShader(this.graphicsContext, "Noesis/" + vertexFilename, ShaderStages.Vertex, "main");
            var pixelShaderDescription = await assetsDirectory.ReadAndCompileShader(this.graphicsContext, "Noesis/" + pixelFilename, ShaderStages.Pixel, "main");
            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

#if DEBUG
            vertexShader.Name = vertexFilename;
            pixelShader.Name = pixelFilename;
#endif

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    SampleMask = -1,
                    StencilReference = 0,
                    RasterizerState = this.rasterDescs[2],
                    BlendState = this.blendDescs[0],
                    DepthStencilState = this.depthDescs[4],
                }
            };

            pipelineDescription.Outputs = new OutputDescription(
                new OutputAttachmentDescription(PixelFormat.D24_UNorm_S8_UInt),
                new[] { new OutputAttachmentDescription(PixelFormat.R8G8B8A8_UNorm) }, TextureSampleCount.None);

            this.renderTargetPipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
        }

        private void CreateStatesObjects()
        {
            // Rasterized states
            this.rasterDescs = new RasterizerStateDescription[4];
            {
                var rasterizerStateDesc = new RasterizerStateDescription();
                rasterizerStateDesc.CullMode = CullMode.None;
                rasterizerStateDesc.FrontCounterClockwise = false;
                rasterizerStateDesc.DepthBias = 0;
                rasterizerStateDesc.DepthBiasClamp = 0.0f;
                rasterizerStateDesc.SlopeScaledDepthBias = 0.0f;
                rasterizerStateDesc.DepthClipEnable = true;
                //rasterizerStateDesc.MultisampleEnable = true;
                rasterizerStateDesc.AntialiasedLineEnable = false;

                rasterizerStateDesc.FillMode = FillMode.Solid;
                rasterizerStateDesc.ScissorEnable = false;
                this.rasterDescs[0] = rasterizerStateDesc;

                rasterizerStateDesc.FillMode = FillMode.Wireframe;
                rasterizerStateDesc.ScissorEnable = false;
                this.rasterDescs[1] = rasterizerStateDesc;

                rasterizerStateDesc.FillMode = FillMode.Solid;
                rasterizerStateDesc.ScissorEnable = true;
                this.rasterDescs[2] = rasterizerStateDesc;

                rasterizerStateDesc.FillMode = FillMode.Wireframe;
                rasterizerStateDesc.ScissorEnable = true;
                this.rasterDescs[3] = rasterizerStateDesc;
            }

            // Blend states
            this.blendDescs = new BlendStateDescription[4];
            {
                var blendStateDesc = new BlendStateDescription();
                blendStateDesc.AlphaToCoverageEnable = false;
                blendStateDesc.IndependentBlendEnable = false;
                blendStateDesc.RenderTarget0.SourceBlendColor = Blend.One;
                blendStateDesc.RenderTarget0.DestinationBlendColor = Blend.InverseSourceAlpha;
                blendStateDesc.RenderTarget0.BlendOperationColor = BlendOperation.Add;
                blendStateDesc.RenderTarget0.SourceBlendAlpha = Blend.One;
                blendStateDesc.RenderTarget0.DestinationBlendAlpha = Blend.InverseSourceAlpha;
                blendStateDesc.RenderTarget0.BlendOperationAlpha = BlendOperation.Add;
                blendStateDesc.RenderTarget0.ColorWriteChannels = ColorWriteChannels.All;

                // Src
                blendStateDesc.RenderTarget0.BlendEnable = false;
                this.blendDescs[0] = blendStateDesc;

                // SrcOver
                blendStateDesc.RenderTarget0.BlendEnable = true;
                this.blendDescs[1] = blendStateDesc;

                // SrcOver_Dual
                blendStateDesc.RenderTarget0.SourceBlendColor = Blend.One;
                blendStateDesc.RenderTarget0.DestinationBlendColor = Blend.InverseSecondarySourceColor;
                blendStateDesc.RenderTarget0.SourceBlendAlpha = Blend.One;
                blendStateDesc.RenderTarget0.DestinationBlendAlpha = Blend.InverseSecondarySourceAlpha;
                this.blendDescs[2] = blendStateDesc;

                // Color disabled
                blendStateDesc.RenderTarget0.BlendEnable = false;
                blendStateDesc.RenderTarget0.ColorWriteChannels = ColorWriteChannels.None;
                this.blendDescs[3] = blendStateDesc;
            }

            // Depth states
            this.depthDescs = new DepthStencilStateDescription[5];
            {
                var depthStateDesc = new DepthStencilStateDescription();
                depthStateDesc.DepthEnable = false;
                depthStateDesc.DepthWriteMask = false;
                depthStateDesc.DepthFunction = ComparisonFunction.Never;
                depthStateDesc.StencilReadMask = 0xff;
                depthStateDesc.StencilWriteMask = 0xff;
                depthStateDesc.FrontFace.StencilFunction = ComparisonFunction.Equal;
                depthStateDesc.FrontFace.StencilDepthFailOperation = StencilOperation.Keep;
                depthStateDesc.FrontFace.StencilFailOperation = StencilOperation.Keep;
                depthStateDesc.BackFace.StencilFunction = ComparisonFunction.Equal;
                depthStateDesc.BackFace.StencilDepthFailOperation = StencilOperation.Keep;
                depthStateDesc.BackFace.StencilFailOperation = StencilOperation.Keep;

                // Disabled
                depthStateDesc.StencilEnable = false;
                depthStateDesc.FrontFace.StencilPassOperation = StencilOperation.Keep;
                depthStateDesc.BackFace.StencilPassOperation = StencilOperation.Keep;
                this.depthDescs[0] = depthStateDesc;

                // Equal_Keep
                depthStateDesc.StencilEnable = true;
                depthStateDesc.FrontFace.StencilPassOperation = StencilOperation.Keep;
                depthStateDesc.BackFace.StencilPassOperation = StencilOperation.Keep;
                this.depthDescs[1] = depthStateDesc;

                // Equal_Incr
                depthStateDesc.StencilEnable = true;
                depthStateDesc.FrontFace.StencilPassOperation = StencilOperation.Increment;
                depthStateDesc.BackFace.StencilPassOperation = StencilOperation.Increment;
                this.depthDescs[2] = depthStateDesc;

                // Equal_Decr
                depthStateDesc.StencilEnable = true;
                depthStateDesc.FrontFace.StencilPassOperation = StencilOperation.Decrement;
                depthStateDesc.BackFace.StencilPassOperation = StencilOperation.Decrement;
                this.depthDescs[3] = depthStateDesc;

                // Zero
                depthStateDesc.StencilEnable = true;
                depthStateDesc.FrontFace.StencilFunction = ComparisonFunction.Always;
                depthStateDesc.FrontFace.StencilPassOperation = StencilOperation.Zero;
                depthStateDesc.BackFace.StencilFunction = ComparisonFunction.Always;
                depthStateDesc.BackFace.StencilPassOperation = StencilOperation.Zero;
                this.depthDescs[4] = depthStateDesc;
            }
        }

        private GraphicsPipelineState GetPipelineState(ref Batch batch, OutputDescription outputDescription)
        {
            var renderState = batch.RenderState;
            var shaderIndex = batch.Shader.Index;
            var stencilRef = batch.StencilRef;

            var hashCode = HashCode.Combine(
                renderState,
                shaderIndex,
                stencilRef,
                outputDescription.CachedHashCode);

            if (!this.pipelineStateCache.TryGetValue(hashCode, out var result))
            {
                var rasterizerIndex = renderState.Wireframe ? 1 : 0 | (renderState.ScissorEnable ? 2 : 0);
                var blendIndex = renderState.ColorEnable ? (int)renderState.BlendMode : 3;
                var depthIndex = (int)renderState.StencilMode;

                var desc = this.graphicPipelineDescsByShader[shaderIndex];
                desc.RenderStates.RasterizerState = this.rasterDescs[rasterizerIndex];
                desc.RenderStates.BlendState = this.blendDescs[blendIndex];
                desc.RenderStates.DepthStencilState = this.depthDescs[depthIndex];
                desc.RenderStates.StencilReference = stencilRef;
                desc.Outputs = outputDescription;
                result = this.graphicsContext.Factory.CreateGraphicsPipeline(ref desc);
                this.pipelineStateCache.Add(hashCode, result);

#if DEBUG
                result.Name = $"Noesis_{batch.Shader.Name}_{rasterizerIndex}{blendIndex}{depthIndex}_{stencilRef}";
#endif

#if TRACE_RENDER_DEVICE
                System.Diagnostics.Trace.WriteLine($"New Pipeline state -> {result.Name}");
#endif
            }

            return result;
        }

        private ResourceSet GetResourceSet(ref Batch batch)
        {
            var hash = new HashCode();
            hash.Add(batch.Pattern);
            hash.Add(batch.PatternSampler);
            hash.Add(batch.Ramps);
            hash.Add(batch.RampsSampler);
            hash.Add(batch.Image);
            hash.Add(batch.ImageSampler);
            hash.Add(batch.Glyphs);
            hash.Add(batch.GlyphsSampler);
            hash.Add(batch.Shadow);
            hash.Add(batch.ShadowSampler);

            var hashCode = hash.ToHashCode();
            if (!this.resourceSetsCache.TryGetValue(hashCode, out var result))
            {
                var patternTexture = ((WaveTexture)batch.Pattern)?.Texture;
                var rampsTexture = ((WaveTexture)batch.Ramps)?.Texture;
                var imageTexture = ((WaveTexture)batch.Image)?.Texture;
                var glyphsTexture = ((WaveTexture)batch.Glyphs)?.Texture;
                var shadowTexture = ((WaveTexture)batch.Shadow)?.Texture;
                var patternSampler = batch.PatternSampler;
                var rampsSampler = batch.RampsSampler;
                var imageSampler = batch.ImageSampler;
                var glyphsSampler = batch.GlyphsSampler;
                var shadowSampler = batch.ShadowSampler;

                var desc = new ResourceSetDescription(
                    this.resourceLayout,
                    vertexCB.InternalBuffer,
                    texDimensionsCB.InternalBuffer,
                    pixelCB.InternalBuffer,
                    effectCB.InternalBuffer,
                    patternTexture,
                    this.GetSamplerState(ref patternSampler),
                    rampsTexture,
                    this.GetSamplerState(ref rampsSampler),
                    imageTexture,
                    this.GetSamplerState(ref imageSampler),
                    glyphsTexture,
                    this.GetSamplerState(ref glyphsSampler),
                    shadowTexture,
                    this.GetSamplerState(ref shadowSampler));

                result = this.graphicsContext.Factory.CreateResourceSet(ref desc);
                this.resourceSetsCache.Add(hashCode, result);
#if DEBUG
                result.Name = $"Noesis_{patternTexture?.Name ?? "NULL"} {rampsTexture?.Name ?? "NULL"} {imageTexture?.Name ?? "NULL"} {glyphsTexture?.Name ?? "NULL"} {shadowTexture?.Name ?? "NULL"}";
#endif

#if TRACE_RENDER_DEVICE
                System.Diagnostics.Trace.WriteLine($"New Resource Set -> {result.Name}");
#endif
            }

            return result;
        }

        private WaveEngine.Common.Graphics.SamplerState GetSamplerState(ref Noesis.SamplerState sampler)
        {
            if (!this.samplerStateCache.TryGetValue(sampler, out var result))
            {
                var desc = SamplerStateDescription.Default;
                switch (sampler.WrapMode)
                {
                    case WrapMode.ClampToEdge:
                        desc.AddressU = TextureAddressMode.Clamp;
                        desc.AddressV = TextureAddressMode.Clamp;
                        break;
                    case WrapMode.ClampToZero:
                        desc.BorderColor = SamplerBorderColor.TransparentBlack;
                        desc.AddressU = TextureAddressMode.Border;
                        desc.AddressV = TextureAddressMode.Border;
                        break;
                    case WrapMode.Repeat:
                        desc.AddressU = TextureAddressMode.Wrap;
                        desc.AddressV = TextureAddressMode.Wrap;
                        break;
                    case WrapMode.MirrorU:
                        desc.AddressU = TextureAddressMode.Mirror;
                        desc.AddressV = TextureAddressMode.Wrap;
                        break;
                    case WrapMode.MirrorV:
                        desc.AddressU = TextureAddressMode.Wrap;
                        desc.AddressV = TextureAddressMode.Mirror;
                        break;
                    case WrapMode.Mirror:
                        desc.AddressU = TextureAddressMode.Mirror;
                        desc.AddressV = TextureAddressMode.Mirror;
                        break;
                    default:
                        throw new InvalidOperationException($"Undefined {nameof(sampler.WrapMode)}: {sampler.WrapMode}");
                }

                switch (sampler.MinMagFilter)
                {
                    case MinMagFilter.Nearest:
                        switch (sampler.MipFilter)
                        {
                            case MipFilter.Disabled:
                                desc.MaxLOD = 0;
                                desc.Filter = TextureFilter.MinPoint_MagPoint_MipPoint;
                                break;
                            case MipFilter.Nearest:
                                desc.Filter = TextureFilter.MinPoint_MagPoint_MipPoint;
                                break;
                            case MipFilter.Linear:
                                desc.Filter = TextureFilter.MinPoint_MagPoint_MipLinear;
                                break;
                            default:
                                throw new InvalidOperationException($"Undefined {nameof(sampler.MipFilter)}: {sampler.MipFilter}");
                        }
                        break;
                    case MinMagFilter.Linear:
                        switch (sampler.MipFilter)
                        {
                            case MipFilter.Disabled:
                                desc.MaxLOD = 0;
                                desc.Filter = TextureFilter.MinLinear_MagLinear_MipPoint;
                                break;
                            case MipFilter.Nearest:
                                desc.Filter = TextureFilter.MinLinear_MagLinear_MipPoint;
                                break;
                            case MipFilter.Linear:
                                desc.Filter = TextureFilter.MinLinear_MagLinear_MipLinear;
                                break;
                            default:
                                throw new InvalidOperationException($"Undefined {nameof(sampler.MipFilter)}: {sampler.MipFilter}");
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Undefined {nameof(sampler.MinMagFilter)}: {sampler.MinMagFilter}");
                }

                desc.MaxLOD = float.MaxValue;
                desc.MinLOD = 0;

                result = this.graphicsContext.Factory.CreateSamplerState(ref desc);
                this.samplerStateCache.Add(sampler, result);

#if DEBUG
                result.Name = $"Noesis_{sampler.WrapMode}_{sampler.MinMagFilter}_{sampler.MipFilter}";
#endif

#if TRACE_RENDER_DEVICE
                System.Diagnostics.Trace.WriteLine($"New Sampler State -> {result.Name}");
#endif
            }

            return result;
        }

        internal void SetSwapChainFrameBuffer(FrameBuffer frameBuffer)
        {
            this.swapChainFrameBuffer = frameBuffer;
        }

        public void SetCommandBuffer(CommandBuffer commandBuffer)
        {
            this.commandBuffer = commandBuffer;
        }

        public unsafe override void DrawBatch(ref Batch batch)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine($"{nameof(DrawBatch)} Shader: {batch.Shader.Name}");
#endif

            var frameBuffer = this.currentSurface?.FrameBuffer ?? this.swapChainFrameBuffer;
            this.SetBuffers(ref batch);

            var renderPassDescription = new RenderPassDescription(frameBuffer, ClearValue.None);
            this.commandBuffer.BeginRenderPass(ref renderPassDescription);
            this.commandBuffer.SetViewports(new Viewport[] { new Viewport(0, 0, frameBuffer.Width, frameBuffer.Height) });

            // Workaround for DirectX12
            if (!batch.RenderState.ScissorEnable)
            {
                this.commandBuffer.SetScissorRectangles(new Rectangle[] { new Rectangle(0, 0, int.MaxValue, int.MaxValue) });
            }

            this.SetGraphicsPipelineAndResourceSet(ref batch, frameBuffer);

            //Set Index Buffer
            this.commandBuffer.SetIndexBuffer(this.indexBuffer.InternalBuffer);

            //Set Vertex Buffer
            uint offset = this.vertexBuffer.DrawPos + batch.VertexOffset;
            this.commandBuffer.SetVertexBuffer(0, this.vertexBuffer.InternalBuffer, offset);

            //Draw
            this.commandBuffer.DrawIndexed(batch.NumIndices, batch.StartIndex + this.indexBuffer.DrawPos / 2);

            this.commandBuffer.EndRenderPass();
        }

        private void SetGraphicsPipelineAndResourceSet(ref Batch batch, FrameBuffer framebuffer)
        {
            // Workaround for DirectX12
            var outputDescription = this.graphicsContext.IsDirectXBackend() ?
                this.renderTargetPipelineState.Description.Outputs :
                framebuffer.OutputDescription;
            var graphicPipeline = this.GetPipelineState(ref batch, outputDescription);
            var resourceSet = this.GetResourceSet(ref batch);

            this.commandBuffer.SetGraphicsPipelineState(graphicPipeline);
            this.commandBuffer.SetResourceSet(resourceSet);
        }

        private unsafe void SetBuffers(ref Batch batch)
        {
            // Vertex Constants
            if (this.vertexCBHash != batch.ProjMtxHash)
            {
                var prjMtx = Matrix4x4.Transpose(*(Matrix4x4*)batch.ProjMtx);
                this.commandBuffer.UpdateBufferData(this.vertexCB.InternalBuffer, ref prjMtx);
                this.vertexCBHash = batch.ProjMtxHash;
            }

            // Pixel Constants
            if (batch.Rgba != IntPtr.Zero || batch.RadialGrad != IntPtr.Zero || batch.Opacity != IntPtr.Zero)
            {
                uint hash = batch.RgbaHash ^ batch.RadialGradHash ^ batch.OpacityHash;
                if (this.pixelCBHash != hash)
                {
                    var pixelData = new float[12];
                    int idx = 0;

                    if (batch.Rgba != IntPtr.Zero)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            pixelData[idx++] = ((float*)batch.Rgba)[i];
                        }
                    }

                    if (batch.RadialGrad != IntPtr.Zero)
                    {
                        for (int i = 0; i < 8; ++i)
                        {
                            pixelData[idx++] = ((float*)batch.RadialGrad)[i];
                        }
                    }

                    if (batch.Opacity != IntPtr.Zero)
                    {
                        pixelData[idx++] = ((float*)batch.Opacity)[0];
                    }

                    this.commandBuffer.UpdateBufferData(this.pixelCB.InternalBuffer, pixelData);
                    this.pixelCBHash = hash;
                }
            }

            // Texture dimensions
            if (batch.Glyphs != null || batch.Image != null)
            {
                var texture = batch.Glyphs ?? batch.Image;
                uint hash = texture.Width << 16 | texture.Height;
                if (this.texDimensionsCBHash != hash)
                {
                    var data = new Vector4(texture.Width, texture.Height, 1f / texture.Width, 1f / texture.Height);
                    this.commandBuffer.UpdateBufferData(this.texDimensionsCB.InternalBuffer, ref data);
                    this.texDimensionsCBHash = hash;
                }
            }

            //Effects
            if (batch.EffectParamsSize != 0)
            {
                if (this.effectCBHash != batch.EffectParamsHash)
                {
                    float[] effectData = new float[16];
                    for (int i = 0; i < batch.EffectParamsSize; i++)
                    {
                        effectData[i] = ((float*)batch.EffectParams)[i];
                    }

                    if (batch.EffectParamsSize > 4)
                    {
                        // Workaround for buffer block pack bug on OpenGL and ESSL. Swap blurSize (4) and ShadowOffset (5, 6)
                        var blurSize = effectData[4];
                        effectData[4] = effectData[5];
                        effectData[5] = effectData[6];
                        effectData[6] = blurSize;
                    }

                    this.commandBuffer.UpdateBufferData(this.effectCB.InternalBuffer, effectData);
                    this.effectCBHash = batch.EffectParamsHash;
                }
            }
        }

        public unsafe override IntPtr MapVertices(uint bytes)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(MapVertices));
#endif
            return this.vertexBuffer.Map(bytes);
        }

        public unsafe override void UnmapVertices()
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(UnmapVertices));
#endif

            this.vertexBuffer.Unmap(this.commandBuffer);
        }

        public unsafe override IntPtr MapIndices(uint bytes)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(MapIndices));
#endif

            return this.indexBuffer.Map(bytes);
        }

        public unsafe override void UnmapIndices()
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(UnmapIndices));
#endif

            this.indexBuffer.Unmap(this.commandBuffer);
        }

        public override void BeginRender(bool offscreen)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine($"{nameof(BeginRender)} Offscreen: {offscreen}");
#endif
        }

        public override void EndRender()
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(EndRender));
#endif
            this.currentSurface = null;
        }

        public override Noesis.Texture CreateTexture(string label, uint width, uint height, uint numLevels, TextureFormat format, IntPtr data)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine($"{nameof(CreateTexture)}: {label} -> {width} {height} {numLevels} {format}");
#endif

            return WaveTexture.Create(this.graphicsContext, label, width, height, numLevels, ref format, data);
        }

        public override void UpdateTexture(Noesis.Texture texture, uint level, uint x, uint y, uint width, uint height, IntPtr data)
        {
            (texture as WaveTexture).UpdateTexture(level, x, y, width, height, data);
        }

        public override RenderTarget CreateRenderTarget(string label, uint width, uint height, uint sampleCount)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine($"{nameof(CreateRenderTarget)}: {label} -> {width} {height} {sampleCount}");
#endif

            return new WaveRenderTarget(this.graphicsContext, label, width, height, sampleCount);
        }

        public override RenderTarget CloneRenderTarget(string label, RenderTarget surface)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(CloneRenderTarget));
#endif

            var waveSurface = (WaveRenderTarget)surface;
            return waveSurface.Clone(this.graphicsContext, label);
        }

        public override void SetRenderTarget(RenderTarget surface)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(SetRenderTarget));
#endif

            this.currentSurface = (WaveRenderTarget)surface;
            var frameBuffer = this.currentSurface.FrameBuffer;
            this.currentSurface.SetRenderTarget(this.commandBuffer);
        }

        public override void BeginTile(ref Tile tile, uint surfaceWidth, uint surfaceHeight)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(BeginTile));
#endif

            int x = (int)tile.X;
            int y = (int)(surfaceHeight - (tile.Y + tile.Height));
            int width = (int)tile.Width;
            int height = (int)tile.Height;

            var rect = new Rectangle(x, y, width, height);
            var frameBuffer = this.currentSurface.FrameBuffer;

            var renderPassDescription = new RenderPassDescription(frameBuffer, ClearValue.None);
            this.commandBuffer.BeginRenderPass(ref renderPassDescription);

            this.commandBuffer.SetScissorRectangles(new Rectangle[] { rect });

            // Clear render target
            this.commandBuffer.SetGraphicsPipelineState(this.renderTargetPipelineState);
            this.commandBuffer.Draw(3);
            this.commandBuffer.EndRenderPass();
        }

        public override void EndTile()
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(EndTile));
#endif
        }

        public override void ResolveRenderTarget(RenderTarget surface, Tile[] tiles)
        {
#if TRACE_RENDER_DEVICE
            System.Diagnostics.Trace.WriteLine(nameof(ResolveRenderTarget));
#endif

            var waveSurface = (WaveRenderTarget)surface;
            if (waveSurface.SampleCount != 1)
            {
                throw new NotImplementedException();

                /*DX_BEGIN_EVENT(L"Resolve");

                SetInputLayout(0);
                SetVertexShader(mQuadVS);
                NS_ASSERT(surface->msaa - 1 < NS_COUNTOF(mResolvePS));
                SetPixelShader(mResolvePS[surface->msaa - 1]);

                SetRasterizerState(mRasterizerStates[2]);
                SetBlendState(mBlendStates[0]);
                SetDepthStencilState(mDepthStencilStates[0], 0);

                ClearTextures();
                mContext->OMSetRenderTargets(1, &surface->textureRTV, 0);
                SetTexture(0, surface->colorSRV);

                for (uint32_t i = 0; i < size; i++)
                {
                    const Tile&tile = tiles[i];

                    D3D11_RECT rect;
                    rect.left = tile.x;
                    rect.top = surface->height - (tile.y + tile.height);
                    rect.right = tile.x + tile.width;
                    rect.bottom = surface->height - tile.y;
                    mContext->RSSetScissorRects(1, &rect);

                    mContext->Draw(3, 0);
                }

                DX_END_EVENT();*/
            }
        }
    }
}
