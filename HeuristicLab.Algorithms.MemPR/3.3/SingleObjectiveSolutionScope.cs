#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR {
  [Item("MemPRScope", "Scope for a MemPR individual.")]
  [StorableClass]
  public sealed class SingleObjectiveSolutionScope<T> : NamedItem, ISingleObjectiveSolutionScope<T> where T : class, IItem {
    public new static Image StaticItemImage {
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

    [Storable]
    private Variable solution;
    public T Solution {
      get { return (T)solution.Value; }
      set { solution.Value = value; }
    }

    [Storable]
    private Variable fitness;
    public double Fitness {
      get { return ((DoubleValue)fitness.Value).Value; }
      set { ((DoubleValue)fitness.Value).Value = value; }
    }

    [StorableConstructor]
    private SingleObjectiveSolutionScope(bool deserializing) : base(deserializing) { }
    private SingleObjectiveSolutionScope(SingleObjectiveSolutionScope<T> original, Cloner cloner)
      : base(original, cloner) {
      // the parent will not be deep-cloned
      parent = original.parent;
      variables = cloner.Clone(original.variables);
      subScopes = cloner.Clone(original.subScopes);
      foreach (var child in SubScopes)
        child.Parent = this;
      solution = cloner.Clone(original.solution);
      fitness = cloner.Clone(original.fitness);

      RegisterSubScopesEvents();
    }
    public SingleObjectiveSolutionScope(T code, string codeName, double fitness, string fitnessName) {
      this.solution = new Variable(codeName, code);
      this.fitness = new Variable(fitnessName, new DoubleValue(fitness));
      variables = new VariableCollection(2) { this.solution, this.fitness };
      subScopes = new ScopeList(2);

      RegisterSubScopesEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveSolutionScope<T>(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterSubScopesEvents();
    }

    public void Clear() {
      variables.Clear();
      subScopes.Clear();
    }

    #region SubScopes Events
    private void RegisterSubScopesEvents() {
      if (subScopes != null) {
        subScopes.ItemsAdded += SubScopes_ItemsAdded;
        subScopes.ItemsRemoved += SubScopes_ItemsRemoved;
        subScopes.ItemsReplaced += SubScopes_ItemsReplaced;
        subScopes.CollectionReset += SubScopes_CollectionReset;
      }
    }
    private void SubScopes_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (var item in e.Items) item.Value.Parent = this;
    }
    private void SubScopes_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (var item in e.Items) item.Value.Parent = null;
    }
    private void SubScopes_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (var oldItem in e.OldItems) oldItem.Value.Parent = null;
      foreach (var item in e.Items) item.Value.Parent = this;
    }
    private void SubScopes_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IScope>> e) {
      foreach (var oldItem in e.OldItems) oldItem.Value.Parent = null;
      foreach (var item in e.Items) item.Value.Parent = this;
    }
    #endregion

    public void Adopt(ISingleObjectiveSolutionScope<T> orphan) {
      Solution = orphan.Solution;
      Fitness = orphan.Fitness;
    }
  }
}
