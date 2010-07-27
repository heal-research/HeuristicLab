#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using System.Collections.Generic;
using System;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Symbols {
  [StorableClass]
  [Item("IntegratedVariable", "Represents an integrated variable value with a time offset.")]
  public sealed class IntegratedVariable : HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable {
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
    public IntegratedVariable()
      : base("IntegratedVariable", "Represents an integrated variable value with a time offset.") {
      minLag = -1; maxLag = -1;
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new IntegratedVariableTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      IntegratedVariable clone = (IntegratedVariable)base.Clone(cloner);
      clone.minLag = minLag;
      clone.maxLag = maxLag;
      return clone;
    }
  }
}
