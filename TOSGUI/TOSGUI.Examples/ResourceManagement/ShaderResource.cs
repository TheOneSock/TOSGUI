using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace TOSGUI.Examples.ResourceManagement
{
    public class ShaderResource : IResource
    {
        private readonly Dictionary<string, int> _programLocation = new Dictionary<string, int>();

        private ShaderResource(string name, int glHandle)
        {
            Name = name;
            GlHandle = glHandle;
        }

        public int GlHandle { get; }

        public string Name { get; }

        public void Dispose()
        {
            GL.DeleteProgram(GlHandle);
        }

        public int GetProgramLocation(string variableName)
        {
            int location;
            if (_programLocation.TryGetValue(variableName, out location))
                return location;

            location = GL.GetUniformLocation(GlHandle, variableName);
            _programLocation[variableName] = location;

            return location;
        }

        public static IResource LoadResource(string name)
        {
            var vertexFilePath = @"Resources\Shaders\" + name + ".vert";
            var fragmentFilePath = @"Resources\Shaders\" + name + ".frag";


            int programId;
            int vertexShader;
            int fragmentShader;


            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText(vertexFilePath));
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(fragmentFilePath));
            GL.CompileShader(fragmentShader);
            Console.Out.WriteLine(GL.GetShaderInfoLog(fragmentShader));

            programId = GL.CreateProgram();
            GL.AttachShader(programId, vertexShader);
            GL.AttachShader(programId, fragmentShader);
            GL.LinkProgram(programId);


            var vertexLog = GL.GetShaderInfoLog(vertexShader);
            var fragmentLog = GL.GetShaderInfoLog(fragmentShader);
            var programLog = GL.GetProgramInfoLog(programId);


            if (!string.IsNullOrWhiteSpace(vertexLog)
                || !string.IsNullOrWhiteSpace(fragmentLog)
                || !string.IsNullOrWhiteSpace(programLog))
                throw new Exception(
                    $"Error Loading Shader {name}\nVertex shader compile log:\n{vertexLog}\nFragment shader compile log:\n{fragmentLog}\nProgram compile log:\n{programLog}");


            GL.DetachShader(programId, vertexShader);
            GL.DetachShader(programId, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return new ShaderResource(name, programId);
        }
    }
}