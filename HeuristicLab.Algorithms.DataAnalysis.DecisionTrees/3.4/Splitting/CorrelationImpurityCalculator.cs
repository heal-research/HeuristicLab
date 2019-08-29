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
  internal class CorreleationImpurityCalculator {
    #region state
    //Data
    private readonly List<double> attributeValues;
    private readonly List<double> targetValues;
    private readonly double order;
    private readonly UnivariateOnlineLR left;
    private readonly UnivariateOnlineLR right;
    #endregion

    #region Properties
    public double Impurity { get; private set; }
    public double SplitValue {
      get {
        if (left.Size <= 0) return double.NegativeInfinity;
        if (left.Size >= attributeValues.Count) return double.PositiveInfinity;
        return (attributeValues[left.Size - 1] + attributeValues[left.Size]) / 2;
      }
    }
    public bool ValidPosition {
      get { return !attributeValues[left.Size - 1].IsAlmost(attributeValues[left.Size]); }
    }
    public int LeftSize {
      get { return left.Size; }
    }
    #endregion

    #region Constructors
    public CorreleationImpurityCalculator(int partition, IEnumerable<double> atts, IEnumerable<double> targets, double order) {
      if (order <= 0) throw new ArgumentException("Splitter order must be larger than 0");
      this.order = order;
      attributeValues = atts.ToList();
      targetValues = targets.ToList();
      left = new UnivariateOnlineLR(attributeValues.Take(partition).ToList(), targetValues.Take(partition).ToList());
      right = new UnivariateOnlineLR(attributeValues.Skip(partition).ToList(), targetValues.Skip(partition).ToList());
      UpdateImpurity();
    }
    #endregion

    #region IImpurityCalculator
    public void Increment() {
      var target = targetValues[left.Size];
      var att = attributeValues[left.Size];
      left.Add(att, target);
      right.Remove(att, target);
      UpdateImpurity();
    }
    #endregion

    private void UpdateImpurity() {
      var yl = Math.Pow(left.Ssr, 1.0 / order);
      var yr = Math.Pow(right.Ssr, 1.0 / order);
      if (left.Size > 1 && right.Size > 1) Impurity = -yl - yr;
      else Impurity = double.MinValue;
    }
  }
}