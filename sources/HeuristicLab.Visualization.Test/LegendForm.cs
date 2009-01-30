using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Test {
  public partial class LegendForm : Form {
    public LegendForm() {
      InitializeComponent();
      canvasUI.MainCanvas.WorldShape = new WorldShape(new RectangleD(0, 0, 800, 600), new RectangleD(0, 0, 800, 600));

      CreateLegendShape();

    }

    private void CreateLegendShape() {
      WorldShape mainShape = canvasUI.MainCanvas.WorldShape;
      LegendShape ls = new LegendShape();
      ls.AddLegendItem(new LegendItem("test", Color.Black, 5));
      ls.AddLegendItem(new LegendItem("test2", Color.Red, 5));

      mainShape.AddShape(ls);
    }
  }
}