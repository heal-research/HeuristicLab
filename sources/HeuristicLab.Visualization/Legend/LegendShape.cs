using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization.Legend {
  public class LegendShape : WorldShape {
    private readonly IList<LegendItem> legendItems = new List<LegendItem>();

    private Color color = Color.Blue;
    private Font font = new Font("Arial", 8);

    public LegendShape() {
      CreateLegend();
    }

    public void CreateLegend() {
      ClearShapes();
      double y = ClippingArea.Y2;
      foreach (LegendItem item in legendItems) {
        AddShape(new LineShape(10, y - 10, 30, y - 10, item.Color, item.Thickness, DrawingStyle.Solid));
        AddShape(new TextShape(35, y, item.Label, font, color));
        y -= 15;
      }
    }

    public void AddLegendItem(LegendItem item) {
      legendItems.Add(item);
    }

    public void RemoveLegendItem(LegendItem item) {
      legendItems.Remove(item);
    }

    public void ClearLegendItems() {
      legendItems.Clear();
    }

    public Color Color {
      get { return color; }
      set {
        color = value;
        UpdateTextShapes();
      }
    }

    public Font Font {
      get { return font; }
      set {
        font = value;
        UpdateTextShapes();
      }
    }

    private void UpdateTextShapes() {
      foreach (IShape shape in shapes) {
        TextShape textShape = shape as TextShape;

        if (textShape != null) {
          textShape.Font = font;
          textShape.Color = color;
        }
      }
    }
  }
}