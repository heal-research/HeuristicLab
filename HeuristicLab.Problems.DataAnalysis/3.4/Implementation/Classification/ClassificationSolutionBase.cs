#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  public abstract class ClassificationSolutionBase : DataAnalysisSolution, IClassificationSolution {
    private const string TrainingAccuracyResultName = "Accuracy (training)";
    private const string TestAccuracyResultName = "Accuracy (test)";
    private const string TrainingNormalizedGiniCoefficientResultName = "Normalized Gini Coefficient (training)";
    private const string TestNormalizedGiniCoefficientResultName = "Normalized Gini Coefficient (test)";

    public new IClassificationModel Model {
      get { return (IClassificationModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IClassificationProblemData ProblemData {
      get { return (IClassificationProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    #region Results
    public double TrainingAccuracy {
      get { return ((DoubleValue)this[TrainingAccuracyResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingAccuracyResultName].Value).Value = value; }
    }
    public double TestAccuracy {
      get { return ((DoubleValue)this[TestAccuracyResultName].Value).Value; }
      private set { ((DoubleValue)this[TestAccuracyResultName].Value).Value = value; }
    }
    public double TrainingNormalizedGiniCoefficient {
      get { return ((DoubleValue)this[TrainingNormalizedGiniCoefficientResultName].Value).Value; }
      protected set { ((DoubleValue)this[TrainingNormalizedGiniCoefficientResultName].Value).Value = value; }
    }
    public double TestNormalizedGiniCoefficient {
      get { return ((DoubleValue)this[TestNormalizedGiniCoefficientResultName].Value).Value; }
      protected set { ((DoubleValue)this[TestNormalizedGiniCoefficientResultName].Value).Value = value; }
    }
    #endregion

    [StorableConstructor]
    protected ClassificationSolutionBase(bool deserializing) : base(deserializing) { }
    protected ClassificationSolutionBase(ClassificationSolutionBase original, Cloner cloner)
      : base(original, cloner) {
    }
    protected ClassificationSolutionBase(IClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingAccuracyResultName, "Accuracy of the model on the training partition (percentage of correctly classified instances).", new PercentValue()));
      Add(new Result(TestAccuracyResultName, "Accuracy of the model on the test partition (percentage of correctly classified instances).", new PercentValue()));
      Add(new Result(TrainingNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the training partition.", new DoubleValue()));
      Add(new Result(TestNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the test partition.", new DoubleValue()));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!this.ContainsKey(TrainingNormalizedGiniCoefficientResultName))
        Add(new Result(TrainingNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the training partition.", new DoubleValue()));
      if (!this.ContainsKey(TestNormalizedGiniCoefficientResultName))
        Add(new Result(TestNormalizedGiniCoefficientResultName, "Normalized Gini coefficient of the model on the test partition.", new DoubleValue()));
    }

    protected void CalculateResults() {
      double[] estimatedTrainingClassValues = EstimatedTrainingClassValues.ToArray(); // cache values
      double[] originalTrainingClassValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes).ToArray();
      double[] estimatedTestClassValues = EstimatedTestClassValues.ToArray(); // cache values
      double[] originalTestClassValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TestIndizes).ToArray();

      OnlineCalculatorError errorState;
      double trainingAccuracy = OnlineAccuracyCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) trainingAccuracy = double.NaN;
      double testAccuracy = OnlineAccuracyCalculator.Calculate(originalTestClassValues, estimatedTestClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) testAccuracy = double.NaN;

      TrainingAccuracy = trainingAccuracy;
      TestAccuracy = testAccuracy;

      double trainingNormalizedGini = NormalizedGiniCalculator.Calculate(originalTrainingClassValues, estimatedTrainingClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) trainingNormalizedGini = double.NaN;
      double testNormalizedGini = NormalizedGiniCalculator.Calculate(originalTestClassValues, estimatedTestClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) testNormalizedGini = double.NaN;

      TrainingNormalizedGiniCoefficient = trainingNormalizedGini;
      TestNormalizedGiniCoefficient = testNormalizedGini;
    }

    public abstract IEnumerable<double> EstimatedClassValues { get; }
    public abstract IEnumerable<double> EstimatedTrainingClassValues { get; }
    public abstract IEnumerable<double> EstimatedTestClassValues { get; }

    public abstract IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows);
  }
}
