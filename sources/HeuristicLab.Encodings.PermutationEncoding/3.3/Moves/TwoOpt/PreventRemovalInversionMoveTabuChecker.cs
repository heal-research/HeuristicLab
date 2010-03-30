#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("PreventRemovalInversionMoveTabuChecker", "Prevents deleting of previously added edges.")]
  [StorableClass]
  public class PreventRemovalInversionMoveTabuChecker : SingleSuccessorOperator, IPermutationInversionMoveOperator, ITabuChecker {
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
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }

    public PreventRemovalInversionMoveTabuChecker()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope."));
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      InversionMove move = InversionMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      int length = permutation.Length;
      bool isTabu = false;
      foreach (IItem tabuMove in tabuList) {
        switch (permutation.PermutationType) {
          case PermutationTypes.RelativeUndirected: {
              int E1S = permutation.GetCircular(move.Index1 - 1);
              int E1T = permutation[move.Index1];
              int E2S = permutation[move.Index2];
              int E2T = permutation.GetCircular(move.Index2 + 1);
              InversionMoveRelativeAttribute relAttrib = (tabuMove as InversionMoveRelativeAttribute);
              if (relAttrib != null) {
                if (relAttrib.Edge1Source == E1S && relAttrib.Edge2Source == E1T || relAttrib.Edge1Source == E1T && relAttrib.Edge2Source == E1S
                  || relAttrib.Edge1Source == E2S && relAttrib.Edge2Source == E2T || relAttrib.Edge1Source == E2T && relAttrib.Edge2Source == E2S
                  // if previously added Edge1Target-Edge2Target is deleted
                  || relAttrib.Edge1Target == E2S && relAttrib.Edge2Target == E2T || relAttrib.Edge1Target == E2T && relAttrib.Edge2Target == E2S
                  || relAttrib.Edge1Target == E1S && relAttrib.Edge2Target == E1T || relAttrib.Edge1Target == E1T && relAttrib.Edge2Target == E1S) {
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
              InversionMoveRelativeAttribute relAttrib = (tabuMove as InversionMoveRelativeAttribute);
              if (relAttrib != null) {
                if (relAttrib.Edge1Source == E1S && relAttrib.Edge2Source == E1T
                  || relAttrib.Edge1Source == E2S && relAttrib.Edge2Source == E2T
                  // if previously added Edge1Target-Edge2Target is deleted
                  || relAttrib.Edge1Target == E2S && relAttrib.Edge2Target == E2T
                  || relAttrib.Edge1Target == E1S && relAttrib.Edge2Target == E1T) {
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
              InversionMoveAbsoluteAttribute absAttrib = (tabuMove as InversionMoveAbsoluteAttribute);
              if (absAttrib != null) {
                if (absAttrib.Number1 == n1 || absAttrib.Number1 == n2
                  || absAttrib.Number2 == n2 || absAttrib.Number2 == n1)
                  isTabu = true;

              }
            }
            break;
          default: {
              throw new InvalidOperationException(Name + ": Unknown permutation type.");
            }
        }
        if (isTabu) break;
      }
      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }
  }
}
