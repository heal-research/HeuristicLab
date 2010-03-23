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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("PreventFallBackToSkippedPositions", "Prevents falling back into ranges that have been moved over before.")]
  [StorableClass]
  public class PreventFallBackToSkippedPositions : SingleSuccessorOperator, IAdditiveRealVectorMoveOperator, ITabuMoveEvaluator {
    public ILookupParameter<AdditiveMove> AdditiveMoveParameter {
      get { return (LookupParameter<AdditiveMove>)Parameters["AdditiveMove"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (LookupParameter<RealVector>)Parameters["RealVector"]; }
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

    public PreventFallBackToSkippedPositions()
      : base() {
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The solution as real vector."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope."));
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      AdditiveMove move = AdditiveMoveParameter.ActualValue;
      RealVector vector = RealVectorParameter.ActualValue;
      bool isTabu = false;
      foreach (IItem tabuMove in tabuList) {
        AdditiveMoveTabuAttribute attribute = (tabuMove as AdditiveMoveTabuAttribute);
        if (attribute != null && attribute.Dimension == move.Dimension) {
          double newPos = vector[move.Dimension] + move.MoveDistance;
          if (Math.Min(attribute.MovedPosition, attribute.OriginalPosition) < newPos
            && newPos < Math.Max(attribute.MovedPosition, attribute.OriginalPosition)) {
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
