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
  [Item("AnalyticalPTSPMoveEvaluator", "A base class for operators which evaluate PTSP moves.")]
  [StorableType("D23CCC08-AA7D-47C0-A465-FEAA16FECB80")]
  public abstract class AnalyticalPTSPMoveEvaluator : SingleSuccessorOperator, IAnalyticalPTSPMoveEvaluator {

    public override bool CanChangeName {
      get { return false; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<IProbabilisticTSPData> ProbabilisticTSPDataParameter {
      get { return (ILookupParameter<IProbabilisticTSPData>)Parameters["PTSP Data"]; }
    }

    [StorableConstructor]
    protected AnalyticalPTSPMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected AnalyticalPTSPMoveEvaluator(AnalyticalPTSPMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected AnalyticalPTSPMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a TSP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a TSP solution."));
      Parameters.Add(new LookupParameter<IProbabilisticTSPData>("PTSP Data", "The main parameters of the pTSP."));
    }

    public override IOperation Apply() {
      var permutation = PermutationParameter.ActualValue;
      var data = ProbabilisticTSPDataParameter.ActualValue;

      // here moves are not delta-evaluated
      var newQuality = EvaluateMove(permutation, data);
      var moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) MoveQualityParameter.ActualValue = new DoubleValue(newQuality);
      else moveQuality.Value = newQuality;

      return base.Apply();
    }

    protected abstract double EvaluateMove(Permutation tour, IProbabilisticTSPData data);
  }
}
