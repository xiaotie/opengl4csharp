#load "common.csx"

using Glfw3;
using OpenGL;

private static int width = 600, height = 400;
private static ShaderProgram program;
private static VBO<Vector3> triangle, square;
private static VBO<uint> triangleElements, squareElements;

void OnDisplay(GlWindow window)
{
    // compile the shader program
    program = new ShaderProgram(Shaders.DefaultVertexShader, Shaders.DefaultBinaryFragmentShader);
    // set the view and projection matrix, which are static throughout this tutorial
    program.Use();
    program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
    program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

    // create a triangle
    triangle = new VBO<Vector3>(new Vector3[] { new Vector3(0, 1, 0), new Vector3(-1, -1, 0), new Vector3(1, -1, 0) });
    triangleElements = new VBO<uint>(new uint[] { 0, 1, 2 }, BufferTarget.ElementArrayBuffer);

    // create a square
    square = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0) });
    squareElements = new VBO<uint>(new uint[] { 0, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);
}

void OnClose()
{
    // dispose of all of the resources that were created
    triangle.Dispose();
    triangleElements.Dispose();
    square.Dispose();
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

    // transform the triangle
    program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(-1.5f, 0, 0)));

    // bind the vertex attribute arrays for the triangle (the hard way)
    uint vertexPositionIndex = (uint)Gl.GetAttribLocation(program.ProgramID, "vertexPosition");
    Gl.EnableVertexAttribArray(vertexPositionIndex);
    Gl.BindBuffer(triangle);
    Gl.VertexAttribPointer(vertexPositionIndex, triangle.Size, triangle.PointerType, true, 12, IntPtr.Zero);
    Gl.BindBuffer(triangleElements);

    // draw the triangle
    Gl.DrawElements(BeginMode.Triangles, triangleElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

    // transform the square
    program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));

    // bind the vertex attribute arrays for the square (the easy way)
    Gl.BindBufferToShaderAttribute(square, program, "vertexPosition");
    Gl.BindBuffer(squareElements);

    // draw the square
    Gl.DrawElements(BeginMode.Quads, squareElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
}

GlWindow.Show(600,400,OnDisplay,OnRenderFrame);