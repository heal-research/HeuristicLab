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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  public abstract class KernelBase : ParameterizedNamedItem, IKernel {

    private const string DistanceParameterName = "Distance";

    public IValueParameter<IDistance> DistanceParameter {
      get { return (IValueParameter<IDistance>)Parameters[DistanceParameterName]; }
    }

    [Storable]
    private double? beta;
    public double? Beta {
      get { return beta; }
      set {
        if (value != beta) {
          beta = value;
          RaiseBetaChanged();
        }
      }
    }

    public IDistance Distance {
      get { return DistanceParameter.Value; }
      set {
        if (DistanceParameter.Value != value) {
          DistanceParameter.Value = value;
        }
      }
    }

    [StorableConstructor]
    protected KernelBase(bool deserializing) : base(deserializing) { }

    protected KernelBase(KernelBase original, Cloner cloner)
      : base(original, cloner) {
      beta = original.beta;
      RegisterEvents();
    }

    protected KernelBase() {
      Parameters.Add(new ValueParameter<IDistance>(DistanceParameterName, "The distance function used for kernel calculation"));
      DistanceParameter.Value = new EuclideanDistance();
      RegisterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      DistanceParameter.ValueChanged += (sender, args) => RaiseDistanceChanged();
    }

    public double Get(object a, object b) {
      return Get(Distance.Get(a, b));
    }

    protected abstract double Get(double norm);

    public int GetNumberOfParameters(int numberOfVariables) {
      return Beta.HasValue ? 0 : 1;
    }

    public void SetParameter(double[] p) {
      if (p != null && p.Length == 1) Beta = new double?(p[0]);
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      if (p.Length != GetNumberOfParameters(columnIndices.Length)) throw new ArgumentException("Illegal parametrization");
      var myClone = (KernelBase)Clone();
      myClone.SetParameter(p);
      var cov = new ParameterizedCovarianceFunction {
        Covariance = (x, i, j) => myClone.Get(GetNorm(x, x, i, j, columnIndices)),
        CrossCovariance = (x, xt, i, j) => myClone.Get(GetNorm(x, xt, i, j, columnIndices)),
        CovarianceGradient = (x, i, j) => new List<double> { myClone.GetGradient(GetNorm(x, x, i, j, columnIndices)) }
      };
      return cov;
    }

    protected abstract double GetGradient(double norm);

    protected double GetNorm(double[,] x, double[,] xt, int i, int j, int[] columnIndices) {
      var dist = Distance as IDistance<IEnumerable<double>>;
      if (dist == null) throw new ArgumentException("The distance needs to apply to double vectors");
      var r1 = columnIndices.Select(c => x[i, c]);
      var r2 = columnIndices.Select(c => xt[j, c]);
      return dist.Get(r1, r2);
    }

    #region events
    public event EventHandler BetaChanged;
    public event EventHandler DistanceChanged;

    protected void RaiseBetaChanged() {
      var handler = BetaChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    protected void RaiseDistanceChanged() {
      var handler = DistanceChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}