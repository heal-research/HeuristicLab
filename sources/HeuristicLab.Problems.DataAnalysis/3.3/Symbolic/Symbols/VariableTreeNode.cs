#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using System;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols {
  [StorableClass]
  public sealed class VariableTreeNode : SymbolicExpressionTreeNode {
    private static List<SymbolicExpressionTreeNode> emptySubTrees = new List<SymbolicExpressionTreeNode>();
    public override bool HasLocalParameters {
      get {
        return true;
      }
    }

    private double weight;
    [Storable]
    public double Weight {
      get { return weight; }
      set { weight = value; }
    }
    private string variableName;
    [Storable]
    public string VariableName {
      get { return variableName; }
      set { variableName = value; }
    }

    // copy constructor
    private VariableTreeNode(VariableTreeNode original)
      : base(original) {
      weight = original.weight;
      variableName = original.variableName;
    }

    public VariableTreeNode(Variable variableSymbol) : base(variableSymbol) { }

    public override object Clone() {
      return new VariableTreeNode(this);
    }

    protected override void AddSubTree(SymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    protected override void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    protected override void RemoveSubTree(int index) {
      throw new NotSupportedException();
    }
    public override System.Collections.Generic.IList<SymbolicExpressionTreeNode> SubTrees {
      get {
        return emptySubTrees;
      }
    }
  }
}
