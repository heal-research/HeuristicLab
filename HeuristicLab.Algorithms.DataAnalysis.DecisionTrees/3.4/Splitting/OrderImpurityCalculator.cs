#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Helper class for incremental split calculation.
  /// Used while moving a potential splitter along the ordered training instances
  /// </summary>
  internal class OrderImpurityCalculator {
    internal enum IncrementType {
      Left,
      Right,
      None
    }

    #region Properties
    private double SqSumLeft { get; set; }
    private double SqSumRight { get; set; }
    private double VarLeft { get; set; }
    private double VarRight { get; set; }
    private double Order { get; set; }
    private double VarTotal { get; set; }
    private int NoInstances { get; set; }

    private double NoLeft { get; set; }
    private double NoRight { get; set; }
    private double SumLeft { get; set; }
    private double SumRight { get; set; }
    public double Impurity { get; private set; }
    #endregion

    #region Constructors
    public OrderImpurityCalculator(int partition, IReadOnlyCollection<double> data, double order) {
      var values = data;
      NoInstances = data.Count;
      VarTotal = values.VariancePop();

      values = data.Take(partition).ToArray();
      NoLeft = partition;
      SumLeft = values.Sum();
      SqSumLeft = values.Sum(x => x * x);

      values = data.Skip(partition).ToArray();
      NoRight = NoInstances - NoLeft;
      SumRight = values.Sum();
      SqSumRight = values.Sum(x => x * x);

      Order = order;
      Increment(0.0, IncrementType.None);
    }
    #endregion

    #region IImpurityCalculator
    public void Increment(double value, IncrementType type) {
      double y, yl, yr;
      var valSq = value * value;

      switch (type) {
        case IncrementType.Left:
          NoLeft++;
          NoRight--;
          SumLeft += value;
          SumRight -= value;
          SqSumLeft += valSq;
          SqSumRight -= valSq;
          break;
        case IncrementType.Right:
          NoLeft--;
          NoRight++;
          SumLeft -= value;
          SumRight += value;
          SqSumLeft -= valSq;
          SqSumRight += valSq;
          break;
        case IncrementType.None:
          break;
        default:
          throw new ArgumentOutOfRangeException(type.ToString(), type, null);
      }

      VarLeft = NoLeft <= 0 ? 0 : Math.Abs(NoLeft * SqSumLeft - SumLeft * SumLeft) / (NoLeft * NoLeft);
      VarRight = NoRight <= 0 ? 0 : Math.Abs(NoRight * SqSumRight - SumRight * SumRight) / (NoRight * NoRight);

      if (Order <= 0) throw new ArgumentException("Splitter order must be larger than 0");
      if (Order.IsAlmost(1)) {
        y = VarTotal;
        yl = VarLeft;
        yr = VarRight;
      }
      else {
        y = Math.Pow(VarTotal, 1.0 / Order);
        yl = Math.Pow(VarLeft, 1.0 / Order);
        yr = Math.Pow(VarRight, 1.0 / Order);
      }
      if (NoLeft <= 0.0 || NoRight <= 0.0) Impurity = double.MinValue; //Splitter = 0;
      else Impurity = y - (NoLeft * yl + NoRight * yr) / (NoRight + NoLeft);
    }
    #endregion
  }
}