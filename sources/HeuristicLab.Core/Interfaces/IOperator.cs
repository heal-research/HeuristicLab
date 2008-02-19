#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;

namespace HeuristicLab.Core {
  public interface IOperator : IConstrainedItem {
    string Name { get; set; }
    string Description { get; }

    bool Canceled { get; }
    bool Breakpoint { get; set; }

    IList<IOperator> SubOperators { get; }
    ICollection<IVariableInfo> VariableInfos { get; }
    ICollection<IVariable> Variables { get; }

    void AddSubOperator(IOperator op);
    bool TryAddSubOperator(IOperator op);
    bool TryAddSubOperator(IOperator op, out ICollection<IConstraint> violatedConstraints);
    void AddSubOperator(IOperator op, int index);
    bool TryAddSubOperator(IOperator op, int index);
    bool TryAddSubOperator(IOperator op, int index, out ICollection<IConstraint> violatedConstraints);
    void RemoveSubOperator(int index);
    bool TryRemoveSubOperator(int index);
    bool TryRemoveSubOperator(int index, out ICollection<IConstraint> violatedConstraints);

    IVariableInfo GetVariableInfo(string formalName);
    void AddVariableInfo(IVariableInfo variableInfo);
    bool TryAddVariableInfo(IVariableInfo variableInfo);
    bool TryAddVariableInfo(IVariableInfo variableInfo, out ICollection<IConstraint> violatedConstraints);
    void RemoveVariableInfo(string formalName);
    bool TryRemoveVariableInfo(string formalName);
    bool TryRemoveVariableInfo(string formalName, out ICollection<IConstraint> violatedConstraints);

    IVariable GetVariable(string name);
    void AddVariable(IVariable variable);
    bool TryAddVariable(IVariable variable);
    bool TryAddVariable(IVariable variable, out ICollection<IConstraint> violatedConstraints);
    void RemoveVariable(string name);
    bool TryRemoveVariable(string name);
    bool TryRemoveVariable(string name, out ICollection<IConstraint> violatedConstraints);
    T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup) where T : class, IItem;
    T GetVariableValue<T>(string formalName, IScope scope, bool recursiveLookup, bool throwOnError) where T : class, IItem;
    IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup);
    IItem GetVariableValue(string formalName, IScope scope, bool recursiveLookup, bool throwOnError);

    IOperation Execute(IScope scope);
    void Abort();
    void Reset();

    event EventHandler NameChanged;
    event EventHandler BreakpointChanged;
    event EventHandler<OperatorIndexEventArgs> SubOperatorAdded;
    event EventHandler<OperatorIndexEventArgs> SubOperatorRemoved;
    event EventHandler<VariableInfoEventArgs> VariableInfoAdded;
    event EventHandler<VariableInfoEventArgs> VariableInfoRemoved;
    event EventHandler<VariableEventArgs> VariableAdded;
    event EventHandler<VariableEventArgs> VariableRemoved;
    event EventHandler Initialized;
    event EventHandler Executed;
  }
}
