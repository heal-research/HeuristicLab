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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanLinear", Description = "Linear mean function for Gaussian processes.")]
  public sealed class MeanLinear : ParameterizedNamedItem, IMeanFunction {
    public IValueParameter<DoubleArray> WeightsParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Weights"]; }
    }

    [StorableConstructor]
    private MeanLinear(bool deserializing) : base(deserializing) { }
    private MeanLinear(MeanLinear original, Cloner cloner)
      : base(original, cloner) {
    }
    public MeanLinear()
      : base() {
      Parameters.Add(new OptionalValueParameter<DoubleArray>("Weights", "The weights parameter for the linear mean function."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanLinear(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return WeightsParameter.Value != null ? 0 : numberOfVariables;
    }

    public void SetParameter(double[] p) {
      double[] weights;
      GetParameter(p, out weights);
      WeightsParameter.Value = new DoubleArray(weights);
    }

    public void GetParameter(double[] p, out double[] weights) {
      if (WeightsParameter.Value == null) {
        weights = p;
      } else {
        if (p.Length != 0) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for the linear mean function.", "p");
        weights = WeightsParameter.Value.ToArray();
      }
    }

    public ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, int[] columnIndices) {
      double[] weights;
      int[] columns = columnIndices;
      GetParameter(p, out weights);
      var mf = new ParameterizedMeanFunction();
      mf.Mean = (x, i) => {
        // sanity check
        if (weights.Length != columns.Length) throw new ArgumentException("The number of rparameters must match the number of variables for the linear mean function.");
        return Util.ScalarProd(weights, Util.GetRow(x, i, columns).ToArray());
      };
      mf.Gradient = (x, i, k) => {
        if (k > columns.Length) throw new ArgumentException();
        return x[i, columns[k]];
      };
      return mf;
    }
  }
}
