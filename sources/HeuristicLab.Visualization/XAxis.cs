using System;
using System.Collections.Generic;
using System.Drawing;
using HeuristicLab.Visualization.LabelProvider;

namespace HeuristicLab.Visualization {
  public class XAxis : WorldShape {
    public const int PixelsPerInterval = 100;
    private ILabelProvider labelProvider = new ContinuousLabelProvider("0.####");

    public ILabelProvider LabelProvider {
      get { return labelProvider; }
      set { labelProvider = value; }
    }

    public static IEnumerable<double> GetTicks(int pixelsPerInterval, int screenSize, double worldSize, double worldStart) {
      int intervals = screenSize/pixelsPerInterval;

      if (intervals > 0) {
        double step = worldSize/intervals;
        step = Math.Pow(10, Math.Floor(Math.Log10(step)));
        if (worldSize/(step*5) > intervals)
          step = step*5;
        else if (worldSize/(step*2) > intervals)
          step = step*2;

        for (double x = Math.Floor(worldStart/step)*step;
             x <= worldStart + worldSize;
             x += step)
          yield return x;
      }
    }

    public override void Draw(Graphics graphics, Rectangle viewport, RectangleD clippingArea) {
      shapes.Clear();

      foreach (double x in GetTicks(PixelsPerInterval, viewport.Width, ClippingArea.Width, ClippingArea.X1)) {
        TextShape label = new TextShape(x, ClippingArea.Height - 3,
                                        labelProvider.GetLabel(x));
        label.AnchorPositionX = AnchorPositionX.Middle;
        label.AnchorPositionY = AnchorPositionY.Top;
        shapes.Add(label);
      }

      base.Draw(graphics, viewport, clippingArea);
    }
  }
}