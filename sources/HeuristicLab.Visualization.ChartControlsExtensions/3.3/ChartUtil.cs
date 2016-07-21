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
      if (double.IsInfinity(min) || double.IsNaN(min) || double.IsInfinity(max) || double.IsNaN(max) || (min >= max))
        throw new ArgumentOutOfRangeException("Invalid range provided.");

      var range = max - min;
      var dRange = (int)Math.Round(Math.Log10(range));
      int decimalRank = dRange - 1;
      var aMin = min.RoundDown(decimalRank);
      var aMax = max.RoundUp(decimalRank);

      // if one of the interval ends is a multiple of 5 or 10, change the other interval end to be a multiple as well
      if ((aMin.Mod(5).IsAlmost(0) || aMin.Mod(10).IsAlmost(0)) && Math.Abs(aMax) >= 5 && !(aMax.Mod(5).IsAlmost(0) || aMax.Mod(10).IsAlmost(0))) {
        aMax = Math.Min(aMax + 5 - aMax % 5, aMax + 10 - aMax % 10);
      } else if ((aMax.Mod(5).IsAlmost(0) || aMax.Mod(10).IsAlmost(0)) && Math.Abs(aMin) >= 5 && !(aMin.Mod(5).IsAlmost(0) || aMin.Mod(10).IsAlmost(0))) {
        aMin = Math.Max(aMin - aMin.Mod(5), aMin - aMin.Mod(10));
      }

      axisMin = aMin;
      axisMax = aMax;
      axisInterval = (aMax - aMin) / ticks;
    }

    /// <summary>
    /// Tries to find an axis interval with as few fractional digits as possible (because it looks nicer).  we only try between 3 and 5 ticks (inclusive) because it wouldn't make sense to exceed this interval.
    /// </summary>
    public static void CalculateOptimalAxisInterval(double min, double max, out double axisMin, out double axisMax, out double axisInterval) {
      CalculateAxisInterval(min, max, 5, out axisMin, out axisMax, out axisInterval);
      int bestLsp = int.MaxValue;
      for (int ticks = 3; ticks <= 5; ++ticks) {
        double aMin, aMax, aInterval;
        CalculateAxisInterval(min, max, ticks, out aMin, out aMax, out aInterval);
        var x = aInterval;
        int lsp = 0; // position of the least significant fractional digit
        while (x - Math.Floor(x) > 0) {
          ++lsp;
          x *= 10;
        }
        if (lsp <= bestLsp) {
          axisMin = aMin;
          axisMax = aMax;
          axisInterval = aInterval;
          bestLsp = lsp;
        }
      }
    }

    // find the number of decimals needed to represent the value
    private static int Decimals(this double x) {
      if (x.IsAlmost(0) || double.IsInfinity(x) || double.IsNaN(x))
        return 0;

      var v = Math.Abs(x);
      int d = 1;
      while (v < 1) {
        v *= 10;
        d++;
      }
      return d;
    }

    private static double RoundDown(this double value, int decimalRank) {
      if (decimalRank > 0) {
        var floor = (int)Math.Floor(value);
        var pow = (int)Math.Pow(10, decimalRank);
        var mod = Mod(floor, pow);
        return floor - mod;
      }
      return value.Floor(Math.Abs(decimalRank));
    }

    private static double RoundUp(this double value, int decimalRank) {
      if (decimalRank > 0) {
        var ceil = (int)Math.Ceiling(value);
        var pow = (int)Math.Pow(10, decimalRank);
        var mod = Mod(ceil, pow);
        return ceil - mod + pow;
      }
      return value.Ceil(Math.Abs(decimalRank));
    }

    private static double RoundNearest(this double value, int decimalRank) {
      var nearestDown = value.RoundDown(decimalRank);
      var nearestUp = value.RoundUp(decimalRank);

      if (nearestUp - value > value - nearestDown)
        return nearestDown;

      return nearestUp;
    }

    // rounds down to the nearest value according to the given number of decimal precision
    private static double Floor(this double value, int precision) {
      var n = Math.Pow(10, precision);
      return Math.Round(Math.Floor(value * n) / n, precision);
    }

    // rounds up to the nearest value according to the given number of decimal precision
    private static double Ceil(this double value, int precision) {
      var n = Math.Pow(10, precision);
      return Math.Round(Math.Ceiling(value * n) / n, precision);
    }

    private static bool IsAlmost(this double value, double other, double eps = 1e-12) {
      return Math.Abs(value - other) < eps;
    }

    private static double Mod(this double a, double b) {
      return a - b * Math.Floor(a / b);
    }
  }
}
