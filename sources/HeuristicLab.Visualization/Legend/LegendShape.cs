using System;
using System.Collections.Generic;
using System.Drawing;

namespace HeuristicLab.Visualization.Legend {
  public enum LegendPosition {
    Top,
    Bottom,
    Left,
    Right
  }

  public class LegendShape : WorldShape {
    private readonly IList<LegendItem> legendItems = new List<LegendItem>();
    // legend draw default value: column
    private bool row;
    private bool top;


    private Color color = Color.Blue;
    private Font font = new Font("Arial", 8);

    public LegendShape() {
      CreateLegend();
    }

    public bool Row {
      set { row = value; }
      get { return row; }
    }

    public bool Top {
      set { top = value; }
      get { return top; }
    }

    public void CreateLegend() {
      ClearShapes();
      double x = ClippingArea.X1;
      double y = ClippingArea.Y2;
      int numberOfItemsPerRow = 0;
      int rowCounter = 0;
      int rowsToDraw = 1;
      if (row) {
        numberOfItemsPerRow = (int)ClippingArea.Width/LegendItem.WIDTH;
        int nrOfItems = legendItems.Count;
        if (nrOfItems > numberOfItemsPerRow) {
          int rest;
          int value = Math.DivRem(nrOfItems, numberOfItemsPerRow, out rest);
          rowsToDraw = value + rest;
        }
        if (!top)
          y = 25*rowsToDraw;
      }
      foreach (LegendItem item in legendItems) {
        if (!row) {
          CreateColumn(item, y);
          y -= 15;
        } else {
          if (rowCounter >= numberOfItemsPerRow) {
            x = ClippingArea.X1;
            y -= 25;
          }
          CreateRow(item, x, y);
          x += LegendItem.WIDTH;
          rowCounter++;
        }
      }
    }

    /// <summary>
    /// draws the legend as a row at the top or bottom of the WorldShape
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreateRow(LegendItem item, double x, double y) {
      AddShape(new LineShape(x, y - 10, x + 20, y - 10, item.Color, item.Thickness, DrawingStyle.Solid));
      AddShape(new TextShape(x + 25, y, item.Label, Font, Color));
    }

    /// <summary>
    /// draws the legend as a column on the right or left side of the WorldShape
    /// </summary>
    /// <param name="item"></param>
    /// <param name="y"></param>
    private void CreateColumn(LegendItem item, double y) {
      AddShape(new LineShape(10, y - 10, 30, y - 10, item.Color, item.Thickness, DrawingStyle.Solid));
      AddShape(new TextShape(35, y, item.Label, Font, Color));
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
