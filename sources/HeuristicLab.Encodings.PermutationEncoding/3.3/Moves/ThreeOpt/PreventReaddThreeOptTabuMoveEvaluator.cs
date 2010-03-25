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
  [Item("ThreeOptPreventEdgeReadding", "Prevents readding of previously deleted edges.")]
  [StorableClass]
  public class PreventReaddThreeOptTabuMoveEvaluator : SingleSuccessorOperator, IThreeOptPermutationMoveOperator, ITabuMoveEvaluator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<ThreeOptMove> ThreeOptMoveParameter {
      get { return (LookupParameter<ThreeOptMove>)Parameters["ThreeOptMove"]; }
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

    public PreventReaddThreeOptTabuMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<ThreeOptMove>("ThreeOptMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope."));
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      ThreeOptMove move = ThreeOptMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      int length = permutation.Length;
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
      bool isTabu = (move.Index1 == move.Index3
        || move.Index2 - move.Index1 >= permutation.Length - 2
        || move.Index1 == permutation.Length - 1 && move.Index3 == 0
        || move.Index3 == permutation.Length - 1 && move.Index1 == 0);
      if (!isTabu) {
        foreach (IItem tabuMove in tabuList) {
          ThreeOptTabuMoveAttribute attribute = (tabuMove as ThreeOptTabuMoveAttribute);
          if (attribute != null) {
            // if previously deleted Edge1Source-Target is readded
            if (attribute.Edge1Source == E3S && attribute.Edge1Target == E1T || attribute.Edge1Source == E1T && attribute.Edge1Target == E3S
              || attribute.Edge1Source == E2S && attribute.Edge1Target == E3T || attribute.Edge1Source == E3T && attribute.Edge1Target == E2S
              || attribute.Edge1Source == E1S && attribute.Edge1Target == E2T || attribute.Edge1Source == E2T && attribute.Edge1Target == E1S
              // if previously deleted Edge2Source-Target is readded
              || attribute.Edge2Source == E3S && attribute.Edge2Target == E1T || attribute.Edge2Source == E1T && attribute.Edge2Target == E3S
              || attribute.Edge2Source == E2S && attribute.Edge2Target == E3T || attribute.Edge2Source == E3T && attribute.Edge2Target == E2S
              || attribute.Edge2Source == E1S && attribute.Edge2Target == E2T || attribute.Edge2Source == E2T && attribute.Edge2Target == E1S
              // if previously deleted Edge3Source-Target is readded
              || attribute.Edge3Source == E3S && attribute.Edge3Target == E1T || attribute.Edge3Source == E1T && attribute.Edge3Target == E3S
              || attribute.Edge3Source == E2S && attribute.Edge3Target == E3T || attribute.Edge3Source == E3T && attribute.Edge3Target == E2S
              || attribute.Edge3Source == E1S && attribute.Edge3Target == E2T || attribute.Edge3Source == E2T && attribute.Edge3Target == E1S) {
              isTabu = true;
              break;
            }
          }
        }
      }
      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }
  }
}
