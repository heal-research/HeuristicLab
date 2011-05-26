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
using HeuristicLab.Data;
using System;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents classification solutions that contain an ensemble of multiple classification models
  /// </summary>
  [StorableClass]
  [Item("Classification Ensemble Solution", "A classification solution that contains an ensemble of multiple classification models")]
  // [Creatable("Data Analysis")]
  public class ClassificationEnsembleSolution : ClassificationSolution, IClassificationEnsembleSolution {

    public new IClassificationEnsembleModel Model {
      set { base.Model = value; }
      get { return (IClassificationEnsembleModel)base.Model; }
    }

    [Storable]
    private Dictionary<IClassificationModel, IntRange> trainingPartitions;
    [Storable]
    private Dictionary<IClassificationModel, IntRange> testPartitions;


    [StorableConstructor]
    protected ClassificationEnsembleSolution(bool deserializing) : base(deserializing) { }
    protected ClassificationEnsembleSolution(ClassificationEnsembleSolution original, Cloner cloner)
      : base(original, cloner) {
      trainingPartitions = new Dictionary<IClassificationModel, IntRange>();
      testPartitions = new Dictionary<IClassificationModel, IntRange>();
      foreach (var pair in original.trainingPartitions) {
        trainingPartitions[cloner.Clone(pair.Key)] = cloner.Clone(pair.Value);
      }
      foreach (var pair in original.testPartitions) {
        testPartitions[cloner.Clone(pair.Key)] = cloner.Clone(pair.Value);
      }
      RecalculateResults();
    }
    public ClassificationEnsembleSolution(IEnumerable<IClassificationModel> models, IClassificationProblemData problemData)
      : base(new ClassificationEnsembleModel(models), new ClassificationEnsembleProblemData(problemData)) {
      this.name = ItemName;
      this.description = ItemDescription;
      trainingPartitions = new Dictionary<IClassificationModel, IntRange>();
      testPartitions = new Dictionary<IClassificationModel, IntRange>();
      foreach (var model in models) {
        trainingPartitions[model] = (IntRange)problemData.TrainingPartition.Clone();
        testPartitions[model] = (IntRange)problemData.TestPartition.Clone();
      }
      RecalculateResults();
    }

    public ClassificationEnsembleSolution(IEnumerable<IClassificationModel> models, IClassificationProblemData problemData, IEnumerable<IntRange> trainingPartitions, IEnumerable<IntRange> testPartitions)
      : base(new ClassificationEnsembleModel(models), new ClassificationEnsembleProblemData(problemData)) {
      this.trainingPartitions = new Dictionary<IClassificationModel, IntRange>();
      this.testPartitions = new Dictionary<IClassificationModel, IntRange>();
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
      return new ClassificationEnsembleSolution(this, cloner);
    }

    public override IEnumerable<double> EstimatedTrainingClassValues {
      get {
        var rows = ProblemData.TrainingIndizes;
        var estimatedValuesEnumerators = (from model in Model.Models
                                          select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedClassValues(ProblemData.Dataset, rows).GetEnumerator() })
                                         .ToList();
        var rowsEnumerator = rows.GetEnumerator();
        // aggregate to make sure that MoveNext is called for all enumerators 
        while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.Select(en => en.EstimatedValuesEnumerator.MoveNext()).Aggregate(true, (acc, b) => acc & b)) {
          int currentRow = rowsEnumerator.Current;

          var selectedEnumerators = from pair in estimatedValuesEnumerators
                                    where RowIsTrainingForModel(currentRow, pair.Model) && !RowIsTestForModel(currentRow, pair.Model)
                                    select pair.EstimatedValuesEnumerator;
          yield return AggregateEstimatedClassValues(selectedEnumerators.Select(x => x.Current));
        }
      }
    }

    public override IEnumerable<double> EstimatedTestClassValues {
      get {
        var rows = ProblemData.TestIndizes;
        var estimatedValuesEnumerators = (from model in Model.Models
                                          select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedClassValues(ProblemData.Dataset, rows).GetEnumerator() })
                                         .ToList();
        var rowsEnumerator = ProblemData.TestIndizes.GetEnumerator();
        // aggregate to make sure that MoveNext is called for all enumerators 
        while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.Select(en => en.EstimatedValuesEnumerator.MoveNext()).Aggregate(true, (acc, b) => acc & b)) {
          int currentRow = rowsEnumerator.Current;

          var selectedEnumerators = from pair in estimatedValuesEnumerators
                                    where RowIsTestForModel(currentRow, pair.Model)
                                    select pair.EstimatedValuesEnumerator;

          yield return AggregateEstimatedClassValues(selectedEnumerators.Select(x => x.Current));
        }
      }
    }

    private bool RowIsTrainingForModel(int currentRow, IClassificationModel model) {
      return trainingPartitions == null || !trainingPartitions.ContainsKey(model) ||
              (trainingPartitions[model].Start <= currentRow && currentRow < trainingPartitions[model].End);
    }

    private bool RowIsTestForModel(int currentRow, IClassificationModel model) {
      return testPartitions == null || !testPartitions.ContainsKey(model) ||
              (testPartitions[model].Start <= currentRow && currentRow < testPartitions[model].End);
    }

    public override IEnumerable<double> GetEstimatedClassValues(IEnumerable<int> rows) {
      return from xs in GetEstimatedClassValueVectors(ProblemData.Dataset, rows)
             select AggregateEstimatedClassValues(xs);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedClassValueVectors(Dataset dataset, IEnumerable<int> rows) {
      var estimatedValuesEnumerators = (from model in Model.Models
                                        select model.GetEstimatedClassValues(dataset, rows).GetEnumerator())
                                       .ToList();

      while (estimatedValuesEnumerators.All(en => en.MoveNext())) {
        yield return from enumerator in estimatedValuesEnumerators
                     select enumerator.Current;
      }
    }

    private double AggregateEstimatedClassValues(IEnumerable<double> estimatedClassValues) {
      return estimatedClassValues
      .GroupBy(x => x)
      .OrderBy(g => -g.Count())
      .Select(g => g.Key)
      .DefaultIfEmpty(double.NaN)
      .First();
    }
  }
}
