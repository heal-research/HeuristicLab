#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  [Item("BestTimeWindowedVRPSolutionAnalyzer", "An operator for analyzing the best solution of time windowed Vehicle Routing Problems.")]
  [StorableClass]
  public sealed class BestTimeWindowedVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ITimeWindowedOperator, ISingleObjectiveOperator {
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> TardinessParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }

    public LookupParameter<VRPSolution> BestSolutionParameter {
      get { return (LookupParameter<VRPSolution>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private BestTimeWindowedVRPSolutionAnalyzer(bool deserializing) : base(deserializing) { }

    public BestTimeWindowedVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the VRP solutions which should be analyzed."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel times of the VRP solutions which should be analyzed."));

      Parameters.Add(new LookupParameter<VRPSolution>("BestSolution", "The best VRP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestTimeWindowedVRPSolutionAnalyzer(this, cloner);
    }

    private BestTimeWindowedVRPSolutionAnalyzer(BestTimeWindowedVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      IVRPProblemInstance problemInstance = ProblemInstanceParameter.ActualValue;
      ItemArray<IVRPEncoding> solutions = VRPToursParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<DoubleValue> tardinesses = TardinessParameter.ActualValue;
      ItemArray<DoubleValue> travelTimes = TravelTimeParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      VRPSolution solution = BestSolutionParameter.ActualValue;
      if (solution != null) {
        if (!results.ContainsKey("Best VRP Solution Tardiness")) {
          results.Add(new Result("Best VRP Solution Tardiness", new DoubleValue(tardinesses[i].Value)));
          results.Add(new Result("Best VRP Solution TravelTime", new DoubleValue(travelTimes[i].Value)));
        } else {
          VRPEvaluation eval = problemInstance.Evaluate(solution.Solution);
          if (qualities[i].Value <= eval.Quality) {
            (results["Best VRP Solution Tardiness"].Value as DoubleValue).Value = tardinesses[i].Value;
            (results["Best VRP Solution TravelTime"].Value as DoubleValue).Value = travelTimes[i].Value;
          }
        }
      }

      return base.Apply();
    }
  }
}
