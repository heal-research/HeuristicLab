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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Algorithms.SingleObjective;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.GeneticAlgorithm {
  [Item("Evolutionary Algorithm Context", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution> : HeuristicAlgorithmContext<TProblem, TEncoding, TSolution>,
      ISingleObjectivePopulationContext<TSolution>, ISingleObjectiveMatingpoolContext<TSolution>, ISingleObjectiveMatingContext<TSolution>,
      ISingleObjectiveSolutionContext<TSolution>, IIterationsContext
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution>
      where TEncoding : class, IEncoding<TSolution>
      where TSolution : class, ISolution {

    [StorableConstructor]
    protected EvolutionaryAlgorithmContext(bool deserializing) : base(deserializing) { }

    protected EvolutionaryAlgorithmContext(EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      matingPool = original.matingPool != null ? original.matingPool.Select(cloner.Clone).ToList() : null;
      parents = original.parents != null ? Tuple.Create(cloner.Clone(original.parents.Item1), cloner.Clone(original.parents.Item2)) : null;
      child = original.child != null ? cloner.Clone(child) : null;
    }
    public EvolutionaryAlgorithmContext() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvolutionaryAlgorithmContext<TProblem, TEncoding, TSolution>(this, cloner);
    }

    IEnumerable<IScope> IPopulationContext.Population {
      get { return Scope.SubScopes; }
    }

    IEnumerable<ISolutionScope<TSolution>> IPopulationContext<TSolution>.Population {
      get { return Scope.SubScopes.OfType<ISolutionScope<TSolution>>(); }
    }
    
    public IEnumerable<ISingleObjectiveSolutionScope<TSolution>> Population {
      get { return Scope.SubScopes.OfType<ISingleObjectiveSolutionScope<TSolution>>(); }
    }

    [Storable]
    private IEnumerable<ISingleObjectiveSolutionScope<TSolution>> matingPool;
    public IEnumerable<ISingleObjectiveSolutionScope<TSolution>> MatingPool {
      get { return matingPool; }
      set { matingPool = value; }
    }
    IEnumerable<ISolutionScope<TSolution>> IMatingpoolContext<TSolution>.MatingPool {
      get { return matingPool; }
      set { matingPool = value.OfType<ISingleObjectiveSolutionScope<TSolution>>(); }
    }

    [Storable]
    private Tuple<ISingleObjectiveSolutionScope<TSolution>, ISingleObjectiveSolutionScope<TSolution>> parents;
    public Tuple<ISingleObjectiveSolutionScope<TSolution>, ISingleObjectiveSolutionScope<TSolution>> Parents {
      get { return parents; }
      set { parents = value; }
    }
    Tuple<ISolutionScope<TSolution>, ISolutionScope<TSolution>> IMatingContext<TSolution>.Parents {
      get { return Tuple.Create((ISolutionScope<TSolution>)Parents.Item1, (ISolutionScope<TSolution>)Parents.Item2); }
    }

    [Storable]
    private ISingleObjectiveSolutionScope<TSolution> child;


    public ISingleObjectiveSolutionScope<TSolution> Child {
      get { return child; }
      set { child = value; }
    }
    ISolutionScope<TSolution> IMatingContext<TSolution>.Child {
      get { return Child; }
    }
    public ISingleObjectiveSolutionScope<TSolution> Solution {
      get { return child; }
    }
    ISolutionScope<TSolution> ISolutionContext<TSolution>.Solution { get { return child; } }
    IScope ISolutionContext.Solution { get { return child; } }

    [Storable]
    private int iterations;
    public int Iterations {
      get { return iterations; }
      set { iterations = value; }
    }
  }
}
