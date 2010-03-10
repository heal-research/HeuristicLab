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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Evolutionary {
  /// <summary>
  /// An operator which creates a new population of solutions.
  /// </summary>
  [Item("PopulationCreator", "An operator which creates a new population of solutions.")]
  [StorableClass(StorableClassType.Empty)]
  [Creatable("Test")]
  public sealed class PopulationCreator : SingleSuccessorOperator {
    public ValueLookupParameter<IntData> PopulationSizeParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["PopulationSize"]; }
    }
    public ValueLookupParameter<IOperator> SolutionCreatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["SolutionCreator"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    private ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }

    public PopulationCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntData>("PopulationSize", "The number of individuals that should be created."));
      Parameters.Add(new ValueLookupParameter<IOperator>("SolutionCreator", "The operator which is used to create new solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator which is used to evaluate new solutions."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope which represents the population."));
    }

    public override IOperation Apply() {
      int size = PopulationSizeParameter.ActualValue.Value;
      IOperator creator = SolutionCreatorParameter.ActualValue;
      IOperator evaluator = EvaluatorParameter.ActualValue;

      if (CurrentScope.SubScopes.Count > 0) throw new InvalidOperationException("Population is not empty. PopulationCreator cannot be applied on scopes which already contain sub-scopes.");

      for (int i = 0; i < size; i++)
        CurrentScope.SubScopes.Add(new Scope(i.ToString()));

      OperationCollection next = new OperationCollection();
      for (int i = 0; i < CurrentScope.SubScopes.Count; i++) {
        if (creator != null) next.Add(ExecutionContext.CreateOperation(creator, CurrentScope.SubScopes[i]));
        if (evaluator != null) next.Add(ExecutionContext.CreateOperation(evaluator, CurrentScope.SubScopes[i]));
      }
      next.Add(base.Apply());
      return next;
    }
  }
}
