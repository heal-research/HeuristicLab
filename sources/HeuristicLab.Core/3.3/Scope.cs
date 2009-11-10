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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Core {
  /// <summary>
  /// Hierarchical container of variables (and of subscopes).
  /// </summary>
  public class Scope : ItemBase, IScope {

    [Storable]
    private IScope parent;

    [Storable]
    private string myName;
    /// <summary>
    /// Gets the name of the current scope.
    /// </summary>
    public string Name {
      get { return myName; }
    }
    
    private IDictionary<string, IVariable> myVariables;
    /// <inheritdoc/>    
    public ICollection<IVariable> Variables {
      get { return myVariables.Values; }
    }

    [Storable(Name="Variables")]
    private List<IVariable> VariablePersistence {
      get { return new List<IVariable>(myVariables.Values); }
      set {
        myVariables.Clear();
        foreach (IVariable var in value) {
          AddVariable(var);
        }
      }
    }
    
    [Storable]
    private IDictionary<string, string> myAliases;
    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> Aliases {
      get { return myAliases; }
    }

    [Storable]
    private List<IScope> mySubScopes;
    /// <summary>
    /// Gets all subscopes of the current instance.
    /// <note type="caution"> The subscopes are returned as read-only.</note>
    /// </summary>
    public IList<IScope> SubScopes {
      get { return mySubScopes.AsReadOnly(); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> having "Anonymous" as default name.
    /// </summary>
    public Scope() {
      myName = "Anonymous";
      myVariables = new Dictionary<string, IVariable>();
      myAliases = new Dictionary<string, string>();
      mySubScopes = new List<IScope>();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the scope.</param>
    public Scope(string name)
      : this() {
      if (name != null) myName = name;
    }

    /// <inheritdoc/>
    public void SetParent(IScope scope) {
      parent = scope;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ScopeView"/> to represent the current instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="ScopeView"/>.</returns>
    public override IView CreateView() {
      return new ScopeView(this);
    }

    /// <inheritdoc/>
    public IVariable GetVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable))
        return variable;
      else
        return null;
    }
    /// <inheritdoc/>
    public void AddVariable(IVariable variable) {
      myVariables.Add(variable.Name, variable);
      variable.NameChanging += new EventHandler<CancelEventArgs<string>>(Variable_NameChanging);
      variable.NameChanged += new EventHandler(Variable_NameChanged);
      OnVariableAdded(variable);
    }

    /// <inheritdoc/>
    public void RemoveVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        variable.NameChanging -= new EventHandler<CancelEventArgs<string>>(Variable_NameChanging);
        variable.NameChanged -= new EventHandler(Variable_NameChanged);
        myVariables.Remove(name);
        OnVariableRemoved(variable);
      }
    }
    private void Variable_NameChanging(object sender, CancelEventArgs<string> e) {
      e.Cancel = myVariables.ContainsKey(e.Value);
    }
    private void Variable_NameChanged(object sender, EventArgs e) {
      IVariable variable = (IVariable)sender;
      string oldName = null;
      foreach (KeyValuePair<string, IVariable> element in myVariables) {
        if (element.Value == variable)
          oldName = element.Key;
      }
      myVariables.Remove(oldName);
      myVariables.Add(variable.Name, variable);
    }
    /// <inheritdoc cref="IScope.GetVariableValue&lt;T&gt;(string, bool)"/>
    public T GetVariableValue<T>(string name, bool recursiveLookup) where T : class, IItem {
      return GetVariableValue<T>(name, recursiveLookup, true);
    }
    /// <inheritdoc cref="IScope.GetVariableValue&lt;T&gt;(string, bool, bool)"/>
    public T GetVariableValue<T>(string name, bool recursiveLookup, bool throwOnError) where T : class, IItem {
      return (T)GetVariableValue(name, recursiveLookup, throwOnError);
    }
    /// <inheritdoc cref="IScope.GetVariableValue(string, bool)"/>
    public IItem GetVariableValue(string name, bool recursiveLookup) {
      return GetVariableValue(name, recursiveLookup, true);
    }
    /// <inheritdoc cref="IScope.GetVariableValue(string, bool, bool)"/>
    public IItem GetVariableValue(string name, bool recursiveLookup, bool throwOnError) {
      IVariable variable;
      name = TranslateName(name);
      if (myVariables.TryGetValue(name, out variable)) {
        return variable.Value;
      } else {
        if (recursiveLookup && (parent != null))
          return parent.GetVariableValue(name, recursiveLookup, throwOnError);
        else {
          if (throwOnError)
            throw new ArgumentException("Variable " + name + " not found");
          else
            return null;
        }
      }
    }
    /// <inheritdoc/>
    public string TranslateName(string name) {
      while (myAliases.ContainsKey(name))
        name = myAliases[name];
      if (parent != null)
        name = parent.TranslateName(name);
      return name;
    }
    /// <inheritdoc/>
    public void AddAlias(string alias, string name) {
      RemoveAlias(alias);
      if (alias != name) {
        myAliases.Add(alias, name);
        OnAliasAdded(alias);
      }
    }
    /// <inheritdoc/>
    public void RemoveAlias(string alias) {
      if (myAliases.ContainsKey(alias)) {
        myAliases.Remove(alias);
        OnAliasRemoved(alias);
      }
    }

    /// <inheritdoc/>
    public void AddSubScope(IScope scope) {
      scope.SetParent(this);
      mySubScopes.Add(scope);
      OnSubScopeAdded(scope, mySubScopes.Count - 1);
    }
    /// <inheritdoc/>
    public void RemoveSubScope(IScope scope) {
      int index = mySubScopes.IndexOf(scope);
      if (mySubScopes.Remove(scope)) {
        scope.SetParent(null);
        OnSubScopeRemoved(scope, index);
      }
    }
    /// <inheritdoc/>
    public void ReorderSubScopes(int[] sequence) {
      IScope[] scopes = mySubScopes.ToArray();
      mySubScopes.Clear();
      for (int i = 0; i < scopes.Length; i++)
        mySubScopes.Add(scopes[sequence[i]]);
      OnSubScopesReordered();
    }
    /// <inheritdoc/>
    public IScope GetScope(Guid guid) {
      if (Guid == guid) return this;
      else {
        for (int i = 0; i < mySubScopes.Count; i++) {
          IScope s = mySubScopes[i].GetScope(guid);
          if (s != null) return s;
        }
      }
      return null;
    }
    /// <inheritdoc/>
    public IScope GetScope(string name) {
      if (Name == name) return this;
      else {
        for (int i = 0; i < mySubScopes.Count; i++) {
          IScope s = mySubScopes[i].GetScope(name);
          if (s != null) return s;
        }
      }
      return null;
    }

    /// <inheritdoc/>
    public void Clear() {
      string[] variableNames = new string[Variables.Count];
      int i = 0;
      foreach (IVariable variable in Variables) {
        variableNames[i] = variable.Name;
        i++;
      }
      for (int j = 0; j < variableNames.Length; j++)
        RemoveVariable(variableNames[j]);

      KeyValuePair<string, string>[] aliases = new KeyValuePair<string, string>[myAliases.Count];
      myAliases.CopyTo(aliases, 0);
      for (int j = 0; j < aliases.Length; j++)
        RemoveAlias(aliases[j].Key);

      while (SubScopes.Count > 0)
        RemoveSubScope(SubScopes[0]);
    }

    /// <inheritdoc/>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Scope clone = (Scope)base.Clone(clonedObjects);
      clone.myName = Name;

      foreach (IVariable variable in myVariables.Values)
        clone.AddVariable((IVariable)Auxiliary.Clone(variable, clonedObjects));
      foreach (KeyValuePair<string, string> alias in myAliases)
        clone.AddAlias(alias.Key, alias.Value);
      for (int i = 0; i < SubScopes.Count; i++)
        clone.AddSubScope((IScope)Auxiliary.Clone(SubScopes[i], clonedObjects));

      return clone;
    }

    /// <inheritdoc />
    public event EventHandler<EventArgs<IVariable>> VariableAdded;
    /// <summary>
    /// Fires a new <c>VariableAdded</c> event.
    /// </summary>
    /// <param name="variable">The variable that has been added.</param>
    protected virtual void OnVariableAdded(IVariable variable) {
      if (VariableAdded != null)
        VariableAdded(this, new EventArgs<IVariable>(variable));
    }
    /// <inheritdoc />
    public event EventHandler<EventArgs<IVariable>> VariableRemoved;
    /// <summary>
    /// Fires a new <c>VariableRemoved</c>.
    /// </summary>
    /// <param name="variable">The variable that has been deleted.</param>
    protected virtual void OnVariableRemoved(IVariable variable) {
      if (VariableRemoved != null)
        VariableRemoved(this, new EventArgs<IVariable>(variable));
    }
    /// <inheritdoc /> 
    public event EventHandler<EventArgs<string>> AliasAdded;
    /// <summary>
    /// Fires a new <c>AliasAdded</c> event.
    /// </summary>
    /// <param name="alias">The alias that has been added.</param>
    protected virtual void OnAliasAdded(string alias) {
      if (AliasAdded != null)
        AliasAdded(this, new EventArgs<string>(alias));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<string>> AliasRemoved;
    /// <summary>
    /// Fires a new <c>AliasRemoved</c> event.
    /// </summary>
    /// <param name="alias">The alias that has been deleted.</param>
    protected virtual void OnAliasRemoved(string alias) {
      if (AliasRemoved != null)
        AliasRemoved(this, new EventArgs<string>(alias));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IScope, int>> SubScopeAdded;
    /// <summary>
    /// Fires a new <c>SubScopeAdded</c> event.
    /// </summary>
    /// <param name="scope">The sub scope that has been added.</param>
    /// <param name="index">The index where the scope has been added.</param>
    protected virtual void OnSubScopeAdded(IScope scope, int index) {
      if (SubScopeAdded != null)
        SubScopeAdded(this, new EventArgs<IScope, int>(scope, index));
    }
    /// <inheritdoc/>
    public event EventHandler<EventArgs<IScope, int>> SubScopeRemoved;
    /// <summary>
    /// Fires a new <c>SubScopeRemoved</c> event.
    /// </summary>
    /// <param name="scope">The sub scope that has been deleted.</param>
    /// <param name="index">The position of the sub scope.</param>
    protected virtual void OnSubScopeRemoved(IScope scope, int index) {
      if (SubScopeRemoved != null)
        SubScopeRemoved(this, new EventArgs<IScope, int>(scope, index));
    }
    /// <inheritdoc />
    public event EventHandler SubScopesReordered;
    /// <summary>
    /// Fires a new <c>SubScopesReordered</c> event
    /// </summary>
    protected virtual void OnSubScopesReordered() {
      if (SubScopesReordered != null)
        SubScopesReordered(this, new EventArgs());
    }
  }
}
