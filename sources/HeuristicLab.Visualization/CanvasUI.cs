using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class CanvasUI : Control {
    private readonly Canvas mainCanvas = new Canvas();
    private MouseEventListener mouseEventListener;

    public CanvasUI() {
      InitializeComponent();
    }

    public Canvas MainCanvas {
      get { return mainCanvas; }
    }

    public MouseEventListener MouseEventListener {
      get { return mouseEventListener; }
      set { mouseEventListener = value; }
    }

    protected override void OnPaint(PaintEventArgs pe) {
      try {
        Graphics g = pe.Graphics;

        g.FillRectangle(Brushes.White, ClientRectangle);

        mainCanvas.Draw(g, ClientRectangle);

        g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);

        base.OnPaint(pe);
      } catch (Exception e) {
       Debug.WriteLine(e);
      }
    }

    private void CanvasUI_MouseMove(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseMove(e.Location);
      }
    }

    private void CanvasUI_MouseUp(object sender, MouseEventArgs e) {
      if (mouseEventListener != null) {
        mouseEventListener.MouseUp(e.Location);
      }
    }
  }
}