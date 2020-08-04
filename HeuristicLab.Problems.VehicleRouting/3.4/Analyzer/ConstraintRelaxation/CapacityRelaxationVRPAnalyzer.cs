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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.ProblemInstances;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for adaptive constraint relaxation.
  /// </summary>
  [Item("CapacityRelaxationVRPAnalyzer", "An operator for adaptively relaxing the capacity constraints.")]
  [StorableType("0e244bff-3d76-4af1-95b8-4316c25096be")]
  public class CapacityRelaxationVRPAnalyzer : SingleSuccessorOperator, IAnalyzer, ICapacitatedOperator, ISingleObjectiveOperator {
    [Storable] public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter { get; private set; }
    [Storable] public IScopeTreeLookupParameter<IVRPEncodedSolution> VRPToursParameter { get; private set; }
    [Storable] public IScopeTreeLookupParameter<CVRPEvaluation> EvaluationResultParameter { get; private set; }

    [Storable] public IValueParameter<DoubleValue> SigmaParameter { get; private set; }
    [Storable] public IValueParameter<DoubleValue> PhiParameter { get; private set; }
    [Storable] public IValueParameter<DoubleValue> MinPenaltyFactorParameter { get; private set; }
    [Storable] public IValueParameter<DoubleValue> MaxPenaltyFactorParameter { get; private set; }

    [Storable] public IResultParameter<DoubleValue> CurrentOverloadPenaltyResult { get; private set; }

    public bool EnabledByDefault {
      get { return false; }
    }

    [StorableConstructor]
    protected CapacityRelaxationVRPAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected CapacityRelaxationVRPAnalyzer(CapacityRelaxationVRPAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      ProblemInstanceParameter = cloner.Clone(original.ProblemInstanceParameter);
      VRPToursParameter = cloner.Clone(original.VRPToursParameter);
      EvaluationResultParameter = cloner.Clone(original.EvaluationResultParameter);
      SigmaParameter = cloner.Clone(original.SigmaParameter);
      PhiParameter = cloner.Clone(original.PhiParameter);
      MinPenaltyFactorParameter = cloner.Clone(original.MinPenaltyFactorParameter);
      MaxPenaltyFactorParameter = cloner.Clone(original.MaxPenaltyFactorParameter);
      CurrentOverloadPenaltyResult = cloner.Clone(original.CurrentOverloadPenaltyResult);
    }
    public CapacityRelaxationVRPAnalyzer()
      : base() {
      Parameters.Add(ProblemInstanceParameter = new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));
      Parameters.Add(VRPToursParameter = new ScopeTreeLookupParameter<IVRPEncodedSolution>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(EvaluationResultParameter = new ScopeTreeLookupParameter<CVRPEvaluation>("EvaluationResult", "The evaluations of the VRP solutions which should be analyzed."));

      Parameters.Add(SigmaParameter = new ValueParameter<DoubleValue>("Sigma", "The sigma applied to the penalty factor.", new DoubleValue(0.5)));
      Parameters.Add(PhiParameter = new ValueParameter<DoubleValue>("Phi", "The phi applied to the penalty factor.", new DoubleValue(0.5)));
      Parameters.Add(MinPenaltyFactorParameter = new ValueParameter<DoubleValue>("MinPenaltyFactor", "The minimum penalty factor.", new DoubleValue(0.01)));
      Parameters.Add(MaxPenaltyFactorParameter = new ValueParameter<DoubleValue>("MaxPenaltyFactor", "The maximum penalty factor.", new DoubleValue(100000)));
      Parameters.Add(CurrentOverloadPenaltyResult = new ResultParameter<DoubleValue>("Current Overload Penalty", "The current penalty applied to exceeding the capacity constraint.", new DoubleValue(double.NaN)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CapacityRelaxationVRPAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var cvrp = (ICapacitatedProblemInstance)ProblemInstanceParameter.ActualValue;
  
      ItemArray<CVRPEvaluation> evaluations = EvaluationResultParameter.ActualValue;

      double sigma = SigmaParameter.Value.Value;
      double phi = PhiParameter.Value.Value;
      double minPenalty = MinPenaltyFactorParameter.Value.Value;
      double maxPenalty = MaxPenaltyFactorParameter.Value.Value;

      for (int j = 0; j < evaluations.Length; j++) {
        evaluations[j].Quality -= evaluations[j].Overload * cvrp.OverloadPenalty.Value;
      }

      int validCount = 0;
      for (int j = 0; j < evaluations.Length; j++) {
        if (evaluations[j].Overload == 0)
          validCount++;
      }

      double factor = 1.0 - ((double)validCount / (double)evaluations.Length);

      double min = cvrp.OverloadPenalty.Value / (1 + sigma);
      double max = cvrp.OverloadPenalty.Value * (1 + phi);

      cvrp.CurrentOverloadPenalty = new DoubleValue(min + (max - min) * factor);
      if (cvrp.CurrentOverloadPenalty.Value < minPenalty)
        cvrp.CurrentOverloadPenalty.Value = minPenalty;
      if (cvrp.CurrentOverloadPenalty.Value > maxPenalty)
        cvrp.CurrentOverloadPenalty.Value = maxPenalty;

      for (int j = 0; j < evaluations.Length; j++) {
        evaluations[j].Quality += evaluations[j].Overload * cvrp.CurrentOverloadPenalty.Value;
      }

      CurrentOverloadPenaltyResult.ActualValue = new DoubleValue(cvrp.CurrentOverloadPenalty.Value);

      return base.Apply();
    }
  }
}
