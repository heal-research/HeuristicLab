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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.PTSP {
  [Item("EstimatedPTSPMoveEvaluator", "A base class for operators which evaluate PTSP moves.")]
  [StorableType("F2F7F857-F2CD-4AB2-8656-5158BD04EFDD")]
  public abstract class EstimatedPTSPMoveEvaluator : SingleSuccessorOperator, IEstimatedPTSPMoveEvaluator {

    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<IProbabilisticTSPData> ProbabilisticTSPDataParameter {
      get { return (ILookupParameter<IProbabilisticTSPData>)Parameters["PTSP Data"]; }
    }
    public ILookupParameter<ReadOnlyItemList<BoolArray>> RealizationDataParameter {
      get { return (ILookupParameter<ReadOnlyItemList<BoolArray>>)Parameters["RealizationData"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }

    [StorableConstructor]
    protected EstimatedPTSPMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected EstimatedPTSPMoveEvaluator(EstimatedPTSPMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected EstimatedPTSPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<IProbabilisticTSPData>("PTSP Data", "The main parameters of the pTSP."));
      Parameters.Add(new LookupParameter<ReadOnlyItemList<BoolArray>>("RealizationData", "The list of samples drawn from all possible stochastic instances."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a TSP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a TSP solution."));
    }

    public override IOperation Apply() {
      var permutation = PermutationParameter.ActualValue;
      var realizations = RealizationDataParameter.ActualValue;
      var data = ProbabilisticTSPDataParameter.ActualValue;
      var relativeQualityDifference = EvaluateMove(permutation, data, realizations);
      var moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) MoveQualityParameter.ActualValue = new DoubleValue(QualityParameter.ActualValue.Value + relativeQualityDifference);
      else moveQuality.Value = QualityParameter.ActualValue.Value + relativeQualityDifference;
      return base.Apply();
    }

    protected abstract double EvaluateMove(Permutation tour, IProbabilisticTSPData data, ReadOnlyItemList<BoolArray> realizations);
  }
}
