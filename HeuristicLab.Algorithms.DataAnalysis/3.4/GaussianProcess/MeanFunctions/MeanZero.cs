#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("7B76ECDD-A7B1-450F-B542-D25E19480FC5")]
  [Item(Name = "MeanZero", Description = "Constant zero mean function for Gaussian processes.")]
  public sealed class MeanZero : Item, IMeanFunction {
    [StorableConstructor]
    private MeanZero(StorableConstructorFlag _) : base(_) { }
    private MeanZero(MeanZero original, Cloner cloner)
      : base(original, cloner) {
    }
    public MeanZero() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanZero(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 0;
    }

    public void SetParameter(double[] p) {
      if (p.Length > 0) throw new ArgumentException("No parameters allowed for zero mean function.", "p");
    }

    public ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, int[] columnIndices) {
      if (p.Length > 0) throw new ArgumentException("No parameters allowed for zero mean function.", "p");
      var mf = new ParameterizedMeanFunction();
      mf.Mean = (x, i) => 0.0;
      mf.Gradient = (x, i, k) => {
        if (k > 0)
          throw new ArgumentException();
        return 0.0;
      };
      return mf;
    }
  }
}
