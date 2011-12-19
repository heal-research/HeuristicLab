#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableClass]
  [Item("Constant", "Represents a constant value.")]
  public sealed class Constant : Symbol {
    #region Propeties
    [Storable]
    private double minValue;
    public double MinValue {
      get { return minValue; }
      set {
        if (value != minValue) {
          minValue = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double maxValue;
    public double MaxValue {
      get { return maxValue; }
      set {
        if (value != maxValue) {
          maxValue = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double manipulatorNu;
    public double ManipulatorNu {
      get { return manipulatorNu; }
      set {
        if (value != manipulatorNu) {
          manipulatorNu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double manipulatorSigma;
    public double ManipulatorSigma {
      get { return manipulatorSigma; }
      set {
        if (value < 0) throw new ArgumentException();
        if (value != manipulatorSigma) {
          manipulatorSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    #endregion

    [StorableConstructor]
    private Constant(bool deserializing) : base(deserializing) { }
    private Constant(Constant original, Cloner cloner)
      : base(original, cloner) {
      minValue = original.minValue;
      maxValue = original.maxValue;
      manipulatorNu = original.manipulatorNu;
      manipulatorSigma = original.manipulatorSigma;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Constant(this, cloner);
    }
    public Constant()
      : base("Constant", "Represents a constant value.") {
      manipulatorNu = 0.0;
      manipulatorSigma = 1.0;
      minValue = -20.0;
      maxValue = 20.0;
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new ConstantTreeNode(this);
    }
  }
}
