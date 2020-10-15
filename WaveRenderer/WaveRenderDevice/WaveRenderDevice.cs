using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Mathematics;
using WaveEngine.Platform;
using VisualTests.Runners.Common;
using Buffer = WaveEngine.Common.Graphics.Buffer;
using NoesisManagedRenderer;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace WaveRenderer.WaveRenderDevice
{
    public class WaveRenderDevice : ManagedRenderDevice
    {
        class DynamicBuffer
        {
            private readonly uint size;
            private uint pos;
            private uint drawPos;
            private readonly Buffer internalBuffer;

            public uint Size => this.size;

            public uint DrawPos => this.drawPos;

            public Buffer InternalBuffer => this.internalBuffer;

            public DynamicBuffer(GraphicsContext graphicsContext, uint size, BufferFlags flags)
            {
                this.size = size;
                var resourceUsage = flags == BufferFlags.ConstantBuffer ? ResourceUsage.Default : ResourceUsage.Dynamic;
                var resourceCpuAccess = flags == BufferFlags.ConstantBuffer ? ResourceCpuAccess.None : ResourceCpuAccess.Write;
                var bufferDescription = new BufferDescription(size, flags, resourceUsage, resourceCpuAccess);
                this.internalBuffer = graphicsContext.Factory.CreateBuffer(ref bufferDescription);
            }

            public IntPtr Map(GraphicsContext graphicsContext, uint size)
            {
                if (this.pos + size > this.size)
                {
                    this.pos = 0;
                }

                this.drawPos = this.pos;
                this.pos += size;
                var mappedResource = graphicsContext.MapMemory(this.internalBuffer, MapMode.Write);
                return IntPtr.Add(mappedResource.Data, (int)this.drawPos);
            }

            public void Unmap(GraphicsContext graphicsContext)
            {
                graphicsContext.UnmapMemory(this.internalBuffer);
            }
        };

        private GraphicsPipelineDescription[] graphicPipelineDescsByShader = new GraphicsPipelineDescription[NoesisShader.Formats.Length];
        private BlendStateDescription[] blendDescs;
        private RasterizerStateDescription[] rasterDescs;
        private DepthStencilStateDescription[] depthDescs;

        private Dictionary<int, GraphicsPipelineState> pipelineStateCache = new Dictionary<int, GraphicsPipelineState>();
        private Dictionary<int, ResourceSet> resourceSetsCache = new Dictionary<int, ResourceSet>();
        private Dictionary<NoesisSamplerState, SamplerState> samplerStateCache = new Dictionary<NoesisSamplerState, SamplerState>();

        private ResourceLayout resourceLayout;

        private GraphicsPipelineState renderTargetPipelineState;
        //private ResourceSet renderTargetResourceSet;

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

        public WaveRenderDevice(GraphicsContext graphicsContext)
            : base(new NoesisDeviceCaps() { SubpixelRendering = true }, flippedTextures: false)
        {
            this.graphicsContext = graphicsContext;

            this.CreateBuffers();
            this.CreateStatesObjects();
        }

        private void CreateBuffers()
        {
            vertexCBHash = 0;
            pixelCBHash = 0;
            effectCBHash = 0;
            texDimensionsCBHash = 0;

            this.vertexBuffer = new DynamicBuffer(this.graphicsContext, DYNAMIC_VB_SIZE, BufferFlags.VertexBuffer);
            this.indexBuffer = new DynamicBuffer(this.graphicsContext, DYNAMIC_IB_SIZE, BufferFlags.IndexBuffer);
            this.vertexCB = new DynamicBuffer(this.graphicsContext, 16 * sizeof(float), BufferFlags.ConstantBuffer);
            this.pixelCB = new DynamicBuffer(this.graphicsContext, 12 * sizeof(float), BufferFlags.ConstantBuffer);
            this.effectCB = new DynamicBuffer(this.graphicsContext, 16 * sizeof(float), BufferFlags.ConstantBuffer);
            this.texDimensionsCB = new DynamicBuffer(this.graphicsContext, 4 * sizeof(float), BufferFlags.ConstantBuffer);
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

            for (int shaderIndex = 0; shaderIndex < NoesisShader.Formats.Length; ++shaderIndex)
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

            int format = NoesisShader.Formats[shaderIndex];

            string vertexFilename = "";

            var vertexSB = new StringBuilder();
            if ((format & NoesisShader.Pos) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.Position));
                vertexFilename += "Pos";
            }
            if ((format & NoesisShader.Color) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.UByte4Normalized, ElementSemanticType.Color));
                vertexFilename += "Color";
                vertexSB.AppendLine("#define HAS_COLOR 1");
            }
            if ((format & NoesisShader.Tex0) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 0));
                vertexFilename += "Tex0";
                vertexSB.AppendLine("#define HAS_UV0 1");
            }
            if ((format & NoesisShader.Tex1) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 1));
                vertexFilename += "Tex1";
                vertexSB.AppendLine("#define HAS_UV1 1");
            }
            if ((format & NoesisShader.Tex2) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.UShort4Normalized, ElementSemanticType.TexCoord, 2));
                vertexFilename += "Tex2";
                vertexSB.AppendLine("#define HAS_UV2 1");
            }
            if ((format & NoesisShader.Coverage) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float, ElementSemanticType.TexCoord, 3));
                vertexFilename += "Coverage";
                vertexSB.AppendLine("#define HAS_COVERAGE 1");
            }
            if ((format & NoesisShader.SDF) != 0)
            {
                vertexFilename += "_SDF";
                vertexSB.AppendLine("#define GEN_ST1 1");
            }
            vertexFilename += "_VS";

            string pixelFilename = ((NoesisShader.Enum)shaderIndex).ToString() + "_FS";

            /*var pixelSB = new StringBuilder();
            var effectName = ((NoesisShader.Enum)shaderIndex).ToString().ToUpper()
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
                .Replace("register(b0)", "register(b2)"));

            var outputBasePath = "../../../../" + assetsDirectory.RootPath + "/" + noesisShadersPath;
            System.IO.File.WriteAllText(outputBasePath + vertexFilename + ".fx", vertexSB.ToString());
            System.IO.File.WriteAllText(outputBasePath + pixelFilename + ".fx", pixelSB.ToString());

            return;*/

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

            //var resourceLayoutDescription = new ResourceLayoutDescription();
            //resourceLayoutDescription.Elements = new LayoutElementDescription[0];
            //var resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);
            //var resourceSetDescription = new ResourceSetDescription(resourceLayout);

            //this.renderTargetResourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

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

        private GraphicsPipelineState GetPipelineState(ref NoesisBatch batch, OutputDescription outputDescription)
        {
            var renderState = batch.renderState;
            var shaderIndex = batch.shader.Index;
            var stencilRef = batch.stencilRef;

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
                result.Name = $"Noesis_{batch.shader.Name}_{rasterizerIndex}{blendIndex}{depthIndex}_{stencilRef}";
#endif

#if TRACE_RENDER_DEVICE
                Trace.WriteLine($"New Pipeline state -> {result.Name}");
#endif
            }

            return result;
        }

        private ResourceSet GetResourceSet(ref NoesisBatch batch)
        {
            var hash = new HashCode();
            hash.Add(batch.Pattern);
            hash.Add(batch.patternSampler);
            hash.Add(batch.Ramps);
            hash.Add(batch.rampsSampler);
            hash.Add(batch.Image);
            hash.Add(batch.imageSampler);
            hash.Add(batch.Glyphs);
            hash.Add(batch.glyphsSampler);
            hash.Add(batch.Shadow);
            hash.Add(batch.shadowSampler);

            var hashCode = hash.ToHashCode();
            if (!this.resourceSetsCache.TryGetValue(hashCode, out var result))
            {
                Texture patternTexture = ((WaveTexture)batch.Pattern)?.Texture;
                Texture rampsTexture = ((WaveTexture)batch.Ramps)?.Texture;
                Texture imageTexture = ((WaveTexture)batch.Image)?.Texture;
                Texture glyphsTexture = ((WaveTexture)batch.Glyphs)?.Texture;
                Texture shadowTexture = ((WaveTexture)batch.Shadow)?.Texture;

                var desc = new ResourceSetDescription(
                    this.resourceLayout,
                    vertexCB.InternalBuffer,
                    texDimensionsCB.InternalBuffer,
                    pixelCB.InternalBuffer,
                    effectCB.InternalBuffer,
                    patternTexture,
                    this.GetSamplerState(ref batch.patternSampler),
                    rampsTexture,
                    this.GetSamplerState(ref batch.rampsSampler),
                    imageTexture,
                    this.GetSamplerState(ref batch.imageSampler),
                    glyphsTexture,
                    this.GetSamplerState(ref batch.glyphsSampler),
                    shadowTexture,
                    this.GetSamplerState(ref batch.shadowSampler));

                result = this.graphicsContext.Factory.CreateResourceSet(ref desc);
                this.resourceSetsCache.Add(hashCode, result);
#if DEBUG
                result.Name = $"Noesis_{patternTexture?.Name ?? "NULL"} {rampsTexture?.Name ?? "NULL"} {imageTexture?.Name ?? "NULL"} {glyphsTexture?.Name ?? "NULL"} {shadowTexture?.Name ?? "NULL"}";
#endif

#if TRACE_RENDER_DEVICE
                Trace.WriteLine($"New Resource Set -> {result.Name}");
#endif
            }

            return result;
        }

        private SamplerState GetSamplerState(ref NoesisSamplerState sampler)
        {
            if (!this.samplerStateCache.TryGetValue(sampler, out var result))
            {
                var desc = SamplerStateDescription.Default;
                switch (sampler.WrapMode)
                {
                    case NoesisSamplerState.WrapModes.ClampToEdge:
                        desc.AddressU = TextureAddressMode.Clamp;
                        desc.AddressV = TextureAddressMode.Clamp;
                        break;
                    case NoesisSamplerState.WrapModes.ClampToZero:
                        desc.BorderColor = SamplerBorderColor.TransparentBlack;
                        desc.AddressU = TextureAddressMode.Border;
                        desc.AddressV = TextureAddressMode.Border;
                        break;
                    case NoesisSamplerState.WrapModes.Repeat:
                        desc.AddressU = TextureAddressMode.Wrap;
                        desc.AddressV = TextureAddressMode.Wrap;
                        break;
                    case NoesisSamplerState.WrapModes.MirrorU:
                        desc.AddressU = TextureAddressMode.Mirror;
                        desc.AddressV = TextureAddressMode.Wrap;
                        break;
                    case NoesisSamplerState.WrapModes.MirrorV:
                        desc.AddressU = TextureAddressMode.Wrap;
                        desc.AddressV = TextureAddressMode.Mirror;
                        break;
                    case NoesisSamplerState.WrapModes.Mirror:
                        desc.AddressU = TextureAddressMode.Mirror;
                        desc.AddressV = TextureAddressMode.Mirror;
                        break;
                    default:
                        throw new InvalidOperationException($"Undefined {nameof(sampler.WrapMode)}: {sampler.WrapMode}");
                }

                switch (sampler.MinMagFilter)
                {
                    case NoesisSamplerState.MinMagFilters.Nearest:
                        switch (sampler.MipFilter)
                        {
                            case NoesisSamplerState.MipFilters.Disabled:
                                desc.MaxLOD = 0;
                                desc.Filter = TextureFilter.MinPoint_MagPoint_MipPoint;
                                break;
                            case NoesisSamplerState.MipFilters.Nearest:
                                desc.Filter = TextureFilter.MinPoint_MagPoint_MipPoint;
                                break;
                            case NoesisSamplerState.MipFilters.Linear:
                                desc.Filter = TextureFilter.MinPoint_MagPoint_MipLinear;
                                break;
                            default:
                                throw new InvalidOperationException($"Undefined {nameof(sampler.MipFilter)}: {sampler.MipFilter}");
                        }
                        break;
                    case NoesisSamplerState.MinMagFilters.Linear:
                        switch (sampler.MipFilter)
                        {
                            case NoesisSamplerState.MipFilters.Disabled:
                                desc.MaxLOD = 0;
                                desc.Filter = TextureFilter.MinLinear_MagLinear_MipPoint;
                                break;
                            case NoesisSamplerState.MipFilters.Nearest:
                                desc.Filter = TextureFilter.MinLinear_MagLinear_MipPoint;
                                break;
                            case NoesisSamplerState.MipFilters.Linear:
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
                Trace.WriteLine($"New Sampler State -> {result.Name}");
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

        unsafe protected override void DrawBatch(ref NoesisBatch batch)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine($"{nameof(DrawBatch)} Shader: {batch.shader.Name}");
#endif
            var frameBuffer = this.currentSurface?.FrameBuffer ?? this.swapChainFrameBuffer;
            this.SetBuffers(ref batch);

            var renderPassDescription = new RenderPassDescription(frameBuffer, ClearValue.None);
            this.commandBuffer.BeginRenderPass(ref renderPassDescription);
            this.commandBuffer.SetViewports(new Viewport[] { new Viewport(0, 0, frameBuffer.Width, frameBuffer.Height) });

            // Workaround for DirectX12
            if (!batch.renderState.ScissorEnable)
            {
                this.commandBuffer.SetScissorRectangles(new Rectangle[] { new Rectangle(0, 0, int.MaxValue, int.MaxValue) });
            }

            this.SetGraphicsPipelineAndResourceSet(ref batch, frameBuffer);

            //Set Index Buffer
            this.commandBuffer.SetIndexBuffer(this.indexBuffer.InternalBuffer);

            //Set Vertex Buffer
            uint offset = this.vertexBuffer.DrawPos + batch.vertexOffset;
            this.commandBuffer.SetVertexBuffer(0, this.vertexBuffer.InternalBuffer, offset);

            //Draw
            this.commandBuffer.DrawIndexed(batch.numIndices, batch.startIndex + this.indexBuffer.DrawPos / 2);

            this.commandBuffer.EndRenderPass();
        }

        private void SetGraphicsPipelineAndResourceSet(ref NoesisBatch batch, FrameBuffer framebuffer)
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

        private unsafe void SetBuffers(ref NoesisBatch batch)
        {
            // Vertex Constants
            if (this.vertexCBHash != batch.projMtxHash)
            {
                var prjMtx = Matrix4x4.Transpose(*(Matrix4x4*)batch.projMtx);
                this.commandBuffer.UpdateBufferData(this.vertexCB.InternalBuffer, ref prjMtx);
                this.vertexCBHash = batch.projMtxHash;
            }

            // Pixel Constants
            if (batch.rgba != IntPtr.Zero || batch.radialGrad != IntPtr.Zero || batch.opacity != IntPtr.Zero)
            {
                uint hash = batch.rgbaHash ^ batch.radialGradHash ^ batch.opacityHash;
                if (this.pixelCBHash != hash)
                {
                    var pixelData = new float[12];
                    int idx = 0;

                    if (batch.rgba != IntPtr.Zero)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            pixelData[idx++] = ((float*)batch.rgba)[i];
                        }
                    }

                    if (batch.radialGrad != IntPtr.Zero)
                    {
                        for (int i = 0; i < 8; ++i)
                        {
                            pixelData[idx++] = ((float*)batch.radialGrad)[i];
                        }
                    }

                    if (batch.opacity != IntPtr.Zero)
                    {
                        pixelData[idx++] = ((float*)batch.opacity)[0];
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
                    Vector4 data = new Vector4(texture.Width, texture.Height, 1f / texture.Width, 1f / texture.Height);
                    this.commandBuffer.UpdateBufferData(this.texDimensionsCB.InternalBuffer, ref data);
                    this.texDimensionsCBHash = hash;
                }
            }

            //Effects
            if (batch.effectParamsSize != 0)
            {
                if (this.effectCBHash != batch.effectParamsHash)
                {
                    float[] effectData = new float[16];
                    for (int i = 0; i < batch.effectParamsSize; ++i)
                    {
                        effectData[i] = ((float*)batch.effectParams)[i];
                    }
                    this.commandBuffer.UpdateBufferData(this.effectCB.InternalBuffer, effectData);
                    this.effectCBHash = batch.effectParamsHash;
                }
            }
        }

        unsafe protected override IntPtr MapVertices(uint bytes)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(MapVertices));
#endif
            return this.vertexBuffer.Map(this.graphicsContext, bytes);
        }

        unsafe protected override void UnmapVertices()
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(UnmapVertices));
#endif

            this.vertexBuffer.Unmap(this.graphicsContext);
        }

        unsafe protected override IntPtr MapIndices(uint bytes)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(MapIndices));
