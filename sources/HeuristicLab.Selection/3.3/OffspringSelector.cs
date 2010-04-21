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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  [Item("OffspringSelector", "Selects among the offspring population those that are designated successful and discards the unsuccessful offspring, except for some lucky losers. It expects the parent scopes to be below the first sub-scope, and offspring scopes to be below the second sub-scope separated again in two sub-scopes, the first with the failed offspring and the second with successful offspring.")]
  [StorableClass]
  public class OffspringSelector : SingleSuccessorOperator {

    public ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public LookupParameter<DoubleValue> SelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SelectionPressure"]; }
    }
    public LookupParameter<DoubleValue> CurrentSuccessRatioParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["CurrentSuccessRatio"]; }
    }
    public LookupParameter<ItemList<IScope>> WinnersParameter {
      get { return (LookupParameter<ItemList<IScope>>)Parameters["Winners"]; }
    }
    public LookupParameter<ItemList<IScope>> LuckyLosersParameter {
      get { return (LookupParameter<ItemList<IScope>>)Parameters["LuckyLosers"]; }
    }
    public OperatorParameter OffspringCreatorParameter {
      get { return (OperatorParameter)Parameters["OffspringCreator"]; }
    }

    public IOperator OffspringCreator {
      get { return OffspringCreatorParameter.Value; }
      set { OffspringCreatorParameter.Value = value; }
    }

    public OffspringSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure which prematurely terminates the offspring selection step."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful offspring that has to be produced."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SelectionPressure", "The amount of selection pressure currently necessary to fulfill the success ratio."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentSuccessRatio", "The current success ratio indicates how much of the successful offspring have already been generated."));
      Parameters.Add(new LookupParameter<ItemList<IScope>>("Winners", "Temporary store of the successful offspring."));
      Parameters.Add(new LookupParameter<ItemList<IScope>>("LuckyLosers", "Temporary store of the lucky losers."));
      Parameters.Add(new OperatorParameter("OffspringCreator", "The operator used to create new offspring."));
    }

    public override IOperation Apply() {
      double maxSelPress = MaximumSelectionPressureParameter.ActualValue.Value;
      double successRatio = SuccessRatioParameter.ActualValue.Value;
      IScope scope = ExecutionContext.Scope;
      IScope parents = scope.SubScopes[0];
      IScope children = scope.SubScopes[1];
      int populationSize = parents.SubScopes.Count;

      // retrieve actual selection pressure and success ratio
      DoubleValue selectionPressure = SelectionPressureParameter.ActualValue;
      if (selectionPressure == null) {
        selectionPressure = new DoubleValue(0);
        SelectionPressureParameter.ActualValue = selectionPressure;
      }
      DoubleValue currentSuccessRatio = CurrentSuccessRatioParameter.ActualValue;
      if (currentSuccessRatio == null) {
        currentSuccessRatio = new DoubleValue(0);
        CurrentSuccessRatioParameter.ActualValue = currentSuccessRatio;
      }

      // retrieve winners and lucky losers
      ItemList<IScope> winners = WinnersParameter.ActualValue;
      if (winners == null) {
        winners = new ItemList<IScope>();
        WinnersParameter.ActualValue = winners;
        selectionPressure.Value = 0; // initialize selection pressure for this round
        currentSuccessRatio.Value = 0; // initialize current success ratio for this round
      }
      ItemList<IScope> luckyLosers = LuckyLosersParameter.ActualValue;
      if (luckyLosers == null) {
        luckyLosers = new ItemList<IScope>();
        LuckyLosersParameter.ActualValue = luckyLosers;
      }

      // separate new offspring in winners and lucky losers, the unlucky losers are discarded, sorry guys
      int winnersCount = 0;
      int losersCount = 0;
      ScopeList offspring = children.SubScopes[1].SubScopes; // the winners
      winnersCount += offspring.Count;
      winners.AddRange(offspring);
      offspring = children.SubScopes[0].SubScopes; // the losers
      while (offspring.Count > 0 && ((1 - successRatio) * populationSize > luckyLosers.Count ||
            selectionPressure.Value >= maxSelPress)) {
        luckyLosers.Add(offspring[0]);
        losersCount++;
        offspring.RemoveAt(0);
      }
      losersCount += offspring.Count;
      children.SubScopes.Clear();

      // calculate actual selection pressure and success ratio
      selectionPressure.Value += (winnersCount + losersCount) / ((double)populationSize);
      currentSuccessRatio.Value = winners.Count / ((double)populationSize);

      // check if enough children have been generated
      if (((selectionPressure.Value < maxSelPress) && (currentSuccessRatio.Value < successRatio)) ||
          ((winners.Count + luckyLosers.Count) < populationSize)) {
        // more children required -> reduce left and start children generation again
        scope.SubScopes.Remove(parents);
        scope.SubScopes.Remove(children);
        while(parents.SubScopes.Count > 0)
          scope.SubScopes.Add(parents.SubScopes[0]);

        IOperator moreOffspring = OffspringCreatorParameter.ActualValue as IOperator;
        if (moreOffspring == null) throw new InvalidOperationException(Name + ": More offspring are required, but no operator specified for creating them.");
        return ExecutionContext.CreateOperation(moreOffspring);
      } else {
        // enough children generated
        while (children.SubScopes.Count < populationSize) {
          if (winners.Count > 0) {
            children.SubScopes.Add((IScope)winners[0]);
            winners.RemoveAt(0);
          } else {
            children.SubScopes.Add((IScope)luckyLosers[0]);
            luckyLosers.RemoveAt(0);
          }
        }

        scope.Variables.Remove(WinnersParameter.ActualName);
        scope.Variables.Remove(LuckyLosersParameter.ActualName);
        return base.Apply();
      }
    }
  }
}
