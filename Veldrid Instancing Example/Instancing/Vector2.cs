using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid_Instancing_Example.Instancing
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float value) : this(value, value) { }
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

    }
}
