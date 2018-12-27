#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("AdditiveMoveTabuChecker", "Prevents falling back into ranges that have been moved over before.")]
  [StorableClass]
  public class AdditiveMoveTabuChecker : SingleSuccessorOperator, IAdditiveRealVectorMoveOperator, ITabuChecker {
    public override bool CanChangeName {
      get { return false; }
    }
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
    protected AdditiveMoveTabuChecker(bool deserializing) : base(deserializing) { }
    protected AdditiveMoveTabuChecker(AdditiveMoveTabuChecker original, Cloner cloner) : base(original, cloner) { }
    public AdditiveMoveTabuChecker()
      : base() {
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The solution as real vector."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdditiveMoveTabuChecker(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      AdditiveMove move = AdditiveMoveParameter.ActualValue;
      RealVector vector = RealVectorParameter.ActualValue;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      bool useAspiration = UseAspirationCriterion.Value;
      bool isTabu = false;
      foreach (IItem tabuMove in tabuList) {
        AdditiveMoveTabuAttribute attribute = (tabuMove as AdditiveMoveTabuAttribute);
        if (attribute != null && attribute.Dimension == move.Dimension) {
          if (!useAspiration
            || maximization && moveQuality <= attribute.MoveQuality
            || !maximization && moveQuality >= attribute.MoveQuality) {

            double newPos = vector[move.Dimension] + move.MoveDistance;
            if (Math.Min(attribute.MovedPosition, attribute.OriginalPosition) < newPos
              && newPos < Math.Max(attribute.MovedPosition, attribute.OriginalPosition)) {
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
