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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableClass]
  [Item("Variable", "Represents a variable value.")]
  public sealed class Variable : Symbol {
    #region Properties
    [Storable]
    private double weightNu;
    public double WeightNu {
      get { return weightNu; }
      set {
        if (value != weightNu) {
          weightNu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double weightSigma;
    public double WeightSigma {
      get { return weightSigma; }
      set {
        if (weightSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != weightSigma) {
          weightSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double weightManipulatorNu;
    public double WeightManipulatorNu {
      get { return weightManipulatorNu; }
      set {
        if (value != weightManipulatorNu) {
          weightManipulatorNu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double weightManipulatorSigma;
    public double WeightManipulatorSigma {
      get { return weightManipulatorSigma; }
      set {
        if (weightManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != weightManipulatorSigma) {
          weightManipulatorSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    private List<string> variableNames;
    [Storable]
    public IEnumerable<string> VariableNames {
      get { return variableNames; }
      set {
        if (value == null) throw new ArgumentNullException();
        variableNames.Clear();
        variableNames.AddRange(value);
        OnChanged(EventArgs.Empty);
      }
    }
    #endregion

    [StorableConstructor]
    private Variable(bool deserializing) : base(deserializing) {
      variableNames = new List<string>();
    }
    private Variable(Variable original, Cloner cloner)
      : base(original, cloner) {
      weightNu = original.weightNu;
      weightSigma = original.weightSigma;
      variableNames = new List<string>(original.variableNames);
      weightManipulatorNu = original.weightManipulatorNu;
      weightManipulatorSigma = original.weightManipulatorSigma;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Variable(this, cloner);
    }

    public Variable()
      : base("Variable", "Represents a variable value.") {
      weightNu = 1.0;
      weightSigma = 1.0;
      weightManipulatorNu = 0.0;
      weightManipulatorSigma = 1.0;
      variableNames = new List<string>();
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new VariableTreeNode(this);
    }
  }
}
