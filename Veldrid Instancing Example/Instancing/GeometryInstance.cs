using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Veldrid_Instancing_Example.Instancing;

struct GeometryInstance
{
    public RgbaByte Position;
    public RgbaByte TexCoord;
    public GeometryInstance(RgbaByte position, RgbaByte texCoord)
    {
        Position = position;
        TexCoord = texCoord;
    }

    public static VertexLayoutDescription VertexLayoutDescriptionData =>
      new VertexLayoutDescription(
        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4),
        new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4));

    public static uint SizeInBytes { get; } = (uint)Unsafe.SizeOf<GeometryInstance>();
}

