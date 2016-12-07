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
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.LinearLinkage {
  [Item("MemPR Population Context (linear linkage)", "MemPR population context for linear linkage encoded problems.")]
  [StorableClass]
  public sealed class LinearLinkageMemPRPopulationContext : MemPRPopulationContext<SingleObjectiveBasicProblem<LinearLinkageEncoding>, Encodings.LinearLinkageEncoding.LinearLinkage, LinearLinkageMemPRPopulationContext, LinearLinkageMemPRSolutionContext> {

    [StorableConstructor]
    private LinearLinkageMemPRPopulationContext(bool deserializing) : base(deserializing) { }
    private LinearLinkageMemPRPopulationContext(LinearLinkageMemPRPopulationContext original, Cloner cloner)
      : base(original, cloner) { }
    public LinearLinkageMemPRPopulationContext() : base("LinearLinkageMemPRPopulationContext") { }
    public LinearLinkageMemPRPopulationContext(string name) : base(name) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearLinkageMemPRPopulationContext(this, cloner);
    }

    public override LinearLinkageMemPRSolutionContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> solution) {
      return new LinearLinkageMemPRSolutionContext(this, solution);
    }
  }

  [Item("MemPR Solution Context (linear linkage)", "MemPR solution context for linear linkage encoded problems.")]
  [StorableClass]
  public sealed class LinearLinkageMemPRSolutionContext : MemPRSolutionContext<SingleObjectiveBasicProblem<LinearLinkageEncoding>, Encodings.LinearLinkageEncoding.LinearLinkage, LinearLinkageMemPRPopulationContext, LinearLinkageMemPRSolutionContext>, ILinearLinkageSubspaceContext {

    [Storable]
    private IValueParameter<LinearLinkageSolutionSubspace> subspace;
    public LinearLinkageSolutionSubspace Subspace {
      get { return subspace.Value; }
    }
    ISolutionSubspace<Encodings.LinearLinkageEncoding.LinearLinkage> ISolutionSubspaceContext<Encodings.LinearLinkageEncoding.LinearLinkage>.Subspace {
      get { return Subspace; }
    }

    [StorableConstructor]
    private LinearLinkageMemPRSolutionContext(bool deserializing) : base(deserializing) { }
    private LinearLinkageMemPRSolutionContext(LinearLinkageMemPRSolutionContext original, Cloner cloner)
      : base(original, cloner) {

    }
    public LinearLinkageMemPRSolutionContext(LinearLinkageMemPRPopulationContext baseContext, ISingleObjectiveSolutionScope<Encodings.LinearLinkageEncoding.LinearLinkage> solution)
      : base(baseContext, solution) {

      Parameters.Add(subspace = new ValueParameter<LinearLinkageSolutionSubspace>("Subspace", new LinearLinkageSolutionSubspace(null)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearLinkageMemPRSolutionContext(this, cloner);
    }
  }
}
