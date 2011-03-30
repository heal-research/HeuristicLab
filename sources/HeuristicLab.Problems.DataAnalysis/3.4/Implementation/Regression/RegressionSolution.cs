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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Abstract base class for regression data analysis solutions
  /// </summary>
  [StorableClass]
  public abstract class RegressionSolution : DataAnalysisSolution, IRegressionSolution {
    private const string TrainingMeanSquaredErrorResultName = "Mean squared error (training)";
    private const string TestMeanSquaredErrorResultName = "Mean squared error (test)";
    private const string TrainingSquaredCorrelationResultName = "Pearson's R² (training)";
    private const string TestSquaredCorrelationResultName = "Pearson's R² (test)";
    private const string TrainingRelativeErrorResultName = "Average relative error (training)";
    private const string TestRelativeErrorResultName = "Average relative error (test)";

    public new IRegressionModel Model {
      get { return (IRegressionModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IRegressionProblemData ProblemData {
      get { return (IRegressionProblemData)base.ProblemData; }
      protected set { base.ProblemData = value; }
    }

    public double TrainingMeanSquaredError {
      get { return ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingMeanSquaredErrorResultName].Value).Value = value; }
    }

    public double TestMeanSquaredError {
      get { return ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestMeanSquaredErrorResultName].Value).Value = value; }
    }

    public double TrainingRSquared {
      get { return ((DoubleValue)this[TrainingSquaredCorrelationResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingSquaredCorrelationResultName].Value).Value = value; }
    }

    public double TestRSquared {
      get { return ((DoubleValue)this[TestSquaredCorrelationResultName].Value).Value; }
      private set { ((DoubleValue)this[TestSquaredCorrelationResultName].Value).Value = value; }
    }

    public double TrainingRelativeError {
      get { return ((DoubleValue)this[TrainingRelativeErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingRelativeErrorResultName].Value).Value = value; }
    }

    public double TestRelativeError {
      get { return ((DoubleValue)this[TestRelativeErrorResultName].Value).Value; }
      private set { ((DoubleValue)this[TestRelativeErrorResultName].Value).Value = value; }
    }


    [StorableConstructor]
    protected RegressionSolution(bool deserializing) : base(deserializing) { }
    protected RegressionSolution(RegressionSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public RegressionSolution(IRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingMeanSquaredErrorResultName, "Mean of squared errors of the model on the training partition", new DoubleValue()));
      Add(new Result(TestMeanSquaredErrorResultName, "Mean of squared errors of the model on the test partition", new DoubleValue()));
      Add(new Result(TrainingSquaredCorrelationResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the training partition", new DoubleValue()));
      Add(new Result(TestSquaredCorrelationResultName, "Squared Pearson's correlation coefficient of the model output and the actual values on the test partition", new DoubleValue()));
      Add(new Result(TrainingRelativeErrorResultName, "Average of the relative errors of the model output and the actual values on the training partition", new PercentValue()));
      Add(new Result(TestRelativeErrorResultName, "Average of the relative errors of the model output and the actual values on the test partition", new PercentValue()));

      RecalculateResults();
    }

    protected override void OnProblemDataChanged(EventArgs e) {
      base.OnProblemDataChanged(e);
      RecalculateResults();
    }
    protected override void OnModelChanged(EventArgs e) {
      base.OnModelChanged(e);
      RecalculateResults();
    }

    protected void RecalculateResults() {
      double[] estimatedTrainingValues = EstimatedTrainingValues.ToArray(); // cache values
      IEnumerable<double> originalTrainingValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes);
      double[] estimatedTestValues = EstimatedTestValues.ToArray(); // cache values
      IEnumerable<double> originalTestValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TestIndizes);

      OnlineEvaluatorError errorState;
      double trainingMSE = OnlineMeanSquaredErrorEvaluator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineEvaluatorError.None ? trainingMSE : double.NaN;
      double testMSE = OnlineMeanSquaredErrorEvaluator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineEvaluatorError.None ? testMSE : double.NaN;

      double trainingR2 = OnlinePearsonsRSquaredEvaluator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineEvaluatorError.None ? trainingR2 : double.NaN;
      double testR2 = OnlinePearsonsRSquaredEvaluator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRSquared = errorState == OnlineEvaluatorError.None ? testR2 : double.NaN;

      double trainingRelError = OnlineMeanAbsolutePercentageErrorEvaluator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRelativeError = errorState == OnlineEvaluatorError.None ? trainingRelError : double.NaN;
      double testRelError = OnlineMeanAbsolutePercentageErrorEvaluator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRelativeError = errorState == OnlineEvaluatorError.None ? testRelError : double.NaN;
    }

    public virtual IEnumerable<double> EstimatedValues {
      get {
        return GetEstimatedValues(Enumerable.Range(0, ProblemData.Dataset.Rows));
      }
    }

    public virtual IEnumerable<double> EstimatedTrainingValues {
      get {
        return GetEstimatedValues(ProblemData.TrainingIndizes);
      }
    }

    public virtual IEnumerable<double> EstimatedTestValues {
      get {
        return GetEstimatedValues(ProblemData.TestIndizes);
      }
    }

    public virtual IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows) {
      return Model.GetEstimatedValues(ProblemData.Dataset, rows);
    }
  }
}
