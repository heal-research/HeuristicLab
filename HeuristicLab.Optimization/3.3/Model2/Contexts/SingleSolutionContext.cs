#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Model2 {
  [Item("Single Solution Context", "A context for single solution algorithms.")]
  [StorableClass]
  public abstract class SingleSolutionContext<TSolutionScope> : StochasticContext
    where TSolutionScope: class, IScope {
    
    [Storable]
    private TSolutionScope incumbent;
    public TSolutionScope Incumbent {
      get { return incumbent; }
    }

    public void ReplaceIncumbent(TSolutionScope solScope) {
      if (incumbent != null) Scope.SubScopes.Remove(incumbent);
      this.incumbent = solScope;
      if (incumbent != null) Scope.SubScopes.Add(incumbent);
    }


    [StorableConstructor]
    protected SingleSolutionContext(bool deserializing) : base(deserializing) { }
    protected SingleSolutionContext(SingleSolutionContext<TSolutionScope> original, Cloner cloner)
      : base(original, cloner) {
      incumbent = cloner.Clone(original.incumbent);
    }
    protected SingleSolutionContext() : base() { }
    protected SingleSolutionContext(string name) : base(name) { }
    protected SingleSolutionContext(string name, ParameterCollection parameters) : base(name, parameters) { }
    protected SingleSolutionContext(string name, string description) : base(name, description) { }
    protected SingleSolutionContext(string name, string description, ParameterCollection parameters) : base(name, description, parameters) { }
  }
}
