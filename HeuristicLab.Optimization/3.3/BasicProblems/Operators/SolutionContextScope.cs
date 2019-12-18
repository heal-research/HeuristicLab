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


using System;
using HeuristicLab.Common;
using HeuristicLab.Core;

//TODO: don't derive from NamedItem, this has been done to have a fast working prototype
namespace HeuristicLab.Optimization {
  internal abstract class SolutionContextScope<TEncodedSolution> : SolutionContext<TEncodedSolution>, ISolutionScope
   where TEncodedSolution : class, IEncodedSolution {
    protected static string EVALUATION_RESULT_VARIABLE_NAME = "Evaluation Result";

    protected IScope Scope { get; }

    public SolutionContextScope(IScope scope, TEncodedSolution solution) : base(solution) {
      Scope = scope;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      throw new NotSupportedException();
    }

    IScope IScope.Parent {
      get { return Scope.Parent; }
      set { throw new NotSupportedException(); }
    }

    VariableCollection IScope.Variables => Scope.Variables;
    ScopeList IScope.SubScopes => Scope.SubScopes;

    void IScope.Clear() { throw new NotImplementedException(); }

    #region INamedItem members
    string INamedItem.Name { get { return Scope.Name; } set => throw new NotImplementedException(); }
    bool INamedItem.CanChangeName => false;

    string INamedItem.Description { get { return Scope.Description; } set => throw new NotImplementedException(); }
    bool INamedItem.CanChangeDescription => false;

    event EventHandler<CancelEventArgs<string>> INamedItem.NameChanging {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }

    event EventHandler INamedItem.NameChanged {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }

    event EventHandler INamedItem.DescriptionChanged {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }
    #endregion
  }

  internal class SingleObjectiveSolutionContextScope<TEncodedSolution> : SolutionContextScope<TEncodedSolution>, ISingleObjectiveSolutionScope<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {

    public new ISingleObjectiveEvaluationResult EvaluationResult {
      get {
        return (ISingleObjectiveEvaluationResult)Scope.Variables[EVALUATION_RESULT_VARIABLE_NAME].Value;
      }
      set {
        Scope.Variables.Remove(EVALUATION_RESULT_VARIABLE_NAME);
        Scope.Variables.Add(new Variable(EVALUATION_RESULT_VARIABLE_NAME, value));
      }
    }

    public SingleObjectiveSolutionContextScope(IScope scope, TEncodedSolution solution) : base(scope, solution) {
    }
  }

  internal class MultiObjectiveSolutionContextScope<TEncodedSolution> : SolutionContextScope<TEncodedSolution>, IMultiObjectiveSolutionScope<TEncodedSolution>
  where TEncodedSolution : class, IEncodedSolution {

    public new IMultiObjectiveEvaluationResult EvaluationResult {
      get {
        return (IMultiObjectiveEvaluationResult)Scope.Variables[EVALUATION_RESULT_VARIABLE_NAME].Value;
      }
      set {
        Scope.Variables.Remove(EVALUATION_RESULT_VARIABLE_NAME);
        Scope.Variables.Add(new Variable(EVALUATION_RESULT_VARIABLE_NAME, value));
      }
    }
    public MultiObjectiveSolutionContextScope(IScope scope, TEncodedSolution solution) : base(scope, solution) {
    }
  }
}
