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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.GeneticProgramming.Robocode {
  [StorableClass]
  public class BooleanValue : Symbol {
    public override int MinimumArity { get { return 0; } }
    public override int MaximumArity { get { return 0; } }

    [StorableConstructor]
    protected BooleanValue(bool deserializing) : base(deserializing) { }
    protected BooleanValue(BooleanValue original, Cloner cloner) : base(original, cloner) { }

    public BooleanValue() : base("BooleanValue", "A Boolean value.") { }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new BooleanTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BooleanValue(this, cloner);
    }
  }
}
