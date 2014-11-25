using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DirectInput;
using SlimDX.DXGI;
using SlimDX.Multimedia;
using SlimDX.Windows;


namespace Cube
{
    public class TriangleGameWindow : SlimFramework.GameWindow, IDisposable
    {

        // MEMBER VARIABLES
        // ======================================================================================================================

        // The basic Direct3D stuff.
        SlimDX.Direct3D11.Device m_Device; // The Direct3D device.
        SlimDX.Direct3D11.DeviceContext m_DeviceContext; // This is just a convenience member.  It holds the context for the Direct3D device.
        RenderTargetView m_RenderTargetView; // Our render target.
        SwapChain m_SwapChain; // Our swap chain.
        Viewport m_Viewport; // The viewport.

        InputLayout m_InputLayout;  // Tells Direct3D about the vertex format we are using.
        
        VertexShader m_VertexShader; // This is the vertex shader.
        ShaderSignature m_VShaderSignature; // The vertex shader signature.
        PixelShader m_PixelShader; // This is the pixel shader.


        SlimDX.Direct3D11.Buffer m_VertexBuffer; // This will hold our geometry.



        // CONSTRUCTORS
        // ======================================================================================================================

        /// <summary>
        /// This is the constructor.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="fullscreen">Whether the window should be fullscreen or not.</param>
        public TriangleGameWindow(string title, int width, int height, bool fullscreen)
            : base(title, width, height, fullscreen)
        {
            // Initialize Direct3D.
            InitD3D();

            // Initialize our Shaders.
            InitShaders();

            // Initialize our scene.
            InitScene();
        }




        // PUBLIC METHODS
        // ======================================================================================================================

        /// <summary>
        /// This method initializes Direct3D for us.
        /// </summary>
        public void InitD3D()
        {
            // Setup the configuration for the SwapChain.
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 2, // 2 back buffers (a.k.a. Triple Buffering).  Note that you should usually never use more than 4 buffers as you will get a performance decrease with too many.  In windowed mode, the Desktop is used as the frontbuffer, while in fullscreen mode the swap chain must include a dedicated front buffer.
                Usage = Usage.RenderTargetOutput,
                OutputHandle = FormObject.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(FormObject.Width, FormObject.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };


            // Create the SwapChain and check for errors.
            if (SlimDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware,
                                                         DeviceCreationFlags.Debug,
                                                         new FeatureLevel[] { FeatureLevel.Level_11_0 },
                                                         swapChainDesc,
                                                         out m_Device,
                                                         out m_SwapChain).IsFailure)
            {
                // An error has occurred.  Initialization of the Direct3D device has failed for some reason.
                return;
            }





            // Create a view of our render target, which is the backbuffer of the swap chain we just created
            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(m_SwapChain, 0))
            {
                m_RenderTargetView = new RenderTargetView(m_Device, resource);
            }


            // Get the device context and store it in our m_DeviceContext member variable.
            m_DeviceContext = m_Device.ImmediateContext;


            // Setting a viewport is required if you want to actually see anything
            m_Viewport = new Viewport(0.0f,
                                    0.0f,
                                    FormObject.Width,
                                    FormObject.Height,
                                    0.0f,
                                    1.0f);

            m_DeviceContext.Rasterizer.SetViewports(m_Viewport);
            m_DeviceContext.OutputMerger.SetTargets(m_RenderTargetView);


