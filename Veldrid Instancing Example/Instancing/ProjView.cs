using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid_Instancing_Example.Instancing
{
    public struct ProjView
    {
        public Matrix4x4 WorldViewProjection;
        public Matrix4x4 Proj;
    }
}
