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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("TranslocationMoveHardTabuCriterion", @"For relative postion encoded permutations it prevents readding of previously deleted edges as well as deleting previously added edges.
  For absolute position encoded permutations it prevents moving a number if it was previously moved.

If the aspiration condition is activated, a move will not be considered tabu against a move in the tabu list if it leads to a better solution than the quality recorded with the move in the tabu list.")]
  [StorableClass]
  public class TranslocationMoveHardTabuCriterion : SingleSuccessorOperator, IPermutationTranslocationMoveOperator, ITabuChecker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (LookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (LookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<ItemList<IItem>> TabuListParameter {
      get { return (ILookupParameter<ItemList<IItem>>)Parameters["TabuList"]; }
    }
    public ILookupParameter<BoolValue> MoveTabuParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["MoveTabu"]; }
    }
    public IValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ValueParameter<BoolValue> UseAspirationCriterionParameter {
      get { return (ValueParameter<BoolValue>)Parameters["UseAspirationCriterion"]; }
    }

    public BoolValue UseAspirationCriterion {
      get { return UseAspirationCriterionParameter.Value; }
      set { UseAspirationCriterionParameter.Value = value; }
    }

    [StorableConstructor]
    protected TranslocationMoveHardTabuCriterion(bool deserializing) : base(deserializing) { }
    protected TranslocationMoveHardTabuCriterion(TranslocationMoveHardTabuCriterion original, Cloner cloner) : base(original, cloner) { }
    public TranslocationMoveHardTabuCriterion()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationMoveHardTabuCriterion(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      TranslocationMove move = TranslocationMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      int length = permutation.Length;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      bool useAspiration = UseAspirationCriterion.Value;
      bool isTabu = false;

      if (permutation.PermutationType == PermutationTypes.Absolute) {
        int count = move.Index2 - move.Index1 + 1;
        int[] numbers = new int[count];
        for (int i = move.Index1; i <= move.Index2; i++)
          numbers[i - move.Index1] = permutation[i];

        foreach (IItem tabuMove in tabuList) {
          TranslocationMoveAbsoluteAttribute attribute = (tabuMove as TranslocationMoveAbsoluteAttribute);
          if (attribute != null) {
            if (!useAspiration
              || maximization && moveQuality <= attribute.MoveQuality
              || !maximization && moveQuality >= attribute.MoveQuality) { // if the move quality is improving beyond what was recorded when the move in the tabu list was recorded the move is regarded as okay

              for (int i = 0; i < count; i++) {
                for (int j = 0; j < attribute.Number.Length; j++) {
                  if (attribute.Number[j] == numbers[i]) {
                    isTabu = true;
                    break;
                  }
                }
                if (isTabu) break;
              }
            }
          }
          if (isTabu) break;
        }
      } else {
        int E1S = permutation.GetCircular(move.Index1 - 1);
        int E1T = permutation[move.Index1];
        int E2S = permutation[move.Index2];
        int E2T = permutation.GetCircular(move.Index2 + 1);
        int E3S, E3T;
        if (move.Index3 > move.Index1) {
          E3S = permutation.GetCircular(move.Index3 + move.Index2 - move.Index1);
          E3T = permutation.GetCircular(move.Index3 + move.Index2 - move.Index1 + 1);
        } else {
          E3S = permutation.GetCircular(move.Index3 - 1);
          E3T = permutation[move.Index3];
        }
        foreach (IItem tabuMove in tabuList) {
          TranslocationMoveRelativeAttribute attribute = (tabuMove as TranslocationMoveRelativeAttribute);
          if (attribute != null) {
            if (!useAspiration
              || maximization && moveQuality <= attribute.MoveQuality
              || !maximization && moveQuality >= attribute.MoveQuality) {

              if (permutation.PermutationType == PermutationTypes.RelativeUndirected) {
                if (// if previously added Edge3Source-Edge1Target is deleted
                  attribute.Edge3Source == E1S && attribute.Edge1Target == E1T || attribute.Edge3Source == E1T && attribute.Edge1Target == E1S
                  || attribute.Edge3Source == E2S && attribute.Edge1Target == E2T || attribute.Edge3Source == E2T && attribute.Edge1Target == E2S
                  || attribute.Edge3Source == E3S && attribute.Edge1Target == E3T || attribute.Edge3Source == E3T && attribute.Edge1Target == E3S
                  // if previously added Edge2Source-Edge3Target is deleted
                  || attribute.Edge2Source == E1S && attribute.Edge3Target == E1T || attribute.Edge2Source == E1T && attribute.Edge3Target == E1S
                  || attribute.Edge2Source == E2S && attribute.Edge3Target == E2T || attribute.Edge2Source == E2T && attribute.Edge3Target == E2S
                  || attribute.Edge2Source == E3S && attribute.Edge3Target == E3T || attribute.Edge2Source == E3T && attribute.Edge3Target == E3S
                  // if previously added Edge1Source-Edge2Target is deleted
                  || attribute.Edge1Source == E1S && attribute.Edge2Target == E1T || attribute.Edge1Source == E1T && attribute.Edge2Target == E1S
                  || attribute.Edge1Source == E2S && attribute.Edge2Target == E2T || attribute.Edge1Source == E2T && attribute.Edge2Target == E2S
                  || attribute.Edge1Source == E3S && attribute.Edge2Target == E3T || attribute.Edge1Source == E3T && attribute.Edge2Target == E3S) {
                  isTabu = true;
                  break;
                }
              } else {
                if (// if previously added Edge3Source-Edge1Target is deleted
                  attribute.Edge3Source == E1S && attribute.Edge1Target == E1T
                  || attribute.Edge3Source == E2S && attribute.Edge1Target == E2T
                  || attribute.Edge3Source == E3S && attribute.Edge1Target == E3T
                  // if previously added Edge2Source-Edge3Target is deleted
                  || attribute.Edge2Source == E1S && attribute.Edge3Target == E1T
                  || attribute.Edge2Source == E2S && attribute.Edge3Target == E2T
                  || attribute.Edge2Source == E3S && attribute.Edge3Target == E3T
                  // if previously added Edge1Source-Edge2Target is deleted
                  || attribute.Edge1Source == E1S && attribute.Edge2Target == E1T
                  || attribute.Edge1Source == E2S && attribute.Edge2Target == E2T
                  || attribute.Edge1Source == E3S && attribute.Edge2Target == E3T) {
                  isTabu = true;
                  break;
                }
              }
            }
          }
        }
      }
      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }
  }
}
