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
  [Item("Capacitated Tour Analyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableType("72fdaf0a-4ac7-4ac5-a563-3efb0e3fae9e")]
  public sealed class CapacitatedTourAnalyzer : InstrumentedOperator, IAnalyzer, ICapacitatedOperator {
    [Storable] public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter { get; private set; }
    [Storable] public IScopeTreeLookupParameter<CVRPEvaluation> EvaluationResultParameter { get; private set; }

    [Storable] public IResultParameter<DataTable> OverloadResult { get; private set; }

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private CapacitatedTourAnalyzer(StorableConstructorFlag _) : base(_) { }
    private CapacitatedTourAnalyzer(CapacitatedTourAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      ProblemInstanceParameter = cloner.Clone(original.ProblemInstanceParameter);
      EvaluationResultParameter = cloner.Clone(original.EvaluationResultParameter);
      OverloadResult = cloner.Clone(original.OverloadResult);
    }
    public CapacitatedTourAnalyzer()
      : base() {
      Parameters.Add(ProblemInstanceParameter = new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(EvaluationResultParameter = new ScopeTreeLookupParameter<CVRPEvaluation>("EvaluationResult", "The evaluations of the VRP solutions which should be analyzed."));
      DataTable defaultOverloadTable = new DataTable("Overload");
      Parameters.Add(OverloadResult = new ResultParameter<DataTable>("Overload", "The solution overload progress in each iteration.", defaultOverloadTable));

      defaultOverloadTable.Rows.Add(new DataRow("Best (monotonic)"));
      defaultOverloadTable.Rows.Add(new DataRow("Best"));
      defaultOverloadTable.Rows.Add(new DataRow("Average"));
      defaultOverloadTable.Rows.Add(new DataRow("Worst"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CapacitatedTourAnalyzer(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      var evaluations = EvaluationResultParameter.ActualValue;
      var overloads = evaluations.Select(x => x.Overload);

      BasicVRPTourAnalyzer.Analyze(overloads, OverloadResult.ActualValue);

      return base.InstrumentedApply();
    }
  }
}
