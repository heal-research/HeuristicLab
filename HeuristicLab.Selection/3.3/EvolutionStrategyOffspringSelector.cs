#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.InteropServices;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Selection {
  [Item("EvolutionStrategyOffspringSelector", "Selects among the offspring population those that are designated successful and discards the unsuccessful offspring, except for some lucky losers. It expects the parent scopes to be below the first sub-scope, and offspring scopes to be below the second sub-scope separated again in two sub-scopes, the first with the failed offspring and the second with successful offspring.")]
  [StorableClass]
  public class EvolutionStrategyOffspringSelector : SingleSuccessorOperator {

    private class FitnessComparer : IComparer<IScope> {

      #region IComparer<IScope> Member

      private String qualityParameterName;
      private bool maximization;

      public FitnessComparer(String qualityParamName, bool maximization) {
        this.qualityParameterName = qualityParamName;
        this.maximization = maximization;
      }

      // less than zero if x is-better-than y 
      // larger than zero if y is-better-than x
      // zero if both are the same
      public int Compare(IScope x, IScope y) {
        IVariable quality1, quality2;

        if (x.Variables.TryGetValue(qualityParameterName, out quality1)
          && y.Variables.TryGetValue(qualityParameterName, out quality2)) {
          DoubleValue left = quality1.Value as DoubleValue;
          DoubleValue right = quality2.Value as DoubleValue;
          var res = left.CompareTo(right);
          if (maximization) return -res; // in the maximization case the largest value should preceed all others in the sort order
          else return res;
        } else
          throw new ArgumentException("Quality variable " + qualityParameterName + " not found.");
      }

      #endregion
    }

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
    public LookupParameter<ItemList<IScope>> OffspringVirtualPopulationParameter {
      get { return (LookupParameter<ItemList<IScope>>)Parameters["OffspringVirtualPopulation"]; }
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
    public LookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }
    private LookupParameter<IntValue> MaximumEvaluatedSolutionsParameter {
      get { return (LookupParameter<IntValue>)Parameters["MaximumEvaluatedSolutions"]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public IOperator OffspringCreator {
      get { return OffspringCreatorParameter.Value; }
      set { OffspringCreatorParameter.Value = value; }
    }

    [StorableConstructor]
    protected EvolutionStrategyOffspringSelector(bool deserializing) : base(deserializing) { }
    protected EvolutionStrategyOffspringSelector(EvolutionStrategyOffspringSelector original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvolutionStrategyOffspringSelector(this, cloner);
    }
    public EvolutionStrategyOffspringSelector()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure which prematurely terminates the offspring selection step."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful offspring that has to be produced."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SelectionPressure", "The amount of selection pressure currently necessary to fulfill the success ratio."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentSuccessRatio", "The current success ratio indicates how much of the successful offspring have already been generated."));
      Parameters.Add(new LookupParameter<ItemList<IScope>>("OffspringPopulation", "Temporary store of the offspring population."));
      Parameters.Add(new LookupParameter<ItemList<IScope>>("OffspringVirtualPopulation", "Temporary store of the offspring population."));
      Parameters.Add(new LookupParameter<IntValue>("OffspringPopulationWinners", "Temporary store the number of successful offspring in the offspring population."));
      Parameters.Add(new ScopeTreeLookupParameter<BoolValue>("SuccessfulOffspring", "True if the offspring was more successful than its parents.", 2));
      Parameters.Add(new OperatorParameter("OffspringCreator", "The operator used to create new offspring."));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The number of solution evaluations."));
      Parameters.Add(new LookupParameter<IntValue>("MaximumEvaluatedSolutions", "The maximum number of evaluated solutions (approximately)."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a child"));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "Flag that indicates if the problem is a maximization problem"));
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
      ItemList<IScope> virtual_population = OffspringVirtualPopulationParameter.ActualValue;

      IntValue successfulOffspring;
      if (population == null) {
        population = new ItemList<IScope>();
        OffspringPopulationParameter.ActualValue = population;
        selectionPressure.Value = 0; // initialize selection pressure for this round
        currentSuccessRatio.Value = 0; // initialize current success ratio for this round
        successfulOffspring = new IntValue(0);
        OffspringPopulationWinnersParameter.ActualValue = successfulOffspring;

        virtual_population = new ItemList<IScope>();
        OffspringVirtualPopulationParameter.ActualValue = virtual_population;
      } else successfulOffspring = OffspringPopulationWinnersParameter.ActualValue;

      int successfulOffspringAdded = 0;

      // implement the ActualValue fetch here - otherwise the parent scope would also be included, given that there may be 1000 or more parents, this is quite unnecessary
      string tname = SuccessfulOffspringParameter.TranslatedName;
      double tmpSelPress = selectionPressure.Value;
      double tmpSelPressInc = 1.0 / populationSize;
      for (int i = 0; i < offspring.SubScopes.Count; i++) {
        // fetch value
        IVariable tmpVar;
        if (!offspring.SubScopes[i].Variables.TryGetValue(tname, out tmpVar)) throw new InvalidOperationException(Name + ": Could not determine if an offspring was successful or not.");
        BoolValue tmp = (tmpVar.Value as BoolValue);
        if (tmp == null) throw new InvalidOperationException(Name + ": The variable that indicates whether an offspring is successful or not must contain a BoolValue.");

        // add to population
        if (tmp.Value) {
          IScope currentOffspring = offspring.SubScopes[i];
          offspring.SubScopes.RemoveAt(i);
          i--; // next loop should continue with the subscope at index i which replaced currentOffspring
          population.Add(currentOffspring);
          successfulOffspringAdded++;
        } else {
          IScope currentOffspring = offspring.SubScopes[i];
          offspring.SubScopes.RemoveAt(i);
          i--;
          virtual_population.Add(currentOffspring); // add to losers pool
        }
        tmpSelPress += tmpSelPressInc;

        double tmpSuccessRatio = (successfulOffspring.Value + successfulOffspringAdded) / ((double)populationSize);
        if (tmpSuccessRatio >= successRatio && (population.Count + virtual_population.Count) >= populationSize)
          break;
      }
      successfulOffspring.Value += successfulOffspringAdded;

      // calculate actual selection pressure and success ratio
      selectionPressure.Value = tmpSelPress;
      currentSuccessRatio.Value = successfulOffspring.Value / ((double)populationSize);

      // check if enough children have been generated (or limit of selection pressure or evaluted solutions is reached)
      if (((EvaluatedSolutionsParameter.ActualValue.Value < MaximumEvaluatedSolutionsParameter.ActualValue.Value)
          && (selectionPressure.Value < maxSelPress)
          && (currentSuccessRatio.Value < successRatio))
        || ((population.Count + virtual_population.Count) < populationSize)) {
        // more children required -> reduce left and start children generation again
        scope.SubScopes.Remove(parents);
        scope.SubScopes.Remove(offspring);
        while (parents.SubScopes.Count > 0) {
          IScope parent = parents.SubScopes[0];
          parents.SubScopes.RemoveAt(0); // TODO: repeated call of RemoveAt(0) is inefficient?
          scope.SubScopes.Add(parent); // order of subscopes is reversed
        }

        IOperator offspringCreator = OffspringCreatorParameter.ActualValue as IOperator;
        if (offspringCreator == null) throw new InvalidOperationException(Name + ": More offspring are required, but no operator specified for creating them.");
        return ExecutionContext.CreateOperation(offspringCreator); // this assumes that this operator will be called again indirectly or directly
      } else {
        // enough children generated
        var fitnessComparer = new FitnessComparer(QualityParameter.TranslatedName, MaximizationParameter.ActualValue.Value);
        population.Sort(fitnessComparer); // sort individuals by descending fitness

        // keeps only the best successRatio * populationSize solutions in the population (the remaining ones are added to the virtual population)
        int removed = 0;
        for (int i = 0; i < population.Count; i++) {
          double tmpSuccessRatio = i / (double)populationSize;
          if (tmpSuccessRatio > successRatio) {
            virtual_population.Add(population[i]);
            removed++;
          }
        }
        population.RemoveRange(population.Count - removed, removed);

        //fill up population with best remaining children (successful or unsuccessful)
        virtual_population.Sort(fitnessComparer);
        int offspringNeeded = populationSize - population.Count;
        for (int i = 0; i < offspringNeeded && i < virtual_population.Count; i++) {
          population.Add(virtual_population[i]); // children are sorted by descending fitness
        }

        offspring.SubScopes.Clear();
        offspring.SubScopes.AddRange(population);

        scope.Variables.Remove(OffspringPopulationParameter.TranslatedName);
        scope.Variables.Remove(OffspringVirtualPopulationParameter.TranslatedName);
        scope.Variables.Remove(OffspringPopulationWinnersParameter.TranslatedName);
        return base.Apply();
      }
    }
  }
}
