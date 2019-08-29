#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// An operator which creates new solutions. Evaluation of the new solutions is executed in parallel, if an engine is used which supports parallelization.
  /// </summary>
  [Item("SolutionsCreator", "An operator which creates new solutions. Evaluation of the new solutions is executed in parallel, if an engine is used which supports parallelization.")]
  [StorableType("3EE12E32-F8AF-4C7E-A9C9-B5DC5561CFE2")]
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
    public ValueLookupParameter<BoolValue> ParallelParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Parallel"]; }
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

    [StorableConstructor]
    private SolutionsCreator(StorableConstructorFlag _) : base(_) { }
    private SolutionsCreator(SolutionsCreator original, Cloner cloner) : base(original, cloner) { }
    public SolutionsCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>("NumberOfSolutions", "The number of solutions that should be created."));
      Parameters.Add(new ValueLookupParameter<IOperator>("SolutionCreator", "The operator which is used to create new solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator which is used to evaluate new solutions. This operator is executed in parallel, if an engine is used which supports parallelization."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Parallel", "True if the operator should be applied in parallel on all sub-scopes, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope to which the new solutions are added as sub-scopes."));      
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("Parallel")) Parameters.Add(new ValueLookupParameter<BoolValue>("Parallel", "True if the operator should be applied in parallel on all sub-scopes, otherwise false.", new BoolValue(true))); // backwards compatibility
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SolutionsCreator(this, cloner);
    }

    public override IOperation Apply() {
      int count = NumberOfSolutionsParameter.ActualValue.Value;
      IOperator creator = SolutionCreatorParameter.ActualValue;
      IOperator evaluator = EvaluatorParameter.ActualValue;
      bool parallel = ParallelParameter.ActualValue.Value;

      int current = CurrentScope.SubScopes.Count;
      for (int i = 0; i < count; i++)
        CurrentScope.SubScopes.Add(new Scope((current + i).ToString()));

      OperationCollection creation = new OperationCollection();
      OperationCollection evaluation = new OperationCollection() { Parallel = parallel };
      for (int i = 0; i < count; i++) {
        if (creator != null) creation.Add(ExecutionContext.CreateOperation(creator, CurrentScope.SubScopes[current + i]));
        if (evaluator != null) evaluation.Add(ExecutionContext.CreateOperation(evaluator, CurrentScope.SubScopes[current + i]));
      }
      OperationCollection next = new OperationCollection();
      next.Add(creation);
      next.Add(evaluation);
      next.Add(base.Apply());
      return next;
    }
  }
}
