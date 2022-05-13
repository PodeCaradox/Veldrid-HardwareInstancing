using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid_Instancing_Example.Instancing;
using Veldrid_Instancing_Example.Types;
using Veldrid_Instancing_Example.Upate;

namespace Veldrid_Instancing_Example;
internal class Map
{

    private GraphicsDevice _graphicsDevice;
    private CommandList _commandList;
    private DeviceBuffer _geometryBuffer;
    private DeviceBuffer _instanceBuffer;
    private DeviceBuffer _indexBuffer;
    private Shader[] _shaders;
    private Pipeline _pipeline;
    private int _width;
    private int _height;
    private Vector2 _tileSizeHalf = new Vector2(16, 8);
    private Vector2 _cameraPosition = new Vector2(0, -1500);
    private Random _randomTile = new Random();
    private ResourceSet _instanceTextureSet;
    private ResourceSet _sharedResourceSet;
    private DeviceBuffer _cameraProjViewBuffer;
    private DeviceBuffer _imageDataBuffer;
    private ProjView _projView;
    private float scale = 1;
    private readonly TileInstance[] _instances;
    public Map(GraphicsDevice graphicsDevice, int width, int height)
    {
        _width = width;
        _height = height;
        _graphicsDevice = graphicsDevice;
        _instances = new TileInstance[width * height];
        CreateResources();
    }
    private void CreateResources()
    {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;

        byte[] tileTextureData = DataManager.LoadTexture("tile_atlas_array.ktx");

        Texture textureAtlases = KtxFile.LoadTexture(
            _graphicsDevice,
            factory,
            tileTextureData,
            PixelFormat.R8_G8_B8_A8_UNorm);
        TextureView texureAtlasArray = factory.CreateTextureView(textureAtlases);

        _projView = new ProjView();
        _projView.Proj = Matrix4x4.CreateOrthographicOffCenter(0, 2560, 1440, 0, 0, -1);






        ResourceLayoutElementDescription[] textureLayoutDescriptions =
           {
                new ResourceLayoutElementDescription("Tex", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("Samp", ResourceKind.Sampler, ShaderStages.Fragment)
            };
        ResourceLayout textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(textureLayoutDescriptions));
    
        BindableResource[] instanceBindableResources = { texureAtlasArray, _graphicsDevice.PointSampler };
        _instanceTextureSet = factory.CreateResourceSet(new ResourceSetDescription(textureLayout, instanceBindableResources));
        _instanceTextureSet.Name = "TextureSet";
        ResourceLayoutElementDescription[] resourceLayoutElementDescriptions =
           {
                new ResourceLayoutElementDescription("ProjView", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ImageData", ResourceKind.UniformBuffer, ShaderStages.Vertex)
            };
        ResourceLayoutDescription resourceLayoutDescription = new ResourceLayoutDescription(resourceLayoutElementDescriptions);
        ResourceLayout sharedLayout = factory.CreateResourceLayout(resourceLayoutDescription);


        _cameraProjViewBuffer = factory.CreateBuffer(
                new BufferDescription((uint)(Unsafe.SizeOf<ProjView>()), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        _cameraProjViewBuffer.Name = "CameraProjViewBuffer";
        UpdateCameraPos();


        _imageDataBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<Vector2>() * 1000 * 2, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        _imageDataBuffer.Name = "ImageDataBuffer";

        var imageSizeArray = new Vector2[2];
        for (int i = 0; i < imageSizeArray.Length; i++)
        {
            imageSizeArray[i] = new Vector2(30, 64);
        }
        _graphicsDevice.UpdateBuffer(_imageDataBuffer, 0, imageSizeArray);



        BindableResource[] bindableResources = new BindableResource[] { _cameraProjViewBuffer, _imageDataBuffer };//, _imageDataBuffer
        ResourceSetDescription resourceSetDescription = new ResourceSetDescription(sharedLayout, bindableResources);
        _sharedResourceSet = factory.CreateResourceSet(resourceSetDescription);






        ShaderDescription vertexShaderDesc = CreateShader(ShaderStages.Vertex, "map_vertex.shader");
        ShaderDescription fragmentShaderDesc = CreateShader(ShaderStages.Fragment, "map_fragment.shader");

        _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleAlphaBlend;
        pipelineDescription.DepthStencilState = CreateDepthStencil();
        pipelineDescription.RasterizerState = CreateRasterizerStateDescription();
        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
        pipelineDescription.ResourceLayouts = new ResourceLayout[] { sharedLayout, textureLayout };
        pipelineDescription.ShaderSet = CreateShaderSetDescription();
        pipelineDescription.Outputs = _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription;
        _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

        _commandList = factory.CreateCommandList();

        _geometryBuffer = CreateGeometryBuffer(factory, _graphicsDevice);
        _instanceBuffer = CreateInstanceBuffer(factory, _graphicsDevice);
        _indexBuffer = CreateIndexBuffer(factory);

    }

    private ShaderDescription CreateShader(ShaderStages vertex, string shaderName)
    {
        return new ShaderDescription(
              vertex,
              Encoding.UTF8.GetBytes(DataManager.LoadShader(shaderName)),
              "main", true);
    }

    private DeviceBuffer CreateIndexBuffer(ResourceFactory factory)
    {
        ushort[] quadIndices = new ushort[6];
        quadIndices[0] = 0; quadIndices[1] = 1; quadIndices[2] = 2;
        quadIndices[3] = 1; quadIndices[4] = 3; quadIndices[5] = 2;
        var indexBuffer = factory.CreateBuffer(new BufferDescription((uint)quadIndices.Length * sizeof(ushort), BufferUsage.IndexBuffer));
        indexBuffer.Name = "IndexBuffer";
        _graphicsDevice.UpdateBuffer(indexBuffer, 0, quadIndices);
        return indexBuffer;
    }

    private RasterizerStateDescription CreateRasterizerStateDescription()
    {
        return new RasterizerStateDescription(
                    cullMode: FaceCullMode.None,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false
                );
    }

    private ShaderSetDescription CreateShaderSetDescription()
    {
        VertexLayoutDescription sharedVertexLayout = GeometryInstance.VertexLayoutDescriptionData;
        VertexLayoutDescription vertexLayout = TileInstance.VertexLayoutDescriptionData;
        vertexLayout.InstanceStepRate = 1;
        
        return new ShaderSetDescription(
             vertexLayouts: new VertexLayoutDescription[] { sharedVertexLayout, vertexLayout },
             shaders: _shaders);
    }

    private DepthStencilStateDescription CreateDepthStencil()
    {
        return DepthStencilStateDescription.Disabled;
    }

    private DeviceBuffer CreateInstanceBuffer(in ResourceFactory factory, in GraphicsDevice graphicsDevice)
    {
    

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {

                _instances[y * _width + x].InstanceTransform = new RgbaByte((byte)(x % 256), (byte)(x / 256), (byte)(y % 256), (byte)(y / 256));
                _instances[y * _width + x].AtlasCoord = new RgbaByte((byte)_randomTile.Next(0, 28), 0, 0, 0);
            }
        }



        var instanceBuffer = factory.CreateBuffer(new BufferDescription((uint)_instances.Length * TileInstance.SizeInBytes, BufferUsage.VertexBuffer));
        instanceBuffer.Name = "InstanceBuffer";
        _graphicsDevice.UpdateBuffer(instanceBuffer, 0, _instances);

        return instanceBuffer;
    }

    private DeviceBuffer CreateGeometryBuffer(in ResourceFactory factory, in GraphicsDevice graphicsDevice)
    {

        GeometryInstance[] quadVertices = new GeometryInstance[4];

        byte size = 1;
        #region filling vertices
        quadVertices[0].Position = new RgbaByte(0, 0, 0, 0);
        quadVertices[0].TexCoord = new RgbaByte(0, 0, 0, 0);
        quadVertices[1].Position = new RgbaByte(size, 0, 0, 0);
        quadVertices[1].TexCoord = new RgbaByte(size, 0, 0, 0);
        quadVertices[2].Position = new RgbaByte(0, size, 0, 0);
        quadVertices[2].TexCoord = new RgbaByte(0, size, 0, 0);
        quadVertices[3].Position = new RgbaByte(size, size, 0, 0);
        quadVertices[3].TexCoord = new RgbaByte(size, size, 0, 0);



        #endregion

        var geometryBuffer = factory.CreateBuffer(new BufferDescription((uint)quadVertices.Length * GeometryInstance.SizeInBytes, BufferUsage.VertexBuffer));
        _graphicsDevice.UpdateBuffer(geometryBuffer, 0, quadVertices);
        geometryBuffer.Name = "GeometryBuffer";
        
        return geometryBuffer;
    }

    internal void DisposeResources()
    {
        _pipeline.Dispose();
        _shaders[0].Dispose();
        _shaders[1].Dispose();
        _commandList.Dispose();
        _geometryBuffer.Dispose();
        _indexBuffer.Dispose();
        _graphicsDevice.Dispose();
    }
   
    internal void Draw()
    {
        UpdateCamera();

        _commandList.Begin();
        //ChangeInstances();
        _commandList.UpdateBuffer(_cameraProjViewBuffer, 0, _projView);
        _commandList.UpdateBuffer(_instanceBuffer, 0, _instances);

        _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
        _commandList.ClearColorTarget(0, RgbaFloat.Grey);
        //_commandList.ClearDepthStencil(1f);



        _commandList.SetPipeline(_pipeline);
        
        _commandList.SetGraphicsResourceSet(0, _sharedResourceSet); 
        _commandList.SetGraphicsResourceSet(1, _instanceTextureSet);


        _commandList.SetVertexBuffer(0, _geometryBuffer);
        _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        _commandList.SetVertexBuffer(1, _instanceBuffer);

        _commandList.DrawIndexed(
            indexCount: 6,
            instanceCount: (uint)(_width * _height),
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);

        _commandList.End();
        _graphicsDevice.SubmitCommands(_commandList);
        //_graphicsDevice.WaitForIdle();
        _graphicsDevice.SwapBuffers();
    }

    private void ChangeInstances()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _instances[y * _width + x].AtlasCoord = new RgbaByte((byte)_randomTile.Next(0, 28), 0, 0, 0);
            }
        }
    }

    private void UpdateCamera()
    {
        if (InputTracker.GetKey(Key.S))
        {
            _cameraPosition.Y -= (float)1;
            UpdateCameraPos();
        }
        else if (InputTracker.GetKey(Key.W))
        {
            _cameraPosition.Y += (float)1;
            UpdateCameraPos();
        }
        if (InputTracker.GetKey(Key.D))
        {
            _cameraPosition.X -= (float)1;

            UpdateCameraPos();
        }
        else if (InputTracker.GetKey(Key.A))
        {
            _cameraPosition.X += (float)1;

            UpdateCameraPos();
        }
       
        if (InputTracker.GetPressedOnce(Key.BracketRight))
        {
            scale += 0.2f;

            UpdateCamera();
        }
        else if (InputTracker.GetPressedOnce(Key.Slash))
        {
            if (scale > 0.4f) scale -= 0.2f;

            UpdateCamera();
        }
    }

    private void UpdateCameraPos()
    {
        _projView.WorldViewProjection = Matrix4x4.CreateTranslation(new Vector3(_cameraPosition.X, _cameraPosition.Y, 0)) *
        Matrix4x4.CreateScale(scale, scale, 1) *
        Matrix4x4.CreateTranslation(new Vector3(2560 * 0.5f, 1440 * 0.5f, 0));
    }
}

