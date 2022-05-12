
using System.Runtime.CompilerServices;
using Veldrid;

namespace Veldrid_Instancing_Example.Instancing;

struct TileInstance
{
    public Vector2 InstanceTransform;
    public RgbaByte AtlasCoord;
    public TileInstance(Vector2 instanceTransform, RgbaByte atlasCoord)
    {
        InstanceTransform = instanceTransform;
        AtlasCoord = atlasCoord;
    }

    public static VertexLayoutDescription VertexLayoutDescriptionData =>
     new VertexLayoutDescription(
       new VertexElementDescription("InstanceTransform", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
       new VertexElementDescription("AtlasCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4));

    public static uint SizeInBytes { get; } = (uint)Unsafe.SizeOf<TileInstance>();
}

