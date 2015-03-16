using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.D3DCompiler;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace TheManager.Renderables
{
    public class Triangle : Renderable
    {
        ShaderSignature inputSignature;
        VertexShader vertexShader;
        PixelShader pixelShader;
        InputLayout layout;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        public Triangle()
        {
            #region shader and triangle
            // load and compile the vertex shader
            using (var bytecode = ShaderBytecode.CompileFromFile("triangle.fx", "VShader", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(DeviceManager.Instance.device, bytecode);
            }

            // load and compile the pixel shader
            using (var bytecode = ShaderBytecode.CompileFromFile("triangle.fx", "PShader", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(DeviceManager.Instance.device, bytecode);

            // create test vertex data, making sure to rewind the stream afterward
            var vertices = new DataStream(12 * 3, true, true);
            vertices.Write(new Vector3(0.0f, 0.5f, 0.5f));
            vertices.Write(new Vector3(0.5f, -0.5f, 0.5f));
            vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f));
            vertices.Position = 0;

            // create the vertex layout and buffer
            var elements = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0) };
            layout = new InputLayout(DeviceManager.Instance.device, inputSignature, elements);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.Instance.device, vertices, 12 * 3, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            #endregion
        }

        public override void render()
	        {
	            // configure the Input Assembler portion of the pipeline with the vertex data
	            DeviceManager.Instance.context.InputAssembler.InputLayout = layout;
                DeviceManager.Instance.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
	            DeviceManager.Instance.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 12, 0));
	 
	            // set the shaders
	            DeviceManager.Instance.context.VertexShader.Set(vertexShader);
	            DeviceManager.Instance.context.PixelShader.Set(pixelShader);
	 
	            // render the triangle
	            DeviceManager.Instance.context.Draw(3, 0);
	        }

        public void dispose()
        {
            pixelShader.Dispose();
            vertexShader.Dispose();
            inputSignature.Dispose();
        }
    }
}
