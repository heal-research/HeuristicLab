#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  public sealed class Solution : NamedItem {
    private int width;
    [Storable]
    public int Width {
      get { return width; }
      private set { this.width = value; }
    }
    private int length;
    [Storable]
    public int Length {
      get { return length; }
      private set { this.length = value; }
    }
    private ISymbolicExpressionTree tree;
    [Storable]
    public ISymbolicExpressionTree Tree {
      get { return tree; }
      private set { this.tree = value; }
    }
    [StorableConstructor]
    private Solution(bool deserializing) : base(deserializing) { }
    private Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
      this.length = original.length;
      this.width = original.width;
      this.tree = cloner.Clone(tree);
    }

    public Solution(ISymbolicExpressionTree tree, int length, int width)
      : base("Solution", "A lawn mower solution.") {
      this.tree = tree;
      this.length = length;
      this.width = width;
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }
  }
}
