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
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      foreach (var model in Model.Models) {
        trainingPartitions[model] = (IntRange)ProblemData.TrainingPartition.Clone();
        testPartitions[model] = (IntRange)ProblemData.TestPartition.Clone();
      }
    }

    public RegressionEnsembleSolution(IEnumerable<IRegressionModel> models, IRegressionProblemData problemData)
      : base(new RegressionEnsembleModel(models), new RegressionEnsembleProblemData(problemData)) {
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      foreach (var model in models) {
        trainingPartitions[model] = (IntRange)problemData.TrainingPartition.Clone();
        testPartitions[model] = (IntRange)problemData.TestPartition.Clone();
      }
      RecalculateResults();
    }

    public RegressionEnsembleSolution(IEnumerable<IRegressionModel> models, IRegressionProblemData problemData, IEnumerable<IntRange> trainingPartitions, IEnumerable<IntRange> testPartitions)
      : base(new RegressionEnsembleModel(models), new RegressionEnsembleProblemData(problemData)) {
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

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionEnsembleSolution(this, cloner);
    }

    public override IEnumerable<double> EstimatedTrainingValues {
      get {
        var rows = ProblemData.TrainingIndizes;
        var estimatedValuesEnumerators = (from model in Model.Models
                                          select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedValues(ProblemData.Dataset, rows).GetEnumerator() })
                                         .ToList();
        var rowsEnumerator = rows.GetEnumerator();
        // aggregate to make sure that MoveNext is called for all enumerators 
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
        var rows = ProblemData.TestIndizes;
        var estimatedValuesEnumerators = (from model in Model.Models
                                          select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedValues(ProblemData.Dataset, rows).GetEnumerator() })
                                         .ToList();
        var rowsEnumerator = ProblemData.TestIndizes.GetEnumerator();
        // aggregate to make sure that MoveNext is called for all enumerators 
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
      return estimatedValues.DefaultIfEmpty(double.NaN).Average();
    }   
  }
}
