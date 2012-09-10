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
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanLinear", Description = "Linear mean function for Gaussian processes.")]
  public sealed class MeanLinear : ParameterizedNamedItem, IMeanFunction {
    [Storable]
    private double[] weights;
    [Storable]
    private readonly HyperParameter<DoubleArray> weightsParameter;
    public IValueParameter<DoubleArray> WeightsParameter { get { return weightsParameter; } }

    [StorableConstructor]
    private MeanLinear(bool deserializing) : base(deserializing) { }
    private MeanLinear(MeanLinear original, Cloner cloner)
      : base(original, cloner) {
      if (original.weights != null) {
        this.weights = new double[original.weights.Length];
        Array.Copy(original.weights, weights, original.weights.Length);
      }
      weightsParameter = cloner.Clone(original.weightsParameter);
      RegisterEvents();
    }
    public MeanLinear()
      : base() {
      this.weightsParameter = new HyperParameter<DoubleArray>("Weights", "The weights parameter for the linear mean function.");
      Parameters.Add(weightsParameter);
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanLinear(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      Util.AttachArrayChangeHandler<DoubleArray, double>(weightsParameter, () => {
        weights = weightsParameter.Value.ToArray();
      });
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return weightsParameter.Fixed ? 0 : numberOfVariables;
    }

    public void SetParameter(double[] hyp) {
      if (!weightsParameter.Fixed) {
        weightsParameter.SetValue(new DoubleArray(hyp));
      } else if (hyp.Length != 0) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for the linear mean function.", "hyp");
    }

    public double[] GetMean(double[,] x) {
      // sanity check
      if (weights.Length != x.GetLength(1)) throw new ArgumentException("The number of hyperparameters must match the number of variables for the linear mean function.");
      int cols = x.GetLength(1);
      int n = x.GetLength(0);
      return (from i in Enumerable.Range(0, n)
              let rowVector = Enumerable.Range(0, cols).Select(j => x[i, j])
              select Util.ScalarProd(weights, rowVector))
        .ToArray();
    }

    public double[] GetGradients(int k, double[,] x) {
      int cols = x.GetLength(1);
      int n = x.GetLength(0);
      if (k > cols) throw new ArgumentException();
      return (Enumerable.Range(0, n).Select(r => x[r, k])).ToArray();
    }
  }
}
