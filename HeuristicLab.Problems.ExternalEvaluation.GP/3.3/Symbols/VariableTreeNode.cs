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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [StorableClass]
  public sealed class VariableTreeNode : SymbolicExpressionTreeTerminalNode {
    public new Variable Symbol {
      get { return (Variable)base.Symbol; }
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


    private VariableTreeNode() { }

    [StorableConstructor]
    private VariableTreeNode(bool deserializing) : base(deserializing) { }
    private VariableTreeNode(VariableTreeNode original, Cloner cloner)
      : base(original, cloner) {
      weight = original.weight;
      variableName = original.variableName;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableTreeNode(this, cloner);
    }

    public VariableTreeNode(Variable variableSymbol) : base(variableSymbol) { }

    public override bool HasLocalParameters {
      get { return true; }
    }

    public override void ResetLocalParameters(IRandom random) {
      base.ResetLocalParameters(random);
      var normalDistributedRNG = new NormalDistributedRandom(random, Symbol.WeightNu, Symbol.WeightSigma);
      weight = normalDistributedRNG.NextDouble();
      variableName = Symbol.VariableNames.SelectRandom(random);
    }

    public override void ShakeLocalParameters(IRandom random, double shakingFactor) {
      base.ShakeLocalParameters(random, shakingFactor);
      var normalDistributedRNG = new NormalDistributedRandom(random, Symbol.WeightManipulatorNu, Symbol.WeightManipulatorSigma);
      double x = normalDistributedRNG.NextDouble();
      weight = weight + x * shakingFactor;
      variableName = Symbol.VariableNames.SelectRandom(random);
    }

    public override string ToString() {
      return ";" + variableName + ";" + weight.ToString("E4");
    }
  }
}
