using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Mathematics;
using WaveEngine.Platform;
using VisualTests.Runners.Common;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace WaveRenderer
{
    class WaveRenderer : ManagedRenderDevice
    {
        // List of shaders to be implemented by the device with expected vertex format
        //
        //  Name       Format                   Size (bytes)      Semantic
        //  ---------------------------------------------------------------------------------
        //  Pos        R32G32_FLOAT             8                 Position (x,y)
        //  Color      R8G8B8A8_UNORM           4                 Color (rgba)
        //  Tex0       R32G32_FLOAT             8                 Texture (u,v)
        //  Tex1       R32G32_FLOAT             8                 Texture (u,v)
        //  Tex2       R16G16B16A16_UNORM       8                 Rect (x0,y0, x1,y1)
        //  Coverage   R32_FLOAT                4                 Coverage (x)

        const int Pos = 1 << 0;
        const int Color = 1 << 1;
        const int Tex0 = 1 << 2;
        const int Tex1 = 1 << 3;
        const int Tex2 = 1 << 4;
        const int Coverage = 1 << 5;
        const int SDF = 1 << 6;

        enum ShaderName
        {
            RGBA,
            Mask,

            Path_Solid,
            Path_Linear,
            Path_Radial,
            Path_Pattern,

            PathAA_Solid,
            PathAA_Linear,
            PathAA_Radial,
            PathAA_Pattern,

            SDF_Solid,
            SDF_Linear,
            SDF_Radial,
            SDF_Pattern,

            SDF_LCD_Solid,
            SDF_LCD_Linear,
            SDF_LCD_Radial,
            SDF_LCD_Pattern,

            Image_Opacity_Solid,
            Image_Opacity_Linear,
            Image_Opacity_Radial,
            Image_Opacity_Pattern,

            Image_Shadow35V,
            Image_Shadow63V,
            Image_Shadow127V,

            Image_Shadow35H_Solid,
            Image_Shadow35H_Linear,
            Image_Shadow35H_Radial,
            Image_Shadow35H_Pattern,

            Image_Shadow63H_Solid,
            Image_Shadow63H_Linear,
            Image_Shadow63H_Radial,
            Image_Shadow63H_Pattern,

            Image_Shadow127H_Solid,
            Image_Shadow127H_Linear,
            Image_Shadow127H_Radial,
            Image_Shadow127H_Pattern,

            Image_Blur35V,
            Image_Blur63V,
            Image_Blur127V,

            Image_Blur35H_Solid,
            Image_Blur35H_Linear,
            Image_Blur35H_Radial,
            Image_Blur35H_Pattern,

            Image_Blur63H_Solid,
            Image_Blur63H_Linear,
            Image_Blur63H_Radial,
            Image_Blur63H_Pattern,

            Image_Blur127H_Solid,
            Image_Blur127H_Linear,
            Image_Blur127H_Radial,
            Image_Blur127H_Pattern,
        }

        int[] formats =
        {
            Pos,                                 //RGBA,                      
            Pos,                                 //Mask,                      

            Pos | Color,                         //Path_Solid,                
            Pos | Tex0,                          //Path_Linear,               
            Pos | Tex0,                          //Path_Radial,               
            Pos | Tex0,                          //Path_Pattern,              

            Pos | Color | Coverage,              //PathAA_Solid,              
            Pos | Tex0  | Coverage,              //PathAA_Linear,             
            Pos | Tex0  | Coverage,              //PathAA_Radial,             
            Pos | Tex0  | Coverage,              //PathAA_Pattern,            

            Pos | Color | Tex1 | SDF,            //SDF_Solid,                 
            Pos | Tex0  | Tex1 | SDF,            //SDF_Linear,                
            Pos | Tex0  | Tex1 | SDF,            //SDF_Radial,                
            Pos | Tex0  | Tex1 | SDF,            //SDF_Pattern,               

            Pos | Color | Tex1 | SDF,            //SDF_LCD_Solid,             
            Pos | Tex0  | Tex1 | SDF,            //SDF_LCD_Linear,            
            Pos | Tex0  | Tex1 | SDF,            //SDF_LCD_Radial,            
            Pos | Tex0  | Tex1 | SDF,            //SDF_LCD_Pattern,           

            Pos | Color | Tex1,                  //Image_Opacity_Solid,       
            Pos | Tex0  | Tex1,                  //Image_Opacity_Linear,      
            Pos | Tex0  | Tex1,                  //Image_Opacity_Radial,      
            Pos | Tex0  | Tex1,                  //Image_Opacity_Pattern,     

            Pos | Color | Tex1 | Tex2,           //Image_Shadow35V,           
            Pos | Color | Tex1 | Tex2,           //Image_Shadow63V,           
            Pos | Color | Tex1 | Tex2,           //Image_Shadow127V,          

            Pos | Color | Tex1 | Tex2,           //Image_Shadow35H_Solid,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Linear,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Radial,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Pattern,   

            Pos | Color | Tex1 | Tex2,           //Image_Shadow63H_Solid,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Linear,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Radial,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Pattern,   

            Pos | Color | Tex1 | Tex2,           //Image_Shadow127H_Solid,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Linear,   
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Radial,   
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Pattern,  

            Pos | Color | Tex1 | Tex2,           //Image_Blur35V,             
            Pos | Color | Tex1 | Tex2,           //Image_Blur63V,             
            Pos | Color | Tex1 | Tex2,           //Image_Blur127V,            

            Pos | Color | Tex1 | Tex2,           //Image_Blur35H_Solid,       
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Linear,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Radial,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Pattern,     

            Pos | Color | Tex1 | Tex2,           //Image_Blur63H_Solid,       
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Linear,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Radial,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Pattern,     

            Pos | Color | Tex1 | Tex2,           //Image_Blur127H_Solid,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Linear,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Radial,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Pattern,    
        };

        GraphicsPipelineState[] graphicPipelineStates = new GraphicsPipelineState[Enum.GetNames(typeof(ShaderName)).Length];

        ResourceSet[] resourceSets = new ResourceSet[Enum.GetNames(typeof(ShaderName)).Length];

        async void InitGraphicsPipelineState(int shader)
        {
            InputLayouts vertexLayouts = new InputLayouts();
            LayoutDescription layoutDescription = new LayoutDescription();
            vertexLayouts.Add(layoutDescription);

            int format = formats[shader];

            string vertexShaderPath = "";

            if ((format & Pos) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.Position));
                vertexShaderPath += "Pos";
            }
            if ((format & Color) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.UByte4Normalized, ElementSemanticType.Color));
                vertexShaderPath += "Color";
            }
            if ((format & Tex0) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 0));
                vertexShaderPath += "Tex0";
            }
            if ((format & Tex1) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 1));
                vertexShaderPath += "Tex1";
            }
            if ((format & Tex2) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 2));
                vertexShaderPath += "Tex2";
            }
            if ((format & Coverage) != 0)
            {
                layoutDescription.Add(new ElementDescription(ElementFormat.Float, ElementSemanticType.TexCoord, 3));
                vertexShaderPath += "Coverage";
            }
            if ((format & SDF) != 0)
            {
                vertexShaderPath += "_SDF";
            }
            vertexShaderPath += "_VS";

            string pixelShaderPath = ((ShaderName)shader).ToString() + "_FS";
            string vsEntryPoint = "main";
            string psEntryPoint = "main";
            if (shader != 6 && shader != 7 && shader != 8 && shader != 10)
            {
                vertexShaderPath = "HLSLVertex";
                pixelShaderPath = "HLSLVertex";
                vsEntryPoint = "VS";
                psEntryPoint = "PS";
            }

            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, vertexShaderPath, "VertexShader", ShaderStages.Vertex, vsEntryPoint);
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, pixelShaderPath, "FragmentShader", ShaderStages.Pixel, psEntryPoint);
            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            var resourceLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                new LayoutElementDescription(1, ResourceType.ConstantBuffer, ShaderStages.Vertex)
            );

            var resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);

            var resourceSetDescription = new ResourceSetDescription(
                resourceLayout, prjMtxBuffer, textureSizeBuffer
            );

            resourceSets[shader] = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                ResourceLayouts = new[] { resourceLayout },
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.None,
                    BlendState = BlendStates.AlphaBlend,
                    DepthStencilState = DepthStencilStates.None,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            graphicPipelineStates[shader] = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
        }

        public CommandBuffer commandBuffer;
        public GraphicsContext graphicsContext { private set; get; }
        AssetsDirectory assetsDirectory;
        FrameBuffer frameBuffer;

        Buffer[] vertexBuffers;
        Buffer indexBuffer;

        //constant buffers
        Buffer prjMtxBuffer;
        Buffer textureSizeBuffer;
        IntPtr[] texturePtrs = new IntPtr[5];

        MappedResource vertexBufferWritableResource;
        MappedResource indexBufferWritableResource;

        public static WaveRenderer Instance { private set; get; }

        public WaveRenderer(GraphicsContext graphicsContext, AssetsDirectory assetsDirectory, FrameBuffer frameBuffer)
        {
            Instance = this;

            this.graphicsContext = graphicsContext;
            this.assetsDirectory = assetsDirectory;
            this.frameBuffer = frameBuffer;

            var constantBufferDescription = new BufferDescription(64, BufferFlags.ConstantBuffer, ResourceUsage.Default);
            prjMtxBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            constantBufferDescription = new BufferDescription(16, BufferFlags.ConstantBuffer, ResourceUsage.Default);
            textureSizeBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            for (int i = 0; i < formats.Length; ++i)
            {
                InitGraphicsPipelineState(i);
            }
        }

        void SetTexture(IntPtr texturePtr, uint slot, byte sampler)
        {
            if(texturePtr != IntPtr.Zero /*&& texturePtrs[slot] != texturePtr*/)
            {
                WaveTexture texture = (WaveTexture)ManagedRenderDevice.textures[texturePtr];
                if (texture.resourceSet == null)
                {
                    texture.SetResourceSet(this.graphicsContext, slot, sampler);
                }

                texturePtrs[slot] = texturePtr;
                commandBuffer.SetResourceSet(texture.resourceSet);
            }
        }

        public override void DrawBatch(ref Batch batch)
        {
            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, ClearValue.None);
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            //Update textures
            SetTexture(batch.pattern, 0, batch.patternSampler);
            SetTexture(batch.ramps, 1, batch.rampsSampler);
            SetTexture(batch.image, 2, batch.imageSampler);
            SetTexture(batch.glyphs, 3, batch.glyphsSampler);
            SetTexture(batch.shadow, 4, batch.shadowSampler);

            //Set graphics pipeline
            commandBuffer.SetGraphicsPipelineState(graphicPipelineStates[batch.shader]);

            //Set resource set
            commandBuffer.SetResourceSet(resourceSets[batch.shader]);

            //Set Vertex Buffer
            int[] offsets = { (int)batch.vertexOffset };
            commandBuffer.SetVertexBuffers(vertexBuffers, offsets);

            //Set Index Buffer
            commandBuffer.SetIndexBuffer(indexBuffer);

            //Draw
            commandBuffer.DrawIndexed(batch.numIndices, batch.startIndex);

            commandBuffer.EndRenderPass();
        }
        
        unsafe public override IntPtr MapVertices(UInt32 bytes)
        {
            if (vertexBuffers == null || bytes > vertexBuffers[0].Description.SizeInBytes)
            {
                if (vertexBuffers != null)
                {
                    foreach(Buffer b in vertexBuffers)
                        b.Dispose();
                }

                var vertexBufferDescription = new BufferDescription(
                    bytes,
                    BufferFlags.VertexBuffer,
                    ResourceUsage.Dynamic,
                    ResourceCpuAccess.Write);

                var vertexBuffer = graphicsContext.Factory.CreateBuffer(ref vertexBufferDescription);
                vertexBuffers = new Buffer[] { vertexBuffer };    
            }

            vertexBufferWritableResource = graphicsContext.MapMemory(vertexBuffers[0], MapMode.Write);
            return vertexBufferWritableResource.Data;
        }

        unsafe public override void UnmapVertices()
        {
            graphicsContext.UnmapMemory(vertexBuffers[0]);
        }

        unsafe public override IntPtr MapIndices(uint bytes)
        {
            UInt32 size = bytes / sizeof(UInt16);
            if (indexBuffer == null || size > indexBuffer.Description.SizeInBytes)
            {
                if (indexBuffer != null)
                {
                    indexBuffer.Dispose();
                }

                var indexBufferDescription = new BufferDescription(
                    bytes,
                    BufferFlags.VertexBuffer,
                    ResourceUsage.Dynamic,
                    ResourceCpuAccess.Write);

                indexBuffer = graphicsContext.Factory.CreateBuffer(ref indexBufferDescription);
            }

            indexBufferWritableResource = graphicsContext.MapMemory(indexBuffer, MapMode.Write);
            return indexBufferWritableResource.Data;
        }

        unsafe public override void UnmapIndices()
        {
            graphicsContext.UnmapMemory(indexBuffer);
        }

        public override void BeginRender()
        {
            Matrix4x4 prjMtx = Matrix4x4.CreateOrthographicOffCenter(0.0f, frameBuffer.Width, 0.0f, frameBuffer.Height, 0.0f, 1.0f);
            commandBuffer.UpdateBufferData(this.prjMtxBuffer, ref prjMtx);

            Vector2 textureSize = new Vector2(1024, 1024); //TODO
            commandBuffer.UpdateBufferData(this.textureSizeBuffer, ref textureSize);
        }

        public override void EndRender()
        {
            
        }

        public override ManagedTexture CreateTexture()
        {
            return new WaveTexture();
        }
    }
}
