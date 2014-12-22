using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Windows;
using SlimFramework;

namespace Ch3
{
    class GameWindow2D : HarWindow, IDisposable
    {
        WindowRenderTarget m_RenderTarget;
        Factory m_Factory;
        PathGeometry m_Geometry;
        SolidColorBrush m_BrushRed;
        SolidColorBrush m_BrushGreen;
        SolidColorBrush m_BrushBlue;
        public GameWindow2D(string title, int width, int height, bool fullscreen) : base(title, width, height, fullscreen)
        {
            m_Factory = new Factory();
            WindowRenderTargetProperties properties = new WindowRenderTargetProperties();
            properties.Handle = FormObject.Handle;
            properties.PixelSize = new Size(width, height);
            m_RenderTarget = new WindowRenderTarget(m_Factory, properties);
            m_BrushRed = new SolidColorBrush(m_RenderTarget, new Color4(1.0f, 1.0f, 0.0f, 0.0f));
            m_BrushGreen = new SolidColorBrush(m_RenderTarget, new Color4(1.0f, 0.0f, 1.0f, 0.0f));
            m_BrushBlue = new SolidColorBrush(m_RenderTarget, new Color4(1.0f, 0.0f, 0.0f, 1.0f));
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
        public override void UpdateScene(double frameTime)
        {
            base.UpdateScene(frameTime);
        }
        public override void RenderScene()
        {
            if ((!this.IsInitialized) || this.IsDisposed)
            {
                return;
            }
            m_RenderTarget.BeginDraw();
            m_RenderTarget.Clear(ClearColor);
            m_RenderTarget.FillGeometry(m_Geometry, m_BrushGreen);
            m_RenderTarget.DrawGeometry(m_Geometry, m_BrushRed, 1.0f);
            m_RenderTarget.EndDraw();
        }

    }
}
