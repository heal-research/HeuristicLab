#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

using System;

namespace HeuristicLab.Visualization.ChartControlsExtensions {
  public static class ChartUtil {
    public static void CalculateAxisInterval(double min, double max, int ticks, out double axisMin, out double axisMax, out double axisInterval) {
      var dmin = min.Decimals();
      axisMin = min.Floor(dmin);
      var range = max - axisMin;
      var slice = range / ticks;
      var dslice = slice.Decimals();
      var floor = slice.Floor(dslice);
      var ceil = slice.Ceil(dslice);
      var axisRange = floor * ticks;
      axisInterval = floor;
      if (axisRange < max - axisMin) {
        axisRange = ceil * ticks;
        axisInterval = ceil;
      }
      axisMax = axisMin + axisRange;
    }

    private static int Decimals(this double x) {
      if (x.IsAlmost(0) || double.IsInfinity(x) || double.IsNaN(x))
        return 0;

      var v = Math.Abs(x);
      int d = 0;
      while (v < 1) {
        v *= 10;
        d++;
      }
      return d;
    }

    // rounds down to the nearest value according to the given number of decimal precision
    private static double Floor(this double value, int precision) {
      var n = Math.Pow(10, precision);
      return Math.Round(Math.Floor(value * n) / n, precision);
    }

    private static double Ceil(this double value, int precision) {
      var n = Math.Pow(10, precision);
      return Math.Round(Math.Ceiling(value * n) / n, precision);
    }

    private static double Round(this double value, int precision) {
      var n = Math.Pow(10, precision);
      return Math.Round(Math.Round(value * n) / n, precision);
    }

    private static bool IsAlmost(this double value, double other, double eps = 1e-12) {
      return Math.Abs(value - other) < eps;
    }
  }
}
