// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using NoesisManagedRenderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VisualTests.Runners.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Mathematics;
using WaveRenderer.WaveRenderDevice;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace VisualTests.LowLevel.Tests
{
    public class WaveMain : VisualTestDefinition
    {
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
        private float timeAccum = 0.0f;

        private WaveRenderDevice waveRenderer;
        private List<IntPtr> noesisViews = new List<IntPtr>();

        public WaveMain()
            : base("DrawTriangle")
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);

            foreach (var view in this.noesisViews)
            {
                NoesisApp.SetViewSize(view, (int)width, (int)height);
            }
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

            await InitializeNoesis(width, height);

            this.MarkAsLoaded();
        }

        private async Task InitializeNoesis(uint width, uint height)
        {
            this.waveRenderer = new WaveRenderDevice(this.graphicsContext);
            await this.waveRenderer.InitializeAsync(this.assetsDirectory, this.frameBuffer);

            NoesisApp.NoesisInit();

            string xamlString = await this.assetsDirectory.ReadAsStringAsync("xamls/test.xaml");
            this.noesisViews.Add(NoesisApp.CreateView(this.waveRenderer, xamlString));

            xamlString = await this.assetsDirectory.ReadAsStringAsync("xamls/HelloWorld.xaml");
            this.noesisViews.Add(NoesisApp.CreateView(this.waveRenderer, xamlString));

            foreach (var view in this.noesisViews)
            {
                NoesisApp.SetViewSize(view, (int)width, (int)height);
            }

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
                foreach (var view in this.noesisViews)
                {
                    NoesisApp.ViewMouseMove(view, e.Position.X, e.Position.Y);
                }
            };

            mouseDispatcher.MouseScroll += (s, e) =>
            {
                foreach (var view in this.noesisViews)
                {
                    NoesisApp.ViewMouseWheel(view, e.Position.X, e.Position.Y, e.Delta.Y);
                }
            };
        }

        private void SendMouseButton(Func<IntPtr, int, int, int, bool> call, MouseButtonEventArgs args)
        {
            var buttons = args.Button;
            foreach (var view in this.noesisViews)
            {
                if ((buttons & MouseButtons.Left) != 0)
                {
                    call(view, args.Position.X, args.Position.Y, 0);
                }

                if ((buttons & MouseButtons.Right) != 0)
                {
                    call(view, args.Position.X, args.Position.Y, 1);
                }

                if ((buttons & MouseButtons.Middle) != 0)
                {
                    call(view, args.Position.X, args.Position.Y, 2);
                }

                if ((buttons & MouseButtons.XButton1) != 0)
                {
                    call(view, args.Position.X, args.Position.Y, 3);
                }

                if ((buttons & MouseButtons.XButton2) != 0)
                {
                    call(view, args.Position.X, args.Position.Y, 4);
                }
            }
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            var mouseDispatcher = this.surface.MouseDispatcher;
            mouseDispatcher.DispatchEvents();

            timeAccum += (float)gameTime.TotalSeconds;
            foreach (var view in this.noesisViews)
            {
                NoesisApp.UpdateView(view, timeAccum);
                NoesisApp.UpdateView(view, timeAccum);
            }

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

            foreach (var view in this.noesisViews)
            {
                NoesisApp.RenderView(view);
                NoesisApp.RenderView(view);
            }

            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
