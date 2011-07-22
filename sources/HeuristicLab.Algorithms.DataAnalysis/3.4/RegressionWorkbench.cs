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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("Regression Workbench", "Experiment containing multiple algorithms for regression analysis.")]
  [Creatable("Data Analysis")]
  [StorableClass]
  public sealed class RegressionWorkbench : ParameterizedNamedItem, IOptimizer, IStorableContent {
    public string Filename { get; set; }

    private const string ProblemDataParameterName = "ProblemData";

    #region parameter properties
    public IValueParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (IValueParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    #endregion
    #region properties
    public IRegressionProblemData ProblemData {
      get { return ProblemDataParameter.Value; }
      set { ProblemDataParameter.Value = value; }
    }
    #endregion
    [Storable]
    private Experiment experiment;

    [StorableConstructor]
    private RegressionWorkbench(bool deserializing)
      : base(deserializing) {
    }
    private RegressionWorkbench(RegressionWorkbench original, Cloner cloner)
      : base(original, cloner) {
      experiment = cloner.Clone(original.experiment);
      RegisterEventHandlers();
    }
    public RegressionWorkbench()
      : base() {
      name = ItemName;
      description = ItemDescription;

      Parameters.Add(new ValueParameter<IRegressionProblemData>(ProblemDataParameterName, "The regression problem data that should be used for modeling.", new RegressionProblemData()));

      experiment = new Experiment();

      //var svmExperiments = CreateSvmExperiment();
      var rfExperiments = CreateRandomForestExperiments();

      experiment.Optimizers.Add(new LinearRegression());
      experiment.Optimizers.Add(rfExperiments);
      //experiment.Optimizers.Add(svmExperiments);

      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RegressionWorkbench(this, cloner);
    }

    private void RegisterEventHandlers() {
      ProblemDataParameter.ValueChanged += ProblemDataParameterValueChanged;

      experiment.ExceptionOccurred += (sender, e) => OnExceptionOccured(e);
      experiment.ExecutionStateChanged += (sender, e) => OnExecutionStateChanged(e);
      experiment.ExecutionTimeChanged += (sender, e) => OnExecutionTimeChanged(e);
      experiment.Paused += (sender, e) => OnPaused(e);
      experiment.Prepared += (sender, e) => OnPrepared(e);
      experiment.Started += (sender, e) => OnStarted(e);
      experiment.Stopped += (sender, e) => OnStopped(e);
    }

    private IOptimizer CreateRandomForestExperiments() {
      var exp = new Experiment();
      double[] rs = new double[] { 0.2, 0.3, 0.4, 0.5, 0.6, 0.65 };
      foreach (var r in rs) {
        var cv = new CrossValidation();
        var rf = new RandomForestRegression();
        rf.R = r;
        cv.Algorithm = rf;
        cv.Folds.Value = 5;
        exp.Optimizers.Add(cv);
      }
      return exp;
    }

    private IOptimizer CreateSvmExperiment() {
      var exp = new Experiment();
      var costs = new double[] { Math.Pow(2, -5), Math.Pow(2, -3), Math.Pow(2, -1), Math.Pow(2, 1), Math.Pow(2, 3), Math.Pow(2, 5), Math.Pow(2, 7), Math.Pow(2, 9), Math.Pow(2, 11), Math.Pow(2, 13), Math.Pow(2, 15) };
      var gammas = new double[] { Math.Pow(2, -15), Math.Pow(2, -13), Math.Pow(2, -11), Math.Pow(2, -9), Math.Pow(2, -7), Math.Pow(2, -5), Math.Pow(2, -3), Math.Pow(2, -1), Math.Pow(2, 1), Math.Pow(2, 3) };
      var nus = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };
      foreach (var gamma in gammas)
        foreach (var cost in costs)
          foreach (var nu in nus) {
            var cv = new CrossValidation();
            var svr = new SupportVectorRegression();        
            svr.Nu.Value = nu;
            svr.Cost.Value = cost;
            svr.Gamma.Value = gamma;
            cv.Algorithm = svr;
            cv.Folds.Value = 5;
            exp.Optimizers.Add(cv);
          }
      return exp;
    }

    public RunCollection Runs {
      get { return experiment.Runs; }
    }

    public void Prepare(bool clearRuns) {
      experiment.Prepare(clearRuns);
    }

    public IEnumerable<IOptimizer> NestedOptimizers {
      get { return experiment.NestedOptimizers; }
    }

    public ExecutionState ExecutionState {
      get { return experiment.ExecutionState; }
    }

    public TimeSpan ExecutionTime {
      get { return experiment.ExecutionTime; }
    }

    public void Prepare() {
      experiment.Prepare();
    }

    public void Start() {
      experiment.Start();
    }

    public void Pause() {
      experiment.Pause();
    }

    public void Stop() {
      experiment.Stop();
    }

    public event EventHandler ExecutionStateChanged;
    private void OnExecutionStateChanged(EventArgs e) {
      var handler = ExecutionStateChanged;
      if (handler != null) handler(this, e);
    }

    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged(EventArgs e) {
      var handler = ExecutionTimeChanged;
      if (handler != null) handler(this, e);
    }

    public event EventHandler Prepared;
    private void OnPrepared(EventArgs e) {
      var handler = Prepared;
      if (handler != null) handler(this, e);
    }

    public event EventHandler Started;
    private void OnStarted(EventArgs e) {
      var handler = Started;
      if (handler != null) handler(this, e);
    }

    public event EventHandler Paused;
    private void OnPaused(EventArgs e) {
      var handler = Paused;
      if (handler != null) handler(this, e);
    }

    public event EventHandler Stopped;
    private void OnStopped(EventArgs e) {
      var handler = Stopped;
      if (handler != null) handler(this, e);
    }

    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccured(EventArgs<Exception> e) {
      var handler = ExceptionOccurred;
      if (handler != null) handler(this, e);
    }

    public void ProblemDataParameterValueChanged(object source, EventArgs e) {
      foreach (var op in NestedOptimizers.OfType<IDataAnalysisAlgorithm<IRegressionProblem>>()) {
        op.Problem.ProblemDataParameter.Value = ProblemData;
      }
    }
  }
}
