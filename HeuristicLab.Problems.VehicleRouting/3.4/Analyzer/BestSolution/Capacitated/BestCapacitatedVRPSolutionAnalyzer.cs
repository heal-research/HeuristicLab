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
using HEAL.Attic;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  [Item("BestCapacitatedVRPSolutionAnalyzer", "An operator for analyzing the best solution of capacitated Vehicle Routing Problems.")]
  [StorableType("5E50BE52-E801-4F84-8070-298368FEC208")]
  public sealed class BestCapacitatedVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer, ICapacitatedOperator, ISingleObjectiveOperator {
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
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
    private BestCapacitatedVRPSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }

    public BestCapacitatedVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the VRP solutions which should be analyzed."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));

      Parameters.Add(new LookupParameter<VRPSolution>("BestSolution", "The best VRP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestCapacitatedVRPSolutionAnalyzer(this, cloner);
    }

    private BestCapacitatedVRPSolutionAnalyzer(BestCapacitatedVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IOperation Apply() {
      IVRPProblemInstance problemInstance = ProblemInstanceParameter.ActualValue;
      ItemArray<IVRPEncoding> solutions = VRPToursParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<DoubleValue> overloads = OverloadParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      VRPSolution solution = BestSolutionParameter.ActualValue;
      if (solution != null) {
        if (!results.ContainsKey("Best VRP Solution Overload")) {
          results.Add(new Result("Best VRP Solution Overload", new DoubleValue(overloads[i].Value)));
        } else {
          VRPEvaluation eval = problemInstance.Evaluate(solution.Solution);
          if (qualities[i].Value <= eval.Quality) {
            (results["Best VRP Solution Overload"].Value as DoubleValue).Value = overloads[i].Value;
          }
        }
      }

      return base.Apply();
    }
  }
}
