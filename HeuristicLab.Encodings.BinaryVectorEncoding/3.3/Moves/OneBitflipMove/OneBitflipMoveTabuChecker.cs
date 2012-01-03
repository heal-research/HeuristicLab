#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("OneBitflipMoveTabuChecker", "Prevents peforming a one bitflip move again.")]
  [StorableClass]
  public class OneBitflipMoveTabuChecker : SingleSuccessorOperator, IOneBitflipMoveOperator, ITabuChecker {
    public override bool CanChangeName {
      get { return false; }
    }
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
    protected OneBitflipMoveTabuChecker(bool deserializing) : base(deserializing) { }
    protected OneBitflipMoveTabuChecker(OneBitflipMoveTabuChecker original, Cloner cloner) : base(original, cloner) { }
    public OneBitflipMoveTabuChecker()
      : base() {
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BoolValue>("MoveTabu", "The variable to store if a move was tabu."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The solution as permutation."));
      Parameters.Add(new LookupParameter<ItemList<IItem>>("TabuList", "The tabu list."));
      Parameters.Add(new ValueParameter<BoolValue>("UseAspirationCriterion", "Whether to use the aspiration criterion or not.", new BoolValue(true)));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, else if it is a minimization problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The quality of the current move."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneBitflipMoveTabuChecker(this, cloner);
    }

    public override IOperation Apply() {
      ItemList<IItem> tabuList = TabuListParameter.ActualValue;
      OneBitflipMove move = OneBitflipMoveParameter.ActualValue;
      double moveQuality = MoveQualityParameter.ActualValue.Value;
      bool maximization = MaximizationParameter.ActualValue.Value;
      bool useAspiration = UseAspirationCriterion.Value;
      bool isTabu = false;
      foreach (IItem tabuMove in tabuList) {
        OneBitflipMoveAttribute attribute = (tabuMove as OneBitflipMoveAttribute);
        if (attribute != null) {
          if (!useAspiration
            || maximization && moveQuality <= attribute.MoveQuality
            || !maximization && moveQuality >= attribute.MoveQuality) {
            if (attribute.Index == move.Index) {
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
