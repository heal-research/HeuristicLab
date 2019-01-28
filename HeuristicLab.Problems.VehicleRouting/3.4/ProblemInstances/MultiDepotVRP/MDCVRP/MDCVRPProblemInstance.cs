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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting.ProblemInstances {
  [Item("MDCVRPProblemInstance", "Represents a multi depot CVRP instance.")]
  [StorableType("19F824AB-6C66-4FB6-88D5-3C163BF789E6")]
  public class MDCVRPProblemInstance : MultiDepotVRPProblemInstance, IHeterogenousCapacitatedProblemInstance {
    protected IValueParameter<DoubleArray> CapacityParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Capacity"]; }
    }
    protected IValueParameter<DoubleValue> OverloadPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalOverloadPenalty"]; }
    }

    public DoubleArray Capacity {
      get { return CapacityParameter.Value; }
      set { CapacityParameter.Value = value; }
    }

    protected IValueParameter<DoubleValue> CurrentOverloadPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["CurrentOverloadPenalty"]; }
    }

    public DoubleValue OverloadPenalty {
      get {
        DoubleValue currentOverloadPenalty = CurrentOverloadPenaltyParameter.Value;
        if (currentOverloadPenalty != null)
          return currentOverloadPenalty;
        else
          return OverloadPenaltyParameter.Value;
      }
    }
    DoubleValue ICapacitatedProblemInstance.CurrentOverloadPenalty {
      get { return CurrentOverloadPenaltyParameter.Value; }
      set { CurrentOverloadPenaltyParameter.Value = value; }
    }

    protected override IEnumerable<IOperator> GetOperators() {
      return base.GetOperators()
        .Where(o => o is IHeterogenousCapacitatedOperator).Cast<IOperator>();
    }

    protected override IEnumerable<IOperator> GetAnalyzers() {
      return ApplicationManager.Manager.GetInstances<ICapacitatedOperator>()
        .Where(o => o is IAnalyzer)
        .Cast<IOperator>().Union(base.GetAnalyzers());
    }

    protected override IVRPEvaluator Evaluator {
      get {
        return new MDCVRPEvaluator();
      }
    }

    [StorableConstructor]
    protected MDCVRPProblemInstance(StorableConstructorFlag _) : base(_) { }

    public MDCVRPProblemInstance() {
      Parameters.Add(new ValueParameter<DoubleArray>("Capacity", "The capacity of each vehicle.", new DoubleArray()));
      Parameters.Add(new ValueParameter<DoubleValue>("EvalOverloadPenalty", "The overload penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentOverloadPenalty", "The current overload penalty considered in the evaluation.") { Hidden = true });

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MDCVRPProblemInstance(this, cloner);
    }

    protected MDCVRPProblemInstance(MDCVRPProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      CapacityParameter.ValueChanged += CapacityParameter_ValueChanged;
      Capacity.Reset += Capacity_Changed;
      Capacity.ItemChanged += Capacity_Changed;
      OverloadPenaltyParameter.ValueChanged += OverloadPenaltyParameter_ValueChanged;
      OverloadPenalty.ValueChanged += OverloadPenalty_ValueChanged;
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentOverloadPenaltyParameter.Value = null;
    }

    #region Event handlers
    void CapacityParameter_ValueChanged(object sender, EventArgs e) {
      Capacity.Reset += Capacity_Changed;
      Capacity.ItemChanged += Capacity_Changed;
      EvalBestKnownSolution();
    }
    private void Capacity_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    void OverloadPenaltyParameter_ValueChanged(object sender, EventArgs e) {
      OverloadPenalty.ValueChanged += OverloadPenalty_ValueChanged;
      EvalBestKnownSolution();
    }
    void OverloadPenalty_ValueChanged(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}