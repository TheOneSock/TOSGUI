using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using TOSGUI.Examples.Annotations;
using TOSGUI.Examples.ResourceManagement;

namespace TOSGUI.Examples
{
    public static class Program
    {
        public static void Main()
        {
            using (var game = new TestGLWindow())
            {
                // Run the game at 60 updates per second 
                game.Run(60.0);
            }

            /*using (ITOSGUI root = test)
            {
                
            
                root.Setup(sizex, sizey);
                root.Bind(model, view);
    
                // int input loop
    
                ITOSGUIInput input;
                root.ProcessInput(input);
                // was input consumed? 
    
                // in draw loop
                //  draw only if necesary "something changed"
                root.Render();
                int texture = root.GlTextureInt;

            }*/
        }

        public class TestGLWindow : GameWindow
        {
            private readonly ResourceManager _resourceManager = new ResourceManager();
            private int _vertexArrayId;
            private int _vertexbuffer;
            private double _time;

            protected override void OnLoad(EventArgs e)
            {
                _resourceManager
                    .Load<ShaderResource>("example");

                GL.GenVertexArrays(1, out _vertexArrayId);
                GL.BindVertexArray(_vertexArrayId);

                var gVertexBufferData = new[]
                {
                    1.0f, 1.0f, 0.0f,
                    -1.0f, 1.0f, 0.0f,
                    -1.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 0.0f,
                };

                GL.GenBuffers(1, out _vertexbuffer);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexbuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, gVertexBufferData.Length * sizeof(float), gVertexBufferData, BufferUsageHint.StaticDraw);
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                _time += e.Time;
                base.OnRenderFrame(e);
                // render graphics 
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                var shader = _resourceManager.Get<ShaderResource>("example");
                GL.UseProgram(shader.GlHandle);

                GL.EnableVertexAttribArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexbuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);

                var mvp = Matrix4.Identity; //Matrix4.CreateOrthographic(Width * 2.0f / Height, 2.0f, -1.0f, 1.0f);
                GL.UniformMatrix4(shader.GetProgramLocation("MVP"), false, ref mvp);
                GL.Uniform1(shader.GetProgramLocation("time"), (float) _time);
                GL.Uniform2(shader.GetProgramLocation("resolution"), new Vector2(Width, Height));

                GL.DrawArrays(PrimitiveType.Quads, 0, 4);

                GL.DisableVertexAttribArray(0);
                GL.UseProgram(0);

                SwapBuffers();
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                base.OnUpdateFrame(e);
                // add game logic, input handling 
                if (Keyboard[Key.Escape]) Exit();
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                GL.Viewport(0, 0, Width, Height);
            }

            protected override void Dispose(bool manual)
            {
                GL.DeleteVertexArrays(1, ref _vertexArrayId);
                GL.DeleteBuffers(1, ref _vertexbuffer);
                _resourceManager.Dispose();
                base.Dispose(manual);
            }
        }

        public class SimpleVerticalMenuViewModel : INotifyPropertyChanged
        {
            private bool _button3Visibility;
            private string _button2Caption = "Test";
            private bool _shouldExitApplication;

            public void Button1Click()
            {
                ShouldExitApplication = true;
            }

            public bool ShouldExitApplication
            {
                get { return _shouldExitApplication; }
                set
                {
                    if (value == _shouldExitApplication) return;
                    _shouldExitApplication = value;
                    OnPropertyChanged();
                }
            }

            public void Button2Click()
            {
                Button3Visibility = !Button3Visibility;
            }

            public void Button3Click()
            {
                Button2Caption = Button2Caption == "Test" ? "Test 2" : "Test";
            }

            public bool Button3Visibility
            {
                get { return _button3Visibility; }
                set
                {
                    if (value == _button3Visibility)
                        return;
                    _button3Visibility = value;
                    OnPropertyChanged();
                }
            }

            public string Button2Caption
            {
                get { return _button2Caption; }
                set
                {
                    if (value == _button2Caption) return;
                    _button2Caption = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}