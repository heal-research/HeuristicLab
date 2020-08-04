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

using System.Collections.Generic;
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
  [Item("Basic Tour Analyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableType("7a135e25-fec8-4960-b412-249c56bb3aab")]
  public sealed class BasicVRPTourAnalyzer : InstrumentedOperator, IAnalyzer, IGeneralVRPOperator {
    [Storable] public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter { get; private set; }
    [Storable] public IScopeTreeLookupParameter<VRPEvaluation> EvaluationResultParameter { get; private set; }

    [Storable] public IResultParameter<DataTable> DistancesResult { get; private set; }
    [Storable] public IResultParameter<DataTable> VehiclesUtilizedResult { get; private set; }

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private BasicVRPTourAnalyzer(StorableConstructorFlag _) : base(_) { }
    private BasicVRPTourAnalyzer(BasicVRPTourAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      ProblemInstanceParameter = cloner.Clone(original.ProblemInstanceParameter);
      EvaluationResultParameter = cloner.Clone(original.EvaluationResultParameter);
      DistancesResult = cloner.Clone(original.DistancesResult);
      VehiclesUtilizedResult = cloner.Clone(original.VehiclesUtilizedResult);
    }
    public BasicVRPTourAnalyzer()
      : base() {
      Parameters.Add(ProblemInstanceParameter = new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(EvaluationResultParameter = new ScopeTreeLookupParameter<VRPEvaluation>("EvaluationResult", "The evaluations of the VRP solutions which should be analyzed."));
      DataTable defaultDistanceTable = new DataTable("Distances"), defaultVehiclesUtilizedTable = new DataTable("Vehicles Utilized");
      Parameters.Add(DistancesResult = new ResultParameter<DataTable>("Distances", "The solution distance progress in each iteration.", defaultDistanceTable));
      Parameters.Add(VehiclesUtilizedResult = new ResultParameter<DataTable>("Vehicles Utilized", "The solution vehicle utilization progress in each iteration.", defaultVehiclesUtilizedTable));

      defaultDistanceTable.Rows.Add(new DataRow("Best (monotonic)"));
      defaultDistanceTable.Rows.Add(new DataRow("Best"));
      defaultDistanceTable.Rows.Add(new DataRow("Average"));
      defaultDistanceTable.Rows.Add(new DataRow("Worst"));

      defaultVehiclesUtilizedTable.Rows.Add(new DataRow("Best (monotonic)"));
      defaultVehiclesUtilizedTable.Rows.Add(new DataRow("Best"));
      defaultVehiclesUtilizedTable.Rows.Add(new DataRow("Average"));
      defaultVehiclesUtilizedTable.Rows.Add(new DataRow("Worst"));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BasicVRPTourAnalyzer(this, cloner);
    }

    public override IOperation InstrumentedApply() {
      var evaluations = EvaluationResultParameter.ActualValue;
      var distances = evaluations.Select(x => x.Distance);
      var vehicles = evaluations.Select(x => (double)x.VehicleUtilization);

      Analyze(distances, DistancesResult.ActualValue);
      Analyze(vehicles, VehiclesUtilizedResult.ActualValue);

      return base.InstrumentedApply();
    }

    internal static void Analyze(IEnumerable<double> sequence, DataTable table) {
      var bestSoFar = table.Rows["Best (monotonic)"];
      var best = table.Rows["Best"];
      var average = table.Rows["Average"];
      var worst = table.Rows["Worst"];

      double min = double.MaxValue, max = double.MinValue, sum = 0.0;
      double minmin = bestSoFar.Values.Count > 0 ? bestSoFar.Values.Last() : double.MaxValue;
      int count = 0;

      foreach (var s in sequence) {
        if (min > s) min = s;
        if (minmin > s) minmin = s;
        if (max < s) max = s;
        sum += s;
        count++;
      }

      bestSoFar.Values.Add(minmin);
      best.Values.Add(min);
      average.Values.Add(sum / count);
      worst.Values.Add(max);
    }
  }
}
