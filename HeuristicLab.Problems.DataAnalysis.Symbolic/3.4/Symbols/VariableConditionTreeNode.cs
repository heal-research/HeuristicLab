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
using HEAL.Fossil;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("D3076BBC-7CF3-4768-8C4B-3FC4FEF041B3")]
  public sealed class VariableConditionTreeNode : SymbolicExpressionTreeNode, IVariableTreeNode {
    #region properties
    public new VariableCondition Symbol {
      get { return (VariableCondition)base.Symbol; }
    }
    [Storable]
    private double threshold;
    public double Threshold {
      get { return threshold; }
      set { threshold = value; }
    }
    [Storable]
    private string variableName;
    public string VariableName {
      get { return variableName; }
      set { variableName = value; }
    }
    [Storable]
    private double slope;
    public double Slope {
      get { return slope; }
      set { slope = value; }
    }
    #endregion

    [StorableConstructor]
    private VariableConditionTreeNode(StorableConstructorFlag _) : base(_) { }
    private VariableConditionTreeNode(VariableConditionTreeNode original, Cloner cloner)
      : base(original, cloner) {
      threshold = original.threshold;
      variableName = original.variableName;
      slope = original.slope;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableConditionTreeNode(this, cloner);
    }

    public VariableConditionTreeNode(VariableCondition variableConditionSymbol) : base(variableConditionSymbol) { }
    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      threshold = NormalDistributedRandom.NextDouble(random, Symbol.ThresholdInitializerMu, Symbol.ThresholdInitializerSigma);

#pragma warning disable 612, 618
      variableName = Symbol.VariableNames.SelectRandom(random);
#pragma warning restore 612, 618

      slope = NormalDistributedRandom.NextDouble(random, Symbol.SlopeInitializerMu, Symbol.SlopeInitializerSigma);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);
      double x = NormalDistributedRandom.NextDouble(random, Symbol.ThresholdManipulatorMu, Symbol.ThresholdManipulatorSigma);
      threshold = threshold + x * shakingFactor;

#pragma warning disable 612, 618
      variableName = Symbol.VariableNames.SelectRandom(random);
#pragma warning restore 612, 618

      x = NormalDistributedRandom.NextDouble(random, Symbol.SlopeManipulatorMu, Symbol.SlopeManipulatorSigma);
      slope = slope + x * shakingFactor;
    }

    public override string ToString() {
      if (slope.IsAlmost(0.0) || Symbol.IgnoreSlope) {
        return variableName + " < " + threshold.ToString("E4");
      } else {
        return variableName + " > " + threshold.ToString("E4") + Environment.NewLine +
               "slope: " + slope.ToString("E4");
      }
    }
  }
}
