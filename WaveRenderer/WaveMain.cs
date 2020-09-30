// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using NoesisManagedRenderer;
using System;
using System.Runtime.CompilerServices;
using VisualTests.Runners.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Mathematics;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace VisualTests.LowLevel.Tests
{
    public class WaveMain : VisualTestDefinition
    {
        private const string xamlString = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <Grid.Background>
                <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
                    <GradientStop Offset=""0"" Color=""#FF123F61""/>
                    <GradientStop Offset=""0.6"" Color=""#FF0E4B79""/>
                    <GradientStop Offset=""0.7"" Color=""#FF106097""/>
                </LinearGradientBrush>
            </Grid.Background>
            <Viewbox>
                <StackPanel Margin=""50"">
                    <Button Content=""Hello World!"" Margin=""0,30,0,0""/>
                    <Rectangle Height=""5"" Margin=""-10,20,-10,0"">
                        <Rectangle.Fill>
                            <RadialGradientBrush>
                                <GradientStop Offset=""0"" Color=""#40000000""/>
                                <GradientStop Offset=""1"" Color=""#00000000""/>
                            </RadialGradientBrush>
                        </Rectangle.Fill>
                    </Rectangle>
                </StackPanel>
            </Viewbox>
</Grid>";

        private Vector4[] vertexData = new Vector4[]
        {
            // TriangleList
            new Vector4(0f, 0.5f, 0.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(-0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
        };

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState;
        private Buffer[] vertexBuffers;

        private WaveRenderer.WaveRenderer waveRenderer;

        public WaveMain()
            : base("DrawTriangle")
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            NoesisApp.SetViewSize((int)width, (int)height);
        }

        protected override async void InternalLoad()
        {
            // Compile Vertex and Pixel shaders
            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS");

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            var vertexBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Vector4>() * (uint)this.vertexData.Length, BufferFlags.VertexBuffer, ResourceUsage.Default);
            var vertexBuffer = this.graphicsContext.Factory.CreateBuffer(this.vertexData, ref vertexBufferDescription);

            // Prepare Pipeline
            var vertexLayouts = new InputLayouts()
                  .Add(new LayoutDescription()
                              .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.Position))
                              .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.Color)));

            var pipelineDescription = new GraphicsPipelineDescription
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.CullBack,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.pipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
            this.commandQueue = this.graphicsContext.Factory.CreateCommandQueue();

            var swapChainDescription = this.swapChain?.SwapChainDescription;
            var width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            var height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors = new Rectangle[1];
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);

            this.vertexBuffers = new Buffer[1];
            this.vertexBuffers[0] = vertexBuffer;

            this.MarkAsLoaded();

            waveRenderer = new WaveRenderer.WaveRenderer(this.graphicsContext, assetsDirectory, frameBuffer);

            NoesisApp.NoesisInit(waveRenderer, xamlString);
            NoesisApp.SetViewSize((int)width, (int)height);


            var mouseDispatcher = this.surface.MouseDispatcher;
            mouseDispatcher.MouseButtonUp += (s, e) =>
            {
                this.SendMouseButton(NoesisApp.ViewMouseButtonUp, e);
            };

            mouseDispatcher.MouseButtonDown += (s, e) =>
            {
                this.SendMouseButton(NoesisApp.ViewMouseButtonDown, e);
            };

            mouseDispatcher.MouseMove += (s, e) =>
            {
                NoesisApp.ViewMouseMove(e.Position.X, e.Position.Y);
            };

            mouseDispatcher.MouseScroll += (s, e) =>
            {
                NoesisApp.ViewMouseWheel(e.Position.X, e.Position.Y, e.Delta.Y);
            };
        }

        private void SendMouseButton(Func<int, int, int, bool> call, MouseButtonEventArgs args)
        {
            var buttons = args.Button;
            if ((buttons & MouseButtons.Left) != 0)
            {
                call(args.Position.X, args.Position.Y, 0);
            }

            if ((buttons & MouseButtons.Right) != 0)
            {
                call(args.Position.X, args.Position.Y, 1);
            }

            if ((buttons & MouseButtons.Middle) != 0)
            {
                call(args.Position.X, args.Position.Y, 2);
            }

            if ((buttons & MouseButtons.XButton1) != 0)
            {
                call(args.Position.X, args.Position.Y, 3);
            }

            if ((buttons & MouseButtons.XButton2) != 0)
            {
                call(args.Position.X, args.Position.Y, 4);
            }
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            var mouseDispatcher = this.surface.MouseDispatcher;
            mouseDispatcher.DispatchEvents();

            NoesisApp.UpdateView((float)gameTime.TotalMilliseconds);

            var commandBuffer = this.commandQueue.CommandBuffer();
            waveRenderer.commandBuffer = commandBuffer;

            commandBuffer.Begin();

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.Target, Color.CornflowerBlue));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);

            commandBuffer.Draw((uint)this.vertexData.Length / 2);
            commandBuffer.EndRenderPass();

            NoesisApp.RenderView();

            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
