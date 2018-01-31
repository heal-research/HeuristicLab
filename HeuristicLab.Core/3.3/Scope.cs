#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Hierarchical container of variables (and of subscopes).
  /// </summary>
  [Item("Scope", "A scope which contains variables and sub-scopes.")]
  [StorableClass]
  public sealed class Scope : NamedItem, IScope {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.OrgChart; }
    }

    [Storable]
    private IScope parent;
    public IScope Parent {
      get { return parent; }
      set {
        if (parent != value) {
          parent = value;
        }
      }
    }

    [Storable]
    private VariableCollection variables;
    public VariableCollection Variables {
      get { return variables; }
    }

    [Storable]
    private ScopeList subScopes;
    public ScopeList SubScopes {
      get { return subScopes; }
    }

    [StorableConstructor]
    private Scope(bool deserializing) : base(deserializing) { }
    private Scope(Scope original, Cloner cloner)
      : base(original, cloner) {
      if (original.variables.Count > 0) variables = cloner.Clone(original.variables);
      else variables = new VariableCollection();
      if (original.subScopes.Count > 0) {
        subScopes = cloner.Clone(original.subScopes);
        foreach (IScope child in SubScopes)
          child.Parent = this;
      } else subScopes = new ScopeList();
      RegisterSubScopesEvents();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> having "Anonymous" as default name.
    /// </summary>
    public Scope()
      : base("Anonymous") {
      parent = null;
      variables = new VariableCollection();
      subScopes = new ScopeList();
      RegisterSubScopesEvents();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Scope"/> with the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the scope.</param>
    public Scope(string name)
      : base(name) {
      parent = null;
      variables = new VariableCollection();
      subScopes = new ScopeList();
      RegisterSubScopesEvents();
    }
    public Scope(string name, string description)
      : base(name, description) {
      parent = null;
      variables = new VariableCollection();
      subScopes = new ScopeList();
      RegisterSubScopesEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterSubScopesEvents();
    }

    /// <inheritdoc/>
    public void Clear() {
      variables.Clear();
      subScopes.Clear();
    }

    /// <inheritdoc/>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Scope(this, cloner);
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
