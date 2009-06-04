using System;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public class ZoomListener : IMouseEventListener {
    private readonly Point startPoint;

    public event RectangleHandler DrawRectangle;
    public event RectangleHandler Zoom;

    public ZoomListener(Point startPoint) {
      this.startPoint = startPoint;
    }

    #region IMouseEventListener Members

    public void MouseMove(object sender, MouseEventArgs e) {
      if(DrawRectangle != null) {
        DrawRectangle(CalcRectangle(e.Location));
      }
    }

    public void MouseUp(object sender, MouseEventArgs e) {
     if(Zoom != null) {
       Zoom(CalcRectangle(e.Location));
     }
    }

    #endregion

    private Rectangle CalcRectangle(Point actualPoint) {
      int x = Math.Min(startPoint.X, actualPoint.X);
      int y = Math.Min(startPoint.Y, actualPoint.Y);
      int width = Math.Abs(actualPoint.X - startPoint.X);
      int height = Math.Abs(actualPoint.Y - startPoint.Y);
      
      return new Rectangle(x, y, width, height);
    }
  }
}