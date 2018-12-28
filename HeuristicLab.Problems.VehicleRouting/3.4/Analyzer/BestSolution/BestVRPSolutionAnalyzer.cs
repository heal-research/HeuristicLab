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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  [Item("BestVRPSolutionAnalyzer", "An operator for analyzing the best solution of Vehicle Routing Problems.")]
  [StorableType("42DFB4D7-0DDA-424A-9DB3-1EDAB917A26B")]
  public sealed class BestVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, IGeneralVRPOperator, ISingleObjectiveOperator {
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> DistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> VehiclesUtilizedParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }

    public LookupParameter<VRPSolution> BestSolutionParameter {
      get { return (LookupParameter<VRPSolution>)Parameters["BestSolution"]; }
    }
    public LookupParameter<VRPSolution> BestValidSolutionParameter {
      get { return (LookupParameter<VRPSolution>)Parameters["BestValidSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<VRPSolution> BestKnownSolutionParameter {
      get { return (LookupParameter<VRPSolution>)Parameters["BestKnownSolution"]; }
    }

    public bool EnabledByDefault {
      get { return true; }
    }

    [StorableConstructor]
    private BestVRPSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }

    public BestVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));

      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this VRP instance."));
      Parameters.Add(new LookupParameter<VRPSolution>("BestKnownSolution", "The best known solution of this VRP instance."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distances of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("VehiclesUtilized", "The utilized vehicles of the VRP solutions which should be analyzed."));

      Parameters.Add(new LookupParameter<VRPSolution>("BestSolution", "The best VRP solution."));
      Parameters.Add(new LookupParameter<VRPSolution>("BestValidSolution", "The best valid VRP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestVRPSolutionAnalyzer(this, cloner);
    }

    private BestVRPSolutionAnalyzer(BestVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      #region Backwards Compatibility
      if (!Parameters.ContainsKey("BestKnownQuality")) {
        Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this VRP instance."));
      }
      if (!Parameters.ContainsKey("BestKnownSolution")) {
        Parameters.Add(new LookupParameter<VRPSolution>("BestKnownSolution", "The best known solution of this VRP instance."));
      }
      #endregion
    }

    public override IOperation Apply() {
      IVRPProblemInstance problemInstance = ProblemInstanceParameter.ActualValue;
      ItemArray<IVRPEncoding> solutions = VRPToursParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<DoubleValue> distances = DistanceParameter.ActualValue;
      ItemArray<DoubleValue> vehiclesUtilizations = VehiclesUtilizedParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      IVRPEncoding best = solutions[i].Clone() as IVRPEncoding;
      VRPSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new VRPSolution(problemInstance, best.Clone() as IVRPEncoding, new DoubleValue(qualities[i].Value));
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best VRP Solution", solution));

        results.Add(new Result("Best VRP Solution Distance", new DoubleValue(distances[i].Value)));
        results.Add(new Result("Best VRP Solution VehicleUtilization", new DoubleValue(vehiclesUtilizations[i].Value)));
      } else {
        VRPEvaluation eval = problemInstance.Evaluate(solution.Solution);
        if (qualities[i].Value <= eval.Quality) {
          solution.ProblemInstance = problemInstance;
          solution.Solution = best.Clone() as IVRPEncoding;
          solution.Quality.Value = qualities[i].Value;
          (results["Best VRP Solution Distance"].Value as DoubleValue).Value = distances[i].Value;
          (results["Best VRP Solution VehicleUtilization"].Value as DoubleValue).Value = vehiclesUtilizations[i].Value;
        }
      }

      var idx = qualities.Select((x, index) => new { index, x.Value }).Where(index => problemInstance.Feasible(solutions[index.index])).OrderBy(x => x.Value).FirstOrDefault();
      if (idx != null) {
        int j = idx.index;
        IVRPEncoding bestFeasible = solutions[j].Clone() as IVRPEncoding;
        VRPSolution validSolution = BestValidSolutionParameter.ActualValue;
        if (validSolution == null) {
          validSolution = new VRPSolution(problemInstance, best.Clone() as IVRPEncoding, new DoubleValue(qualities[j].Value));
          BestValidSolutionParameter.ActualValue = validSolution;
          if (results.ContainsKey("Best valid VRP Solution"))
            results["Best valid VRP Solution"].Value = validSolution;
          else
            results.Add(new Result("Best valid VRP Solution", validSolution));

          results.Add(new Result("Best valid VRP Solution Distance", new DoubleValue(distances[j].Value)));
          results.Add(new Result("Best valid VRP Solution VehicleUtilization", new DoubleValue(vehiclesUtilizations[j].Value)));
        } else {
          if (qualities[j].Value <= validSolution.Quality.Value) {
            if (ProblemInstanceParameter.ActualValue.Feasible(best)) {
              validSolution.ProblemInstance = problemInstance;
              validSolution.Solution = best.Clone() as IVRPEncoding;
              validSolution.Quality.Value = qualities[j].Value;
              (results["Best valid VRP Solution Distance"].Value as DoubleValue).Value = distances[j].Value;
              (results["Best valid VRP Solution VehicleUtilization"].Value as DoubleValue).Value = vehiclesUtilizations[j].Value;
            }
          }
        }

        DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;
        if (bestKnownQuality == null || qualities[j].Value < bestKnownQuality.Value) {
          BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[j].Value);
          BestKnownSolutionParameter.ActualValue = (VRPSolution)validSolution.Clone();
        }
      }

      return base.Apply();
    }
  }
}
