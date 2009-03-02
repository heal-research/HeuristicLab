using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class CanvasUI : Control {
    private readonly Canvas canvas = new Canvas();
    private IMouseEventListener mouseEventListener;

    public CanvasUI() {
      InitializeComponent();

      DoubleBuffered = true;
    }

    public Canvas Canvas {
      get { return canvas; }
    }

    public IMouseEventListener MouseEventListener {
      get { return mouseEventListener; }
      set { mouseEventListener = value; }
    }

    protected override void OnPaint(PaintEventArgs pe) {
      try {
        Graphics g = pe.Graphics;

        canvas.Viewport = ClientRectangle;
        canvas.Draw(g);

        base.OnPaint(pe);
      } catch (Exception e) {
       Trace.WriteLine(e);
      }
    }

    protected override void OnResize(EventArgs e) {
      Invalidate();

      canvas.Viewport = ClientRectangle;

      base.OnResize(e);
    }

    private void CanvasUI_MouseMove(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseMove(sender, e);
      }
    }

    private void CanvasUI_MouseUp(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseUp(sender, e);
      }
    }
  }
}