#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item(Name = "GaussianProcessClassificationSolutionCreator",
    Description = "Creates a Gaussian process solution from a trained model.")]
  public sealed class GaussianProcessClassificationSolutionCreator : SingleSuccessorOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string ModelParameterName = "GaussianProcessClassificationModel";
    private const string SolutionParameterName = "Solution";
    private const string ResultsParameterName = "Results";
    private const string TrainingAccuracyResultName = "Accuracy (training)";
    private const string TestAccuracyResultName = "Accuracy (test)";

    #region Parameter Properties
    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<IDiscriminantFunctionClassificationSolution> SolutionParameter {
      get { return (ILookupParameter<IDiscriminantFunctionClassificationSolution>)Parameters[SolutionParameterName]; }
    }
    public ILookupParameter<IGaussianProcessModel> ModelParameter {
      get { return (ILookupParameter<IGaussianProcessModel>)Parameters[ModelParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }
    #endregion

    [StorableConstructor]
    private GaussianProcessClassificationSolutionCreator(bool deserializing) : base(deserializing) { }
    private GaussianProcessClassificationSolutionCreator(GaussianProcessClassificationSolutionCreator original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessClassificationSolutionCreator()
      : base() {
      // in
      Parameters.Add(new LookupParameter<IClassificationProblemData>(ProblemDataParameterName, "The classification problem data for the Gaussian process solution."));
      Parameters.Add(new LookupParameter<IGaussianProcessModel>(ModelParameterName, "The Gaussian process classification model to use for the solution."));
      // in & out
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection of the algorithm."));
      // out
      Parameters.Add(new LookupParameter<IDiscriminantFunctionClassificationSolution>(SolutionParameterName, "The produced Gaussian process solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessClassificationSolutionCreator(this, cloner);
    }

    public override IOperation Apply() {
      if (ModelParameter.ActualValue != null) {
        var m = (IGaussianProcessModel)ModelParameter.ActualValue.Clone();
        m.FixParameters();
        var data = (IClassificationProblemData)ProblemDataParameter.ActualValue.Clone();
        var model = new DiscriminantFunctionClassificationModel(m, new NormalDistributionCutPointsThresholdCalculator());
        model.RecalculateModelParameters(data, data.TrainingIndices);
        var s = model.CreateDiscriminantFunctionClassificationSolution(data);

        SolutionParameter.ActualValue = s;
        var results = ResultsParameter.ActualValue;
        if (!results.ContainsKey(SolutionParameterName)) {
          results.Add(new Result(SolutionParameterName, "The Gaussian process classification solution", s));
          results.Add(new Result(TrainingAccuracyResultName,
                                 "The accuracy of the Gaussian process solution on the training partition.",
                                 new DoubleValue(s.TrainingAccuracy)));
          results.Add(new Result(TestAccuracyResultName,
                                 "The accuracy of the Gaussian process solution on the test partition.",
                                 new DoubleValue(s.TestAccuracy)));
        } else {
          results[SolutionParameterName].Value = s;
          results[TrainingAccuracyResultName].Value = new DoubleValue(s.TrainingAccuracy);
          results[TestAccuracyResultName].Value = new DoubleValue(s.TestAccuracy);
        }
      }
      return base.Apply();
    }
  }
}
