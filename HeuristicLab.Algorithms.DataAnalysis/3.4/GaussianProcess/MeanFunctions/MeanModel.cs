#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "MeanModel", Description = "A mean function for Gaussian processes that uses a regression solution created with a different algorithm to calculate the mean.")]
  // essentially an adaptor which maps from IMeanFunction to IRegressionSolution
  public sealed class MeanModel : ParameterizedNamedItem, IMeanFunction {
    public IValueParameter<IRegressionSolution> RegressionSolutionParameter {
      get { return (IValueParameter<IRegressionSolution>)Parameters["RegressionSolution"]; }
    }
    public IRegressionSolution RegressionSolution {
      get { return RegressionSolutionParameter.Value; }
      set { RegressionSolutionParameter.Value = value; }
    }

    [StorableConstructor]
    private MeanModel(bool deserializing) : base(deserializing) { }
    private MeanModel(MeanModel original, Cloner cloner)
      : base(original, cloner) {
    }

    public MeanModel()
      : base("MeanModel", "A mean function for Gaussian processes that uses a regression solution created with a different algorithm to calculate the mean.") {
      Parameters.Add(new ValueParameter<IRegressionSolution>("RegressionSolution", "The solution containing the model that should be used for the mean prediction."));
    }

    public MeanModel(IRegressionSolution solution)
      : this() {
      // here we cannot check if the model is actually compatible (uses only input variables that are available)
      // we only assume that the list of allowed inputs in the regression solution is the same as the list of allowed
      // inputs in the Gaussian process.
      // later we might get an error or bad behaviour when the mean function is evaluated
      RegressionSolution = solution;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MeanModel(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 0; // no support for hyperparameters for regression models yet
    }

    public void SetParameter(double[] p) {
      if (p.Length > 0) throw new ArgumentException("No parameters allowed for model-based mean function.", "p");
    }

    public ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, int[] columnIndices) {
      if (p.Length > 0) throw new ArgumentException("No parameters allowed for model-based mean function.", "p");
      var solution = RegressionSolution;
      var variableNames = solution.ProblemData.AllowedInputVariables.ToArray();
      if (variableNames.Length != columnIndices.Length)
        throw new ArgumentException("The number of input variables does not match in MeanModel");
      var variableValues = variableNames.Select(_ => new List<double>() { 0.0 }).ToArray(); // or of zeros
      // uses modifyable dataset to pass values to the model
      var ds = new ModifiableDataset(variableNames, variableValues);
      var mf = new ParameterizedMeanFunction();
      var model = solution.Model; // effort for parameter access only once
      mf.Mean = (x, i) => {
        ds.ReplaceRow(0, Util.GetRow(x, i, columnIndices).OfType<object>());
        return model.GetEstimatedValues(ds, 0.ToEnumerable()).Single(); // evaluate the model on the specified row only
      };
      mf.Gradient = (x, i, k) => {
        if (k > 0)
          throw new ArgumentException();
        return 0.0;
      };
      return mf;
    }
  }
}
