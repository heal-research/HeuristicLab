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

    public event PaintEventHandler BeforePaint;

    protected override void OnPaint(PaintEventArgs pe) {
      try {
        FireBeforePaint(pe);

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

    private void FireBeforePaint(PaintEventArgs e) {
      if (BeforePaint != null)
        BeforePaint(this, e);
    }
  }
}