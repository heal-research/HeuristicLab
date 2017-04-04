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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  public sealed class BinaryFactorVariableTreeNode : VariableTreeNodeBase {
    public new BinaryFactorVariable Symbol {
      get { return (BinaryFactorVariable)base.Symbol; }
    }

    [Storable]
    private string variableValue;
    public string VariableValue {
      get { return variableValue; }
      set { variableValue = value; }
    }

    [StorableConstructor]
    private BinaryFactorVariableTreeNode(bool deserializing) : base(deserializing) { }
    private BinaryFactorVariableTreeNode(BinaryFactorVariableTreeNode original, Cloner cloner)
      : base(original, cloner) {
      variableValue = original.variableValue;
    }
    public BinaryFactorVariableTreeNode(BinaryFactorVariable variableSymbol) : base(variableSymbol) { }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      variableValue = Symbol.GetVariableValues(VariableName).SampleRandom(random);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      // 50% additive & 50% multiplicative (override of functionality of base class because of a BUG)
      if (random.NextDouble() < 0.5) {
        double x = NormalDistributedRandom.NextDouble(random, Symbol.WeightManipulatorMu, Symbol.WeightManipulatorSigma);
        Weight = Weight + x * shakingFactor;
      } else {
        double x = NormalDistributedRandom.NextDouble(random, 1.0, Symbol.MultiplicativeWeightManipulatorSigma);
        Weight = Weight * x;
      }
      if (random.NextDouble() < Symbol.VariableChangeProbability) {
        var oldName = VariableName;
        VariableName = Symbol.VariableNames.SampleRandom(random);
        // reinitialize weights if variable has changed (similar to FactorVariableTreeNode)
        if (oldName != VariableName)
          Weight = NormalDistributedRandom.NextDouble(random, Symbol.WeightMu, Symbol.WeightSigma);
      }
      variableValue = Symbol.GetVariableValues(VariableName).SampleRandom(random);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryFactorVariableTreeNode(this, cloner);
    }

    public override string ToString() {
      return base.ToString() + " = " + variableValue;
    }
  }
}
