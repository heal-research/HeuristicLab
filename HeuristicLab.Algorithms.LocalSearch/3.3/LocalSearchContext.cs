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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Algorithms.SingleObjective;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.LocalSearch {
  [Item("LocalSearchContext", "")]
  [StorableClass]
  public class LocalSearchContext<TProblem, TEncoding, TSolution> : HeuristicAlgorithmContext<TProblem, TEncoding, TSolution>,
      ISingleObjectiveSolutionContext<TSolution>, IImprovementStepsContext, IIterationsContext
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution> // need both !?
      where TEncoding : class, IEncoding<TSolution>
      where TSolution: class, ISolution {

    [Storable]
    private ISingleObjectiveSolutionScope<TSolution> solution;
    public ISingleObjectiveSolutionScope<TSolution> Solution {
      get { return solution; }
      set {
        solution = value;
        if (Scope.SubScopes.Count == 0)
          Scope.SubScopes.Add(solution);
        else Scope.SubScopes[0] = solution;
      }
    }

    [Storable]
    private int iterations;
    public int Iterations {
      get { return iterations; }
      set { iterations = value; }
    }

    [Storable]
    private int improvementSteps;
    public int ImprovementSteps {
      get { return improvementSteps; }
      set { improvementSteps = value; }
    }

    [StorableConstructor]
    private LocalSearchContext(bool deserializing) : base(deserializing) { }
    private LocalSearchContext(LocalSearchContext<TProblem, TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) { }
    public LocalSearchContext() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LocalSearchContext<TProblem, TEncoding, TSolution>(this, cloner);
    }

    IScope ISolutionContext.Solution { get { return solution; } }
    ISolutionScope<TSolution> ISolutionContext<TSolution>.Solution { get { return solution; } }
  }
}
