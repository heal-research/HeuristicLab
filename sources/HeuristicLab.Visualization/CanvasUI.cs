using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization {
  public partial class CanvasUI : Control {
    private readonly Canvas mainCanvas = new Canvas();

    public CanvasUI() {
      InitializeComponent();
    }

    public Canvas MainCanvas {
      get { return mainCanvas; }
    }

    protected override void OnPaint(PaintEventArgs pe) {
      Graphics g = pe.Graphics;

      g.FillRectangle(Brushes.White, ClientRectangle);

      mainCanvas.Draw(g, ClientRectangle);

      g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);

      base.OnPaint(pe);
    }
  }
}
