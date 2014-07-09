#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "GaussianProcessRegressionSolutionCreator",
    Description = "Creates a Gaussian process solution from a trained model.")]
  public sealed class GaussianProcessRegressionSolutionCreator : SingleSuccessorOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string ModelParameterName = "GaussianProcessRegressionModel";
    private const string SolutionParameterName = "Solution";
    private const string ResultsParameterName = "Results";
    private const string TrainingRSquaredResultName = "Training R²";
    private const string TestRSquaredResultName = "Test R²";

    #region Parameter Properties
    public ILookupParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<IGaussianProcessSolution> SolutionParameter {
      get { return (ILookupParameter<IGaussianProcessSolution>)Parameters[SolutionParameterName]; }
    }
    public ILookupParameter<IGaussianProcessModel> ModelParameter {
      get { return (ILookupParameter<IGaussianProcessModel>)Parameters[ModelParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private GaussianProcessRegressionSolutionCreator(bool deserializing) : base(deserializing) { }
    private GaussianProcessRegressionSolutionCreator(GaussianProcessRegressionSolutionCreator original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessRegressionSolutionCreator()
      : base() {
      // in
      Parameters.Add(new LookupParameter<IRegressionProblemData>(ProblemDataParameterName, "The regression problem data for the Gaussian process solution."));
      Parameters.Add(new LookupParameter<IGaussianProcessModel>(ModelParameterName, "The Gaussian process regression model to use for the solution."));
      // in & out
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection of the algorithm."));
      // out
      Parameters.Add(new LookupParameter<IGaussianProcessSolution>(SolutionParameterName, "The produced Gaussian process solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessRegressionSolutionCreator(this, cloner);
    }

    public override IOperation Apply() {
      if (ModelParameter.ActualValue != null) {
        var m = (IGaussianProcessModel)ModelParameter.ActualValue.Clone();
        m.FixParameters();
        var data = (IRegressionProblemData)ProblemDataParameter.ActualValue.Clone();
        var s = new GaussianProcessRegressionSolution(m, data);


        SolutionParameter.ActualValue = s;
        var results = ResultsParameter.ActualValue;
        if (!results.ContainsKey(SolutionParameterName)) {
          results.Add(new Result(SolutionParameterName, "The Gaussian process regression solution", s));
          results.Add(new Result(TrainingRSquaredResultName,
                                 "The Pearson's R² of the Gaussian process solution on the training partition.",
                                 new DoubleValue(s.TrainingRSquared)));
          results.Add(new Result(TestRSquaredResultName,
                                 "The Pearson's R² of the Gaussian process solution on the test partition.",
                                 new DoubleValue(s.TestRSquared)));
        } else {
          results[SolutionParameterName].Value = s;
          results[TrainingRSquaredResultName].Value = new DoubleValue(s.TrainingRSquared);
          results[TestRSquaredResultName].Value = new DoubleValue(s.TestRSquared);
        }
      }
      return base.Apply();
    }
  }
}
