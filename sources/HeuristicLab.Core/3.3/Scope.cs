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
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  /// <summary>
  /// Hierarchical container of variables (and of subscopes).
  /// </summary>
  [Item("Scope", "A scope which contains variables and sub-scopes.")]
  [Creatable("Test")]
  public sealed class Scope : NamedItem, IScope {
    [Storable]
    private IScope parent;
    public IScope Parent {
      get { return parent; }
      set {
        if (parent != null) parent.SubScopes.Remove(this);
        parent = value;
        if ((parent != null) && !parent.SubScopes.Contains(this)) parent.SubScopes.Add(this);
      }
    }

    private VariableCollection variables;
    [Storable]
    public VariableCollection Variables {
      get { return variables; }
      private set {
        if (variables != null) variables.Changed -= new ChangedEventHandler(Variables_Changed);
        variables = value;
        if (variables != null) variables.Changed += new ChangedEventHandler(Variables_Changed);
      }
    }

    private ScopeList subScopes;
    [Storable]
    public ScopeList SubScopes {
      get { return subScopes; }
      private set {
        DeregisterSubScopesEvents();
        subScopes = value;
        RegisterSubScopesEvents();
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> having "Anonymous" as default name.
    /// </summary>
    public Scope()
      : base("Anonymous") {
      parent = null;
      Variables = new VariableCollection();
      SubScopes = new ScopeList();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the scope.</param>
    public Scope(string name)
      : base(name) {
      parent = null;
      Variables = new VariableCollection();
      SubScopes = new ScopeList();
    }

    /// <inheritdoc/>
    public void Clear() {
      variables.Clear();
      subScopes.Clear();
    }

    /// <inheritdoc/>
    public override IDeepCloneable Clone(Cloner cloner) {
      Scope clone = new Scope();
      cloner.RegisterClonedObject(this, clone);
      clone.name = name;
      clone.description = description;
      clone.parent = (IScope)cloner.Clone(parent);
      clone.Variables = (VariableCollection)cloner.Clone(variables);
      clone.SubScopes = (ScopeList)cloner.Clone(subScopes);
      return clone;
    }

    #region SubScopes Events
    private void RegisterSubScopesEvents() {
      if (subScopes != null) {
        subScopes.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded);
        subScopes.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved);
        subScopes.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced);
        subScopes.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset);
        subScopes.Changed += new ChangedEventHandler(SubScopes_Changed);
      }
    }
    private void DeregisterSubScopesEvents() {
      if (subScopes != null) {
        subScopes.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded);
        subScopes.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved);
        subScopes.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced);
        subScopes.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset);
        subScopes.Changed -= new ChangedEventHandler(SubScopes_Changed);
      }
    }
    private void SubScopes_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (IndexedItem<IScope> item in e.Items)
        item.Value.Parent = this;
    }
    private void SubScopes_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (IndexedItem<IScope> item in e.Items)
        item.Value.Parent = null;
    }
    private void SubScopes_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (IndexedItem<IScope> oldItem in e.OldItems)
        oldItem.Value.Parent = null;
      foreach (IndexedItem<IScope> item in e.Items)
        item.Value.Parent = this;
    }
    private void SubScopes_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (IndexedItem<IScope> oldItem in e.OldItems)
        oldItem.Value.Parent = null;
      foreach (IndexedItem<IScope> item in e.Items)
        item.Value.Parent = this;
    }
    private void SubScopes_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
    #endregion

    #region Variables Events
    private void Variables_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
    #endregion
  }
}
