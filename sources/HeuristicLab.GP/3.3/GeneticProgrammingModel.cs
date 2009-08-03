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
  public class GeneticProgrammingModel : ItemBase, IGeneticProgrammingModel {
    private IFunctionTree functionTree;
    public IFunctionTree FunctionTree {
      get {
        return functionTree;
      }
      set {
        functionTree = value;
        Size = functionTree.GetSize();
        Height = functionTree.GetHeight();
      }
    }
    public int Size { get; set; }
    public int Height { get; set; }

    public GeneticProgrammingModel()
      : base() {
    }

    public GeneticProgrammingModel(IFunctionTree tree) : base() {
      FunctionTree = tree;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      GeneticProgrammingModel clone = (GeneticProgrammingModel)base.Clone(clonedObjects);
      clone.FunctionTree = (IFunctionTree)FunctionTree.Clone();
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      PersistTree(node, document, persistedObjects, FunctionTree);
      return node;
    }

    private void PersistTree(XmlNode node, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects, IFunctionTree tree) {
      XmlNode fNode = PersistenceManager.Persist(tree.Function, document, persistedObjects);
      XmlAttribute subTreesAttr = document.CreateAttribute("Args");
      subTreesAttr.Value = XmlConvert.ToString(tree.SubTrees.Count);
      fNode.Attributes.Append(subTreesAttr);
      node.AppendChild(fNode);
      XmlNode treeNode = tree.GetXmlNode("Data", document, persistedObjects);
      if (treeNode != null) fNode.AppendChild(treeNode);
      foreach (IFunctionTree subTree in tree.SubTrees) {
        PersistTree(node, document, persistedObjects, subTree);
      }
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      int nodeIndex = 0;
      FunctionTree = RestoreTree(node, ref nodeIndex, restoredObjects);
    }

    private IFunctionTree RestoreTree(XmlNode node, ref int nodeIndex, IDictionary<Guid, IStorable> restoredObjects) {
      XmlNode fNode = node.ChildNodes[nodeIndex];
      int subTrees = XmlConvert.ToInt32(fNode.Attributes["Args"].Value);
      IFunction f = (IFunction)PersistenceManager.Restore(fNode, restoredObjects);
      IFunctionTree tree = f.GetTreeNode();
      if(fNode.ChildNodes.Count>0) tree.Populate(fNode.ChildNodes[0], restoredObjects);
      nodeIndex++;
      for (int i = 0; i < subTrees; i++) {
        tree.AddSubTree(RestoreTree(node, ref nodeIndex, restoredObjects));
      }
      return tree;
    }

    public override IView CreateView() {
      return new FunctionTreeView(this.FunctionTree);
    }
  }
}
