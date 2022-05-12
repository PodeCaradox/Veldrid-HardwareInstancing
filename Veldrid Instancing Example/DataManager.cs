using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid_Instancing_Example
{
    internal static class DataManager
    {
        private static char sep = Path.DirectorySeparatorChar;
        internal static char[] LoadShader(string shaderName)
        {
            return File.ReadAllText(Directory.GetCurrentDirectory() + sep + "Content" + sep + shaderName).ToArray();
        }

        internal static byte[] LoadTexture(string textureName)
        {
            return File.ReadAllBytes(Directory.GetCurrentDirectory() + sep + "Content" + sep + textureName);
        }
    }
}
