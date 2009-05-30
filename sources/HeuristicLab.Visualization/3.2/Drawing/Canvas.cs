using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization.Drawing {
  public class Canvas : IShape {
    private readonly WorldShape worldShape;

    private Rectangle viewport;

    public Canvas() {
      worldShape = new WorldShape();
      worldShape.Parent = this;
    }

    public void AddShape(IShape shape) {
      worldShape.AddShape(shape);
    }

    public void RemoveShape(IShape shape) {
      worldShape.RemoveShape(shape);
    }

    public void ClearShapes() {
      worldShape.ClearShapes();
    }

    public Rectangle Viewport {
      get { return viewport; }
      set {
        viewport = value;
        worldShape.ClippingArea = new RectangleD(0, 0, viewport.Width, viewport.Height);
        worldShape.BoundingBox = worldShape.ClippingArea;
      }
    }

    public RectangleD ClippingArea {
      get { return worldShape.ClippingArea; }
    }

    public RectangleD BoundingBox {
      get { throw new InvalidOperationException(); }
    }

    public IShape Parent {
      get { throw new InvalidOperationException(); }
      set { throw new InvalidOperationException(); }
    }

    public void Draw(Graphics graphics) {
      try {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.FillRectangle(Brushes.White, Viewport);

        worldShape.Draw(graphics);

        graphics.DrawRectangle(Pens.Black, 0, 0, Viewport.Width - 1, Viewport.Height - 1);

        sw.Stop();
        Trace.WriteLine(string.Format("Drawing time: {0:0.0}ms", sw.Elapsed.TotalMilliseconds));
      } catch (OverflowException e) {
        Trace.WriteLine(e);

        graphics.FillRectangle(Brushes.White, graphics.ClipBounds);

        using (Font font = new Font("Arial", 14)) {
          const string message = "Zoom level is too high!";
          graphics.DrawString(message, font, Brushes.Red, graphics.ClipBounds.X + 10, graphics.ClipBounds.Y + 10);
        }
      }
    }
  }
}