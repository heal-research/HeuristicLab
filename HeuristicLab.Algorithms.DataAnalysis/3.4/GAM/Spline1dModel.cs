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

using HEAL.Attic;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Spline model (1d)",
    "Univariate spline model (wrapper for alglib.spline1dmodel)")]
  [StorableType("23D71839-E011-4DC5-B451-2D4C1177D743")]
  public sealed class Spline1dModel : RegressionModel {
    // not storable! see persistence properties below
    private alglib.spline1d.spline1dinterpolant interpolant;

    [Storable]
    private readonly string[] variablesUsedForPrediction;
    public override IEnumerable<string> VariablesUsedForPrediction {
      get {
        return variablesUsedForPrediction;
      }
    }

    [StorableConstructor]
    private Spline1dModel(StorableConstructorFlag deserializing) : base(deserializing) {
      this.interpolant = new alglib.spline1d.spline1dinterpolant();
    }

    private Spline1dModel(Spline1dModel orig, Cloner cloner) : base(orig, cloner) {
      this.variablesUsedForPrediction = orig.VariablesUsedForPrediction.ToArray();
      this.interpolant = (alglib.spline1d.spline1dinterpolant)orig.interpolant.make_copy();
    }
    public Spline1dModel(alglib.spline1d.spline1dinterpolant interpolant, string targetVar, string inputVar)
      : base(targetVar, "Spline model (1d)") {
      this.interpolant = (alglib.spline1d.spline1dinterpolant)interpolant.make_copy();      
      this.variablesUsedForPrediction = new string[] { inputVar };
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new Spline1dModel(this, cloner);
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, (IRegressionProblemData)problemData.Clone());
    }

    public double GetEstimatedValue(double x) {
      return alglib.spline1d.spline1dcalc(interpolant, x);
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      var x = dataset.GetDoubleValues(VariablesUsedForPrediction.First(), rows).ToArray();
      foreach (var xi in x) {
        yield return GetEstimatedValue(xi);
      }
    }

    #region persistence
    [Storable]
    private double[] Interpolant_c {
      get { return interpolant.c; }
      set { interpolant.c = value; }
    }
    [Storable]
    private double[] Interpolant_x {
      get { return interpolant.x; }
      set { interpolant.x = value; }
    }
    [Storable]
    private int Interpolant_continuity {
      get { return interpolant.continuity; }
      set { interpolant.continuity = value; }
    }
    [Storable]
    private int Interpolant_k {
      get { return interpolant.k; }
      set { interpolant.k = value; }
    }
    [Storable]
    private int Interpolant_n {
      get { return interpolant.n; }
      set { interpolant.n = value; }
    }
    [Storable]
    private bool Interpolant_periodic {
      get { return interpolant.periodic; }
      set { interpolant.periodic = value; }
    }
    #endregion
  }
}
