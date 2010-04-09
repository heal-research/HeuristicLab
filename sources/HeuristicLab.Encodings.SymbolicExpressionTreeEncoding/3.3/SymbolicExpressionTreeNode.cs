#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableClass]
  public class SymbolicExpressionTreeNode : ICloneable {
    [Storable]
    private List<SymbolicExpressionTreeNode> subTrees;
    [Storable]
    private Symbol symbol;

    public SymbolicExpressionTreeNode() { }

    public SymbolicExpressionTreeNode(Symbol symbol) {
      subTrees = new List<SymbolicExpressionTreeNode>();
      this.symbol = symbol;
    }

    // copy constructor
    protected SymbolicExpressionTreeNode(SymbolicExpressionTreeNode original) {
      symbol = original.symbol;
      subTrees = new List<SymbolicExpressionTreeNode>();
      foreach (var subtree in original.SubTrees) {
        AddSubTree((SymbolicExpressionTreeNode)subtree.Clone());
      }
      dynamicSymbols = new Dictionary<string, int>(original.dynamicSymbols);
    }

    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<SymbolicExpressionTreeNode> SubTrees {
      get { return subTrees; }
    }

    public Symbol Symbol {
      get { return symbol; }
      protected set { symbol = value; }
    }

    public int GetSize() {
      int size = 1;
      foreach (SymbolicExpressionTreeNode tree in SubTrees) size += tree.GetSize();
      return size;
    }

    public int GetHeight() {
      int maxHeight = 0;
      foreach (SymbolicExpressionTreeNode tree in SubTrees) maxHeight = Math.Max(maxHeight, tree.GetHeight());
      return maxHeight + 1;
    }

    [Storable]
    private Dictionary<string, int> dynamicSymbols = new Dictionary<string, int>();
    public void AddDynamicSymbol(string symbolName) {
      Debug.Assert(!dynamicSymbols.ContainsKey(symbolName));
      dynamicSymbols[symbolName] = 0;
    }

    public void AddDynamicSymbol(string symbolName, int nArguments) {
      AddDynamicSymbol(symbolName);
      SetDynamicSymbolArgumentCount(symbolName, nArguments);
    }

    public void RemoveDynamicSymbol(string symbolName) {
      dynamicSymbols.Remove(symbolName);
    }

    public IEnumerable<string> DynamicSymbols {
      get { return dynamicSymbols.Keys; }
    }

    public int GetDynamicSymbolArgumentCount(string symbolName) {
      return dynamicSymbols[symbolName];
    }
    public void SetDynamicSymbolArgumentCount(string symbolName, int nArguments) {
      Debug.Assert(dynamicSymbols.ContainsKey(symbolName));
      dynamicSymbols[symbolName] = nArguments;
    }

    public virtual void ResetLocalParameters(IRandom random) { }
    public virtual void ShakeLocalParameters(IRandom random, double shakingFactor) { }

    protected internal virtual void AddSubTree(SymbolicExpressionTreeNode tree) {
      SubTrees.Add(tree);
    }

    protected internal virtual void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      SubTrees.Insert(index, tree);
    }

    protected internal virtual void RemoveSubTree(int index) {
      SubTrees.RemoveAt(index);
    }

    #region ICloneable Members

    public virtual object Clone() {
      return new SymbolicExpressionTreeNode(this);
    }

    #endregion
  }
}
