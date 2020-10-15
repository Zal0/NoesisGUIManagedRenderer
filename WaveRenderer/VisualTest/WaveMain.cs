// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using NoesisManagedRenderer;
using System;
using System.Collections.Generic;
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

        private Matrix4x4 view;
        private Matrix4x4 proj;

        private Buffer constantBuffer;

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState;
        private ResourceSet resourceSet;
        private Buffer[] vertexBuffers;
        private float time = 0.0f;

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
            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "FragmentShader", ShaderStages.Pixel, "PS");

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            var vertexBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Vector4>() * (uint)this.vertexData.Length, BufferFlags.VertexBuffer, ResourceUsage.Default);
            var vertexBuffer = this.graphicsContext.Factory.CreateBuffer(this.vertexData, ref vertexBufferDescription);

            this.view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), Vector3.UnitY);
            this.proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / (float)this.frameBuffer.Height, 0.1f, 100f);

            // Constant Buffer
            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            // Prepare Pipeline
            var vertexLayouts = new InputLayouts()
                  .Add(new LayoutDescription()
                              .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.Position))
                              .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.Color)));

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex));

            ResourceLayout resourcesLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(resourcesLayout, this.constantBuffer);
            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            var pipelineDescription = new GraphicsPipelineDescription
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                ResourceLayouts = new[] { resourcesLayout },
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.None,
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
            this.waveRenderer.SetSwapChainFrameBuffer(this.frameBuffer);
            await this.waveRenderer.InitializeAsync(this.assetsDirectory);

            NoesisApp.NoesisInit(string.Empty, string.Empty);

            //await this.AddViewAsync("xamls/test.xaml");
            //await this.AddViewAsync("xamls/HelloWorld.xaml");
            //await this.AddViewAsync("xamls/TextSample.xaml");
            //await this.AddViewAsync("xamls/Shadow.xaml");
            await this.AddViewAsync("xamls/Sample3D.xaml");
            //await this.AddViewAsync("xamls/Sample3D_Ortho.xaml");
            //await this.AddViewAsync("xamls/Login.xaml");
            //await this.AddViewAsync("xamls/Brushes.xaml");
            //await this.AddViewAsync("xamls/Detroit.xaml");
            //await this.AddViewAsync("xamls/Win10Login.xaml");

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

            var keyboardDispatcher = this.surface.KeyboardDispatcher;
            keyboardDispatcher.KeyDown += (s, e) =>
            {
                this.SendKey(NoesisApp.ViewKeyDown, e);
            };

            keyboardDispatcher.KeyUp += (s, e) =>
            {
                this.SendKey(NoesisApp.ViewKeyUp, e);
            };

            keyboardDispatcher.KeyChar += (s, e) =>
            {
                foreach (var view in this.noesisViews)
                {
                    NoesisApp.ViewChar(view, e.Character);
                }
            };
        }

        private async Task AddViewAsync(string path)
        {
            var xamlString = await this.assetsDirectory.ReadAsStringAsync(path);
            this.noesisViews.Add(NoesisApp.CreateView(this.waveRenderer, xamlString));
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

        private void SendKey(Func<IntPtr, int, bool> call, WaveEngine.Common.Input.Keyboard.KeyEventArgs args)
        {
            var key = args.Key;
            foreach (var view in this.noesisViews)
            {
                switch(key)
                {
                    case WaveEngine.Common.Input.Keyboard.Keys.Back:
                        call(view, 2);
                        break;
                    case WaveEngine.Common.Input.Keyboard.Keys.Tab:
                        call(view, 3);
                        break;
                    case WaveEngine.Common.Input.Keyboard.Keys.Enter:
                        call(view, 6);
                        break;
                    case WaveEngine.Common.Input.Keyboard.Keys.Escape:
                        call(view, 13);
                        break;
                    case WaveEngine.Common.Input.Keyboard.Keys.Delete:
                        call(view, 32);
                        break;
                }
            }
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            //this.time += 1f / 60f;
            this.view = Matrix4x4.CreateRotationY(this.time * 0.5f) * Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), Vector3.Up);
            var viewProj = Matrix4x4.Multiply(this.view, this.proj);

            this.surface.MouseDispatcher.DispatchEvents();
            this.surface.KeyboardDispatcher.DispatchEvents();

            foreach (var view in this.noesisViews)
            {
                NoesisApp.UpdateView(view, time);
            }

            // Draw
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            commandBuffer.UpdateBufferData(this.constantBuffer, ref viewProj);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, ClearValue.Default);
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetResourceSet(this.resourceSet);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);

            commandBuffer.Draw((uint)this.vertexData.Length / 2);
            commandBuffer.EndRenderPass();

            waveRenderer.SetCommandBuffer(commandBuffer);

            foreach (var view in this.noesisViews)
            {
                NoesisApp.RenderView(view);
            }

            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
