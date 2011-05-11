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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System;
using HeuristicLab.Data;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents regression solutions that contain an ensemble of multiple regression models
  /// </summary>
  [StorableClass]
  [Item("Regression Ensemble Solution", "A regression solution that contains an ensemble of multiple regression models")]
  // [Creatable("Data Analysis")]
  public class RegressionEnsembleSolution : RegressionSolution, IRegressionEnsembleSolution {
    public new IRegressionEnsembleModel Model {
      get { return (IRegressionEnsembleModel)base.Model; }
    }

    [Storable]
    private Dictionary<IRegressionModel, IntRange> trainingPartitions;
    [Storable]
    private Dictionary<IRegressionModel, IntRange> testPartitions;

    [StorableConstructor]
    protected RegressionEnsembleSolution(bool deserializing) : base(deserializing) { }
    protected RegressionEnsembleSolution(RegressionEnsembleSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public RegressionEnsembleSolution(IEnumerable<IRegressionModel> models, IRegressionProblemData problemData)
      : base(new RegressionEnsembleModel(models), problemData) {
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      foreach (var model in models) {
        trainingPartitions[model] = (IntRange)problemData.TrainingPartition.Clone();
        testPartitions[model] = (IntRange)problemData.TestPartition.Clone();
      }
      RecalculateResults();
    }

    public RegressionEnsembleSolution(IEnumerable<IRegressionModel> models, IRegressionProblemData problemData, IEnumerable<IntRange> trainingPartitions, IEnumerable<IntRange> testPartitions)
      : base(new RegressionEnsembleModel(models), problemData) {
      this.trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      this.testPartitions = new Dictionary<IRegressionModel, IntRange>();
      var modelEnumerator = models.GetEnumerator();
      var trainingPartitionEnumerator = trainingPartitions.GetEnumerator();
      var testPartitionEnumerator = testPartitions.GetEnumerator();
      while (modelEnumerator.MoveNext() & trainingPartitionEnumerator.MoveNext() & testPartitionEnumerator.MoveNext()) {
        this.trainingPartitions[modelEnumerator.Current] = (IntRange)trainingPartitionEnumerator.Current.Clone();
        this.testPartitions[modelEnumerator.Current] = (IntRange)testPartitionEnumerator.Current.Clone();
      }
      if (modelEnumerator.MoveNext() | trainingPartitionEnumerator.MoveNext() | testPartitionEnumerator.MoveNext()) {
        throw new ArgumentException();
      }

      RecalculateResults();
    }

    private void RecalculateResults() {
      double[] estimatedTrainingValues = EstimatedTrainingValues.ToArray(); // cache values
      var trainingIndizes = Enumerable.Range(ProblemData.TrainingPartition.Start,
        ProblemData.TrainingPartition.End - ProblemData.TrainingPartition.Start);
      IEnumerable<double> originalTrainingValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, trainingIndizes);
      double[] estimatedTestValues = EstimatedTestValues.ToArray(); // cache values
      IEnumerable<double> originalTestValues = ProblemData.Dataset.GetEnumeratedVariableValues(ProblemData.TargetVariable, ProblemData.TestIndizes);

      OnlineCalculatorError errorState;
      double trainingMSE = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingMSE : double.NaN;
      double testMSE = OnlineMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestMeanSquaredError = errorState == OnlineCalculatorError.None ? testMSE : double.NaN;

      double trainingR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRSquared = errorState == OnlineCalculatorError.None ? trainingR2 : double.NaN;
      double testR2 = OnlinePearsonsRSquaredCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRSquared = errorState == OnlineCalculatorError.None ? testR2 : double.NaN;

      double trainingRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingRelativeError = errorState == OnlineCalculatorError.None ? trainingRelError : double.NaN;
      double testRelError = OnlineMeanAbsolutePercentageErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestRelativeError = errorState == OnlineCalculatorError.None ? testRelError : double.NaN;

      double trainingNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(estimatedTrainingValues, originalTrainingValues, out errorState);
      TrainingNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? trainingNMSE : double.NaN;
      double testNMSE = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(estimatedTestValues, originalTestValues, out errorState);
      TestNormalizedMeanSquaredError = errorState == OnlineCalculatorError.None ? testNMSE : double.NaN;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionEnsembleSolution(this, cloner);
    }

    public override IEnumerable<double> EstimatedTrainingValues {
      get {
        var rows = Enumerable.Range(ProblemData.TrainingPartition.Start, ProblemData.TrainingPartition.End - ProblemData.TrainingPartition.Start);
        var estimatedValuesEnumerators = (from model in Model.Models
                                          select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedValues(ProblemData.Dataset, rows).GetEnumerator() })
                                         .ToList();
        var rowsEnumerator = rows.GetEnumerator();
        while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.Select(en => en.EstimatedValuesEnumerator.MoveNext()).Aggregate(true, (acc, b) => acc & b)) {
          int currentRow = rowsEnumerator.Current;

          var selectedEnumerators = from pair in estimatedValuesEnumerators
                                    where trainingPartitions == null || !trainingPartitions.ContainsKey(pair.Model) ||
                                         (trainingPartitions[pair.Model].Start <= currentRow && currentRow < trainingPartitions[pair.Model].End)
                                    select pair.EstimatedValuesEnumerator;
          yield return AggregateEstimatedValues(selectedEnumerators.Select(x => x.Current));
        }
      }
    }

    public override IEnumerable<double> EstimatedTestValues {
      get {
        var estimatedValuesEnumerators = (from model in Model.Models
                                          select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedValues(ProblemData.Dataset, ProblemData.TestIndizes).GetEnumerator() })
                                         .ToList();
        var rowsEnumerator = ProblemData.TestIndizes.GetEnumerator();
        while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.Select(en => en.EstimatedValuesEnumerator.MoveNext()).Aggregate(true, (acc, b) => acc & b)) {
          int currentRow = rowsEnumerator.Current;

          var selectedEnumerators = from pair in estimatedValuesEnumerators
                                    where testPartitions == null || !testPartitions.ContainsKey(pair.Model) ||
                                      (testPartitions[pair.Model].Start <= currentRow && currentRow < testPartitions[pair.Model].End)
                                    select pair.EstimatedValuesEnumerator;

          yield return AggregateEstimatedValues(selectedEnumerators.Select(x => x.Current));
        }
      }
    }

    public override IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows) {
      return from xs in GetEstimatedValueVectors(ProblemData.Dataset, rows)
             select AggregateEstimatedValues(xs);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedValueVectors(Dataset dataset, IEnumerable<int> rows) {
      var estimatedValuesEnumerators = (from model in Model.Models
                                        select model.GetEstimatedValues(dataset, rows).GetEnumerator())
                                       .ToList();

      while (estimatedValuesEnumerators.All(en => en.MoveNext())) {
        yield return from enumerator in estimatedValuesEnumerators
                     select enumerator.Current;
      }
    }

    private double AggregateEstimatedValues(IEnumerable<double> estimatedValues) {
      return estimatedValues.Average();
    }

    //[Storable]
    //private string name;
    //public string Name {
    //  get {
    //    return name;
    //  }
    //  set {
    //    if (value != null && value != name) {
    //      var cancelEventArgs = new CancelEventArgs<string>(value);
    //      OnNameChanging(cancelEventArgs);
    //      if (cancelEventArgs.Cancel == false) {
    //        name = value;
    //        OnNamedChanged(EventArgs.Empty);
    //      }
    //    }
    //  }
    //}

    //public bool CanChangeName {
    //  get { return true; }
    //}

    //[Storable]
    //private string description;
    //public string Description {
    //  get {
    //    return description;
    //  }
    //  set {
    //    if (value != null && value != description) {
    //      description = value;
    //      OnDescriptionChanged(EventArgs.Empty);
    //    }
    //  }
    //}

    //public bool CanChangeDescription {
    //  get { return true; }
    //}

    //#region events
    //public event EventHandler<CancelEventArgs<string>> NameChanging;
    //private void OnNameChanging(CancelEventArgs<string> cancelEventArgs) {
    //  var listener = NameChanging;
    //  if (listener != null) listener(this, cancelEventArgs);
    //}

    //public event EventHandler NameChanged;
    //private void OnNamedChanged(EventArgs e) {
    //  var listener = NameChanged;
    //  if (listener != null) listener(this, e);
    //}

    //public event EventHandler DescriptionChanged;
    //private void OnDescriptionChanged(EventArgs e) {
    //  var listener = DescriptionChanged;
    //  if (listener != null) listener(this, e);
    //}
    // #endregion
  }
}
