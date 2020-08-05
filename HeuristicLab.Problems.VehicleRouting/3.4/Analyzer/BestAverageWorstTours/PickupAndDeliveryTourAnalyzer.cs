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

using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("Pickup & Delivery Tour Analyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableType("0a108965-fb1a-47a3-a5d4-11d1bfa51b0d")]
  public sealed class PickupAndDeliveryTourAnalyzer : InstrumentedOperator, IAnalyzer, IPickupAndDeliveryOperator {
    [Storable] public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter { get; private set; }
    [Storable] public IScopeTreeLookupParameter<CVRPPDTWEvaluation> EvaluationResultParameter { get; private set; }

    [Storable] public IResultParameter<DataTable> PickupViolationsResult { get; private set; }

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private PickupAndDeliveryTourAnalyzer(StorableConstructorFlag _) : base(_) { }
    private PickupAndDeliveryTourAnalyzer(PickupAndDeliveryTourAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      ProblemInstanceParameter = cloner.Clone(original.ProblemInstanceParameter);
      EvaluationResultParameter = cloner.Clone(original.EvaluationResultParameter);
      PickupViolationsResult = cloner.Clone(original.PickupViolationsResult);
    }
    public PickupAndDeliveryTourAnalyzer()
      : base() {
      Parameters.Add(ProblemInstanceParameter = new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(EvaluationResultParameter = new ScopeTreeLookupParameter<CVRPPDTWEvaluation>("EvaluationResult", "The evaluations of the VRP solutions which should be analyzed."));
      DataTable defaultPickupViolationsTable = new DataTable("Pickup Violations");
      Parameters.Add(PickupViolationsResult = new ResultParameter<DataTable>("Pickup Violations", "The solution pickup violation progress in each iteration.", defaultPickupViolationsTable));

      defaultPickupViolationsTable.Rows.Add(new DataRow("Best (monotonic)"));
      defaultPickupViolationsTable.Rows.Add(new DataRow("Best"));
      defaultPickupViolationsTable.Rows.Add(new DataRow("Average"));
      defaultPickupViolationsTable.Rows.Add(new DataRow("Worst"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PickupAndDeliveryTourAnalyzer(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      var evaluations = EvaluationResultParameter.ActualValue;
      var pickupViolations = evaluations.Select(x => (double)x.PickupViolations);

      BasicVRPTourAnalyzer.Analyze(pickupViolations, PickupViolationsResult.ActualValue);

      return base.InstrumentedApply();
    }
  }
}
