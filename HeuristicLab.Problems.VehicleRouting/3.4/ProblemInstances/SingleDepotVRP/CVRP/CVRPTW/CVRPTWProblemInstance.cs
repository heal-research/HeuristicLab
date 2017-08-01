#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("CVRPTWProblemInstance", "Represents a single depot CVRPTW instance.")]
  [StorableClass]
  public class CVRPTWProblemInstance : CVRPProblemInstance, ITimeWindowedProblemInstance {
    protected IValueParameter<DoubleArray> ReadyTimeParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["ReadyTime"]; }
    }
    protected IValueParameter<DoubleArray> DueTimeParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["DueTime"]; }
    }
    protected IValueParameter<DoubleArray> ServiceTimeParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["ServiceTime"]; }
    }

    protected IValueParameter<DoubleValue> TimeFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalTimeFactor"]; }
    }
    protected IValueParameter<DoubleValue> TardinessPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalTardinessPenalty"]; }
    }

    public DoubleArray ReadyTime {
      get { return ReadyTimeParameter.Value; }
      set { ReadyTimeParameter.Value = value; }
    }
    public DoubleArray DueTime {
      get { return DueTimeParameter.Value; }
      set { DueTimeParameter.Value = value; }
    }
    public DoubleArray ServiceTime {
      get { return ServiceTimeParameter.Value; }
      set { ServiceTimeParameter.Value = value; }
    }
    public DoubleValue TimeFactor {
      get { return TimeFactorParameter.Value; }
      set { TimeFactorParameter.Value = value; }
    }

    protected IValueParameter<DoubleValue> CurrentTardinessPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["CurrentTardinessPenalty"]; }
    }

    public DoubleValue TardinessPenalty {
      get {
        DoubleValue currentTardinessPenalty = CurrentTardinessPenaltyParameter.Value;
        if (currentTardinessPenalty != null)
          return currentTardinessPenalty;
        else
          return TardinessPenaltyParameter.Value;
      }
    }
    DoubleValue ITimeWindowedProblemInstance.CurrentTardinessPenalty {
      get { return CurrentTardinessPenaltyParameter.Value; }
      set { CurrentTardinessPenaltyParameter.Value = value; }
    }

    protected override IEnumerable<IOperator> GetOperators() {
      return base.GetOperators()
        .Where(o => o is ITimeWindowedOperator).Cast<IOperator>();
    }

    protected override IEnumerable<IOperator> GetAnalyzers() {
      return ApplicationManager.Manager.GetInstances<ITimeWindowedOperator>()
        .Where(o => o is IAnalyzer)
        .Cast<IOperator>().Union(base.GetAnalyzers());
    }

    protected override IVRPEvaluator Evaluator {
      get {
        return new CVRPTWEvaluator();
      }
    }

    [StorableConstructor]
    protected CVRPTWProblemInstance(bool deserializing) : base(deserializing) { }

    public CVRPTWProblemInstance() {
      Parameters.Add(new ValueParameter<DoubleArray>("ReadyTime", "The ready time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("DueTime", "The due time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ServiceTime", "The service time of each customer.", new DoubleArray()));

      Parameters.Add(new ValueParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentTardinessPenalty", "The current tardiness penalty considered in the evaluation.") { Hidden = true });

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPTWProblemInstance(this, cloner);
    }

    protected CVRPTWProblemInstance(CVRPTWProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      ReadyTimeParameter.ValueChanged += ReadyTimeParameter_ValueChanged;
      ReadyTime.Reset += ReadyTime_Changed;
      ReadyTime.ItemChanged += ReadyTime_Changed;
      DueTimeParameter.ValueChanged += DueTimeParameter_ValueChanged;
      DueTime.Reset += DueTime_Changed;
      DueTime.ItemChanged += DueTime_Changed;
      ServiceTimeParameter.ValueChanged += ServiceTimeParameter_ValueChanged;
      ServiceTime.Reset += ServiceTime_Changed;
      ServiceTime.ItemChanged += ServiceTime_Changed;
      TardinessPenaltyParameter.ValueChanged += TardinessPenaltyParameter_ValueChanged;
      TardinessPenalty.ValueChanged += TardinessPenalty_ValueChanged;
      TimeFactorParameter.ValueChanged += TimeFactorParameter_ValueChanged;
      TimeFactor.ValueChanged += TimeFactor_ValueChanged;
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentTardinessPenaltyParameter.Value = null;
    }

    #region Event handlers
    private void ReadyTimeParameter_ValueChanged(object sender, EventArgs e) {
      ReadyTime.Reset += ReadyTime_Changed;
      ReadyTime.ItemChanged += ReadyTime_Changed;
      EvalBestKnownSolution();
    }
    private void ReadyTime_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void DueTimeParameter_ValueChanged(object sender, EventArgs e) {
      DueTime.Reset += DueTime_Changed;
      DueTime.ItemChanged += DueTime_Changed;
      EvalBestKnownSolution();
    }
    private void DueTime_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void ServiceTimeParameter_ValueChanged(object sender, EventArgs e) {
      ServiceTime.Reset += ServiceTime_Changed;
      ServiceTime.ItemChanged += ServiceTime_Changed;
      EvalBestKnownSolution();
    }
    private void ServiceTime_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void TardinessPenaltyParameter_ValueChanged(object sender, EventArgs e) {
      TardinessPenaltyParameter.Value.ValueChanged += TardinessPenalty_ValueChanged;
      EvalBestKnownSolution();
    }
    private void TardinessPenalty_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    private void TimeFactorParameter_ValueChanged(object sender, EventArgs e) {
      TimeFactorParameter.Value.ValueChanged += TimeFactor_ValueChanged;
      EvalBestKnownSolution();
    }
    private void TimeFactor_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}