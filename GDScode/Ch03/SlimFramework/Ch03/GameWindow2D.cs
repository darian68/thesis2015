using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Windows;


namespace Ch03
{
    public class GameWindow2D : SlimFramework.GameWindow, IDisposable
    {

        // MEMBER VARIABLES
        // ======================================================================================================================

        WindowRenderTarget m_RenderTarget;
        Factory m_Factory;

        PathGeometry m_Geometry;
        SolidColorBrush m_BrushRed;
        SolidColorBrush m_BrushGreen;
        SolidColorBrush m_BrushBlue;



        // CONSTRUCTORS
        // ======================================================================================================================

        /// <summary>
        /// This is the constructor.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="fullscreen">Whether the window should be fullscreen or not.</param>
        public GameWindow2D(string title, int width, int height, bool fullscreen)
            : base(title, width, height, fullscreen)
        {
            m_Factory = new Factory();

            WindowRenderTargetProperties properties = new WindowRenderTargetProperties();
            properties.Handle = FormObject.Handle;
            properties.PixelSize = new Size(width, height);


            m_RenderTarget = new WindowRenderTarget(m_Factory, properties);

            // Initialize our brushes.
            m_BrushRed = new SolidColorBrush(m_RenderTarget, new Color4(1.0f, 1.0f, 0.0f, 0.0f));
            m_BrushGreen = new SolidColorBrush(m_RenderTarget, new Color4(1.0f, 0.0f, 1.0f, 0.0f));
            m_BrushBlue = new SolidColorBrush(m_RenderTarget, new Color4(1.0f, 0.0f, 0.0f, 1.0f));

            // Initialize our geometry.
            m_Geometry = new PathGeometry(m_RenderTarget.Factory);
            using (GeometrySink sink = m_Geometry.Open())
            {
                int top = (int) (0.25f * FormObject.Height);
                int left = (int) (0.25f * FormObject.Width);
                int right = (int) (0.75f * FormObject.Width);
                int bottom = (int) (0.75f * FormObject.Height);

                PointF p0 = new Point(left, top);
                PointF p1 = new Point(right, top);
                PointF p2 = new Point(right, bottom);
                PointF p3 = new Point(left, bottom);

                sink.BeginFigure(p0, FigureBegin.Filled);
                sink.AddLine(p1);
                sink.AddLine(p2);
                sink.AddLine(p3);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }
        }




        // PUBLIC METHODS
        // ======================================================================================================================

        /// <summary>
        /// This method is called once per frame and is where we update objects in our scene.
        /// <param name="frameTime">The amount of time (in seconds) that has elapsed since the previous update.</param>
        /// </summary>
        public override void UpdateScene(double frameTime)
        {
            base.UpdateScene(frameTime);

        }


        /// <summary>
        /// This function is called each time we need to render the next frame.
        /// </summary>
        public override void RenderScene()
        {
            if ((!this.IsInitialized) || this.IsDisposed)
            {
                return;
            }


            m_RenderTarget.BeginDraw();
            //m_RenderTarget.Transform = Matrix3x2.Identity;

            m_RenderTarget.Clear(ClearColor);

            m_RenderTarget.FillGeometry(m_Geometry, m_BrushBlue);
            m_RenderTarget.DrawGeometry(m_Geometry, m_BrushRed, 1.0f);
            m_RenderTarget.EndDraw();
        }


        /// <summary>
        /// This function toggles fullscreen mode.  It does nothing here (except for updating the m_bFullscreen member variable) because thats all it needs to do in this base class.
        /// This function is overridden by derived classes that need to add code to toggle fullscreen mode.
        /// </summary>
        public override void ToggleFullscreen()
        {
            base.ToggleFullscreen();
        }




        // INTERFACE METHODS
        // ======================================================================================================================

        // This section is for methods that are part of the interfaces that the class implements.

        /// <summary>
        /// This is the GameWindow class's internal Dispose() method that actually disposes of the game window class.
        /// </summary>
        /// <param name="disposing">If this method is being called by this class's own code, then the value true should be passed into this parameter to indicate that this method was not called by the Garbage Collector.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                /*
                * The following text is from MSDN  (http://msdn.microsoft.com/en-us/library/fs2xkftw%28VS.80%29.aspx)
                * 
                * 
                * Dispose(bool disposing) executes in two distinct scenarios:
                * 
                * If disposing equals true, the method has been called directly or indirectly by a user's code and managed and unmanaged resources can be disposed.
                * If disposing equals false, the method has been called by the runtime from inside the finalizer and only unmanaged resources can be disposed. 
                * 
                * When an object is executing its finalization code, it should not reference other objects, because finalizers do not execute in any particular order. 
                * If an executing finalizer references another object that has already been finalized, the executing finalizer will fail.
                */
                if (disposing)
                {
                    // Unregister events


                    // get rid of managed resources
                    if (m_BrushRed != null)
                        m_BrushRed.Dispose();

                    if (m_BrushGreen != null)
                        m_BrushGreen.Dispose();

                    if (m_BrushBlue != null)
                        m_BrushBlue.Dispose();

                    if (m_Geometry != null)
                        m_Geometry.Dispose();

                    if (m_RenderTarget != null)
                        m_RenderTarget.Dispose();

                    if (m_Factory != null)
                        m_Factory.Dispose();
                }

                // get rid of unmanaged resources

            }


            base.Dispose(disposing);

        }


    }
}
