using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class CanvasUI : Control {
    private readonly Canvas mainCanvas = new Canvas();
    private IMouseEventListener mouseEventListener;

    public CanvasUI() {
      InitializeComponent();

      DoubleBuffered = true;
    }

    public Canvas MainCanvas {
      get { return mainCanvas; }
    }

    public IMouseEventListener MouseEventListener {
      get { return mouseEventListener; }
      set { mouseEventListener = value; }
    }

    protected override void OnPaint(PaintEventArgs pe) {
      try {
        Graphics g = pe.Graphics;

        g.SmoothingMode = SmoothingMode.AntiAlias;

        g.FillRectangle(Brushes.White, ClientRectangle);

        mainCanvas.Draw(g, ClientRectangle);

        g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);

        base.OnPaint(pe);
      } catch (Exception e) {
       Trace.WriteLine(e);
      }
    }

    protected override void OnResize(EventArgs e) {
      Invalidate();

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