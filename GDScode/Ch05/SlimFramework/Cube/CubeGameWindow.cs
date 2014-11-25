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
 


// ********************************************************************************************************************************************
// CONTROLS
// ********************************************************************************************************************************************
// Press the up arrow or the W key to move forward.
// Press the down arrow or the S key to move backward.
// ********************************************************************************************************************************************



namespace Cube
{
    public class CubeGameWindow : SlimFramework.GameWindow, IDisposable
    {
        // This is used to specify how we want to draw the cube.
        enum GraphicsMode
        {
            SolidBlue = 0,
            PerVertexColoring,
            Textured
        }

        // This struct stores data for a single vertex.
        struct Vertex
        {
            public Vector4 Position;   // The position of the vertex in 3D space.
            public Color4 Color;       // The color to use for this vertex when we are not using textured mode.
            public Vector2 TexCoord;   // The textures coordinates for this vertex.  We need these when we are using textured mode.
        }



        // MEMBER VARIABLES
        // ======================================================================================================================


        // The basic Direct3D stuff.
        // ==============================================================================================================================================
        SlimDX.Direct3D11.Device m_Device; // The Direct3D device.
        SlimDX.Direct3D11.DeviceContext m_DeviceContext; // This is just a convenience member.  It holds the context for the Direct3D device.
        RenderTargetView m_RenderTargetView; // Our render target.
        SwapChain m_SwapChain; // Our swap chain.
        Viewport m_Viewport; // The viewport.

        InputLayout m_InputLayout;  // Tells Direct3D about the vertex format we are using.
        
        VertexShader m_VertexShader; // This is the vertex shader.
        ShaderSignature m_VShaderSignature; // The vertex shader signature.
        PixelShader m_PixelShader; // This is the pixel shader.


        // Constant Buffers
        // ==============================================================================================================================================
        SlimDX.Direct3D11.Buffer m_CbChangesOnResize;
        SlimDX.Direct3D11.Buffer m_CbChangesPerFrame;
        SlimDX.Direct3D11.Buffer m_CbChangesPerObject;
        
        // We use this to send data into the constant buffers.
        DataStream m_DataStream;




        // Matrices
        // ==============================================================================================================================================
        Matrix m_ViewMatrix;  // This is our view matrix.
        Matrix m_ProjectionMatrix;  // The projection matrix.

        Matrix m_CubeWorldMatrix;   // The world matrix for the cube.  This controls the current position and rotation of the cube.
        Matrix m_CubeRotationMatrix;  // This matrix controls the rotation of our cube.
        



        // Depth Stencil vars
        // ==============================================================================================================================================
        Texture2D m_DepthStencilTexture = null;     // Holds the depth stencil texture.
        DepthStencilView m_DepthStencilView = null; // The depth stencil view object.


        // Sampler vars.
        // ==============================================================================================================================================
        ShaderResourceView m_CubeTexture;        // Holds the texture for our cube.
        SamplerState m_CubeTexSamplerState;      // The sampler state we will use with our cube texture.


        // Other vars
        // ==============================================================================================================================================
        SlimDX.Direct3D11.Buffer m_VertexBuffer; // This will hold our geometry.

        Vector3 m_CameraPosition = new Vector3(0, 2, -5); // The position of our camera in 3D space.

        float m_CubeRotation = 0.005f; // The current rotation amount for the cube on the Y axis.
        float m_MoveSpeed = 0.01f;     // Sets the speed you move around at.

        // Sets how the program will render our cube.
        GraphicsMode m_GraphicsMode = GraphicsMode.Textured;




        // CONSTRUCTORS
        // ======================================================================================================================

