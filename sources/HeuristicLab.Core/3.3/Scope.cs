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

using HeuristicLab.Collections;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Hierarchical container of variables (and of subscopes).
  /// </summary>
  [Item("Scope", "A scope which contains variables and sub-scopes.")]
  [StorableClass]
  public sealed class Scope : NamedItem, IScope {
    [Storable]
    private IScope parent;
    public IScope Parent {
      get { return parent; }
      set {
        if (parent != value) {
          IScope oldParent = parent;
          parent = null;
          if (oldParent != null) oldParent.SubScopes.Remove(this);
          parent = value;
          if ((parent != null) && !parent.SubScopes.Contains(this)) parent.SubScopes.Add(this);
        }
      }
    }

    [Storable]
    private VariableCollection variables;
    public VariableCollection Variables {
      get { return variables; }
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
      variables = new VariableCollection();
      SubScopes = new ScopeList();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the scope.</param>
    public Scope(string name)
      : base(name) {
      parent = null;
      variables = new VariableCollection();
      SubScopes = new ScopeList();
    }
    public Scope(string name, string description)
      : base(name, description) {
      parent = null;
      variables = new VariableCollection();
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
      clone.Name = Name;
      clone.Description = Description;
      if (variables.Count > 0) clone.variables = (VariableCollection)cloner.Clone(variables);
      if (subScopes.Count > 0) {
        clone.SubScopes = (ScopeList)cloner.Clone(subScopes);
        foreach (IScope child in clone.SubScopes)
          child.Parent = clone;
      }
      return clone;
    }

    #region SubScopes Events
    private void RegisterSubScopesEvents() {
      if (subScopes != null) {
        subScopes.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded);
        subScopes.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved);
        subScopes.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced);
        subScopes.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset);
      }
    }
    private void DeregisterSubScopesEvents() {
      if (subScopes != null) {
        subScopes.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsAdded);
        subScopes.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsRemoved);
        subScopes.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_ItemsReplaced);
        subScopes.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IScope>>(SubScopes_CollectionReset);
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
    #endregion
  }
}
