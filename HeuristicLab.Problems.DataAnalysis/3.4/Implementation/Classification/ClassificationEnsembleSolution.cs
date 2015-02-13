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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents classification solutions that contain an ensemble of multiple classification models
  /// </summary>
  [StorableClass]
  [Item("Classification Ensemble Solution", "A classification solution that contains an ensemble of multiple classification models")]
  [Creatable("Data Analysis - Ensembles")]
  public sealed class ClassificationEnsembleSolution : ClassificationSolutionBase, IClassificationEnsembleSolution {
    private readonly Dictionary<int, double> trainingEvaluationCache = new Dictionary<int, double>();
    private readonly Dictionary<int, double> testEvaluationCache = new Dictionary<int, double>();
    private readonly Dictionary<int, double> evaluationCache = new Dictionary<int, double>();

    public new IClassificationEnsembleModel Model {
      get { return (IClassificationEnsembleModel)base.Model; }
    }
    public new ClassificationEnsembleProblemData ProblemData {
      get { return (ClassificationEnsembleProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    private readonly ItemCollection<IClassificationSolution> classificationSolutions;
    public IItemCollection<IClassificationSolution> ClassificationSolutions {
      get { return classificationSolutions; }
    }

    [Storable]
    private Dictionary<IClassificationModel, IntRange> trainingPartitions;
    [Storable]
    private Dictionary<IClassificationModel, IntRange> testPartitions;

    [StorableConstructor]
    private ClassificationEnsembleSolution(bool deserializing)
      : base(deserializing) {
      classificationSolutions = new ItemCollection<IClassificationSolution>();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (var model in Model.Models) {
        IClassificationProblemData problemData = (IClassificationProblemData)ProblemData.Clone();
        problemData.TrainingPartition.Start = trainingPartitions[model].Start;
        problemData.TrainingPartition.End = trainingPartitions[model].End;
        problemData.TestPartition.Start = testPartitions[model].Start;
        problemData.TestPartition.End = testPartitions[model].End;

        classificationSolutions.Add(model.CreateClassificationSolution(problemData));
      }
      RegisterClassificationSolutionsEventHandler();
    }

    private ClassificationEnsembleSolution(ClassificationEnsembleSolution original, Cloner cloner)
      : base(original, cloner) {
      trainingPartitions = new Dictionary<IClassificationModel, IntRange>();
      testPartitions = new Dictionary<IClassificationModel, IntRange>();
      foreach (var pair in original.trainingPartitions) {
        trainingPartitions[cloner.Clone(pair.Key)] = cloner.Clone(pair.Value);
      }
      foreach (var pair in original.testPartitions) {
        testPartitions[cloner.Clone(pair.Key)] = cloner.Clone(pair.Value);
      }

      trainingEvaluationCache = new Dictionary<int, double>(original.ProblemData.TrainingIndices.Count());
      testEvaluationCache = new Dictionary<int, double>(original.ProblemData.TestIndices.Count());

      classificationSolutions = cloner.Clone(original.classificationSolutions);
      RegisterClassificationSolutionsEventHandler();
    }

    public ClassificationEnsembleSolution()
      : base(new ClassificationEnsembleModel(), ClassificationEnsembleProblemData.EmptyProblemData) {
      trainingPartitions = new Dictionary<IClassificationModel, IntRange>();
      testPartitions = new Dictionary<IClassificationModel, IntRange>();
      classificationSolutions = new ItemCollection<IClassificationSolution>();

      RegisterClassificationSolutionsEventHandler();
    }

    public ClassificationEnsembleSolution(IClassificationProblemData problemData) :
      this(Enumerable.Empty<IClassificationModel>(), problemData) { }

    public ClassificationEnsembleSolution(IEnumerable<IClassificationModel> models, IClassificationProblemData problemData)
      : this(models, problemData,
             models.Select(m => (IntRange)problemData.TrainingPartition.Clone()),
             models.Select(m => (IntRange)problemData.TestPartition.Clone())
      ) { }

    public ClassificationEnsembleSolution(IEnumerable<IClassificationModel> models, IClassificationProblemData problemData, IEnumerable<IntRange> trainingPartitions, IEnumerable<IntRange> testPartitions)
      : base(new ClassificationEnsembleModel(Enumerable.Empty<IClassificationModel>()), new ClassificationEnsembleProblemData(problemData)) {
      this.trainingPartitions = new Dictionary<IClassificationModel, IntRange>();
      this.testPartitions = new Dictionary<IClassificationModel, IntRange>();
      this.classificationSolutions = new ItemCollection<IClassificationSolution>();

      List<IClassificationSolution> solutions = new List<IClassificationSolution>();
      var modelEnumerator = models.GetEnumerator();
      var trainingPartitionEnumerator = trainingPartitions.GetEnumerator();
      var testPartitionEnumerator = testPartitions.GetEnumerator();

      while (modelEnumerator.MoveNext() & trainingPartitionEnumerator.MoveNext() & testPartitionEnumerator.MoveNext()) {
        var p = (IClassificationProblemData)problemData.Clone();
        p.TrainingPartition.Start = trainingPartitionEnumerator.Current.Start;
        p.TrainingPartition.End = trainingPartitionEnumerator.Current.End;
        p.TestPartition.Start = testPartitionEnumerator.Current.Start;
        p.TestPartition.End = testPartitionEnumerator.Current.End;

        solutions.Add(modelEnumerator.Current.CreateClassificationSolution(p));
      }
      if (modelEnumerator.MoveNext() | trainingPartitionEnumerator.MoveNext() | testPartitionEnumerator.MoveNext()) {
        throw new ArgumentException();
      }

      trainingEvaluationCache = new Dictionary<int, double>(problemData.TrainingIndices.Count());
      testEvaluationCache = new Dictionary<int, double>(problemData.TestIndices.Count());

      RegisterClassificationSolutionsEventHandler();
      classificationSolutions.AddRange(solutions);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClassificationEnsembleSolution(this, cloner);
    }
    private void RegisterClassificationSolutionsEventHandler() {
      classificationSolutions.ItemsAdded += new CollectionItemsChangedEventHandler<IClassificationSolution>(classificationSolutions_ItemsAdded);
      classificationSolutions.ItemsRemoved += new CollectionItemsChangedEventHandler<IClassificationSolution>(classificationSolutions_ItemsRemoved);
      classificationSolutions.CollectionReset += new CollectionItemsChangedEventHandler<IClassificationSolution>(classificationSolutions_CollectionReset);
    }


    #region Evaluation
    public override IEnumerable<double> EstimatedClassValues {
      get { return GetEstimatedClassValues(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }

    public override IEnumerable<double> EstimatedTrainingClassValues {
      get {
        var rows = ProblemData.TrainingIndices;
        var rowsToEvaluate = rows.Except(trainingEvaluationCache.Keys);
        var rowsEnumerator = rowsToEvaluate.GetEnumerator();
        var valuesEnumerator = GetEstimatedValues(rowsToEvaluate, (r, m) => RowIsTrainingForModel(r, m) && !RowIsTestForModel(r, m)).GetEnumerator();

        while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
          trainingEvaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
        }

        return rows.Select(row => trainingEvaluationCache[row]);
      }
    }

    public override IEnumerable<double> EstimatedTestClassValues {
      get {
        var rows = ProblemData.TestIndices;
        var rowsToEvaluate = rows.Except(testEvaluationCache.Keys);
        var rowsEnumerator = rowsToEvaluate.GetEnumerator();
        var valuesEnumerator = GetEstimatedValues(rowsToEvaluate, RowIsTestForModel).GetEnumerator();

        while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
          testEvaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
        }

        return rows.Select(row => testEvaluationCache[row]);
      }
    }

    private IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows, Func<int, IClassificationModel, bool> modelSelectionPredicate) {
      var estimatedValuesEnumerators = (from model in Model.Models
                                        select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedClassValues(ProblemData.Dataset, rows).GetEnumerator() })
                                       .ToList();
      var rowsEnumerator = rows.GetEnumerator();
      // aggregate to make sure that MoveNext is called for all enumerators 
      while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.Select(en => en.EstimatedValuesEnumerator.MoveNext()).Aggregate(true, (acc, b) => acc & b)) {
        int currentRow = rowsEnumerator.Current;

        var selectedEnumerators = from pair in estimatedValuesEnumerators
                                  where modelSelectionPredicate(currentRow, pair.Model)
                                  select pair.EstimatedValuesEnumerator;

        yield return AggregateEstimatedClassValues(selectedEnumerators.Select(x => x.Current));
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
      var rowsToEvaluate = rows.Except(evaluationCache.Keys);
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = (from xs in GetEstimatedClassValueVectors(ProblemData.Dataset, rowsToEvaluate)
                              select AggregateEstimatedClassValues(xs))
                             .GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        evaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => evaluationCache[row]);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedClassValueVectors(Dataset dataset, IEnumerable<int> rows) {
      if (!Model.Models.Any()) yield break;
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
    #endregion

    protected override void OnProblemDataChanged() {
      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();

      IClassificationProblemData problemData = new ClassificationProblemData(ProblemData.Dataset,
                                                                     ProblemData.AllowedInputVariables,
                                                                     ProblemData.TargetVariable);
      problemData.TrainingPartition.Start = ProblemData.TrainingPartition.Start;
      problemData.TrainingPartition.End = ProblemData.TrainingPartition.End;
      problemData.TestPartition.Start = ProblemData.TestPartition.Start;
      problemData.TestPartition.End = ProblemData.TestPartition.End;

      foreach (var solution in ClassificationSolutions) {
        if (solution is ClassificationEnsembleSolution)
          solution.ProblemData = ProblemData;
        else
          solution.ProblemData = problemData;
      }
      foreach (var trainingPartition in trainingPartitions.Values) {
        trainingPartition.Start = ProblemData.TrainingPartition.Start;
        trainingPartition.End = ProblemData.TrainingPartition.End;
      }
      foreach (var testPartition in testPartitions.Values) {
        testPartition.Start = ProblemData.TestPartition.Start;
        testPartition.End = ProblemData.TestPartition.End;
      }

      base.OnProblemDataChanged();
    }

    public void AddClassificationSolutions(IEnumerable<IClassificationSolution> solutions) {
      classificationSolutions.AddRange(solutions);

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }
    public void RemoveClassificationSolutions(IEnumerable<IClassificationSolution> solutions) {
      classificationSolutions.RemoveRange(solutions);

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }

    private void classificationSolutions_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IClassificationSolution> e) {
      foreach (var solution in e.Items) AddClassificationSolution(solution);
      RecalculateResults();
    }
    private void classificationSolutions_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IClassificationSolution> e) {
      foreach (var solution in e.Items) RemoveClassificationSolution(solution);
      RecalculateResults();
    }
    private void classificationSolutions_CollectionReset(object sender, CollectionItemsChangedEventArgs<IClassificationSolution> e) {
      foreach (var solution in e.OldItems) RemoveClassificationSolution(solution);
      foreach (var solution in e.Items) AddClassificationSolution(solution);
      RecalculateResults();
    }

    private void AddClassificationSolution(IClassificationSolution solution) {
      if (Model.Models.Contains(solution.Model)) throw new ArgumentException();
      Model.Add(solution.Model);
      trainingPartitions[solution.Model] = solution.ProblemData.TrainingPartition;
      testPartitions[solution.Model] = solution.ProblemData.TestPartition;

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }

    private void RemoveClassificationSolution(IClassificationSolution solution) {
      if (!Model.Models.Contains(solution.Model)) throw new ArgumentException();
      Model.Remove(solution.Model);
      trainingPartitions.Remove(solution.Model);
      testPartitions.Remove(solution.Model);

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }
  }
}
