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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Model2 {
  [Item("Population Context", "A context for population-based algorithms.")]
  [StorableClass]
  public class PopulationContext<TSolutionScope> : StochasticContext
    where TSolutionScope: class, IScope {

    public IEnumerable<TSolutionScope> Population {
      get { return Scope.SubScopes.OfType<TSolutionScope>(); }
    }
    public void AddToPopulation(TSolutionScope solScope) {
      Scope.SubScopes.Add(solScope);
    }
    public void ReplaceAtPopulation(int index, TSolutionScope solScope) {
      Scope.SubScopes[index] = solScope;
    }
    public void ReplacePopulation(IEnumerable<TSolutionScope> replacement) {
      Scope.SubScopes.Replace(replacement);
    }
    public TSolutionScope AtPopulation(int index) {
      return Scope.SubScopes[index] as TSolutionScope;
    }
    public TSolutionScope AtRandomPopulation() {
      return Scope.SubScopes[Random.Next(PopulationCount)] as TSolutionScope;
    }
    public int PopulationCount {
      get { return Scope.SubScopes.Count; }
    }


    [StorableConstructor]
    protected PopulationContext(bool deserializing) : base(deserializing) { }
    protected PopulationContext(PopulationContext<TSolutionScope> original, Cloner cloner)
      : base(original, cloner) {
    }
    public PopulationContext() : base() { }
    public PopulationContext(string name) : base(name) { }
    public PopulationContext(string name, ParameterCollection parameters) : base(name, parameters) { }
    public PopulationContext(string name, string description) : base(name, description) { }
    public PopulationContext(string name, string description, ParameterCollection parameters) : base(name, description, parameters) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PopulationContext<TSolutionScope>(this, cloner);
    }

    public void RunOperator(IOperator op, int individual, CancellationToken cancellationToken) {
      RunOperator(op, AtPopulation(individual), cancellationToken);
    }
  }
}
