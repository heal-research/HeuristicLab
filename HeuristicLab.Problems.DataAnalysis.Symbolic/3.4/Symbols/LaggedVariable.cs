#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("LaggedVariable", "Represents a variable value with a time offset.")]
  public class LaggedVariable : Variable {
    [Storable]
    private int minLag;
    public int MinLag {
      get { return minLag; }
      set { minLag = value; }
    }
    [Storable]
    private int maxLag;
    public int MaxLag {
      get { return maxLag; }
      set { maxLag = value; }
    }
    [StorableConstructor]
    protected LaggedVariable(bool deserializing) : base(deserializing) { }
    protected LaggedVariable(LaggedVariable original, Cloner cloner)
      : base(original, cloner) {
      minLag = original.minLag;
      maxLag = original.maxLag;
    }
    public LaggedVariable() : this("LaggedVariable", "Represents a variable value with a time offset.") { }
    protected LaggedVariable(string name, string description)
      : base(name, description) {
      MinLag = -1;
      MaxLag = -1;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new LaggedVariableTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LaggedVariable(this, cloner);
    }
  }
}
