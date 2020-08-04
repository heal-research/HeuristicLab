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
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("Time Window Tour Analyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableType("abc472b6-ee21-4b67-8ad9-f82811b2919e")]
  public sealed class TimeWindowTourAnalyzer : InstrumentedOperator, IAnalyzer, ITimeWindowedOperator {
    [Storable] public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter { get; private set; }
    [Storable] public IScopeTreeLookupParameter<CVRPTWEvaluation> EvaluationResultParameter { get; private set; }

    [Storable] public IResultParameter<DataTable> TardinessResult { get; private set; }
    [Storable] public IResultParameter<DataTable> TravelTimeResult { get; private set; }

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private TimeWindowTourAnalyzer(StorableConstructorFlag _) : base(_) { }
    private TimeWindowTourAnalyzer(TimeWindowTourAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      ProblemInstanceParameter = cloner.Clone(original.ProblemInstanceParameter);
      EvaluationResultParameter = cloner.Clone(original.EvaluationResultParameter);
      TardinessResult = cloner.Clone(original.TardinessResult);
      TravelTimeResult = cloner.Clone(original.TravelTimeResult);
    }
    public TimeWindowTourAnalyzer()
      : base() {
      Parameters.Add(ProblemInstanceParameter = new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(EvaluationResultParameter = new ScopeTreeLookupParameter<CVRPTWEvaluation>("EvaluationResult", "The evaluations of the VRP solutions which should be analyzed."));
      DataTable defaultTardinessTable = new DataTable("Tardiness"), defaultTravelTimeTable = new DataTable("Travel Times");
      Parameters.Add(TardinessResult = new ResultParameter<DataTable>("Tardiness", "The solution tardiness progress in each iteration.", defaultTardinessTable));
      Parameters.Add(TravelTimeResult = new ResultParameter<DataTable>("Travel Times", "The solution travel time progress in each iteration.", defaultTravelTimeTable));

      defaultTardinessTable.Rows.Add(new DataRow("Best (monotonic)"));
      defaultTardinessTable.Rows.Add(new DataRow("Best"));
      defaultTardinessTable.Rows.Add(new DataRow("Average"));
      defaultTardinessTable.Rows.Add(new DataRow("Worst"));

      defaultTravelTimeTable.Rows.Add(new DataRow("Best (monotonic)"));
      defaultTravelTimeTable.Rows.Add(new DataRow("Best"));
      defaultTravelTimeTable.Rows.Add(new DataRow("Average"));
      defaultTravelTimeTable.Rows.Add(new DataRow("Worst"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimeWindowTourAnalyzer(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      var evaluations = EvaluationResultParameter.ActualValue;
      var tardiness = evaluations.Select(x => x.Tardiness);
      var travelTimes = evaluations.Select(x => (double)x.TravelTime);

      BasicVRPTourAnalyzer.Analyze(tardiness, TardinessResult.ActualValue);
      BasicVRPTourAnalyzer.Analyze(travelTimes, TravelTimeResult.ActualValue);

      return base.InstrumentedApply();
    }
  }
}
