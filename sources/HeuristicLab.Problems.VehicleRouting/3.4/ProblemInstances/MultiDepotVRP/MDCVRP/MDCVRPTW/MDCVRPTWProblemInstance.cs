#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("MDCVRPTWProblemInstance", "Represents a multi depot CVRPTW instance.")]
  [StorableClass]
  public class MDCVRPTWProblemInstance : MDCVRPProblemInstance, ITimeWindowedProblemInstance {
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
        return new MDCVRPTWEvaluator();
      }
    }

    [StorableConstructor]
    protected MDCVRPTWProblemInstance(bool deserializing) : base(deserializing) { }

    public MDCVRPTWProblemInstance() {
      Parameters.Add(new ValueParameter<DoubleArray>("ReadyTime", "The ready time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("DueTime", "The due time of each customer.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleArray>("ServiceTime", "The service time of each customer.", new DoubleArray()));

      Parameters.Add(new ValueParameter<DoubleValue>("EvalTimeFactor", "The time factor considered in the evaluation.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalTardinessPenalty", "The tardiness penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentTardinessPenalty", "The current tardiness penalty considered in the evaluation."));

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MDCVRPTWProblemInstance(this, cloner);
    }

    protected MDCVRPTWProblemInstance(MDCVRPTWProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      TardinessPenaltyParameter.ValueChanged += new EventHandler(TardinessPenaltyParameter_ValueChanged);
      TardinessPenaltyParameter.Value.ValueChanged += new EventHandler(TardinessPenalty_ValueChanged);
      TimeFactorParameter.ValueChanged += new EventHandler(TimeFactorParameter_ValueChanged);
      TimeFactorParameter.Value.ValueChanged += new EventHandler(TimeFactor_ValueChanged);
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentTardinessPenaltyParameter.Value = null;
    }

    #region Event handlers
    void TardinessPenaltyParameter_ValueChanged(object sender, EventArgs e) {
      TardinessPenaltyParameter.Value.ValueChanged += new EventHandler(TardinessPenalty_ValueChanged);
      EvalBestKnownSolution();
    }
    void TardinessPenalty_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void TimeFactorParameter_ValueChanged(object sender, EventArgs e) {
      TimeFactorParameter.Value.ValueChanged += new EventHandler(TimeFactor_ValueChanged);
      EvalBestKnownSolution();
    }
    void TimeFactor_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}