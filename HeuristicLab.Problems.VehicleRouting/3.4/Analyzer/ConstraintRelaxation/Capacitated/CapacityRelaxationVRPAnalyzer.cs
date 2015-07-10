#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for adaptive constraint relaxation.
  /// </summary>
  [Item("CapacityRelaxationVRPAnalyzer", "An operator for adaptively relaxing the capacity constraints.")]
  [StorableClass]
  public class CapacityRelaxationVRPAnalyzer : SingleSuccessorOperator, IAnalyzer, ICapacitatedOperator, ISingleObjectiveOperator {
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

    public IValueParameter<DoubleValue> SigmaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Sigma"]; }
    }
    public IValueParameter<DoubleValue> PhiParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["Phi"]; }
    }
    public IValueParameter<DoubleValue> MinPenaltyFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MinPenaltyFactor"]; }
    }
    public IValueParameter<DoubleValue> MaxPenaltyFactorParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MaxPenaltyFactor"]; }
    }

    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public bool EnabledByDefault {
      get { return false; }
    }

    [StorableConstructor]
    protected CapacityRelaxationVRPAnalyzer(bool deserializing) : base(deserializing) { }

    public CapacityRelaxationVRPAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the VRP solutions which should be analyzed."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));

      Parameters.Add(new ValueParameter<DoubleValue>("Sigma", "The sigma applied to the penalty factor.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Phi", "The phi applied to the penalty factor.", new DoubleValue(0.5)));
      Parameters.Add(new ValueParameter<DoubleValue>("MinPenaltyFactor", "The minimum penalty factor.", new DoubleValue(0.01)));
      Parameters.Add(new ValueParameter<DoubleValue>("MaxPenaltyFactor", "The maximum penalty factor.", new DoubleValue(100000)));

      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CapacityRelaxationVRPAnalyzer(this, cloner);
    }

    protected CapacityRelaxationVRPAnalyzer(CapacityRelaxationVRPAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("MaxPenaltyFactor")) {
        Parameters.Add(new ValueParameter<DoubleValue>("MaxPenaltyFactor", "The maximum penalty factor.", new DoubleValue(100000)));
      }
      #endregion
    }

    public override IOperation Apply() {
      ICapacitatedProblemInstance cvrp = ProblemInstanceParameter.ActualValue as ICapacitatedProblemInstance;
      ResultCollection results = ResultsParameter.ActualValue;

      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<DoubleValue> overloads = OverloadParameter.ActualValue;

      double sigma = SigmaParameter.Value.Value;
      double phi = PhiParameter.Value.Value;
      double minPenalty = MinPenaltyFactorParameter.Value.Value;
      double maxPenalty = MaxPenaltyFactorParameter.Value.Value;

      for (int j = 0; j < qualities.Length; j++) {
        qualities[j].Value -= overloads[j].Value * cvrp.OverloadPenalty.Value;
      }

      int validCount = 0;
      for (int j = 0; j < qualities.Length; j++) {
        if (overloads[j].Value == 0)
          validCount++;
      }

      double factor = 1.0 - ((double)validCount / (double)qualities.Length);

      double min = cvrp.OverloadPenalty.Value / (1 + sigma);
      double max = cvrp.OverloadPenalty.Value * (1 + phi);

      cvrp.OverloadPenalty = new DoubleValue(min + (max - min) * factor);
      if (cvrp.OverloadPenalty.Value < minPenalty)
        cvrp.OverloadPenalty.Value = minPenalty;
      if (cvrp.OverloadPenalty.Value > maxPenalty)
        cvrp.OverloadPenalty.Value = maxPenalty;

      for (int j = 0; j < qualities.Length; j++) {
        qualities[j].Value += overloads[j].Value * cvrp.OverloadPenalty.Value;
      }

      if (!results.ContainsKey("Current Overload Penalty")) {
        results.Add(new Result("Current Overload Penalty", new DoubleValue(cvrp.OverloadPenalty.Value)));
      } else {
        (results["Current Overload Penalty"].Value as DoubleValue).Value = cvrp.OverloadPenalty.Value;
      }

      return base.Apply();
    }
  }
}
