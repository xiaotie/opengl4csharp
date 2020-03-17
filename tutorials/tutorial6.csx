#load "common.csx"

using Glfw3;
using OpenGL;

private static int width = 1280, height = 720;
private static ShaderProgram program;
private static VBO<Vector3> cube;
private static VBO<Vector2> cubeUV;
private static VBO<uint> cubeQuads;
private static Texture crateTexture;
private static System.Diagnostics.Stopwatch watch;
private static float angle;

void OnDisplay(GlWindow window)
{
    // compile the shader program
    program = new ShaderProgram(Shaders.DefaultUVVertexShader, Shaders.DefaultUVFragmentShader);

    // set the view and projection matrix, which are static throughout this tutorial
    program.Use();
    program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
    program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

    // load a crate texture
    crateTexture = new Texture("./data/crate.jpg");

    // create a crate with vertices and UV coordinates
    cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1),
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1),
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) });
    cubeUV = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });

    cubeQuads = new VBO<uint>(new uint[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);

    watch = System.Diagnostics.Stopwatch.StartNew();
}

void OnClose()
{
    // dispose of all of the resources that were created
    cube.Dispose();
    cubeUV.Dispose();
    cubeQuads.Dispose();
    crateTexture.Dispose();
    program.DisposeChildren = true;
    program.Dispose();
}

void OnRenderFrame(GlWindow window)
{
    // calculate how much time has elapsed since the last frame
    watch.Stop();
    float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
    watch.Restart();

    // use the deltaTime to adjust the angle of the cube
    angle += deltaTime;

    // set up the OpenGL viewport and clear both the color and depth bits
    Gl.Viewport(0, 0, width, height);
    Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // use our shader program and bind the crate texture
    Gl.UseProgram(program);
    Gl.BindTexture(crateTexture);

    // set the transformation of the cube
    program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle / 2) * Matrix4.CreateRotationX(angle));

    // bind the vertex positions, UV coordinates and element array
    Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
    Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
    Gl.BindBuffer(cubeQuads);

    // draw the textured cube
    Gl.DrawElements(BeginMode.Quads, cubeQuads.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
}

GlWindow.Show(width, height, OnDisplay, OnRenderFrame);