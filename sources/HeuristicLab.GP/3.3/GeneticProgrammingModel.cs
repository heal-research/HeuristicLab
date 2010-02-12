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

    public GeneticProgrammingModel(IFunctionTree tree)
      : base() {
      FunctionTree = tree;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      GeneticProgrammingModel clone = (GeneticProgrammingModel)base.Clone(clonedObjects);
      clone.FunctionTree = (IFunctionTree)FunctionTree.Clone();
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      // persist the tree in linear form
      PersistTree(node, document, persistedObjects, FunctionTree);
      return node;
    }

    private void PersistTree(XmlNode node, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects, IFunctionTree tree) {
      XmlNode fNode = PersistenceManager.Persist(tree.Function, document, persistedObjects);
      // save the number of sub-trees
      XmlAttribute subTreesAttr = document.CreateAttribute("Args");
      subTreesAttr.Value = XmlConvert.ToString(tree.SubTrees.Count);
      fNode.Attributes.Append(subTreesAttr);
      // save the function symbol
      node.AppendChild(fNode);
      // if the tree node has local data save it into a child element called "data"
      XmlNode treeNode = tree.GetXmlNode("Data", document, persistedObjects);
      if (treeNode != null) fNode.AppendChild(treeNode);
      // recursivly store the children into the same linear form
      foreach (IFunctionTree subTree in tree.SubTrees) {
        PersistTree(node, document, persistedObjects, subTree);
      }
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      int nodeIndex = 0;
      // restore linear form back into tree form
      FunctionTree = RestoreTree(node, ref nodeIndex, restoredObjects);
    }

    private IFunctionTree RestoreTree(XmlNode node, ref int nodeIndex, IDictionary<Guid, IStorable> restoredObjects) {
      XmlNode fNode = node.ChildNodes[nodeIndex];
      // restore the number of child nodes
      int subTrees = XmlConvert.ToInt32(fNode.Attributes["Args"].Value);
      // restore the function symbol
      IFunction f = (IFunction)PersistenceManager.Restore(fNode, restoredObjects);
      // create a tree node from the function
      IFunctionTree tree = f.GetTreeNode();
      // check if there is data for the tree node that needs to be restored and restore the data if needed
      var dataNode = fNode.SelectSingleNode("Data");
      if (dataNode!=null) tree.Populate(dataNode, restoredObjects);
      nodeIndex++;
      for (int i = 0; i < subTrees; i++) {
        // recursively read children from linear representation
        tree.AddSubTree(RestoreTree(node, ref nodeIndex, restoredObjects));
      }
      return tree;
    }

    public override IView CreateView() {
      return new GeneticProgrammingModelView(this);
    }
  }
}
