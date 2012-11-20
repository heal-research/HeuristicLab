#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanZero", Description = "Constant zero mean function for Gaussian processes.")]
  public sealed class MeanZero : Item, IMeanFunction {
    [StorableConstructor]
    private MeanZero(bool deserializing) : base(deserializing) { }
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

    public void SetParameter(double[] hyp) {
      if (hyp.Length > 0) throw new ArgumentException("No hyper-parameters allowed for zero mean function.", "hyp");
    }

    public double[] GetMean(double[,] x) {
      return Enumerable.Repeat(0.0, x.GetLength(0)).ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      if (k > 0) throw new ArgumentException();
      return Enumerable.Repeat(0.0, x.GetLength(0)).ToArray();
    }
  }
}
