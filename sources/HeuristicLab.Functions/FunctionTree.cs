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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using System.Xml;

namespace HeuristicLab.Functions {
  public class FunctionTree : ItemBase, IFunctionTree {

    private List<IFunctionTree> subTrees;
    private List<IVariable> localVariables;
    private IFunction function;

    public FunctionTree()
      : base() {
      subTrees = new List<IFunctionTree>();
      localVariables = new List<IVariable>();
    }

    internal FunctionTree(IFunction function)
      : this() {
      this.function = function;
      // create and store clones of all local variables of the function
      foreach(VariableInfo info in function.VariableInfos) {
        if(info.Local) {
          AddVariable((IVariable)function.GetVariable(info.FormalName).Clone());
        }
      }
    }

    #region IFunctionTree Members

    public IList<IFunctionTree> SubTrees {
      get { return subTrees; }
    }

    public ICollection<IVariable> LocalVariables {
      get { return localVariables; }
    }

    public IFunction Function {
      get { return function; }
    }

    public IVariable GetLocalVariable(string name) {
      foreach(IVariable var in localVariables) {
        if(var.Name == name) return var;
      }
      return null;
    }

    public void AddVariable(IVariable variable) {
      localVariables.Add(variable);
    }

    public void RemoveVariable(string name) {
      localVariables.Remove(GetLocalVariable(name));
    }

    public void AddSubTree(IFunctionTree tree) {
      subTrees.Add(tree);
    }

    public void InsertSubTree(int index, IFunctionTree tree) {
      subTrees.Insert(index, tree);
    }

    public void RemoveSubTree(int index) {
      subTrees.RemoveAt(index);
    }

    public virtual double Evaluate(Dataset dataset, int sampleIndex) {
      double[] evaluationResults = new double[SubTrees.Count];
      for(int i = 0; i < evaluationResults.Length; i++) {
        evaluationResults[i] = SubTrees[i].Evaluate(dataset, sampleIndex);
      }
      return function.Apply(dataset, sampleIndex, evaluationResults);
    }
    #endregion

    public override XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Function", function, document, persistedObjects));
      XmlNode subTreesNode = document.CreateNode(XmlNodeType.Element, "SubTrees", null);
      for(int i = 0; i < subTrees.Count; i++)
        subTreesNode.AppendChild(PersistenceManager.Persist(subTrees[i], document, persistedObjects));
      node.AppendChild(subTreesNode);
      XmlNode variablesNode = document.CreateNode(XmlNodeType.Element, "Variables", null);
      foreach(IVariable variable in localVariables)
        variablesNode.AppendChild(PersistenceManager.Persist(variable, document, persistedObjects));
      node.AppendChild(variablesNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      function = (IFunction)PersistenceManager.Restore(node.SelectSingleNode("Function"), restoredObjects);
      XmlNode subTreesNode = node.SelectSingleNode("SubTrees");
      for(int i = 0; i < subTreesNode.ChildNodes.Count; i++)
        AddSubTree((IFunctionTree)PersistenceManager.Restore(subTreesNode.ChildNodes[i], restoredObjects));
      XmlNode variablesNode = node.SelectSingleNode("Variables");
      foreach(XmlNode variableNode in variablesNode.ChildNodes)
        AddVariable((IVariable)PersistenceManager.Restore(variableNode, restoredObjects));
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      FunctionTree clone = (FunctionTree)base.Clone(clonedObjects);
      foreach(IFunctionTree tree in subTrees) {
        clone.AddSubTree((IFunctionTree)tree.Clone(clonedObjects));
      }
      foreach(IVariable variable in localVariables) {
        clone.AddVariable((IVariable)variable.Clone(clonedObjects));
      }
      clone.function = function;
      return clone;
    }

    public override IView CreateView() {
      return new FunctionTreeView(this);
    }
  }
}