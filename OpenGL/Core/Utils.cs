using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenGL
{
    public class Utils
    {
        [DllImport("glew32.dll")]
        public static extern int glewInit();
    }
}
