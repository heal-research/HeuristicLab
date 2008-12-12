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
      LegendShape ls = new LegendShape(0, 0, 100, 100, 0, Color.White);
      ls.AddLegendItem(new LegendItem("test", Color.Black));
      ls.AddLegendItem(new LegendItem("test2", Color.Red));

      mainShape.AddShape(ls);
    }
  }
}