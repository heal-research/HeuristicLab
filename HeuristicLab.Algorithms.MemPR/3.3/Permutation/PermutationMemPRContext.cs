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

using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Permutation {
  [Item("MemPR Population Context (permutation)", "MemPR population context for permutation encoded problems.")]
  [StorableClass]
  public sealed class PermutationMemPRPopulationContext : MemPRPopulationContext<SingleObjectiveBasicProblem<PermutationEncoding>, Encodings.PermutationEncoding.Permutation, PermutationMemPRPopulationContext, PermutationMemPRSolutionContext> {

    [StorableConstructor]
    private PermutationMemPRPopulationContext(bool deserializing) : base(deserializing) { }
    private PermutationMemPRPopulationContext(PermutationMemPRPopulationContext original, Cloner cloner)
      : base(original, cloner) { }
    public PermutationMemPRPopulationContext() : base("PermutationMemPRPopulationContext") { }
    public PermutationMemPRPopulationContext(string name) : base(name) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationMemPRPopulationContext(this, cloner);
    }

    public override PermutationMemPRSolutionContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> solution) {
      return new PermutationMemPRSolutionContext(this, solution);
    }
  }

  [Item("MemPR Solution Context (permutation)", "MemPR solution context for permutation encoded problems.")]
  [StorableClass]
  public sealed class PermutationMemPRSolutionContext : MemPRSolutionContext<SingleObjectiveBasicProblem<PermutationEncoding>, Encodings.PermutationEncoding.Permutation, PermutationMemPRPopulationContext, PermutationMemPRSolutionContext>, IPermutationSubspaceContext {

    [Storable]
    private IValueParameter<PermutationSolutionSubspace> subspace;
    public PermutationSolutionSubspace Subspace {
      get { return subspace.Value; }
    }
    ISolutionSubspace<Encodings.PermutationEncoding.Permutation> ISolutionSubspaceContext<Encodings.PermutationEncoding.Permutation>.Subspace {
      get { return Subspace; }
    }

    [StorableConstructor]
    private PermutationMemPRSolutionContext(bool deserializing) : base(deserializing) { }
    private PermutationMemPRSolutionContext(PermutationMemPRSolutionContext original, Cloner cloner)
      : base(original, cloner) {

    }
    public PermutationMemPRSolutionContext(PermutationMemPRPopulationContext baseContext, ISingleObjectiveSolutionScope<Encodings.PermutationEncoding.Permutation> solution)
      : base(baseContext, solution) {

      Parameters.Add(subspace = new ValueParameter<PermutationSolutionSubspace>("Subspace", new PermutationSolutionSubspace(null)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PermutationMemPRSolutionContext(this, cloner);
    }
  }
}
