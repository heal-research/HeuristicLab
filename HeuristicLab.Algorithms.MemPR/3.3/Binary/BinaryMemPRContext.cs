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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Binary {
  [Item("BinaryMemPRContext", "MemPR context for binary encoded problems.")]
  [StorableClass]
  public sealed class BinaryMemPRContext : MemPRContext<BinaryVector, BinaryMemPRContext, BinarySingleSolutionMemPRContext> {

    [StorableConstructor]
    private BinaryMemPRContext(bool deserializing) : base(deserializing) { }
    private BinaryMemPRContext(BinaryMemPRContext original, Cloner cloner)
      : base(original, cloner) { }
    public BinaryMemPRContext() : base("BinaryMemPRContext") { }
    public BinaryMemPRContext(string name) : base(name) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryMemPRContext(this, cloner);
    }

    public override BinarySingleSolutionMemPRContext CreateSingleSolutionContext(ISingleObjectiveSolutionScope<BinaryVector> solution) {
      return new BinarySingleSolutionMemPRContext(this, solution);
    }
  }

  [Item("BinarySingleSolutionMemPRContext", "Single solution MemPR context for binary encoded problems.")]
  [StorableClass]
  public sealed class BinarySingleSolutionMemPRContext : SingleSolutionMemPRContext<BinaryVector, BinaryMemPRContext, BinarySingleSolutionMemPRContext>, IBinarySolutionSubspaceContext {

    [Storable]
    private IValueParameter<BinarySolutionSubspace> subspace;
    public BinarySolutionSubspace Subspace {
      get { return subspace.Value; }
    }
    ISolutionSubspace ISolutionSubspaceContext.Subspace {
      get { return Subspace; }
    }

    [StorableConstructor]
    private BinarySingleSolutionMemPRContext(bool deserializing) : base(deserializing) { }
    private BinarySingleSolutionMemPRContext(BinarySingleSolutionMemPRContext original, Cloner cloner)
      : base(original, cloner) {

    }
    public BinarySingleSolutionMemPRContext(BinaryMemPRContext baseContext, ISingleObjectiveSolutionScope<BinaryVector> solution)
      : base(baseContext, solution) {

      Parameters.Add(subspace = new ValueParameter<BinarySolutionSubspace>("Subspace", new BinarySolutionSubspace(null)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinarySingleSolutionMemPRContext(this, cloner);
    }
  }
}
