using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Vortice.ShaderCompiler;
using Vortice.Vulkan;
using Vultaik;
using Vultaik.Desktop;
using Buffer = Vultaik.Buffer;

namespace testingShaders
{
    public unsafe class SampleApp
    {
        Window window;
        Device device;
        SwapChain swapChain;
        GraphicsContext context;
        Framebuffer framebuffer;

        GraphicsPipeline pipeline;
        DescriptorSet descriptor;

        Buffer constBuffer;
        
        public SampleApp()
        {
            Init();
            CreateBuffers();
            CreatePipeline();
                        
            window.Show();
            window.RenderLoop(OnRender);
        }

        void Init()
        {
            window = new Window("sample", 800, 600);

            var adapterconfig = new AdapterConfig()
            {
                SwapChain = true
            };
            var adapter = new Adapter(adapterconfig);

            device = new Device(adapter);
            swapChain = new(device, new()
            {
                Source = GetSwapchainSource(window, adapter),
                Width = 800,
                Height = 600,
                ColorSrgb = false,
                DepthFormat = adapter.DepthFormat is VkFormat.Undefined ? null : adapter.DepthFormat
            });

            context = new(device);
            framebuffer = new(swapChain);
        }

        void CreateBuffers()
        {
            constBuffer = new(device, new()
            {
                BufferFlags = BufferFlags.ConstantBuffer,
                Usage = ResourceUsage.CPU_To_GPU,
                SizeInBytes = Vultaik.Interop.SizeOf<Transform2D>()
            });


        }

        void CreatePipeline()
        {
            var compiler = new Compiler();

            var vertex = compiler.Compile(Shaders.vertex, string.Empty, ShaderKind.VertexShader).GetBytecode().ToArray();
            var frag = compiler.Compile(Shaders.fragment, string.Empty, ShaderKind.FragmentShader).GetBytecode().ToArray();

            GraphicsPipelineDescription pd = new();
            pd.SetFramebuffer(framebuffer);
            pd.SetShader(new ShaderBytecode(vertex, ShaderStage.Vertex, ShaderBackend.Glsl));
            pd.SetShader(new ShaderBytecode(frag, ShaderStage.Fragment, ShaderBackend.Glsl));
            pd.SetVertexBinding(VkVertexInputRate.Vertex, VertexPositionColor.Size);
            pd.AddVertexAttribute<VertexPositionColor>();
            pipeline = new(pd);

            DescriptorData dd = new();
            dd.SetUniformBuffer(1, constBuffer);
            descriptor = new(pipeline, dd);
        }


        void OnRender()
        {
            Console.WriteLine("c renderiza");
        }

        public SwapchainSource GetSwapchainSource(Window window, Adapter adapter)
        {
            if (adapter.SupportsSurface)
            {
                if (adapter.SupportsWin32Surface)
                    return window.SwapchainWin32!;

                if (adapter.SupportsX11Surface)
                    return window.SwapchainX11!;

                if (adapter.SupportsWaylandSurface)
                    return window.SwapchainWayland!;

                if (adapter.SupportsMacOSSurface)
                    return window.SwapchainNS!;
            }

            throw new PlatformNotSupportedException("Cannot create a SwapchainSource.");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Transform2D
    {
        public Matrix3x2 model;
        public Matrix4x4 view;
        public Matrix4x4 projection;
    }
}
