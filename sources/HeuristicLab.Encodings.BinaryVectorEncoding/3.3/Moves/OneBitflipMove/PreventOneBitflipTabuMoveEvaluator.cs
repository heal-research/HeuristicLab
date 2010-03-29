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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("PreventOneBitflipTabuMoveEvaluator", "Prevents peforming a one bitflip move again.")]
  [StorableClass]
  public class PreventOneBitflipTabuMoveEvaluator : SingleSuccessorOperator, IOneBitflipMoveOperator, ITabuChecker {
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (LookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (LookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
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

    public PreventOneBitflipTabuMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope."));
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      OneBitflipMove move = OneBitflipMoveParameter.ActualValue;
      bool isTabu = false;
      foreach (IItem tabuMove in tabuList) {
        OneBitflipMove attribute = (tabuMove as OneBitflipMove);
        if (attribute != null) {
          if (attribute.Index == move.Index) {
            isTabu = true;
            break;
          }
        }
      }
      MoveTabuParameter.ActualValue = new BoolValue(isTabu);
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
