#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public LookupParameter<ItemList<IScope>> OffspringPopulationParameter {
      get { return (LookupParameter<ItemList<IScope>>)Parameters["OffspringPopulation"]; }
    }
    public LookupParameter<IntValue> OffspringPopulationWinnersParameter {
      get { return (LookupParameter<IntValue>)Parameters["OffspringPopulationWinners"]; }
    }
    public ScopeTreeLookupParameter<BoolValue> SuccessfulOffspringParameter {
      get { return (ScopeTreeLookupParameter<BoolValue>)Parameters["SuccessfulOffspring"]; }
    }
    public OperatorParameter OffspringCreatorParameter {
      get { return (OperatorParameter)Parameters["OffspringCreator"]; }
    }

    public IOperator OffspringCreator {
      get { return OffspringCreatorParameter.Value; }
      set { OffspringCreatorParameter.Value = value; }
    }

    [StorableConstructor]
    protected OffspringSelector(bool deserializing) : base(deserializing) { }
    protected OffspringSelector(OffspringSelector original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OffspringSelector(this, cloner);
    }
    public OffspringSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure which prematurely terminates the offspring selection step."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful offspring that has to be produced."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SelectionPressure", "The amount of selection pressure currently necessary to fulfill the success ratio."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentSuccessRatio", "The current success ratio indicates how much of the successful offspring have already been generated."));
      Parameters.Add(new LookupParameter<ItemList<IScope>>("OffspringPopulation", "Temporary store of the offspring population."));
      Parameters.Add(new LookupParameter<IntValue>("OffspringPopulationWinners", "Temporary store the number of successful offspring in the offspring population."));
      Parameters.Add(new ScopeTreeLookupParameter<BoolValue>("SuccessfulOffspring", "True if the offspring was more successful than its parents.", 2));
      Parameters.Add(new OperatorParameter("OffspringCreator", "The operator used to create new offspring."));
    }

    public override IOperation Apply() {
      double maxSelPress = MaximumSelectionPressureParameter.ActualValue.Value;
      double successRatio = SuccessRatioParameter.ActualValue.Value;
      IScope scope = ExecutionContext.Scope;
      IScope parents = scope.SubScopes[0];
      IScope offspring = scope.SubScopes[1];
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

      // retrieve next population
      ItemList<IScope> population = OffspringPopulationParameter.ActualValue;
      IntValue successfulOffspring;
      if (population == null) {
        population = new ItemList<IScope>();
        OffspringPopulationParameter.ActualValue = population;
        selectionPressure.Value = 0; // initialize selection pressure for this round
        currentSuccessRatio.Value = 0; // initialize current success ratio for this round
        successfulOffspring = new IntValue(0);
        OffspringPopulationWinnersParameter.ActualValue = successfulOffspring;
      } else successfulOffspring = OffspringPopulationWinnersParameter.ActualValue;

      int worseOffspringNeeded = (int)((1 - successRatio) * populationSize) - (population.Count - successfulOffspring.Value);
      int successfulOffspringAdded = 0;

      // implement the ActualValue fetch here - otherwise the parent scope would also be included, given that there may be 1000 or more parents, this is quite unnecessary
      string tname = SuccessfulOffspringParameter.TranslatedName;
      double tmpSelPress = selectionPressure.Value, tmpSelPressInc = 1.0 / populationSize;
      for (int i = 0; i < offspring.SubScopes.Count; i++) {
        // fetch value
        IVariable tmpVar;
        if (!offspring.SubScopes[i].Variables.TryGetValue(tname, out tmpVar)) throw new InvalidOperationException(Name + ": Could not determine if an offspring was successful or not.");
        BoolValue tmp = (tmpVar.Value as BoolValue);
        if (tmp == null) throw new InvalidOperationException(Name + ": The variable that indicates whether an offspring is successful or not must contain a BoolValue.");

        // add to population
        if (tmp.Value) {
          IScope currentOffspring = offspring.SubScopes[i];
          offspring.SubScopes.Remove(currentOffspring);
          i--;
          population.Add(currentOffspring);
          successfulOffspringAdded++;
        } else if (worseOffspringNeeded > 0 || tmpSelPress >= maxSelPress) {
          IScope currentOffspring = offspring.SubScopes[i];
          offspring.SubScopes.Remove(currentOffspring);
          i--;
          population.Add(currentOffspring);
          worseOffspringNeeded--;
        }
        tmpSelPress += tmpSelPressInc;
        if (population.Count == populationSize) break;
      }
      successfulOffspring.Value += successfulOffspringAdded;

      // calculate actual selection pressure and success ratio
      selectionPressure.Value = tmpSelPress;
      currentSuccessRatio.Value = successfulOffspring.Value / ((double)populationSize);

      // check if enough children have been generated
      if (((selectionPressure.Value < maxSelPress) && (currentSuccessRatio.Value < successRatio)) ||
          (population.Count < populationSize)) {
        // more children required -> reduce left and start children generation again
        scope.SubScopes.Remove(parents);
        scope.SubScopes.Remove(offspring);
        while (parents.SubScopes.Count > 0) {
          IScope parent = parents.SubScopes[0];
          parents.SubScopes.RemoveAt(0);
          scope.SubScopes.Add(parent);
        }

        IOperator moreOffspring = OffspringCreatorParameter.ActualValue as IOperator;
        if (moreOffspring == null) throw new InvalidOperationException(Name + ": More offspring are required, but no operator specified for creating them.");
        return ExecutionContext.CreateOperation(moreOffspring);
      } else {
        // enough children generated
        offspring.SubScopes.Clear();
        offspring.SubScopes.AddRange(population);

        scope.Variables.Remove(OffspringPopulationParameter.TranslatedName);
        scope.Variables.Remove(OffspringPopulationWinnersParameter.TranslatedName);
        return base.Apply();
      }
    }
  }
}
