#load "common.csx"

using Glfw3;
using OpenGL;

private static int width = 1280, height = 720;
private static ShaderProgram program;
private static VBO<Vector3> pyramid, cube;
private static VBO<Vector3> pyramidColor, cubeColor;
private static VBO<uint> pyramidTriangles, cubeQuads;
private static System.Diagnostics.Stopwatch watch;
private static float angle;

void OnDisplay(GlWindow window)
{
    Gl.Enable(EnableCap.DepthTest);
    
    // compile the shader program
    program = new ShaderProgram(Shaders.DefaultVertexShader, Shaders.DefaultColorFragmentShader);

    // set the view and projection matrix, which are static throughout this tutorial
    program.Use();
    program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
    program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

    // create a pyramid with vertices and colors
    pyramid = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),        // front face
                new Vector3(0, 1, 0), new Vector3(1, -1, 1), new Vector3(1, -1, -1),        // right face
                new Vector3(0, 1, 0), new Vector3(1, -1, -1), new Vector3(-1, -1, -1),      // back face
                new Vector3(0, 1, 0), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1) });   // left face
    pyramidColor = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1),
                new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0),
                new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1),
                new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0) });
    pyramidTriangles = new VBO<uint>(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, BufferTarget.ElementArrayBuffer);

    // create a cube with vertices and colors
    cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1),
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1),
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) });
    cubeColor = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),
                new Vector3(1, 0.5f, 0), new Vector3(1, 0.5f, 0), new Vector3(1, 0.5f, 0), new Vector3(1, 0.5f, 0),
                new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0),
                new Vector3(1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 0),
                new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1),
                new Vector3(1, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 1) });
    cubeQuads = new VBO<uint>(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);

    watch = System.Diagnostics.Stopwatch.StartNew();
}

void OnClose()
{
    // dispose of all of the resources that were created
    pyramid.Dispose();
    pyramidColor.Dispose();
    pyramidTriangles.Dispose();
    cube.Dispose();
    cubeColor.Dispose();
    cubeQuads.Dispose();
    program.DisposeChildren = true;
    program.Dispose();
}

void OnRenderFrame(GlWindow window)
{
    // calculate how much time has elapsed since the last frame
    watch.Stop();
    float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
    watch.Restart();

    // use the deltaTime to adjust the angle of the cube and pyramid
    angle += deltaTime;

    // set up the OpenGL viewport and clear both the color and depth bits
    Gl.Viewport(0, 0, width, height);
    Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // use our shader program
    Gl.UseProgram(program);

    // bind the vertex positions, colors and elements of the pyramid
    program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle) * Matrix4.CreateTranslation(new Vector3(-1.5f, 0, 0)));
    Gl.BindBufferToShaderAttribute(pyramid, program, "vertexPosition");
    Gl.BindBufferToShaderAttribute(pyramidColor, program, "vertexColor");
    Gl.BindBuffer(pyramidTriangles);

    // draw the pyramid
    Gl.DrawElements(BeginMode.Triangles, pyramidTriangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

    // bind the vertex positions, colors and elements of the cube
    program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle / 2) * Matrix4.CreateRotationX(angle) * Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));
    Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
    Gl.BindBufferToShaderAttribute(cubeColor, program, "vertexColor");
    Gl.BindBuffer(cubeQuads);

    // draw the cube
    Gl.DrawElements(BeginMode.Quads, cubeQuads.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
}

GlWindow.Show(width, height, OnDisplay, OnRenderFrame);