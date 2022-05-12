
using System.Runtime.CompilerServices;
using Veldrid;

namespace Veldrid_Instancing_Example.Instancing;

struct TileInstance
{
    public RgbaByte InstanceTransform;
    public RgbaByte AtlasCoord;
    public TileInstance(RgbaByte instanceTransform, RgbaByte atlasCoord)
    {
        InstanceTransform = instanceTransform;
        AtlasCoord = atlasCoord;
    }

    public static VertexLayoutDescription VertexLayoutDescriptionData =>
     new VertexLayoutDescription(
       new VertexElementDescription("InstanceTransform", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4),
       new VertexElementDescription("AtlasCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4));

    public static uint SizeInBytes { get; } = (uint)Unsafe.SizeOf<TileInstance>();
}

