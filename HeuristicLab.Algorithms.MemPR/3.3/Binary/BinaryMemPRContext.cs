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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Binary {
  [Item("MemPR Population Context (binary)", "MemPR population context for binary encoded problems.")]
  [StorableClass]
  public sealed class BinaryMemPRPopulationContext : MemPRPopulationContext<SingleObjectiveBasicProblem<BinaryVectorEncoding>, BinaryVector, BinaryMemPRPopulationContext, BinaryMemPRSolutionContext> {

    [StorableConstructor]
    private BinaryMemPRPopulationContext(bool deserializing) : base(deserializing) { }
    private BinaryMemPRPopulationContext(BinaryMemPRPopulationContext original, Cloner cloner)
      : base(original, cloner) { }
    public BinaryMemPRPopulationContext() : base("BinaryMemPRPopulationContext") { }
    public BinaryMemPRPopulationContext(string name) : base(name) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryMemPRPopulationContext(this, cloner);
    }

    public override BinaryMemPRSolutionContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<BinaryVector> solution) {
      return new BinaryMemPRSolutionContext(this, solution);
    }
  }

  [Item("MemPR Solution Context (binary)", "MemPR solution context for binary encoded problems.")]
  [StorableClass]
  public sealed class BinaryMemPRSolutionContext : MemPRSolutionContext<SingleObjectiveBasicProblem<BinaryVectorEncoding>, BinaryVector, BinaryMemPRPopulationContext, BinaryMemPRSolutionContext>, IBinaryVectorSubspaceContext {

    [Storable]
    private IValueParameter<BinarySolutionSubspace> subspace;
    public BinarySolutionSubspace Subspace {
      get { return subspace.Value; }
    }
    ISolutionSubspace<BinaryVector> ISolutionSubspaceContext<BinaryVector>.Subspace {
      get { return Subspace; }
    }

    [StorableConstructor]
    private BinaryMemPRSolutionContext(bool deserializing) : base(deserializing) { }
    private BinaryMemPRSolutionContext(BinaryMemPRSolutionContext original, Cloner cloner)
      : base(original, cloner) {

    }
    public BinaryMemPRSolutionContext(BinaryMemPRPopulationContext baseContext, ISingleObjectiveSolutionScope<BinaryVector> solution)
      : base(baseContext, solution) {

      Parameters.Add(subspace = new ValueParameter<BinarySolutionSubspace>("Subspace", new BinarySolutionSubspace(null)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryMemPRSolutionContext(this, cloner);
    }
  }
}
