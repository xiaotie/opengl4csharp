#load "common.csx"

using Glfw3;
using OpenGL;

void OnRenderFrame(GlWindow window)
{
    Gl.ClearColor(1.0f,0,0,1.0f);
    Gl.Clear(ClearBufferMask.ColorBufferBit);
}

GlWindow.Show(600,400,null, OnRenderFrame);