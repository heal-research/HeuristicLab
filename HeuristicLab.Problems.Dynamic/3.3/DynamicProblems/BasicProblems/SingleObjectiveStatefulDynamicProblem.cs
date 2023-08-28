#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Dynamic {
  [StorableType("F444C6C0-A8F0-406F-9617-587E29A37644")]
  public abstract class SingleObjectiveStatefulDynamicProblem<TEncoding, TSolution, TState>
    : SingleObjectiveDynamicBasicProblem<TEncoding, TSolution, TState>
    where TEncoding : class, IEncoding 
    where TSolution : class, IItem
    where TState : class, IDynamicProblemState<TState>
  {
    #region ParameterNames
    public const string StateVersionParameterName = "StateVersion";
    public const string StateParameterName = "State";
    public const string InitialStateParameterName = "InitialState";
    #endregion

    #region ResultNames
    public const string StatePlotResultName = "StateVersions";
    public const string StatePlotRowName = "Versions";
    #endregion

    #region ParameterProperties
    public IFixedValueParameter<DoubleValue> StateVersionParameter => (IFixedValueParameter<DoubleValue>)Parameters[StateVersionParameterName];
    public IValueParameter<TState> InitialStateParameter => (IValueParameter<TState>)Parameters[InitialStateParameterName];
    public IValueParameter<TState> StateParameter => (IValueParameter<TState>)Parameters[StateParameterName];
    #endregion

    #region Propeties
    public override bool Maximization => true;
    public double StateVersion { get { return StateVersionParameter.Value.Value; } set { StateVersionParameter.Value.Value = value; } }
    public TState InitialState { get { return InitialStateParameter.Value; } set { InitialStateParameter.Value = value; } }
    public TState State { get { return StateParameter.Value; } set { StateParameter.Value = value; } }
    #endregion

    #region Constructors and Cloning

    protected SingleObjectiveStatefulDynamicProblem() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(StateVersionParameterName, "The version of the problem state", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<TState>(StateParameterName, "The internal (changing) problem state"){Hidden = true});
      Parameters.Add(new ValueParameter<TState>(InitialStateParameterName, "The initial (unchanging) problem state"));
    }

    [StorableConstructor]
    protected SingleObjectiveStatefulDynamicProblem(StorableConstructorFlag _) : base(_) { }

    protected SingleObjectiveStatefulDynamicProblem(SingleObjectiveStatefulDynamicProblem<TEncoding,TSolution,TState> original, Cloner cloner) : base(original, cloner) { }
    #endregion

    #region ProblemMethods

    protected sealed override void Update(long version) {
      State.Update(EnvironmentRandom, version);
      StateVersion = version;
    }

    protected override TState GetData() => State;

    protected sealed override void AlgorithmReset() { }

    protected override void AlgorithmStart(){
      ResetState();
    }

    protected void ResetState() {
      StateVersion = 0;
      State = (TState)InitialState.Clone();
      State.Initialize(EnvironmentRandom ?? new FastRandom(0));
      foreach (var tracker in Trackers) {
        tracker.OnEpochChange(GetData(), 0, 0);
      }
      
    }

    protected override void AnalyzeProblem(ResultCollection results, IRandom random, bool dummy) {
      base.AnalyzeProblem(results, random, dummy);
    }

    #endregion

    #region Helpers
    private static ScatterPlot CreateScatterPlot() {
      var plot = new ScatterPlot("StateVersionPlot", "The state over time", new ScatterPlotVisualProperties() {
        XAxisTitle = "Generation",
        YAxisTitle = "StateVersion"
      });
      var row = new ScatterPlotDataRow(StatePlotRowName, "", Array.Empty<Point2D<double>>());
      plot.Rows.Add(row);
      return plot;
    }
    #endregion
  }
}
