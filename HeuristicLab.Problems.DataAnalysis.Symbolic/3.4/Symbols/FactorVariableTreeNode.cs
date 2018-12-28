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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Fossil;
using HeuristicLab.Random;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("A968620F-339E-4C96-B39A-8FC8E42D6509")]
  public sealed class FactorVariableTreeNode : SymbolicExpressionTreeTerminalNode, IVariableTreeNode {
    public new FactorVariable Symbol {
      get { return (FactorVariable)base.Symbol; }
    }
    [Storable]
    private double[] weights;
    public double[] Weights {
      get { return weights; }
      set { weights = value; }
    }
    [Storable]
    private string variableName;
    public string VariableName {
      get { return variableName; }
      set { variableName = value; }
    }

    [StorableConstructor]
    private FactorVariableTreeNode(StorableConstructorFlag _) : base(_) { }
    private FactorVariableTreeNode(FactorVariableTreeNode original, Cloner cloner)
      : base(original, cloner) {
      variableName = original.variableName;
      if (original.weights != null) {
        this.weights = new double[original.Weights.Length];
        Array.Copy(original.Weights, weights, weights.Length);
      }
    }

    public FactorVariableTreeNode(FactorVariable variableSymbol)
      : base(variableSymbol) {
    }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      variableName = Symbol.VariableNames.SampleRandom(random);
      weights =
        Symbol.GetVariableValues(variableName)
        .Select(_ => NormalDistributedRandom.NextDouble(random, 0, 1)).ToArray();
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      // mutate only one randomly selected weight
      var idx = random.Next(weights.Length);
      // 50% additive & 50% multiplicative
      if (random.NextDouble() < 0.5) {
        double x = NormalDistributedRandom.NextDouble(random, Symbol.WeightManipulatorMu,
          Symbol.WeightManipulatorSigma);
        weights[idx] = weights[idx] + x * shakingFactor;
      } else {
        double x = NormalDistributedRandom.NextDouble(random, 1.0, Symbol.MultiplicativeWeightManipulatorSigma);
        weights[idx] = weights[idx] * x;
      }
      if (random.NextDouble() < Symbol.VariableChangeProbability) {
        VariableName = Symbol.VariableNames.SampleRandom(random);
        if (weights.Length != Symbol.GetVariableValues(VariableName).Count()) {
          // if the length of the weight array does not match => re-initialize weights
          weights =
            Symbol.GetVariableValues(variableName)
              .Select(_ => NormalDistributedRandom.NextDouble(random, 0, 1))
              .ToArray();
        }
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FactorVariableTreeNode(this, cloner);
    }

    public double GetValue(string cat) {
      return weights[Symbol.GetIndexForValue(VariableName, cat)];
    }

    public override string ToString() {
      var weightStr = string.Join("; ",
        Symbol.GetVariableValues(VariableName).Select(value => value + ": " + GetValue(value).ToString("E4")));
      return VariableName + " (factor) "
        + "[" + weightStr + "]";
    }
  }
}

