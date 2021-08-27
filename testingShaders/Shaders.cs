using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testingShaders
{
    public static class Shaders
    {
        public static string vertex = 
@"
#version 450
layout(location = 0) in vec3 inPos;
layout(location = 1) uniform Transform
{
    mat3x2 model;
    mat4 view;
    mat4 projection;
} t;

void main()
{
    gl_Position = vec4(inPos, 1.0) * t.model * t.view * t.projection;
}
";
        public static string fragment =
@"
#version 450
layout(location = 0) in vec4 color;
layout(location = 0) out vec4 outColor;

void main()
{
    outColor = color;
}
";
    }
}
