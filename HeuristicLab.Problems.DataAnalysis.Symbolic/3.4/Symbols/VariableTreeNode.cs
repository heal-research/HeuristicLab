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

using HeuristicLab.Common;
using HEAL.Attic;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("E60C4216-A6BA-48DE-BA66-389B9946B70A")]
  public sealed class VariableTreeNode : VariableTreeNodeBase {
    public new Variable Symbol {
      get { return (Variable)base.Symbol; }
    }
    [StorableConstructor]
    private VariableTreeNode(StorableConstructorFlag _) : base(_) { }
    private VariableTreeNode(VariableTreeNode original, Cloner cloner)
      : base(original, cloner) {
    }
    private VariableTreeNode() { }
    public VariableTreeNode(Variable variableSymbol) : base(variableSymbol) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new VariableTreeNode(this, cloner);
    }
  }
}
