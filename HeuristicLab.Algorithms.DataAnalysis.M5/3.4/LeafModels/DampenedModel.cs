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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  //mulitdimensional extension of http://www2.stat.duke.edu/~tjl13/s101/slides/unit6lec3H.pdf
  [StorableType("42E9766F-207F-47B1-890C-D5DFCF469838")]
  public class DampenedModel : RegressionModel {
    [Storable]
    protected IRegressionModel Model;
    [Storable]
    private double Min;
    [Storable]
    private double Max;
    [Storable]
    private double Dampening;

    [StorableConstructor]
    protected DampenedModel(StorableConstructorFlag _) : base(_) { }
    protected DampenedModel(DampenedModel original, Cloner cloner) : base(original, cloner) {
      Model = cloner.Clone(original.Model);
      Min = original.Min;
      Max = original.Max;
      Dampening = original.Dampening;
    }
    protected DampenedModel(IRegressionModel model, IRegressionProblemData pd, double dampening) : base(model.TargetVariable) {
      Model = model;
      Min = pd.TargetVariableTrainingValues.Min();
      Max = pd.TargetVariableTrainingValues.Max();
      Dampening = dampening;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DampenedModel(this, cloner);
    }

    public static IConfidenceRegressionModel DampenModel(IConfidenceRegressionModel model, IRegressionProblemData pd, double dampening) {
      return new ConfidenceDampenedModel(model, pd, dampening);
    }
    public static IRegressionModel DampenModel(IRegressionModel model, IRegressionProblemData pd, double dampening) {
      var cmodel = model as IConfidenceRegressionModel;
      return cmodel != null ? new ConfidenceDampenedModel(cmodel, pd, dampening) : new DampenedModel(model, pd, dampening);
    }

    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return Model.VariablesUsedForPrediction; }
    }

    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      var slow = Sigmoid(-Dampening);
      var shigh = Sigmoid(Dampening);
      foreach (var x in Model.GetEstimatedValues(dataset, rows)) {
        var y = Rescale(x, Min, Max, -Dampening, Dampening);
        y = Sigmoid(y);
        y = Rescale(y, slow, shigh, Min, Max);
        yield return y;
      }
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, problemData);
    }

    private static double Rescale(double x, double oMin, double oMax, double nMin, double nMax) {
      var d = oMax - oMin;
      var nd = nMax - nMin;
      if (d.IsAlmost(0)) {
        d = 1;
        nMin += nd / 2;
        nd = 0;
      }
      return ((x - oMin) / d) * nd + nMin;
    }

    private static double Sigmoid(double x) {
      return 1 / (1 + Math.Exp(-x));
    }


    [StorableType("CCC93BEC-8796-4D8E-AC58-DD175073A79B")]
    private sealed class ConfidenceDampenedModel : DampenedModel, IConfidenceRegressionModel {
      #region HLConstructors
      [StorableConstructor]
      private ConfidenceDampenedModel(StorableConstructorFlag _) : base(_) { }
      private ConfidenceDampenedModel(ConfidenceDampenedModel original, Cloner cloner) : base(original, cloner) { }
      public ConfidenceDampenedModel(IConfidenceRegressionModel model, IRegressionProblemData pd, double dampening) : base(model, pd, dampening) { }
      public override IDeepCloneable Clone(Cloner cloner) {
        return new ConfidenceDampenedModel(this, cloner);
      }
      #endregion

      public IEnumerable<double> GetEstimatedVariances(IDataset dataset, IEnumerable<int> rows) {
        return ((IConfidenceRegressionModel)Model).GetEstimatedVariances(dataset, rows);
      }

      public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
        return new ConfidenceRegressionSolution(this, problemData);
      }
    }
  }
}