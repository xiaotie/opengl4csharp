using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Glfw3
{
    public class GlWindow
    {
        [DllImport("glew32.dll")]
        public static extern int glewInit();

        public bool UseGlew { get; set; } = true;

        public int Width { get; set; }
        public int Height { get; set; }
        public String Title { get; set; }

        private Action<GlWindow> OnDisplayImpl;
        private Action<GlWindow> OnRenderFrameImpl;
        private Action<GlWindow> OnCloseImpl;

        public GlWindow(int width, int height, String title = "OpenGL Window")
        {
            Width = width;
            Height = height;
            Title = title;
        }

        public GlWindow():this(600,400,"OpenGL Window")
        {
        }

        protected virtual void OnDisplay()
        {
            if (OnDisplayImpl != null) OnDisplayImpl(this);
        }

        protected virtual void OnRenderFrame()
        {
            if (OnRenderFrameImpl != null) OnRenderFrameImpl(this);
        }

        protected virtual void OnClose()
        {
            if (OnCloseImpl != null) OnCloseImpl(this);
        }

        public void Show()
        {
            Glfw.Init();
            var _window = Glfw.CreateWindow(Width, Height, Title);
            if (!_window)
            {
                Glfw.Terminate();
                Environment.Exit(-1);
            }

            if(UseGlew == true) glewInit();

            Glfw.MakeContextCurrent(_window);

            OnDisplay();

            while (!Glfw.WindowShouldClose(_window))
            {
                Glfw.PollEvents();
                OnRenderFrame();
                Glfw.SwapBuffers(_window);
            }

            OnClose();
            Glfw.Terminate();
        }

        public static void Show(int width, int height, Action<GlWindow> onDisplay, Action<GlWindow> onRenderFrame, Action<GlWindow> onClose = null)
        {
            GlWindow w = new GlWindow(width, height);
            w.OnDisplayImpl = onDisplay;
            w.OnRenderFrameImpl = onRenderFrame;
            w.OnCloseImpl = onClose;
            w.Show();
        }
    }
}
