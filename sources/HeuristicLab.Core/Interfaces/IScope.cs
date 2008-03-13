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
using System.Xml;

namespace HeuristicLab.Core {
  public interface IScope : IItem {
    string Name { get; }

    ICollection<IVariable> Variables { get; }
    ICollection<string> Aliases { get; }
    IList<IScope> SubScopes { get; }

    void SetParent(IScope scope);

    IVariable GetVariable(string name);
    void AddVariable(IVariable variable);
    void RemoveVariable(string name);
    T GetVariableValue<T>(string name, bool recursiveLookup) where T : class, IItem;
    T GetVariableValue<T>(string name, bool recursiveLookup, bool throwOnError) where T : class, IItem;
    IItem GetVariableValue(string name, bool recursiveLookup);
    IItem GetVariableValue(string name, bool recursiveLookup, bool throwOnError);

    string TranslateName(string name);
    void AddAlias(string alias, string name);
    void RemoveAlias(string alias);

    void AddSubScope(IScope scope);
    void RemoveSubScope(IScope scope);
    void ReorderSubScopes(int[] sequence);
    IScope GetScope(Guid guid);
    IScope GetScope(string name);

    void Clear();

    event EventHandler<VariableEventArgs> VariableAdded;
    event EventHandler<VariableEventArgs> VariableRemoved;
    event EventHandler<AliasEventArgs> AliasAdded;
    event EventHandler<AliasEventArgs> AliasRemoved;
    event EventHandler<ScopeIndexEventArgs> SubScopeAdded;
    event EventHandler<ScopeIndexEventArgs> SubScopeRemoved;
    event EventHandler SubScopesReordered;
  }
}
