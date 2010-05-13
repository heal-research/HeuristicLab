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
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using System;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols {
  [StorableClass]
  [Item("Constant", "Represents a constant value.")]
  public sealed class Constant : Symbol {
    #region Propeties
    [Storable]
    private double minValue;
    public double MinValue {
      get { return minValue; }
      set { minValue = value; }
    }
    [Storable]
    private double maxValue;
    public double MaxValue {
      get { return maxValue; }
      set { maxValue = value; }
    }
    [Storable]
    private double manipulatorNu;
    public double ManipulatorNu {
      get { return manipulatorNu; }
      set { manipulatorNu = value; }
    }
    [Storable]
    private double manipulatorSigma;
    public double ManipulatorSigma {
      get { return manipulatorSigma; }
      set {
        if (value < 0) throw new ArgumentException();
        manipulatorSigma = value;
      }
    }
    #endregion
    public Constant()
      : base() {
      manipulatorNu = 0.0;
      manipulatorSigma = 1.0;
      minValue = -20.0;
      maxValue = 20.0;
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new ConstantTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Constant clone = (Constant)base.Clone(cloner);
      clone.minValue = minValue;
      clone.maxValue = maxValue;
      clone.manipulatorNu = manipulatorNu;
      clone.manipulatorSigma = manipulatorSigma;
      return clone;
    }
  }
}
