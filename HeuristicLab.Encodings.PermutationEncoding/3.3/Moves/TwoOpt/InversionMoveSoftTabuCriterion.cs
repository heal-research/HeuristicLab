#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("InversionMoveSoftTabuCriterion", @"For relative postion encoded permutations it just prevents readding of previously deleted edges, but allows deleting previously added edges.
  For absolute position encoded permutations it prevents moving a number to a position it has previously occupied.

If the aspiration condition is activated, a move will not be considered tabu against a move in the tabu list if it leads to a better solution than the quality recorded with the move in the tabu list.")]
  [StorableClass]
  public class InversionMoveSoftTabuCriterion : SingleSuccessorOperator, IPermutationInversionMoveQualityOperator, ITabuChecker {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (LookupParameter<InversionMove>)Parameters["InversionMove"]; }
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
    protected InversionMoveSoftTabuCriterion(bool deserializing) : base(deserializing) { }
    protected InversionMoveSoftTabuCriterion(InversionMoveSoftTabuCriterion original, Cloner cloner) : base(original, cloner) { }
    public InversionMoveSoftTabuCriterion()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InversionMoveSoftTabuCriterion(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      InversionMove move = InversionMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      int length = permutation.Length;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      bool useAspiration = UseAspirationCriterion.Value;
      bool isTabu = false;

      foreach (IItem tabuMove in tabuList) {
        PermutationMoveAttribute attrib = (tabuMove as PermutationMoveAttribute);
        if (!useAspiration
          || maximization && moveQuality <= attrib.MoveQuality
          || !maximization && moveQuality >= attrib.MoveQuality) {
          switch (permutation.PermutationType) {
            case PermutationTypes.RelativeUndirected: {
                int E1S = permutation.GetCircular(move.Index1 - 1);
                int E1T = permutation[move.Index1];
                int E2S = permutation[move.Index2];
                int E2T = permutation.GetCircular(move.Index2 + 1);
                InversionMoveRelativeAttribute relAttrib = (attrib as InversionMoveRelativeAttribute);
                if (relAttrib != null) {
                  // if previously deleted Edge1Source-Target is readded
                  if (relAttrib.Edge1Source == E1S && relAttrib.Edge1Target == E2S || relAttrib.Edge1Source == E2S && relAttrib.Edge1Target == E1S
                    || relAttrib.Edge1Source == E1T && relAttrib.Edge1Target == E2T || relAttrib.Edge1Source == E2T && relAttrib.Edge1Target == E1T
                    // if previously deleted Edge2Source-Target is readded
                    || relAttrib.Edge2Source == E1T && relAttrib.Edge2Target == E2T || relAttrib.Edge2Source == E2T && relAttrib.Edge2Target == E1T
                    || relAttrib.Edge2Source == E1S && relAttrib.Edge2Target == E2S || relAttrib.Edge2Source == E2S && relAttrib.Edge2Target == E1S) {
                    isTabu = true;
                  }
                }
              }
              break;
            case PermutationTypes.RelativeDirected: {
                int E1S = permutation.GetCircular(move.Index1 - 1);
                int E1T = permutation[move.Index1];
                int E2S = permutation[move.Index2];
                int E2T = permutation.GetCircular(move.Index2 + 1);
                InversionMoveRelativeAttribute relAttrib = (attrib as InversionMoveRelativeAttribute);
                if (relAttrib != null) {
                  if (relAttrib.Edge1Source == E1S && relAttrib.Edge1Target == E2S
                    || relAttrib.Edge1Source == E1T && relAttrib.Edge1Target == E2T
                    // if previously deleted Edge2Source-Target is readded
                    || relAttrib.Edge2Source == E1T && relAttrib.Edge2Target == E2T
                    || relAttrib.Edge2Source == E1S && relAttrib.Edge2Target == E2S) {
                    isTabu = true;
                  }
                }
              }
              break;
            case PermutationTypes.Absolute: {
                int i1 = move.Index1;
                int n1 = permutation[move.Index1];
                int i2 = move.Index2;
                int n2 = permutation[move.Index2];
                InversionMoveAbsoluteAttribute absAttrib = (attrib as InversionMoveAbsoluteAttribute);
                if (absAttrib != null) {
                  if ((absAttrib.Index1 == i1 || absAttrib.Index1 == i2) && (absAttrib.Number1 == n1 || absAttrib.Number1 == n2)
                    || (absAttrib.Index2 == i2 || absAttrib.Index2 == i1) && (absAttrib.Number2 == n2 || absAttrib.Number2 == n1))
                    isTabu = true;

                }
              }
              break;
            default: {
                throw new InvalidOperationException(Name + ": Unknown permutation type.");
              }
          }
        }
        if (isTabu) break;
      }
      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }
  }
}
