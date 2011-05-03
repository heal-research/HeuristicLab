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
  /// Abstract base class for classification data analysis solutions
  /// </summary>
  [StorableClass]
  public abstract class ClassificationSolution : DataAnalysisSolution, IClassificationSolution {
    private const string TrainingAccuracyResultName = "Accuracy (training)";
    private const string TestAccuracyResultName = "Accuracy (test)";

    public new IClassificationModel Model {
      get { return (IClassificationModel)base.Model; }
      protected set { base.Model = value; }
    }

    public new IClassificationProblemData ProblemData {
      get { return (IClassificationProblemData)base.ProblemData; }
      protected set { base.ProblemData = value; }
    }

    public double TrainingAccuracy {
      get { return ((DoubleValue)this[TrainingAccuracyResultName].Value).Value; }
      private set { ((DoubleValue)this[TrainingAccuracyResultName].Value).Value = value; }
    }

    public double TestAccuracy {
      get { return ((DoubleValue)this[TestAccuracyResultName].Value).Value; }
      private set { ((DoubleValue)this[TestAccuracyResultName].Value).Value = value; }
    }

    [StorableConstructor]
    protected ClassificationSolution(bool deserializing) : base(deserializing) { }
    protected ClassificationSolution(ClassificationSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public ClassificationSolution(IClassificationModel model, IClassificationProblemData problemData)
      : base(model, problemData) {
      Add(new Result(TrainingAccuracyResultName, "Accuracy of the model on the training partition (percentage of correctly classified instances).", new PercentValue()));
      Add(new Result(TestAccuracyResultName, "Accuracy of the model on the test partition (percentage of correctly classified instances).", new PercentValue()));
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
      double[] estimatedTrainingClassValues = EstimatedTrainingClassValues.ToArray(); // cache values
      IEnumerable<double> originalTrainingClassValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TrainingIndizes);
      double[] estimatedTestClassValues = EstimatedTestClassValues.ToArray(); // cache values
      IEnumerable<double> originalTestClassValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TestIndizes);

      OnlineCalculatorError errorState;
      double trainingAccuracy = OnlineAccuracyCalculator.Calculate(estimatedTrainingClassValues, originalTrainingClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) trainingAccuracy = double.NaN;
      double testAccuracy = OnlineAccuracyCalculator.Calculate(estimatedTestClassValues, originalTestClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) testAccuracy = double.NaN;

      TrainingAccuracy = trainingAccuracy;
      TestAccuracy = testAccuracy;
    }

    public virtual IEnumerable<double> EstimatedClassValues {
      get {
        return GetEstimatedClassValues(Enumerable.Range(0, ProblemData.Dataset.Rows));
      }
    }

    public virtual IEnumerable<double> EstimatedTrainingClassValues {
      get {
        return GetEstimatedClassValues(ProblemData.TrainingIndizes);
      }
    }

    public virtual IEnumerable<double> EstimatedTestClassValues {
      get {
        return GetEstimatedClassValues(ProblemData.TestIndizes);
      }
    }

    public virtual IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows) {
      return Model.GetEstimatedClassValues(ProblemData.Dataset, rows);
    }
  }
}
