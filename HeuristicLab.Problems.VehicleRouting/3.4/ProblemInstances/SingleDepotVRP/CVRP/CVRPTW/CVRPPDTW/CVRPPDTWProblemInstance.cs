#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("CVRPPDTWProblemInstance", "Represents a single depot CVRPPDTW instance.")]
  [StorableClass]
  public class CVRPPDTWProblemInstance : CVRPTWProblemInstance, IPickupAndDeliveryProblemInstance {
    protected IValueParameter<IntArray> PickupDeliveryLocationParameter {
      get { return (IValueParameter<IntArray>)Parameters["PickupDeliveryLocation"]; }
    }
    protected IValueParameter<DoubleValue> PickupViolationPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["EvalPickupViolationPenalty"]; }
    }

    public IntArray PickupDeliveryLocation {
      get { return PickupDeliveryLocationParameter.Value; }
      set { PickupDeliveryLocationParameter.Value = value; }
    }

    protected IValueParameter<DoubleValue> CurrentPickupViolationPenaltyParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["CurrentPickupViolationPenalty"]; }
    }

    public DoubleValue PickupViolationPenalty {
      get {
        DoubleValue currentPickupViolationPenalty = CurrentPickupViolationPenaltyParameter.Value;
        if (currentPickupViolationPenalty != null)
          return currentPickupViolationPenalty;
        else
          return PickupViolationPenaltyParameter.Value;
      }
    }
    DoubleValue IPickupAndDeliveryProblemInstance.CurrentPickupViolationPenalty {
      get { return CurrentOverloadPenaltyParameter.Value; }
      set { CurrentPickupViolationPenaltyParameter.Value = value; }
    }

    protected override IEnumerable<IOperator> GetOperators() {
      return
        ApplicationManager.Manager.GetInstances<IPickupAndDeliveryOperator>()
        .Where(o => !(o is IAnalyzer))
        .Cast<IOperator>().Union(base.GetOperators());
    }

    protected override IEnumerable<IOperator> GetAnalyzers() {
      return ApplicationManager.Manager.GetInstances<IPickupAndDeliveryOperator>()
        .Where(o => o is IAnalyzer)
        .Cast<IOperator>().Union(base.GetAnalyzers());
    }

    protected override IVRPEvaluator Evaluator {
      get {
        return new CVRPPDTWEvaluator();
      }
    }

    public int GetPickupDeliveryLocation(int city) {
      return PickupDeliveryLocation[city];
    }

    [StorableConstructor]
    protected CVRPPDTWProblemInstance(bool deserializing) : base(deserializing) { }

    public CVRPPDTWProblemInstance() {
      Parameters.Add(new ValueParameter<IntArray>("PickupDeliveryLocation", "The pickup and delivery location for each customer.", new IntArray()));

      Parameters.Add(new ValueParameter<DoubleValue>("EvalPickupViolationPenalty", "The pickup violation penalty considered in the evaluation.", new DoubleValue(100)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("CurrentPickupViolationPenalty", "The current pickup violation penalty considered in the evaluation.") { Hidden = true });

      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CVRPPDTWProblemInstance(this, cloner);
    }

    protected CVRPPDTWProblemInstance(CVRPPDTWProblemInstance original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      PickupDeliveryLocationParameter.ValueChanged += PickupDeliveryLocationParameter_ValueChanged;
      PickupDeliveryLocation.Reset += PickupDeliveryLocation_Changed;
      PickupDeliveryLocation.ItemChanged += PickupDeliveryLocation_Changed;
    }

    public override void InitializeState() {
      base.InitializeState();

      CurrentPickupViolationPenaltyParameter.Value = null;
    }

    #region Event handlers
    void PickupDeliveryLocationParameter_ValueChanged(object sender, EventArgs e) {
      PickupDeliveryLocation.Reset += PickupDeliveryLocation_Changed;
      PickupDeliveryLocation.ItemChanged += PickupDeliveryLocation_Changed;
      EvalBestKnownSolution();
    }
    private void PickupDeliveryLocation_Changed(object sender, EventArgs e) {
      EvalBestKnownSolution();
    }
    #endregion
  }
}