        /// <summary>
        /// This is the constructor.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="fullscreen">Whether the window should be fullscreen or not.</param>
        public CubeGameWindow(string title, int width, int height, bool fullscreen)
            : base(title, width, height, fullscreen)
        {
            // Initialize Direct3D.
            InitD3D();

            // Initialize our Shaders.
            InitShaders();

            // Initialize our scene.
            InitScene();

            // Initialize the depth stencil.
            InitDepthStencil();

            // Initilize the constant buffers.
            InitConstantBuffers();
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
                BufferCount = 2, // 2 back buffers (a.k.a. Triple Buffering).
                Usage = Usage.RenderTargetOutput,
                OutputHandle = FormObject.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
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
            };


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
            };

        }



        /// <summary>
        /// This method initializes our constant buffers.
        /// </summary>
        public void InitConstantBuffers()
        {
            // Create a buffer description.
            BufferDescription bd = new BufferDescription();
            bd.Usage = ResourceUsage.Default;
            bd.BindFlags = BindFlags.ConstantBuffer;
            bd.CpuAccessFlags = CpuAccessFlags.None;
            bd.SizeInBytes = 64;


            // Create the changes on resize buffer.
            m_CbChangesOnResize = new SlimDX.Direct3D11.Buffer(m_Device, bd);

            // Create the changes per frame buffer.
            m_CbChangesPerFrame = new SlimDX.Direct3D11.Buffer(m_Device, bd);

            // Create the changes per object buffer.
            m_CbChangesPerObject = new SlimDX.Direct3D11.Buffer(m_Device, bd);


            // Send the Projection matrix into the changes on resize constant buffer.
            m_DataStream = new DataStream(64, true, true);
            m_DataStream.Position = 0;
            m_DataStream.Write(Matrix.Transpose(m_ProjectionMatrix));
            m_DataStream.Position = 0;
            m_Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, m_DataStream),
                                                        m_CbChangesOnResize,
                                                        0);


            // Send the View matrix into the changes per frame buffer.
            m_DataStream.Position = 0;
            m_DataStream.Write(Matrix.Transpose(m_ViewMatrix));
            m_DataStream.Position = 0;
            m_Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, m_DataStream),
                                                        m_CbChangesPerFrame,
                                                        0);

            // Tell the VertexShader to use our constant buffers.
            m_DeviceContext.VertexShader.SetConstantBuffer(m_CbChangesOnResize, 0);
            m_DeviceContext.VertexShader.SetConstantBuffer(m_CbChangesPerFrame, 1);
            m_DeviceContext.VertexShader.SetConstantBuffer(m_CbChangesPerObject, 2);

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
            string pixelShaderName = "";
            if (m_GraphicsMode == GraphicsMode.SolidBlue)
                pixelShaderName = "Pixel_Shader_Blue";
            else if (m_GraphicsMode == GraphicsMode.PerVertexColoring)
                pixelShaderName = "Pixel_Shader_Color";
            else if (m_GraphicsMode == GraphicsMode.Textured)
                pixelShaderName = "Pixel_Shader_Texture";

            string psCompileError = "Pixel Shader Compile Error!!!";
            using (var bytecode = ShaderBytecode.CompileFromFile("Effects.fx", 
                                                                 pixelShaderName, 
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
            // Create our projection matrix.
            m_ProjectionMatrix = Matrix.PerspectiveFovLH(1.570796f, // this is 90 degrees in radians
                                                         (float) FormObject.Width / (float) FormObject.Height,
                                                         0.5f,
                                                         1000.0f);


            // Create our view matrix.
            m_ViewMatrix = Matrix.LookAtLH(m_CameraPosition,
                                           new Vector3(0, 0, 0),
                                           new Vector3(0, 1, 0));



            // Create the vertices of our cube.
            Vertex[] vertexData =
            {
                // Bottom face of the cube.
                new Vertex() { Position = new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(1, 0) },

                new Vertex() { Position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f), TexCoord = new Vector2(1, 1) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(1, 0) },


                // Front face of the cube.
                new Vertex() { Position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f), TexCoord = new Vector2(1, 1) },

                new Vertex() { Position = new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(0.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(1, 0) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f), TexCoord = new Vector2(1, 1) },


                // Right face of the cube.
                new Vertex() { Position = new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 1.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(1, 1) },

                new Vertex() { Position = new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(1, 0) },
                new Vertex() { Position = new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(1, 1) },


                // Back face of the cube.
                new Vertex() { Position = new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(1, 1) },

                new Vertex() { Position = new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f), TexCoord = new Vector2(1, 0) },
                new Vertex() { Position = new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(1, 1) },


                // Left face of the cube.
                new Vertex() { Position = new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(1, 1) },

                new Vertex() { Position = new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(1, 0) },
                new Vertex() { Position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(1, 1) },


                // Top face of the cube.
                new Vertex() { Position = new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 0.0f, 1.0f), TexCoord = new Vector2(0, 1) },
                new Vertex() { Position = new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(1, 1) },

                new Vertex() { Position = new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 1.0f, 0.0f), TexCoord = new Vector2(0, 0) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), Color = new Color4(1.0f, 1.0f, 0.0f, 0.0f), TexCoord = new Vector2(1, 0) },
                new Vertex() { Position = new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), Color = new Color4(1.0f, 0.0f, 1.0f, 0.0f), TexCoord = new Vector2(1, 1) },
            };


            // Create a DataStream object that we will use to put the vertices into the vertex buffer.
            DataStream DataStream = new DataStream(40 * vertexData.Length, true, true);
            DataStream.Position = 0;
            foreach (Vertex v in vertexData)
                DataStream.Write(v);
            DataStream.Position = 0;


            // Create a description for the vertex buffer.
            BufferDescription bd = new BufferDescription();
            bd.Usage = ResourceUsage.Default;
            bd.SizeInBytes = 40 * vertexData.Length;
            bd.BindFlags = BindFlags.VertexBuffer;
            bd.CpuAccessFlags = CpuAccessFlags.None;
            bd.OptionFlags = ResourceOptionFlags.None;
            bd.StructureByteStride = 40;

            // Create the vertex buffer.
            m_VertexBuffer = new SlimDX.Direct3D11.Buffer(m_Device, DataStream, bd);

            // Dispose of the DataStream since we no longer need it.
            DataStream.Dispose();


            // Define the vertex format.
            // This tells Direct3D what information we are storing for each vertex, and how it is stored.
            InputElement[] InputElements = new InputElement[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0)
            };

            // Create the InputLayout using the vertex format we just created.
            m_InputLayout = new InputLayout(m_Device,
                                            m_VShaderSignature,
                                            InputElements);


            // Setup the InputAssembler stage of the Direct3D 11 graphics pipeline.
            m_DeviceContext.InputAssembler.InputLayout = m_InputLayout;
            m_DeviceContext.InputAssembler.SetVertexBuffers(0,
                                                            new VertexBufferBinding(m_VertexBuffer, 
                                                                                    40,
                                                                                    0));
            // Set the Primitive Topology.
            m_DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            


            // Load the cube texture.
            m_CubeTexture = ShaderResourceView.FromFile(m_Device, Application.StartupPath + "\\Brick.png");

            // Create a SamplerDescription
            SamplerDescription sd = new SamplerDescription();
            sd.Filter = Filter.MinMagMipLinear;
            sd.AddressU = TextureAddressMode.Wrap;
            sd.AddressV = TextureAddressMode.Wrap;
            sd.AddressW = TextureAddressMode.Wrap;
            sd.ComparisonFunction = Comparison.Never;
            sd.MinimumLod = 0;
            sd.MaximumLod = float.MaxValue;

            // Create our SamplerState
            m_CubeTexSamplerState = SamplerState.FromDescription(m_Device, sd);

        }


        /// <summary>
        /// This method initializes the depth stencil for us.
        /// </summary>
        public void InitDepthStencil()
        {
            // Create the depth stencil texture description
            Texture2DDescription DepthStencilTextureDesc = new Texture2DDescription();
            DepthStencilTextureDesc.Width = FormObject.ClientSize.Width;
            DepthStencilTextureDesc.Height = FormObject.ClientSize.Height;
            DepthStencilTextureDesc.MipLevels = 1;
            DepthStencilTextureDesc.ArraySize = 1;
            DepthStencilTextureDesc.Format = Format.D24_UNorm_S8_UInt;
            DepthStencilTextureDesc.SampleDescription = new SampleDescription(1, 0);
            DepthStencilTextureDesc.Usage = ResourceUsage.Default;
            DepthStencilTextureDesc.BindFlags = BindFlags.DepthStencil;
            DepthStencilTextureDesc.CpuAccessFlags = CpuAccessFlags.None;
            DepthStencilTextureDesc.OptionFlags = ResourceOptionFlags.None;


            // Create the Depth Stencil View description
            DepthStencilViewDescription DepthStencilViewDesc = new DepthStencilViewDescription();
            DepthStencilViewDesc.Format = DepthStencilTextureDesc.Format;
            DepthStencilViewDesc.Dimension = DepthStencilViewDimension.Texture2D;
            DepthStencilViewDesc.MipSlice = 0;



            // Create the depth stencil texture.
            m_DepthStencilTexture = new Texture2D(m_Device, DepthStencilTextureDesc);


            // Create the DepthStencilView object.
            m_DepthStencilView = new DepthStencilView(m_Device, m_DepthStencilTexture, DepthStencilViewDesc);


            // Make the DepthStencilView active.
            m_DeviceContext.OutputMerger.SetTargets(m_DepthStencilView, m_RenderTargetView);
        }


        /// <summary>
        /// This method is called once per frame and is where we update objects in our scene.
        /// <param name="frameTime">The amount of time (in seconds) that has elapsed since the previous update.</param>
        /// </summary>
        public override void UpdateScene(double frameTime)
        {
            base.UpdateScene(frameTime);


            
            // Keep the cube rotating by increasing its rotation amount
            m_CubeRotation += 0.00025f;
            if (m_CubeRotation > 6.28f) // 2 times PI
                m_CubeRotation = 0.0f;
            

            // Check for user input.

            // If the player pressed forward.
            if (UserInput.IsKeyPressed(Key.UpArrow) ||
                UserInput.IsKeyPressed(Key.W))
            {
                m_CameraPosition.Z = m_CameraPosition.Z + m_MoveSpeed;
            }

            // If the player pressed back.
            if (UserInput.IsKeyPressed(Key.DownArrow) ||
                UserInput.IsKeyPressed(Key.S))
            {
                m_CameraPosition.Z = m_CameraPosition.Z - m_MoveSpeed;
            }


            // Update the view matrix.
            m_ViewMatrix = Matrix.LookAtLH(m_CameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));

            // Send the updated view matrix into its constant buffer.
            m_DataStream.Position = 0;
            m_DataStream.Write(Matrix.Transpose(m_ViewMatrix));
            m_DataStream.Position = 0;
            m_Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, m_DataStream),
                                                        m_CbChangesPerFrame,
                                                        0);





            // Update the cube's rotation matrix.
            m_CubeRotationMatrix = Matrix.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), m_CubeRotation);


            // Update the cube's world matrix with the new translation and rotation matrices.
            m_CubeWorldMatrix = m_CubeRotationMatrix;
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
            m_DeviceContext.ClearDepthStencilView(m_DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);


            m_DeviceContext.PixelShader.SetShaderResource(m_CubeTexture, 0);
            m_DeviceContext.PixelShader.SetSampler(m_CubeTexSamplerState, 0);



            // Send the cube's world matrix to the changes per object constant buffer.
            m_DataStream.Position = 0;
            m_DataStream.Write(Matrix.Transpose(m_CubeWorldMatrix));
            m_DataStream.Position = 0;
            m_Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, m_DataStream),
                                                        m_CbChangesPerObject,
                                                        0);


            // Draw the triangle that we created in our vertex buffer.
            m_DeviceContext.Draw(36, 0);


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

                    if (m_CbChangesOnResize != null)
                        m_CbChangesOnResize.Dispose();

                    if (m_CbChangesPerFrame != null)
                        m_CbChangesPerFrame.Dispose();

                    if (m_CbChangesPerObject != null)
                        m_CbChangesPerObject.Dispose();

                    if (m_DataStream != null)
                        m_DataStream.Dispose();

                    if (m_DepthStencilTexture != null)
                        m_DepthStencilTexture.Dispose();

                    if (m_DepthStencilView != null)
                        m_DepthStencilView.Dispose();

                    if (m_CubeTexture != null)
                        m_CubeTexture.Dispose();

                    if (m_CubeTexSamplerState != null)
                        m_CubeTexSamplerState.Dispose();

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
