using System;
using System.Collections.Generic;

namespace HeuristicLab.Visualization {
  public static class AxisTicks {
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
  }
}