#endif

            return this.indexBuffer.Map(this.graphicsContext, bytes);
        }

        unsafe protected override void UnmapIndices()
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(UnmapIndices));
#endif

            this.indexBuffer.Unmap(this.graphicsContext);
        }

        protected override void BeginRender(bool offscreen)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine($"{nameof(BeginRender)} Offscreen: {offscreen}");
#endif
        }

        protected override void EndRender()
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(EndRender));
#endif
            this.currentSurface = null;
        }

        protected override ManagedTexture CreateTexture(string label, uint width, uint height, uint numLevels, NoesisTextureFormat format)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine($"{nameof(CreateTexture)}: {label} -> {width} {height} {numLevels} {format}");
#endif

            return WaveTexture.Create(this.graphicsContext, label, width, height, numLevels, ref format, null);
        }

        protected override ManagedRenderTarget CreateRenderTarget(string label, uint width, uint height, uint sampleCount)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine($"{nameof(CreateRenderTarget)}: {label} -> {width} {height} {sampleCount}");
#endif

            return new WaveRenderTarget(this.graphicsContext, label, width, height, sampleCount);
        }

        protected override ManagedRenderTarget CloneRenderTarget(string label, ManagedRenderTarget surface)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(CloneRenderTarget));
#endif

            var waveSurface = (WaveRenderTarget)surface;
            return waveSurface.Clone(this.graphicsContext, label);
        }

        protected override void SetRenderTarget(ManagedRenderTarget surface)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(SetRenderTarget));
#endif

            this.currentSurface = (WaveRenderTarget)surface;
            var frameBuffer = this.currentSurface.FrameBuffer;
            this.currentSurface.SetRenderTarget(this.commandBuffer);
        }

        protected override void BeginTile(ref NoesisTile tile, uint surfaceWidth, uint surfaceHeight)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(BeginTile));
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

        protected override void EndTile()
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(EndTile));
#endif
        }

        protected override void ResolveRenderTarget(ManagedRenderTarget surface, NoesisTile[] tiles)
        {
#if TRACE_RENDER_DEVICE
            Trace.WriteLine(nameof(ResolveRenderTarget));
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