            // Prevent DXGI handling of Alt+Enter since it does not work properly with Winforms
            using (var factory = m_SwapChain.GetParent<Factory>())
            {
                factory.SetWindowAssociation(FormObject.Handle,
                                             WindowAssociationFlags.IgnoreAltEnter);
            }

        }


        /// <summary>
        /// This method initializes our vertex shader and pixel shader.
        /// </summary>
        public void InitShaders()
        {
            // Load and compile the vertex shader
            string vsCompileError = "Vertex Shader Compile Error!!!";
            using (var bytecode = ShaderBytecode.CompileFromFile("Effects.fx", 
                                                                 "Vertex_Shader", 
                                                                 "vs_4_0", 
                                                                 ShaderFlags.Debug, 
                                                                 SlimDX.D3DCompiler.EffectFlags.None, 
                                                                 null, 
                                                                 null, 
                                                                 out vsCompileError))
            {
                m_VShaderSignature = ShaderSignature.GetInputSignature(bytecode);
                m_VertexShader = new VertexShader(m_Device, bytecode);
            }


            // Load and compile the pixel shader
            string psCompileError = "Pixel Shader Compile Error!!!";
            using (var bytecode = ShaderBytecode.CompileFromFile("Effects.fx", 
                                                                 "Pixel_Shader", 
                                                                 "ps_4_0", 
                                                                 ShaderFlags.Debug, 
                                                                 SlimDX.D3DCompiler.EffectFlags.None, 
                                                                 null, 
                                                                 null, 
                                                                 out psCompileError))
            {
                m_PixelShader = new PixelShader(m_Device, bytecode);
            }


            // Set the shaders.
            m_DeviceContext.VertexShader.Set(m_VertexShader);
            m_DeviceContext.PixelShader.Set(m_PixelShader);

        }


        /// <summary>
        /// This method initializes our scene.
        /// </summary>
        public void InitScene()
        {

            // Create the vertices of our triangle.
            Vector3[] vertexData =
            {
                new Vector3(-0.5f,  0.5f,  0.5f),
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3( 0.0f, -0.5f,  0.5f),
            };


            // Create a DataStream object that we will use to put the vertices into the vertex buffer.
            using (DataStream DataStream = new DataStream(Vector3.SizeInBytes * 3, true, true))
            {
                DataStream.Position = 0;
                DataStream.Write(vertexData[0]);
                DataStream.Write(vertexData[1]);
                DataStream.Write(vertexData[2]);
                DataStream.Position = 0;


                // Create a description for the vertex buffer.
                BufferDescription bd = new BufferDescription();
                bd.Usage = ResourceUsage.Default;
                bd.SizeInBytes = Vector3.SizeInBytes * 3;
                bd.BindFlags = BindFlags.VertexBuffer;
                bd.CpuAccessFlags = CpuAccessFlags.None;
                bd.OptionFlags = ResourceOptionFlags.None;

                // Create the vertex buffer.
                m_VertexBuffer = new SlimDX.Direct3D11.Buffer(m_Device, DataStream, bd);
            }


            // Define the vertex format.
            // This tells Direct3D what information we are storing for each vertex, and how it is stored.
            InputElement[] InputElements = new InputElement[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, SlimDX.Direct3D11.InputClassification.PerVertexData, 0),
            };

            // Create the InputLayout using the vertex format we just created.
            m_InputLayout = new InputLayout(m_Device,
                                            m_VShaderSignature,
                                            InputElements);


            // Setup the InputAssembler stage of the Direct3D 11 graphics pipeline.
            m_DeviceContext.InputAssembler.InputLayout = m_InputLayout;
            m_DeviceContext.InputAssembler.SetVertexBuffers(0,
                                                            new VertexBufferBinding(m_VertexBuffer, 
                                                                                    Vector3.SizeInBytes,
                                                                                    0));
            // Set the Primitive Topology.
            m_DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;



        }



        /// <summary>
        /// This method is called once per frame and is where we update objects in our scene.
        /// <param name="frameTime">The amount of time (in seconds) that has elapsed since the previous update.</param>
        /// </summary>
        public override void UpdateScene(double frameTime)
        {
            base.UpdateScene(frameTime);


        }


        /// <summary>
        /// This function is called each time we need to render the next frame.
        /// </summary>
        public override void RenderScene()
        {
            if ((!this.IsInitialized) || this.IsDisposed)
            {
                return;
            }


            // Clear the screen before we draw the next frame.
            m_DeviceContext.ClearRenderTargetView(m_RenderTargetView, ClearColor);


            // Draw the triangle that we created in our vertex buffer.
            m_DeviceContext.Draw(3, 0);


            // Present the frame we just rendered to the user.
            m_SwapChain.Present(0,
                                PresentFlags.None);

        }


        /// <summary>
        /// This function toggles fullscreen mode.  It does nothing here (except for updating the m_bFullscreen member variable) because thats all it needs to do in this base class.
        /// This function is overridden by derived classes that need to add code to toggle fullscreen mode.
        /// </summary>
        public override void ToggleFullscreen()
        {
            base.ToggleFullscreen();

            m_SwapChain.IsFullScreen = this.IsFullscreen;
        }




        // INTERFACE METHODS
        // ======================================================================================================================

        // This section is for methods that are part of the interfaces that the class implements.

        /// <summary>
        /// This is the GameWindow class's internal Dispose() method that actually disposes of the game window class.
        /// </summary>
        /// <param name="disposing">If this method is being called by this class's own code, then the value true should be passed into this parameter to indicate that this method was not called by the Garbage Collector.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                /*
                * The following text is from MSDN  (http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx)
                * 
                * 
                * Dispose(bool disposing) executes in two distinct scenarios:
                * 
                * If disposing equals true, the method has been called directly or indirectly by a user's code and managed and unmanaged resources can be disposed.
                * If disposing equals false, the method has been called by the runtime from inside the finalizer and only unmanaged resources can be disposed. 
                * 
                * When an object is executing its finalization code, it should not reference other objects, because finalizers do not execute in any particular order. 
                * If an executing finalizer references another object that has already been finalized, the executing finalizer will fail.
                */
                if (disposing)
                {
                    // Unregister events


                    // get rid of managed resources
                    if (m_VertexShader != null)
                        m_VertexShader.Dispose();

                    if (m_PixelShader != null)
                        m_PixelShader.Dispose();

                    if (m_VertexBuffer != null)
                        m_VertexBuffer.Dispose();

                    if (m_SwapChain != null)
                        m_SwapChain.Dispose();

                    if (m_RenderTargetView != null)
                        m_RenderTargetView.Dispose();
                    
                    if (m_InputLayout != null)
                        m_InputLayout.Dispose();

                    if (m_Device != null)
                        m_Device.Dispose();

                    if (m_DeviceContext != null)
                        m_DeviceContext.Dispose();
                   
                }


                // get rid of unmanaged resources

            }


            base.Dispose(disposing);
        }

    }
}
