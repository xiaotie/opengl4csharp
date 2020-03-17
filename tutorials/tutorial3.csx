#load "common.csx"

using Glfw3;
using OpenGL;

private static int width = 1280, height = 720;
private static ShaderProgram program;
private static VBO<Vector3> triangle, square;
private static VBO<Vector3> triangleColor, squareColor;
private static VBO<uint> triangleElements, squareElements;

void OnDisplay(GlWindow window)
{
    // compile the shader program
    program = new ShaderProgram(Shaders.DefaultVertexShader, Shaders.DefaultColorFragmentShader);

    // set the view and projection matrix, which are static throughout this tutorial
    program.Use();
    program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
    program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

    // create a triangle with vertices and colors
    triangle = new VBO<Vector3>(new Vector3[] { new Vector3(0, 1, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0) });
    triangleColor = new VBO<Vector3>(new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) });
    triangleElements = new VBO<uint>(new uint[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);

    // create a square with vertices an colors
    square = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0) });
    squareColor = new VBO<Vector3>(new Vector3[] { new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, 0.5f, 1) });
    squareElements = new VBO<uint>(new uint[] { 0, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);
}

void OnClose()
{
    // dispose of all of the resources that were created
    triangle.Dispose();
    triangleColor.Dispose();
    triangleElements.Dispose();
    square.Dispose();
    squareColor.Dispose();
    squareElements.Dispose();
    program.DisposeChildren = true;
    program.Dispose();
}

void OnRenderFrame(GlWindow window)
{
    // set up the OpenGL viewport and clear both the color and depth bits
    Gl.Viewport(0, 0, width, height);
    Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // use our shader program
    Gl.UseProgram(program);

    // bind the vertex positions, colors and elements of the triangle
    program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(-1.5f, 0, 0)));
    Gl.BindBufferToShaderAttribute(triangle, program, "vertexPosition");
    Gl.BindBufferToShaderAttribute(triangleColor, program, "vertexColor");
    Gl.BindBuffer(triangleElements);

    // draw the triangle
    Gl.DrawElements(BeginMode.Triangles, triangleElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

    // bind the vertex positions, colors and elements of the square
    program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));
    Gl.BindBufferToShaderAttribute(square, program, "vertexPosition");
    Gl.BindBufferToShaderAttribute(squareColor, program, "vertexColor");
    Gl.BindBuffer(squareElements);

    // draw the square
    Gl.DrawElements(BeginMode.Quads, squareElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
}

GlWindow.Show(width,height,OnDisplay,OnRenderFrame);