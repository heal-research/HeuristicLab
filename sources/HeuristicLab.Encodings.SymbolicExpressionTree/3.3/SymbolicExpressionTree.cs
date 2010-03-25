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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using System.Xml;

namespace HeuristicLab.Encodings.SymbolicExpressionTree {
  public class SymbolicExpressionTree  {
    private List<SymbolicExpressionTree> subTrees;
    private IFunction function;

    public SymbolicExpressionTree() {
    }

    public SymbolicExpressionTree(Symbol symbol) {
      subTrees = new List<SymbolicExpressionTree>();
      this.function = function;
    }

    protected SymbolicExpressionTree(SymbolicExpressionTree original) {
      this.function = original.Function;
      this.subTrees = new List<SymbolicExpressionTree>(original.SubTrees.Count);
      foreach (SymbolicExpressionTree originalSubTree in original.SubTrees) {
        this.SubTrees.Add((SymbolicExpressionTree)originalSubTree.Clone());
      }
    }

    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<SymbolicExpressionTree> SubTrees {
      get { return subTrees; }
    }

    public Symbol Function {
      get { return function; }
      protected set { function = value; }
    }

    public int GetSize() {
      int size = 1;
      foreach (SymbolicExpressionTree tree in SubTrees) size += tree.GetSize();
      return size;
    }

    public int GetHeight() {
      int maxHeight = 0;
      foreach (SymbolicExpressionTree tree in SubTrees) maxHeight = Math.Max(maxHeight, tree.GetHeight());
      return maxHeight + 1;
    }

    public virtual IOperation CreateShakingOperation(IScope scope) {
      return null;
    }

    public virtual IOperation CreateInitOperation(IScope scope) {
      return null;
    }

    public void AddSubTree(SymbolicExpressionTree tree) {
      SubTrees.Add(tree);
    }

    public virtual void InsertSubTree(int index, SymbolicExpressionTree tree) {
      SubTrees.Insert(index, tree);
    }

    public virtual void RemoveSubTree(int index) {
      SubTrees.RemoveAt(index);
    }

    public override string ToString() {
      return Function.Name;
    }


    #region IStorable Members

    public Guid Guid {
      get { throw new NotSupportedException(); }
    }

    public virtual object Clone() {
      return new SymbolicExpressionTree(this);
    }

    public object Clone(IDictionary<Guid, object> clonedObjects) {
      throw new NotImplementedException();
    }

    public virtual XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      return null;
    }

    public virtual void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
    }

    #endregion
  }
}
