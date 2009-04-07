using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class CanvasUI : Control {
    private readonly Canvas canvas = new Canvas();

    public CanvasUI() {
      InitializeComponent();

      DoubleBuffered = true;
    }

    public Canvas Canvas {
      get { return canvas; }
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
  }
}