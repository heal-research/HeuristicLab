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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  [Item("Variable Condition", "Represents a condition that tests a given variable against a specified threshold.")]
  public sealed class VariableCondition : Symbol {
    #region properties
    [Storable]
    private double thresholdInitializerMu;
    public double ThresholdInitializerMu {
      get { return thresholdInitializerMu; }
      set {
        if (value != thresholdInitializerMu) {
          thresholdInitializerMu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double thresholdInitializerSigma;
    public double ThresholdInitializerSigma {
      get { return thresholdInitializerSigma; }
      set {
        if (thresholdInitializerSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != thresholdInitializerSigma) {
          thresholdInitializerSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable]
    private double thresholdManipulatorMu;
    public double ThresholdManipulatorMu {
      get { return thresholdManipulatorMu; }
      set {
        if (value != thresholdManipulatorMu) {
          thresholdManipulatorMu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double thresholdManipulatorSigma;
    public double ThresholdManipulatorSigma {
      get { return thresholdManipulatorSigma; }
      set {
        if (thresholdManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != thresholdManipulatorSigma) {
          thresholdManipulatorSigma = value;
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

    private List<string> allVariableNames;
    [Storable]
    public IEnumerable<string> AllVariableNames {
      get { return allVariableNames; }
      set {
        if (value == null) throw new ArgumentNullException();
        allVariableNames.Clear();
        allVariableNames.AddRange(value);
      }
    }

    [Storable]
    private double slopeInitializerMu;
    public double SlopeInitializerMu {
      get { return slopeInitializerMu; }
      set {
        if (value != slopeInitializerMu) {
          slopeInitializerMu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double slopeInitializerSigma;
    public double SlopeInitializerSigma {
      get { return slopeInitializerSigma; }
      set {
        if (slopeInitializerSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != slopeInitializerSigma) {
          slopeInitializerSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable]
    private double slopeManipulatorMu;
    public double SlopeManipulatorMu {
      get { return slopeManipulatorMu; }
      set {
        if (value != slopeManipulatorMu) {
          slopeManipulatorMu = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }
    [Storable]
    private double slopeManipulatorSigma;
    public double SlopeManipulatorSigma {
      get { return slopeManipulatorSigma; }
      set {
        if (slopeManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != slopeManipulatorSigma) {
          slopeManipulatorSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    public override bool Enabled {
      get {
        if (variableNames.Count == 0) return false;
        return base.Enabled;
      }
      set {
        if (variableNames.Count == 0) base.Enabled = false;
        else base.Enabled = value;
      }
    }

    private const int minimumArity = 2;
    private const int maximumArity = 2;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }
    #endregion

    #region persistence and cloning
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (allVariableNames == null || (allVariableNames.Count == 0 && variableNames.Count > 0)) {
        allVariableNames = variableNames;
      }
    }

    [StorableConstructor]
    private VariableCondition(bool deserializing)
      : base(deserializing) {
      variableNames = new List<string>();
      allVariableNames = new List<string>();
    }
    private VariableCondition(VariableCondition original, Cloner cloner)
      : base(original, cloner) {
      thresholdInitializerMu = original.thresholdInitializerMu;
      thresholdInitializerSigma = original.thresholdInitializerSigma;
      thresholdManipulatorMu = original.thresholdManipulatorMu;
      thresholdManipulatorSigma = original.thresholdManipulatorSigma;

      variableNames = new List<string>(original.variableNames);
      allVariableNames = new List<string>(original.allVariableNames);

      slopeInitializerMu = original.slopeInitializerMu;
      slopeInitializerSigma = original.slopeInitializerSigma;
      slopeManipulatorMu = original.slopeManipulatorMu;
      slopeManipulatorSigma = original.slopeManipulatorSigma;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableCondition(this, cloner);
    }
    #endregion

    public VariableCondition() : this("Variable Condition", "Represents a condition that tests a given variable.") { }
    public VariableCondition(string name, string description)
      : base(name, description) {
      thresholdInitializerMu = 0.0;
      thresholdInitializerSigma = 0.1;
      thresholdManipulatorMu = 0.0;
      thresholdManipulatorSigma = 0.1;

      variableNames = new List<string>();
      allVariableNames = new List<string>();

      slopeInitializerMu = 0.0;
      slopeInitializerSigma = 0.0;
      slopeManipulatorMu = 0.0;
      slopeManipulatorSigma = 0.0;
    }

    public override ISymbolicExpressionTreeNode CreateTreeNode() {
      return new VariableConditionTreeNode(this);
    }
  }
}
