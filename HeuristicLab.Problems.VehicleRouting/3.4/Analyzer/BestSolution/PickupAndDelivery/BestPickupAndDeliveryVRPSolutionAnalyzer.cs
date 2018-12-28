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
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  [Item("BestPickupAndDeliveryVRPSolutionAnalyzer", "An operator for analyzing the best solution of Pickup and Delivery Routing Problems.")]
  [StorableType("81CA703F-038E-4D41-BB3F-25E04EC46AF9")]
  public sealed class BestPickupAndDeliveryVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, IPickupAndDeliveryOperator, ISingleObjectiveOperator {
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ScopeTreeLookupParameter<IntValue> PickupViolationsParameter {
      get { return (ScopeTreeLookupParameter<IntValue>)Parameters["PickupViolations"]; }
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
    private BestPickupAndDeliveryVRPSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }

    public BestPickupAndDeliveryVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the VRP solutions which should be analyzed."));

      Parameters.Add(new ScopeTreeLookupParameter<IntValue>("PickupViolations", "The pickup violation of the VRP solutions which should be analyzed."));

      Parameters.Add(new LookupParameter<VRPSolution>("BestSolution", "The best VRP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestPickupAndDeliveryVRPSolutionAnalyzer(this, cloner);
    }

    private BestPickupAndDeliveryVRPSolutionAnalyzer(BestPickupAndDeliveryVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      IVRPProblemInstance problemInstance = ProblemInstanceParameter.ActualValue;
      ItemArray<IVRPEncoding> solutions = VRPToursParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<IntValue> pickupViolations = PickupViolationsParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      IVRPEncoding best = solutions[i] as IVRPEncoding;
      VRPSolution solution = BestSolutionParameter.ActualValue;
      if (solution != null) {
        if (!results.ContainsKey("Best VRP Solution PickupViolations")) {
          results.Add(new Result("Best VRP Solution PickupViolations", new DoubleValue(pickupViolations[i].Value)));
        } else {
          VRPEvaluation eval = problemInstance.Evaluate(solution.Solution);
          if (qualities[i].Value <= eval.Quality) {
            (results["Best VRP Solution PickupViolations"].Value as DoubleValue).Value = pickupViolations[i].Value;
          }
        }
      }

      return base.Apply();
    }
  }
}
