#r "../Glfw/bin/Debug/netstandard2.0/Glfw.dll"
#r "../OpenGL/bin/Debug/netstandard2.0/ImageSharp.dll"
#r "../OpenGL/bin/Debug/netstandard2.0/OpenGL.dll"

using Glfw3;
using OpenGL;
using System.Runtime.InteropServices;

class Shaders
{
    public static string DefaultVertexShader = @"
    #version 130

    in vec3 vertexPosition;
    in vec3 vertexColor;

    out vec3 color;

    uniform mat4 projection_matrix;
    uniform mat4 view_matrix;
    uniform mat4 model_matrix;

    void main(void)
    {
        color = vertexColor;
        gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
    }
    ";

    public static string DefaultUVVertexShader = @"
    #version 130

    in vec3 vertexPosition;
    in vec2 vertexUV;

    out vec2 uv;

    uniform mat4 projection_matrix;
    uniform mat4 view_matrix;
    uniform mat4 model_matrix;

    void main(void)
    {
        uv = vertexUV;
        gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
    }
    ";

    public static string DefaultColorFragmentShader = @"
    #version 130

    in vec3 color;

    out vec4 fragment;

    void main(void)
    {
        fragment = vec4(color, 1);
    }
    ";

    public static string DefaultBinaryFragmentShader = @"
    #version 130

    out vec4 fragment;

    void main(void)
    {
        fragment = vec4(1, 1, 1, 1);
    }
    ";

    public static string DefaultUVFragmentShader = @"
    #version 130

    uniform sampler2D texture;

    in vec2 uv;

    out vec4 fragment;

    void main(void)
    {
        fragment = texture2D(texture, uv);
    }
    ";
}

