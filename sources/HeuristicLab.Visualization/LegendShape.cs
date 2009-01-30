using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization {
  public class LegendItem {
    public LegendItem(string label, Color color, int thickness) {
      Label = label;
      Color = color;
      Thickness = thickness;
    }

    public string Label { get; set; }
    public Color Color { get; set; }
    public int Thickness { get; set; }
  }


  public class LegendShape : WorldShape {
    private readonly IList<LegendItem> legendItems = new List<LegendItem>();
    
    public LegendShape() {
      CreateLegend();
    }

    public void CreateLegend() {
      shapes.Clear();
      double y = BoundingBox.Y2;
      foreach (LegendItem item in legendItems) {
        AddShape(new LineShape(10, y - 10, 30, y - 10, 0, item.Color, item.Thickness, DrawingStyle.Solid));
        AddShape(new TextShape(35, y, item.Label));
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
  }
}