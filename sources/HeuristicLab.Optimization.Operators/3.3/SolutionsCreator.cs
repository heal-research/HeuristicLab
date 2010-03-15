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

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An operator which creates new solutions.
  /// </summary>
  [Item("SolutionsCreator", "An operator which creates new solutions.")]
  [StorableClass]
  [Creatable("Test")]
  public sealed class SolutionsCreator : SingleSuccessorOperator {
    public ValueLookupParameter<IntValue> NumberOfSolutionsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["NumberOfSolutions"]; }
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
    public IntValue NumberOfSolutions {
      get { return NumberOfSolutionsParameter.Value; }
      set { NumberOfSolutionsParameter.Value = value; }
    }

    public SolutionsCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSolutions", "The number of solutions that should be created."));
      Parameters.Add(new ValueLookupParameter<IOperator>("SolutionCreator", "The operator which is used to create new solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator which is used to evaluate new solutions."));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope to which the new solutions are added as sub-scopes."));
    }

    public override IOperation Apply() {
      int count = NumberOfSolutionsParameter.ActualValue.Value;
      IOperator creator = SolutionCreatorParameter.ActualValue;
      IOperator evaluator = EvaluatorParameter.ActualValue;

      int current = CurrentScope.SubScopes.Count;
      for (int i = 0; i < count; i++)
        CurrentScope.SubScopes.Add(new Scope((current + i).ToString()));

      OperationCollection next = new OperationCollection();
      for (int i = 0; i < count; i++) {
        if (creator != null) next.Add(ExecutionContext.CreateOperation(creator, CurrentScope.SubScopes[current + i]));
        if (evaluator != null) next.Add(ExecutionContext.CreateOperation(evaluator, CurrentScope.SubScopes[current + i]));
      }
      next.Add(base.Apply());
      return next;
    }
  }
}
