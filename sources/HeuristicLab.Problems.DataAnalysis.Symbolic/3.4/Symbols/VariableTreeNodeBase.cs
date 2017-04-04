#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  public abstract class VariableTreeNodeBase : SymbolicExpressionTreeTerminalNode, IVariableTreeNode {
    public new VariableBase Symbol {
      get { return (VariableBase)base.Symbol; }
    }
    [Storable]
    private double weight;
    public double Weight {
      get { return weight; }
      set { weight = value; }
    }
    [Storable]
    private string variableName;
    public string VariableName {
      get { return variableName; }
      set { variableName = value; }
    }

    [StorableConstructor]
    protected VariableTreeNodeBase(bool deserializing) : base(deserializing) { }
    protected VariableTreeNodeBase(VariableTreeNodeBase original, Cloner cloner)
      : base(original, cloner) {
      weight = original.weight;
      variableName = original.variableName;
    }
    protected VariableTreeNodeBase() { }
    protected VariableTreeNodeBase(VariableBase variableSymbol) : base(variableSymbol) { }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      weight = NormalDistributedRandom.NextDouble(random, Symbol.WeightMu, Symbol.WeightSigma);

#pragma warning disable 612, 618
      variableName = Symbol.VariableNames.SelectRandom(random);
#pragma warning restore 612, 618
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);

      // 50% additive & 50% multiplicative (TODO: BUG in if statement below -> fix in HL 4.0!)
      if (random.NextDouble() < 0) {
        double x = NormalDistributedRandom.NextDouble(random, Symbol.WeightManipulatorMu, Symbol.WeightManipulatorSigma);
        weight = weight + x * shakingFactor;
      } else {
        double x = NormalDistributedRandom.NextDouble(random, 1.0, Symbol.MultiplicativeWeightManipulatorSigma);
        weight = weight * x;
      }

      if (Symbol.VariableChangeProbability >= 1.0 || random.NextDouble() < Symbol.VariableChangeProbability) {
        var oldName = variableName;
#pragma warning disable 612, 618
        variableName = Symbol.VariableNames.SelectRandom(random);
#pragma warning restore 612, 618
        if (oldName != variableName) {
          weight = NormalDistributedRandom.NextDouble(random, Symbol.WeightMu, Symbol.WeightSigma);
        }
      }
    }

    public override string ToString() {
      if (weight.IsAlmost(1.0)) return variableName;
      else return weight.ToString("E4") + " " + variableName;
    }
  }
}
