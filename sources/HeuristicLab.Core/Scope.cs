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
  public class Scope : ItemBase, IScope {
    private IScope parent;

    private string myName;
    public string Name {
      get { return myName; }
    }

    private IDictionary<string, IVariable> myVariables;
    public ICollection<IVariable> Variables {
      get { return myVariables.Values; }
    }
    private IDictionary<string, string> myAliases;
    public IEnumerable<KeyValuePair<string, string>> Aliases {
      get { return myAliases; }
    }
    private List<IScope> mySubScopes;
    public IList<IScope> SubScopes {
      get { return mySubScopes.AsReadOnly(); }
    }

    public Scope() {
      myName = "Anonymous";
      myVariables = new Dictionary<string, IVariable>();
      myAliases = new Dictionary<string, string>();
      mySubScopes = new List<IScope>();
    }
    public Scope(string name)
      : this() {
      if (name != null) myName = name;
    }

    public void SetParent(IScope scope) {
      parent = scope;
    }

    public override IView CreateView() {
      return new ScopeView(this);
    }

    public IVariable GetVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable))
        return variable;
      else
        return null;
    }
    public void AddVariable(IVariable variable) {
      myVariables.Add(variable.Name, variable);
      variable.NameChanging += new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
      variable.NameChanged += new EventHandler(Variable_NameChanged);
      OnVariableAdded(variable);
    }

    public void RemoveVariable(string name) {
      IVariable variable;
      if (myVariables.TryGetValue(name, out variable)) {
        variable.NameChanging -= new EventHandler<NameChangingEventArgs>(Variable_NameChanging);
        variable.NameChanged -= new EventHandler(Variable_NameChanged);
        myVariables.Remove(name);
        OnVariableRemoved(variable);
      }
    }
    private void Variable_NameChanging(object sender, NameChangingEventArgs e) {
      e.Cancel = myVariables.ContainsKey(e.Name);
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
    public T GetVariableValue<T>(string name, bool recursiveLookup) where T : class, IItem {
      return GetVariableValue<T>(name, recursiveLookup, true);
    }
    public T GetVariableValue<T>(string name, bool recursiveLookup, bool throwOnError) where T : class, IItem {
      return (T)GetVariableValue(name, recursiveLookup, throwOnError);
    }
    public IItem GetVariableValue(string name, bool recursiveLookup) {
      return GetVariableValue(name, recursiveLookup, true);
    }
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

    public string TranslateName(string name) {
      while (myAliases.ContainsKey(name))
        name = myAliases[name];
      if (parent != null)
        name = parent.TranslateName(name);
      return name;
    }
    public void AddAlias(string alias, string name) {
      RemoveAlias(alias);
      if (alias != name) {
        myAliases.Add(alias, name);
        OnAliasAdded(alias);
      }
    }
    public void RemoveAlias(string alias) {
      if (myAliases.ContainsKey(alias)) {
        myAliases.Remove(alias);
        OnAliasRemoved(alias);
      }
    }

    public void AddSubScope(IScope scope) {
      scope.SetParent(this);
      mySubScopes.Add(scope);
      OnSubScopeAdded(scope, mySubScopes.Count - 1);
    }
    public void RemoveSubScope(IScope scope) {
      int index = mySubScopes.IndexOf(scope);
      if (mySubScopes.Remove(scope)) {
        scope.SetParent(null);
        OnSubScopeRemoved(scope, index);
      }
    }
    public void ReorderSubScopes(int[] sequence) {
      IScope[] scopes = mySubScopes.ToArray();
      mySubScopes.Clear();
      for (int i = 0; i < scopes.Length; i++)
        mySubScopes.Add(scopes[sequence[i]]);
      OnSubScopesReordered();
    }
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

    public void Clear() {
      string[] variableNames = new string[Variables.Count];
      int i = 0;
      foreach (IVariable variable in Variables) {
        variableNames[i] = variable.Name;
        i++;
      }
      for (int j = 0; j < variableNames.Length; j++)
        RemoveVariable(variableNames[j]);

      KeyValuePair<string, string>[] aliases = new KeyValuePair<string,string>[myAliases.Count];
      myAliases.CopyTo(aliases, 0);
      for (int j = 0; j < aliases.Length; j++)
        RemoveAlias(aliases[j].Key);

      while (SubScopes.Count > 0)
        RemoveSubScope(SubScopes[0]);
    }

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

    public event EventHandler<VariableEventArgs> VariableAdded;
    protected virtual void OnVariableAdded(IVariable variable) {
      if (VariableAdded != null)
        VariableAdded(this, new VariableEventArgs(variable));
    }
    public event EventHandler<VariableEventArgs> VariableRemoved;
    protected virtual void OnVariableRemoved(IVariable variable) {
      if (VariableRemoved != null)
        VariableRemoved(this, new VariableEventArgs(variable));
    }
    public event EventHandler<AliasEventArgs> AliasAdded;
    protected virtual void OnAliasAdded(string alias) {
      if (AliasAdded != null)
        AliasAdded(this, new AliasEventArgs(alias));
    }
    public event EventHandler<AliasEventArgs> AliasRemoved;
    protected virtual void OnAliasRemoved(string alias) {
      if (AliasRemoved != null)
        AliasRemoved(this, new AliasEventArgs(alias));
    }
    public event EventHandler<ScopeIndexEventArgs> SubScopeAdded;
    protected virtual void OnSubScopeAdded(IScope scope, int index) {
      if (SubScopeAdded != null)
        SubScopeAdded(this, new ScopeIndexEventArgs(scope, index));
    }
    public event EventHandler<ScopeIndexEventArgs> SubScopeRemoved;
    protected virtual void OnSubScopeRemoved(IScope scope, int index) {
      if (SubScopeRemoved != null)
        SubScopeRemoved(this, new ScopeIndexEventArgs(scope, index));
    }
    public event EventHandler SubScopesReordered;
    protected virtual void OnSubScopesReordered() {
      if (SubScopesReordered != null)
        SubScopesReordered(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute nameAttribute = document.CreateAttribute("Name");
      nameAttribute.Value = Name.ToString();
      node.Attributes.Append(nameAttribute);

      XmlNode variables = document.CreateNode(XmlNodeType.Element, "Variables", null);
      foreach (IVariable variable in Variables)
        variables.AppendChild(PersistenceManager.Persist(variable, document, persistedObjects));
      node.AppendChild(variables);

      XmlNode aliases = document.CreateNode(XmlNodeType.Element, "Aliases", null);
      foreach (KeyValuePair<string, string> alias in myAliases) {
        XmlNode aliasNode = document.CreateNode(XmlNodeType.Element, "Alias", null);
        XmlAttribute keyAttribute = document.CreateAttribute("Alias");
        keyAttribute.Value = alias.Key;
        aliasNode.Attributes.Append(keyAttribute);
        XmlAttribute valueAttribute = document.CreateAttribute("Name");
        valueAttribute.Value = alias.Value;
        aliasNode.Attributes.Append(valueAttribute);
        aliases.AppendChild(aliasNode);
      }
      node.AppendChild(aliases);

      XmlNode subScopes = document.CreateNode(XmlNodeType.Element, "SubScopes", null);
      for (int i = 0; i < SubScopes.Count; i++)
        subScopes.AppendChild(PersistenceManager.Persist(SubScopes[i], document, persistedObjects));
      node.AppendChild(subScopes);

      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myName = node.Attributes["Name"].Value;

      XmlNode variables = node.SelectSingleNode("Variables");
      foreach (XmlNode variableNode in variables.ChildNodes) {
        IVariable variable = (IVariable)PersistenceManager.Restore(variableNode, restoredObjects);
        AddVariable(variable);
      }

      XmlNode aliases = node.SelectSingleNode("Aliases");
      if (aliases != null) {
        foreach (XmlNode aliasNode in aliases.ChildNodes)
          AddAlias(aliasNode.Attributes["Alias"].Value, aliasNode.Attributes["Name"].Value);
      }

      XmlNode subScopes = node.SelectSingleNode("SubScopes");
      for (int i = 0; i < subScopes.ChildNodes.Count; i++) {
        IScope scope = (IScope)PersistenceManager.Restore(subScopes.ChildNodes[i], restoredObjects);
        AddSubScope(scope);
      }
    }
    #endregion
  }
}
