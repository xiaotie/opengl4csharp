#load "common.csx"

using Glfw3;
using OpenGL;

private static int width = 1280, height = 720;
private static ShaderProgram program;
private static VBO<Vector3> cube, cubeNormals;
private static VBO<Vector2> cubeUV;
private static VBO<uint> cubeQuads;
private static Texture crateTexture;
private static System.Diagnostics.Stopwatch watch;
private static float xangle, yangle;
private static bool autoRotate, lighting = true, fullscreen = false;
private static bool left, right, up, down;

void OnDisplay(GlWindow window)
{
    program = new ShaderProgram(VertexShader, FragmentShader);

    program.Use();
    program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
    program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

    program["light_direction"].SetValue(new Vector3(0, 0, 1));
    program["enable_lighting"].SetValue(lighting);

    crateTexture = new Texture("./data/crate.jpg");

    cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),         // top
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1),     // bottom
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),         // front face
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1),     // back face
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),     // left
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) });      // right
    cubeNormals = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),
                new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0),
                new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1),
                new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0),
                new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0) });
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
    cube.Dispose();
    cubeNormals.Dispose();
    cubeUV.Dispose();
    cubeQuads.Dispose();
    crateTexture.Dispose();
    program.DisposeChildren = true;
    program.Dispose();
}

void OnRenderFrame(GlWindow window)
{
    watch.Stop();
    float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
    watch.Restart();

    // perform rotation of the cube depending on the keyboard state
    if (autoRotate)
    {
        xangle += deltaTime / 2;
        yangle += deltaTime;
    }
    if (right) yangle += deltaTime;
    if (left) yangle -= deltaTime;
    if (up) xangle -= deltaTime;
    if (down) xangle += deltaTime;

    // set up the viewport and clear the previous depth and color buffers
    Gl.Viewport(0, 0, width, height);
    Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // make sure the shader program and texture are being used
    Gl.UseProgram(program);
    Gl.BindTexture(crateTexture);

    // set up the model matrix and draw the cube
    program["model_matrix"].SetValue(Matrix4.CreateRotationY(yangle) * Matrix4.CreateRotationX(xangle));
    program["enable_lighting"].SetValue(lighting);

    Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
    Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
    Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV");
    Gl.BindBuffer(cubeQuads);

    Gl.DrawElements(BeginMode.Quads, cubeQuads.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
}

// private static void OnReshape(int width, int height)
// {
//     Program.width = width;
//     Program.height = height;

//     program.Use();
//     program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
// }

// private static void OnKeyboardDown(byte key, int x, int y)
// {
//     if (key == 'w') up = true;
//     else if (key == 's') down = true;
//     else if (key == 'd') right = true;
//     else if (key == 'a') left = true;
//     else if (key == 27) Glut.glutLeaveMainLoop();
// }

// private static void OnKeyboardUp(byte key, int x, int y)
// {
//     if (key == 'w') up = false;
//     else if (key == 's') down = false;
//     else if (key == 'd') right = false;
//     else if (key == 'a') left = false;
//     else if (key == ' ') autoRotate = !autoRotate;
//     else if (key == 'l') lighting = !lighting;
//     else if (key == 'f')
//     {
//         fullscreen = !fullscreen;
//         if (fullscreen) Glut.glutFullScreen();
//         else
//         {
//             Glut.glutPositionWindow(0, 0);
//             Glut.glutReshapeWindow(1280, 720);
//         }
//     }
// }

public static string VertexShader = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    normal = normalize((model_matrix * vec4(floor(vertexNormal), 0)).xyz);
    uv = vertexUV;

    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

public static string FragmentShader = @"
#version 130

uniform sampler2D texture;
uniform vec3 light_direction;
uniform bool enable_lighting;

in vec3 normal;
in vec2 uv;

out vec4 fragment;

void main(void)
{
    float diffuse = max(dot(normal, light_direction), 0);
    float ambient = 0.3;
    float lighting = (enable_lighting ? max(diffuse, ambient) : 1);

    fragment = lighting * texture2D(texture, uv);
}
";

GlWindow.Show(width, height, OnDisplay, OnRenderFrame);