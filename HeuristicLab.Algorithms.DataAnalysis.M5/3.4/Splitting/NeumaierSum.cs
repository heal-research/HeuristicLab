#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Runtime.CompilerServices;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Helper class for incremental split calculation.
  /// Used while moving a potential Splitter along the ordered training Instances
  /// </summary>
  internal class NeumaierSum {
    #region state
    private double sum;
    private double correction;
    #endregion

    #region Constructors
    public NeumaierSum(double startvalue) {
      sum = startvalue;
      correction = 0;
    }
    #endregion

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void Add(double value) {
      var t = sum + value;
      var absSum = sum > 0 ? sum : -sum;
      var absv = value > 0 ? value : -value;
      if (absSum >= absv)
        correction += (sum - t) + value;
      else
        correction += (value - t) + sum;
      sum = t;
    }

    public double Get() {
      return sum + correction;
    }

    public void Mul(double value) {
      sum *= value;
      correction *= value;
    }
  }
}