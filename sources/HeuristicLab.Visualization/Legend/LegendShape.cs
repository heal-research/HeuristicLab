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

    private bool ExistsLegendItems() {
      return legendItems.Count > 0;
    }
    
    /// <summary>
    /// paints the legend on the canvas
    /// </summary>
    public void CreateLegend() {
      if (ExistsLegendItems()) {
        ClearShapes();
        double x = ClippingArea.X1;
        double y = ClippingArea.Y2;

        int numberOfItemsPerRow = 1;
        if (row) {
          numberOfItemsPerRow = GetNrOfItemsPerRow();
          if (!top) {
            y = GetHeight4Rows();
          }
        }
        int rowCounter = 0;
        foreach (LegendItem item in legendItems) {
          if (!row) {
            CreateColumn(item, y);
            y -= Font.Height;
          } else {
            if (rowCounter >= numberOfItemsPerRow) {
              x = ClippingArea.X1;
              y -= Font.Height+15;
              rowCounter = 0;
            }
            CreateRow(item, x, y);
            x += GetLabelLengthInPixel(item);
            rowCounter++;
          }
        }
      }
    }

    /// <summary>
    /// returns die optimal height for the top or bottom legend
    /// </summary>
    /// <returns></returns>
    private int GetHeight4Rows() {
      int numberOfItemsPerRow = GetNrOfItemsPerRow();
      int nrOfItems = legendItems.Count;
      int rowsToDraw = 1;
      if (nrOfItems > numberOfItemsPerRow) {
        int rest;
        int value = Math.DivRem(nrOfItems, numberOfItemsPerRow, out rest);
        if (rest > 1) {
          rowsToDraw = rest;
        } else {
          rowsToDraw = value + rest;
        }
      }
      return (Font.Height+12)*rowsToDraw;
    }

    /// <summary>
    /// returns the maximum number of items per row to paint
    /// </summary>
    /// <returns></returns>
    private int GetNrOfItemsPerRow() {
      int numberOfItemsPerRow = (int)ClippingArea.Width / GetMaxLabelLength();
      // if the width of the clippingarea < maxLabelLength
      if (numberOfItemsPerRow == 0) numberOfItemsPerRow = 1;
      return numberOfItemsPerRow;
    }

    /// <summary>
    /// returns the length of the current legenditem in pixel
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private int GetLabelLengthInPixel(LegendItem item) {
      int dummy = (int)(item.Label.Length*Font.Size + 20);
      if (dummy < LegendItem.WIDTH) {
        return LegendItem.WIDTH;
      }
      return dummy;
    }

    /// <summary>
    /// draws the legend as a row at the top or bottom of the WorldShape
    /// </summary>
    /// <param name="item">the legenditem to draw</param>
    /// <param name="x">x axis to draw the item</param>
    /// <param name="y">y axis to draw the item</param>
    private void CreateRow(LegendItem item, double x, double y) {
      AddShape(new LineShape(x, y - (Font.Height / 2), x + 20, y - (Font.Height / 2), item.Color, item.Thickness, DrawingStyle.Solid));
      AddShape(new TextShape(x + 25, y, item.Label, Font, Color));
    }

    /// <summary>
    /// draws the legend as a column on the right or left side of the WorldShape
    /// </summary>
    /// <param name="item">the legenditem to draw</param>
    /// <param name="y">y axis to draw the item</param>
    private void CreateColumn(LegendItem item, double y) {
      AddShape(new LineShape(10, y - (Font.Height / 2), 30, y - (Font.Height / 2), item.Color, item.Thickness, DrawingStyle.Solid));
      AddShape(new TextShape(Font.Height+12, y, item.Label, Font, Color));
    }

    /// <summary>
    /// adds a legenditem to the items list
    /// </summary>
    /// <param name="item">legenditem to add</param>
    public void AddLegendItem(LegendItem item) {
      legendItems.Add(item);
    }

    /// <summary>
    /// searches the longest label and returns it with factor of the the current font size
    /// useful to set the width of the legend
    /// </summary>
    /// <returns>max label length with factor of the current font size</returns>
    public int GetMaxLabelLength() {
      int maxLabelLength = 0;
      if (ExistsLegendItems()) {
        maxLabelLength = legendItems[0].Label.Length;
        foreach (var item in legendItems) {
          if (item.Label.Length > maxLabelLength) {
            maxLabelLength = item.Label.Length;
          }
        }
        maxLabelLength = (int)(maxLabelLength*Font.Size);
      }
      return maxLabelLength < LegendItem.WIDTH ? LegendItem.WIDTH : maxLabelLength;
    }

    /// <summary>
    /// removes a legenditem from the items list
    /// </summary>
    /// <param name="item">legenditem to remove</param>
    public void RemoveLegendItem(LegendItem item) {
      legendItems.Remove(item);
    }

    /// <summary>
    /// deletes the legenditem list
    /// </summary>
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

    /// <summary>
    /// updates the font settings of the legend
    /// </summary>
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
