using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Glfw3
{
    public static class Utils
    {
        [DllImport("glew32.dll")]
        public static extern int glewInit();
    }
}
