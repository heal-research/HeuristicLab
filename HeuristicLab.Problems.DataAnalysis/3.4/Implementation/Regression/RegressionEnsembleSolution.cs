#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {
  /// <summary>
  /// Represents regression solutions that contain an ensemble of multiple regression models
  /// </summary>
  [StorableType("C5B38C31-4307-48E4-9BCD-6797C329E018")]
  [Item("Regression Ensemble Solution", "A regression solution that contains an ensemble of multiple regression models")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisEnsembles, Priority = 100)]
  public sealed class RegressionEnsembleSolution : RegressionSolutionBase, IRegressionEnsembleSolution {
    private readonly Dictionary<int, double> trainingEvaluationCache = new Dictionary<int, double>();
    private readonly Dictionary<int, double> testEvaluationCache = new Dictionary<int, double>();
    private readonly Dictionary<int, double> evaluationCache = new Dictionary<int, double>();

    public new IRegressionEnsembleModel Model {
      get { return (IRegressionEnsembleModel)base.Model; }
    }

    public new RegressionEnsembleProblemData ProblemData {
      get { return (RegressionEnsembleProblemData)base.ProblemData; }
      set { base.ProblemData = value; }
    }

    [Storable]
    private readonly ItemCollection<IRegressionSolution> regressionSolutions;
    public IItemCollection<IRegressionSolution> RegressionSolutions {
      get { return regressionSolutions; }
    }

    [Storable]
    private readonly Dictionary<IRegressionModel, IntRange> trainingPartitions;
    [Storable]
    private readonly Dictionary<IRegressionModel, IntRange> testPartitions;

    [StorableConstructor]
    private RegressionEnsembleSolution(StorableConstructorFlag _) : base(_) {
      regressionSolutions = new ItemCollection<IRegressionSolution>();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!regressionSolutions.Any()) {
        foreach (var model in Model.Models) {
          IRegressionProblemData problemData = (IRegressionProblemData)ProblemData.Clone();
          problemData.TrainingPartition.Start = trainingPartitions[model].Start;
          problemData.TrainingPartition.End = trainingPartitions[model].End;
          problemData.TestPartition.Start = testPartitions[model].Start;
          problemData.TestPartition.End = testPartitions[model].End;

          regressionSolutions.Add(model.CreateRegressionSolution(problemData));
        }
      }

      RegisterModelEvents();
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

      evaluationCache = new Dictionary<int, double>(original.ProblemData.Dataset.Rows);
      trainingEvaluationCache = new Dictionary<int, double>(original.ProblemData.TrainingIndices.Count());
      testEvaluationCache = new Dictionary<int, double>(original.ProblemData.TestIndices.Count());

      regressionSolutions = cloner.Clone(original.regressionSolutions);
      RegisterModelEvents();
      RegisterRegressionSolutionsEventHandler();
    }

    public RegressionEnsembleSolution()
      : base(new RegressionEnsembleModel(), RegressionEnsembleProblemData.EmptyProblemData) {
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      regressionSolutions = new ItemCollection<IRegressionSolution>();

      RegisterModelEvents();
      RegisterRegressionSolutionsEventHandler();
    }

    public RegressionEnsembleSolution(IRegressionProblemData problemData)
      : this(new RegressionEnsembleModel(), problemData) {
    }

    public RegressionEnsembleSolution(IRegressionEnsembleModel model, IRegressionProblemData problemData)
      : base(model, new RegressionEnsembleProblemData(problemData)) {
      trainingPartitions = new Dictionary<IRegressionModel, IntRange>();
      testPartitions = new Dictionary<IRegressionModel, IntRange>();
      regressionSolutions = new ItemCollection<IRegressionSolution>();

      evaluationCache = new Dictionary<int, double>(problemData.Dataset.Rows);
      trainingEvaluationCache = new Dictionary<int, double>(problemData.TrainingIndices.Count());
      testEvaluationCache = new Dictionary<int, double>(problemData.TestIndices.Count());


      var solutions = model.Models.Select(m => m.CreateRegressionSolution((IRegressionProblemData)problemData.Clone()));
      foreach (var solution in solutions) {
        regressionSolutions.Add(solution);
        trainingPartitions.Add(solution.Model, solution.ProblemData.TrainingPartition);
        testPartitions.Add(solution.Model, solution.ProblemData.TestPartition);
      }

      RecalculateResults();
      RegisterModelEvents();
      RegisterRegressionSolutionsEventHandler();
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionEnsembleSolution(this, cloner);
    }

    private void RegisterModelEvents() {
      Model.Changed += Model_Changed;
    }
    private void RegisterRegressionSolutionsEventHandler() {
      regressionSolutions.ItemsAdded += new CollectionItemsChangedEventHandler<IRegressionSolution>(regressionSolutions_ItemsAdded);
      regressionSolutions.ItemsRemoved += new CollectionItemsChangedEventHandler<IRegressionSolution>(regressionSolutions_ItemsRemoved);
      regressionSolutions.CollectionReset += new CollectionItemsChangedEventHandler<IRegressionSolution>(regressionSolutions_CollectionReset);
    }

    #region Evaluation
    public override IEnumerable<double> EstimatedValues {
      get { return GetEstimatedValues(Enumerable.Range(0, ProblemData.Dataset.Rows)); }
    }

    public override IEnumerable<double> EstimatedTrainingValues {
      get {
        var rows = ProblemData.TrainingIndices;
        var rowsToEvaluate = rows.Except(trainingEvaluationCache.Keys);

        var rowsEnumerator = rowsToEvaluate.GetEnumerator();
        var valuesEnumerator = Model.GetEstimatedValues(ProblemData.Dataset, rowsToEvaluate, (r, m) => RowIsTrainingForModel(r, m) && !RowIsTestForModel(r, m)).GetEnumerator();

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
        var valuesEnumerator = Model.GetEstimatedValues(ProblemData.Dataset, rowsToEvaluate, RowIsTestForModel).GetEnumerator();

        while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
          testEvaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
        }

        return rows.Select(row => testEvaluationCache[row]);
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
      var valuesEnumerator = Model.GetEstimatedValues(ProblemData.Dataset, rowsToEvaluate).GetEnumerator();

      while (rowsEnumerator.MoveNext() & valuesEnumerator.MoveNext()) {
        evaluationCache.Add(rowsEnumerator.Current, valuesEnumerator.Current);
      }

      return rows.Select(row => evaluationCache[row]);
    }

    public IEnumerable<IEnumerable<double>> GetEstimatedValueVectors(IEnumerable<int> rows) {
      return Model.GetEstimatedValueVectors(ProblemData.Dataset, rows);
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

    private void Model_Changed(object sender, EventArgs e) {
      var modelSet = new HashSet<IRegressionModel>(Model.Models);
      foreach (var model in Model.Models) {
        if (!trainingPartitions.ContainsKey(model)) trainingPartitions.Add(model, ProblemData.TrainingPartition);
        if (!testPartitions.ContainsKey(model)) testPartitions.Add(model, ProblemData.TrainingPartition);
      }
      foreach (var model in trainingPartitions.Keys) {
        if (modelSet.Contains(model)) continue;
        trainingPartitions.Remove(model);
        testPartitions.Remove(model);
      }

      trainingEvaluationCache.Clear();
      testEvaluationCache.Clear();
      evaluationCache.Clear();

      OnModelChanged();
    }

    public void AddRegressionSolutions(IEnumerable<IRegressionSolution> solutions) {
      regressionSolutions.AddRange(solutions);
    }
    public void RemoveRegressionSolutions(IEnumerable<IRegressionSolution> solutions) {
      regressionSolutions.RemoveRange(solutions);
    }

    private void regressionSolutions_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRegressionSolution> e) {
      foreach (var solution in e.Items) {
        trainingPartitions.Add(solution.Model, solution.ProblemData.TrainingPartition);
        testPartitions.Add(solution.Model, solution.ProblemData.TestPartition);
      }
      Model.AddRange(e.Items.Select(s => s.Model));
    }
    private void regressionSolutions_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRegressionSolution> e) {
      foreach (var solution in e.Items) {
        trainingPartitions.Remove(solution.Model);
        testPartitions.Remove(solution.Model);
      }
      Model.RemoveRange(e.Items.Select(s => s.Model));
    }
    private void regressionSolutions_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRegressionSolution> e) {
      foreach (var solution in e.OldItems) {
        trainingPartitions.Remove(solution.Model);
        testPartitions.Remove(solution.Model);
      }
      Model.RemoveRange(e.OldItems.Select(s => s.Model));

      foreach (var solution in e.Items) {
        trainingPartitions.Add(solution.Model, solution.ProblemData.TrainingPartition);
        testPartitions.Add(solution.Model, solution.ProblemData.TestPartition);
      }
      Model.AddRange(e.Items.Select(s => s.Model));
    }
  }
}
