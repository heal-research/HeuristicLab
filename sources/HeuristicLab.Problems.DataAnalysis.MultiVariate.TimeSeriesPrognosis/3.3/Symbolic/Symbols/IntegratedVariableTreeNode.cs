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
using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;
namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Symbols {
  [StorableClass]
  public sealed class IntegratedVariableTreeNode : VariableTreeNode {
    public new IntegratedVariable Symbol {
      get { return (IntegratedVariable)base.Symbol; }
    }
    [Storable]
    private int minTimeOffset;
    public int MinTimeOffset {
      get { return minTimeOffset; }
      set { minTimeOffset = value; }
    }

    [Storable]
    private int maxTimeOffset;
    public int MaxTimeOffset {
      get { return maxTimeOffset; }
      set { maxTimeOffset = value; }
    }

    private IntegratedVariableTreeNode() { }

    // copy constructor
    private IntegratedVariableTreeNode(IntegratedVariableTreeNode original)
      : base(original) {
      minTimeOffset = original.minTimeOffset;
      maxTimeOffset = original.maxTimeOffset;
    }

    public IntegratedVariableTreeNode(IntegratedVariable integralVarSymbol) : base(integralVarSymbol) { }

    public override bool HasLocalParameters {
      get {
        return true;
      }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      minTimeOffset = random.Next(Symbol.MinLag, Symbol.MaxLag + 1);
      maxTimeOffset = random.Next(minTimeOffset, Symbol.MaxLag + 1);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);
      minTimeOffset = Math.Min(Symbol.MaxLag, Math.Max(Symbol.MinLag, minTimeOffset + random.Next(-1, 2)));
      maxTimeOffset = Math.Min(Symbol.MaxLag, Math.Max(minTimeOffset, maxTimeOffset + random.Next(-1, 2)));
    }


    public override object Clone() {
      return new IntegratedVariableTreeNode(this);
    }

    public override string ToString() {
      return Weight.ToString("E4") + " I(" + VariableName +
        ", " + minTimeOffset + ", " + maxTimeOffset + ")";
    }
  }
}
