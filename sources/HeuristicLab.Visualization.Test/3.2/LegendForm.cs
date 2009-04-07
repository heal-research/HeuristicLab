using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Visualization.Legend;

namespace HeuristicLab.Visualization.Test {
  public partial class LegendForm : Form {
    public LegendForm() {
      InitializeComponent();

      CreateLegendShape();
    }

    private void CreateLegendShape() {
      LegendShape ls = new LegendShape();
      ls.AddLegendItem(new LegendItem("test", Color.Black, 5));
      ls.AddLegendItem(new LegendItem("test2", Color.Red, 5));

      canvasUI.Canvas.AddShape(ls);
    }
  }
}