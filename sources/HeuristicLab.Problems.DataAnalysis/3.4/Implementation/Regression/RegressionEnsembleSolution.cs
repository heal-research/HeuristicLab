#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Represents regression solutions that contain an ensemble of multiple regression models
  /// </summary>
  [StorableClass]
  [Item("Regression Ensemble Solution", "A regression solution that contains an ensemble of multiple regression models")]
  [Creatable("Data Analysis - Ensembles")]
  public sealed class RegressionEnsembleSolution : RegressionSolution, IRegressionEnsembleSolution {
    private readonly Dictionary<int, double> trainingEvaluationCache = new Dictionary<int, double>();
    private readonly Dictionary<int, double> testEvaluationCache = new Dictionary<int, double>();

    public new IRegressionEnsembleModel Model {
      get { return (IRegressionEnsembleModel)base.Model; }
    }

    public new RegressionEnsembleProblemData ProblemData {
      get { return (RegressionEnsembleProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    private readonly ItemCollection<IRegressionSolution> regressionSolutions;
    public IItemCollection<IRegressionSolution> RegressionSolutions {
      get { return regressionSolutions; }
    }

    [Storable]
    private readonly Dictionary<IRegressionModel, IntRange> trainingPartitions;
    [Storable]
    private readonly Dictionary<IRegressionModel, IntRange> testPartitions;

    [StorableConstructor]
    private RegressionEnsembleSolution(bool deserializing)
      : base(deserializing) {
      regressionSolutions = new ItemCollection<IRegressionSolution>();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      foreach (var model in Model.Models) {
        IRegressionProblemData problemData = (IRegressionProblemData)ProblemData.Clone();
        problemData.TrainingPartition.Start = trainingPartitions[model].Start;
        problemData.TrainingPartition.End = trainingPartitions[model].End;
        problemData.TestPartition.Start = testPartitions[model].Start;
        problemData.TestPartition.End = testPartitions[model].End;

        regressionSolutions.Add(model.CreateRegressionSolution(problemData));
      }
      RegisterRegressionSolutionsEventHandler();
    }

    private RegressionEnsembleSolution(RegressionEnsembleSolution original, Cloner cloner)
      : base(original, cloner) {
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      foreach (var pair in original.trainingPartitions) {
        trainingPartitions[cloner.Clone(pair.Key)] = cloner.Clone(pair.Value);
      }
      foreach (var pair in original.testPartitions) {
        testPartitions[cloner.Clone(pair.Key)] = cloner.Clone(pair.Value);
      }

      regressionSolutions = cloner.Clone(original.regressionSolutions);
      RegisterRegressionSolutionsEventHandler();
    }

    public RegressionEnsembleSolution()
      : base(new RegressionEnsembleModel(), RegressionEnsembleProblemData.EmptyProblemData) {
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      regressionSolutions = new ItemCollection<IRegressionSolution>();

      RegisterRegressionSolutionsEventHandler();
    }

    public RegressionEnsembleSolution(IRegressionProblemData problemData)
      : this(Enumerable.Empty<IRegressionModel>(), problemData) {
    }

    public RegressionEnsembleSolution(IEnumerable<IRegressionModel> models, IRegressionProblemData problemData)
      : this(models, problemData,
             models.Select(m => (IntRange)problemData.TrainingPartition.Clone()),
             models.Select(m => (IntRange)problemData.TestPartition.Clone())
      ) { }

    public RegressionEnsembleSolution(IEnumerable<IRegressionModel> models, IRegressionProblemData problemData, IEnumerable<IntRange> trainingPartitions, IEnumerable<IntRange> testPartitions)
      : base(new RegressionEnsembleModel(Enumerable.Empty<IRegressionModel>()), new RegressionEnsembleProblemData(problemData)) {
      this.trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      this.testPartitions = new Dictionary<IRegressionModel, IntRange>();
      this.regressionSolutions = new ItemCollection<IRegressionSolution>();

      List<IRegressionSolution> solutions = new List<IRegressionSolution>();
      var modelEnumerator = models.GetEnumerator();
      var trainingPartitionEnumerator = trainingPartitions.GetEnumerator();
      var testPartitionEnumerator = testPartitions.GetEnumerator();

      while (modelEnumerator.MoveNext() & trainingPartitionEnumerator.MoveNext() & testPartitionEnumerator.MoveNext()) {
        var p = (IRegressionProblemData)problemData.Clone();
        p.TrainingPartition.Start = trainingPartitionEnumerator.Current.Start;
        p.TrainingPartition.End = trainingPartitionEnumerator.Current.End;
        p.TestPartition.Start = testPartitionEnumerator.Current.Start;
        p.TestPartition.End = testPartitionEnumerator.Current.End;

        solutions.Add(modelEnumerator.Current.CreateRegressionSolution(p));
      }
      if (modelEnumerator.MoveNext() | trainingPartitionEnumerator.MoveNext() | testPartitionEnumerator.MoveNext()) {
        throw new ArgumentException();
      }

      RegisterRegressionSolutionsEventHandler();
      regressionSolutions.AddRange(solutions);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionEnsembleSolution(this, cloner);
    }
    private void RegisterRegressionSolutionsEventHandler() {
      regressionSolutions.ItemsAdded += new CollectionItemsChangedEventHandler<IRegressionSolution>(regressionSolutions_ItemsAdded);
      regressionSolutions.ItemsRemoved += new CollectionItemsChangedEventHandler<IRegressionSolution>(regressionSolutions_ItemsRemoved);
      regressionSolutions.CollectionReset += new CollectionItemsChangedEventHandler<IRegressionSolution>(regressionSolutions_CollectionReset);
    }

    protected override void RecalculateResults() {
      CalculateResults();
    }

    #region Evaluation
    public override IEnumerable<double> EstimatedTrainingValues {
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

    public override IEnumerable<double> EstimatedTestValues {
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

    private IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows, Func<int, IRegressionModel, bool> modelSelectionPredicate) {
      var estimatedValuesEnumerators = (from model in Model.Models
                                        select new { Model = model, EstimatedValuesEnumerator = model.GetEstimatedValues(ProblemData.Dataset, rows).GetEnumerator() })
                                       .ToList();
      var rowsEnumerator = rows.GetEnumerator();
      // aggregate to make sure that MoveNext is called for all enumerators 
      while (rowsEnumerator.MoveNext() & estimatedValuesEnumerators.Select(en => en.EstimatedValuesEnumerator.MoveNext()).Aggregate(true, (acc, b) => acc & b)) {
        int currentRow = rowsEnumerator.Current;

        var selectedEnumerators = from pair in estimatedValuesEnumerators
                                  where modelSelectionPredicate(currentRow, pair.Model)
                                  select pair.EstimatedValuesEnumerator;

        yield return AggregateEstimatedValues(selectedEnumerators.Select(x => x.Current));
      }
    }

    private bool RowIsTrainingForModel(int currentRow, IRegressionModel model) {
      return trainingPartitions == null || !trainingPartitions.ContainsKey(model) ||
              (trainingPartitions[model].Start <= currentRow && currentRow < trainingPartitions[model].End);
    }

    private bool RowIsTestForModel(int currentRow, IRegressionModel model) {
      return testPartitions == null || !testPartitions.ContainsKey(model) ||
              (testPartitions[model].Start <= currentRow && currentRow < testPartitions[model].End);
    }

    public override IEnumerable<double> GetEstimatedValues(IEnumerable<int> rows) {
      var rowsToEvaluate = rows.Except(evaluationCache.Keys);
      var rowsEnumerator = rowsToEvaluate.GetEnumerator();
      var valuesEnumerator = (from xs in GetEstimatedValueVectors(ProblemData.Dataset, rowsToEvaluate)
                              select AggregateEstimatedValues(xs))
                             .GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        evaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => evaluationCache[row]);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedValueVectors(Dataset dataset, IEnumerable<int> rows) {
      if (!Model.Models.Any()) yield break;
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
    #endregion

    protected override void OnProblemDataChanged() {
      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
      IRegressionProblemData problemData = new RegressionProblemData(ProblemData.Dataset,
                                                                     ProblemData.AllowedInputVariables,
                                                                     ProblemData.TargetVariable);
      problemData.TrainingPartition.Start = ProblemData.TrainingPartition.Start;
      problemData.TrainingPartition.End = ProblemData.TrainingPartition.End;
      problemData.TestPartition.Start = ProblemData.TestPartition.Start;
      problemData.TestPartition.End = ProblemData.TestPartition.End;

      foreach (var solution in RegressionSolutions) {
        if (solution is RegressionEnsembleSolution)
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

    public void AddRegressionSolutions(IEnumerable<IRegressionSolution> solutions) {
      regressionSolutions.AddRange(solutions);

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }
    public void RemoveRegressionSolutions(IEnumerable<IRegressionSolution> solutions) {
      regressionSolutions.RemoveRange(solutions);

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }

    private void regressionSolutions_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRegressionSolution> e) {
      foreach (var solution in e.Items) AddRegressionSolution(solution);
      RecalculateResults();
    }
    private void regressionSolutions_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRegressionSolution> e) {
      foreach (var solution in e.Items) RemoveRegressionSolution(solution);
      RecalculateResults();
    }
    private void regressionSolutions_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRegressionSolution> e) {
      foreach (var solution in e.OldItems) RemoveRegressionSolution(solution);
      foreach (var solution in e.Items) AddRegressionSolution(solution);
      RecalculateResults();
    }

    private void AddRegressionSolution(IRegressionSolution solution) {
      if (Model.Models.Contains(solution.Model)) throw new ArgumentException();
      Model.Add(solution.Model);
      trainingPartitions[solution.Model] = solution.ProblemData.TrainingPartition;
      testPartitions[solution.Model] = solution.ProblemData.TestPartition;

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();
    }

    private void RemoveRegressionSolution(IRegressionSolution solution) {
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
