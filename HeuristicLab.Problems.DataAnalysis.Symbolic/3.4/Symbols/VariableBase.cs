#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("F8A6AD96-28D9-4BEC-8392-8B7BA824B085")]
  public abstract class VariableBase : Symbol, IVariableSymbol {
    #region Properties
    [Storable]
    private double weightMu;
    public double WeightMu {
      get { return weightMu; }
      set {
        if (value != weightMu) {
          weightMu = value;
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
    private double weightManipulatorMu;
    public double WeightManipulatorMu {
      get { return weightManipulatorMu; }
      set {
        if (value != weightManipulatorMu) {
          weightManipulatorMu = value;
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
    [Storable(DefaultValue = 0.0)]
    private double multiplicativeWeightManipulatorSigma;
    public double MultiplicativeWeightManipulatorSigma {
      get { return multiplicativeWeightManipulatorSigma; }
      set {
        if (multiplicativeWeightManipulatorSigma < 0.0) throw new ArgumentException("Negative sigma is not allowed.");
        if (value != multiplicativeWeightManipulatorSigma) {
          multiplicativeWeightManipulatorSigma = value;
          OnChanged(EventArgs.Empty);
        }
      }
    }

    [Storable(DefaultValue = 1.0)]
    private double variableChangeProbability;

    public double VariableChangeProbability {
      get { return variableChangeProbability; }
      set {
        if (value < 0 || value > 1.0) throw new ArgumentException("Variable change probability must lie in the interval [0..1]");
        variableChangeProbability = value;
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

    private const int minimumArity = 0;
    private const int maximumArity = 0;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (allVariableNames == null || (allVariableNames.Count == 0 && variableNames.Count > 0)) {
        allVariableNames = variableNames;
      }
    }

    [StorableConstructor]
    protected VariableBase(StorableConstructorFlag _) : base(_) {
      variableNames = new List<string>();
      allVariableNames = new List<string>();
    }
    protected VariableBase(VariableBase original, Cloner cloner)
      : base(original, cloner) {
      weightMu = original.weightMu;
      weightSigma = original.weightSigma;
      variableNames = new List<string>(original.variableNames);
      allVariableNames = new List<string>(original.allVariableNames);
      weightManipulatorMu = original.weightManipulatorMu;
      weightManipulatorSigma = original.weightManipulatorSigma;
      multiplicativeWeightManipulatorSigma = original.multiplicativeWeightManipulatorSigma;
      variableChangeProbability = original.variableChangeProbability;
    }
    protected VariableBase(string name, string description)
      : base(name, description) {
      weightMu = 1.0;
      weightSigma = 1.0;
      weightManipulatorMu = 0.0;
      weightManipulatorSigma = 0.05;
      multiplicativeWeightManipulatorSigma = 0.03;
      variableChangeProbability = 0.2;
      variableNames = new List<string>();
      allVariableNames = new List<string>();
    }
  }
}
