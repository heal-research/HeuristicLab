#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("5CD355EA-36E4-4E43-B8C4-9E9CF4CBC860")]
  [Item("Constant", "Represents a constant value.")]
  public sealed class Constant : Symbol {
    #region Properties
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
    private double manipulatorMu;
    public double ManipulatorMu {
      get { return manipulatorMu; }
      set {
        if (value != manipulatorMu) {
          manipulatorMu = value;
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
    [Storable(DefaultValue = 0.0)]
    private double multiplicativeManipulatorSigma;
    public double MultiplicativeManipulatorSigma {
      get { return multiplicativeManipulatorSigma; }
      set {
        if (value < 0) throw new ArgumentException();
        if (value != multiplicativeManipulatorSigma) {
          multiplicativeManipulatorSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    private const int minimumArity = 0;
    private const int maximumArity = 0;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }
    #endregion

    [StorableConstructor]
    private Constant(StorableConstructorFlag _) : base(_) { }
    private Constant(Constant original, Cloner cloner)
      : base(original, cloner) {
      minValue = original.minValue;
      maxValue = original.maxValue;
      manipulatorMu = original.manipulatorMu;
      manipulatorSigma = original.manipulatorSigma;
      multiplicativeManipulatorSigma = original.multiplicativeManipulatorSigma;
    }
    public Constant()
      : base("Constant", "Represents a constant value.") {
      manipulatorMu = 0.0;
      manipulatorSigma = 1.0;
      multiplicativeManipulatorSigma = 0.03;
      minValue = -20.0;
      maxValue = 20.0;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new ConstantTreeNode(this);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Constant(this, cloner);
    }
  }
}
