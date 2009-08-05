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
using HeuristicLab.GP.Interfaces;
using System.Xml;

namespace HeuristicLab.GP {
  public class FunctionTreeBase : IFunctionTree {
    private List<IFunctionTree> subTrees;
    private IFunction function;

    public FunctionTreeBase() {
    }

    public FunctionTreeBase(IFunction function) {
      subTrees = new List<IFunctionTree>();
      this.function = function;
    }

    protected FunctionTreeBase(FunctionTreeBase original) {
      this.function = original.Function;
      this.subTrees = new List<IFunctionTree>(original.SubTrees.Count);
      foreach (IFunctionTree originalSubTree in original.SubTrees) {
        this.SubTrees.Add((IFunctionTree)originalSubTree.Clone());
      }
    }

    #region IFunctionTree Members
    public virtual bool HasLocalParameters {
      get { return false; }
    }

    public virtual IList<IFunctionTree> SubTrees {
      get { return subTrees; }
    }

    public IFunction Function {
      get { return function; }
      protected set { function = value; }
    }

    public int GetSize() {
      int size = 1;
      foreach (IFunctionTree tree in SubTrees) size += tree.GetSize();
      return size;
    }

    public int GetHeight() {
      int maxHeight = 0;
      foreach (IFunctionTree tree in SubTrees) maxHeight = Math.Max(maxHeight, tree.GetHeight());
      return maxHeight + 1;
    }

    public virtual IOperation CreateShakingOperation(IScope scope) {
      return null;
    }

    public virtual IOperation CreateInitOperation(IScope scope) {
      return null;
    }

    public void AddSubTree(IFunctionTree tree) {
      SubTrees.Add(tree);
    }

    public virtual void InsertSubTree(int index, IFunctionTree tree) {
      SubTrees.Insert(index, tree);
    }

    public virtual void RemoveSubTree(int index) {
      SubTrees.RemoveAt(index);
    }

    public virtual string ToString() {
      return Function.Name;
    }

    #endregion

    #region IStorable Members

    public Guid Guid {
      get { throw new NotSupportedException(); }
    }

    public virtual object Clone() {
      return new FunctionTreeBase(this);
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
