using System.Collections.Generic;
using HeuristicLab.Visualization.Legend;

namespace HeuristicLab.Visualization.Legend {
  public class LegendShape : WorldShape {
    private readonly IList<LegendItem> legendItems = new List<LegendItem>();
    
    public LegendShape() {
      CreateLegend();
    }

    public void CreateLegend() {
      shapes.Clear();
      double y = ClippingArea.Y2;
      foreach (LegendItem item in legendItems) {
        AddShape(new LineShape(10, y - 10, 30, y - 10, item.Color, item.Thickness, DrawingStyle.Solid));
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