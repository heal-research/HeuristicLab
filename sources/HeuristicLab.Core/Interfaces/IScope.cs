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
  /// <summary>
  /// Interface for a hierarchical container of variables (containing variables and subscopes). 
  /// </summary>
  public interface IScope : IItem {
    /// <summary>
    /// Gets the name of the current instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the varibles of the current scope.
    /// </summary>
    ICollection<IVariable> Variables { get; }
    /// <summary>
    /// Gets all aliases of the current scope.
    /// </summary>
    IEnumerable<KeyValuePair<string, string>> Aliases { get; }
    /// <summary>
    /// Gets all subscopes in the current scope.
    /// </summary>
    IList<IScope> SubScopes { get; }

    /// <summary>
    /// Sets the parent scope for the current instance.
    /// </summary>
    /// <param name="scope">The parent scope of the current instance.</param>
    void SetParent(IScope scope);

    /// <summary>
    /// Gets a variable with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The variable with the specified name.</returns>
    IVariable GetVariable(string name);
    /// <summary>
    /// Adds the specified variable to the curent instance.
    /// </summary>
    /// <param name="variable">The variable to add.</param>
    void AddVariable(IVariable variable);
    /// <summary>
    /// Deletes a variable with the specified <paramref name="name"/> from the current instance.
    /// </summary>
    /// <param name="name">The name of the variable to delete.</param>
    void RemoveVariable(string name);
    
    /// <inheritdoc cref="GetVariableValue(string, bool)"/>
    /// <typeparam name="T">The type of the value that is searched.</typeparam>
    T GetVariableValue<T>(string name, bool recursiveLookup) where T : class, IItem;
    /// <inheritdoc cref="GetVariableValue(string, bool, bool)"/>
    /// <typeparam name="T">The type of the value that is searched.</typeparam>
    T GetVariableValue<T>(string name, bool recursiveLookup, bool throwOnError) where T : class, IItem;
    /// <inheritdoc cref="GetVariableValue(string, bool, bool)" select="summary"/>
    /// <param name="name">The name of the variable.</param>
    /// <param name="recursiveLookup">Boolean value, whether the parent scopes shall be searched
    /// when the variable is not found in the current instance.</param>
    /// <returns>The value of the variable or <c>null</c> if it was not found.</returns>
    IItem GetVariableValue(string name, bool recursiveLookup);
    /// <summary>
    /// Gets the value of the variable with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="recursiveLookup">Boolean value, whether the parent scopes shall be searched
    /// when the variable is not found in the current instance.</param>
    /// <param name="throwOnError">Boolean value, whether an exception shall be thrown when the searched 
    /// variable cannot be found, or only <c>null</c> shall be returned.</param>
    /// <returns>The value of the searched variable or <c>null</c> if <paramref name="throwOnError"/>
    /// is set to <c>false</c>.</returns>
    IItem GetVariableValue(string name, bool recursiveLookup, bool throwOnError);

    /// <summary>
    /// Gets the actual name the given alias.
    /// </summary>
    /// <param name="name">The alias whose actual name is searched.</param>
    /// <returns>The actual name.</returns>
    string TranslateName(string name);
    /// <summary>
    /// Adds an alias to the current instance. 
    /// </summary>
    /// <param name="alias">The alias to add.</param>
    /// <param name="name">The actual name of the alias.</param>
    void AddAlias(string alias, string name);
    /// <summary>
    /// Deletes the specified <paramref name="alias"/> from the current instance.
    /// </summary>
    /// <param name="alias">The alias to delete.</param>
    void RemoveAlias(string alias);

    /// <summary>
    /// Adds the specified sub scope to the current instance.
    /// </summary>
    /// <param name="scope">The sub scope to add.</param>
    void AddSubScope(IScope scope);
    /// <summary>
    /// Deletes the specified sub scope from the current instance.
    /// </summary>
    /// <param name="scope">The sub scope to delete.</param>
    void RemoveSubScope(IScope scope);
    /// <summary>
    /// Reorders all sub scopes according to the specified chronology.
    /// </summary>
    /// <param name="sequence">The chronology how to order the sub scopes.</param>
    void ReorderSubScopes(int[] sequence);
    /// <summary>
    /// Gets the sub scope with the given <paramref name="guid"/>.
    /// </summary>
    /// <param name="guid">The unique identifier of the sub scope.</param>
    /// <returns>The sub scope with the given <paramref name="guid"/>.</returns>
    IScope GetScope(Guid guid);
    /// <summary>
    /// Gets the sub scope with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the sub scope.</param>
    /// <returns>The sub scope with the given <paramref name="name"/>.</returns>
    IScope GetScope(string name);

    /// <summary>
    /// Clears the current instance.
    /// </summary>
    void Clear();

    /// <summary>
    /// Occurs when a variable has been added to the current instance.
    /// </summary>
    event EventHandler<VariableEventArgs> VariableAdded;
    /// <summary>
    /// Occurs when a variable has been deleted from the current instance.
    /// </summary>
    event EventHandler<VariableEventArgs> VariableRemoved;
    /// <summary>
    /// Occurs when an alias has been added to the current instance.
    /// </summary>
    event EventHandler<AliasEventArgs> AliasAdded;
    /// <summary>
    /// Occurs when an alias has been removed from the current instance.
    /// </summary>
    event EventHandler<AliasEventArgs> AliasRemoved;
    /// <summary>
    /// Occurs when a sub scope has been added to the current instance.
    /// </summary>
    event EventHandler<ScopeIndexEventArgs> SubScopeAdded;
    /// <summary>
    /// Occurs when a sub scope has been deleted from the current instance.
    /// </summary>
    event EventHandler<ScopeIndexEventArgs> SubScopeRemoved;
    /// <summary>
    /// Occurs when the sub scopes have been reordered.
    /// </summary>
    event EventHandler SubScopesReordered;
  }
}
