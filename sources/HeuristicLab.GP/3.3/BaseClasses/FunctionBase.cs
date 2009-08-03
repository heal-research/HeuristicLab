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
using System.Diagnostics;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP {
  public abstract class FunctionBase : ItemBase, IFunction {
    private List<List<IFunction>> allowedSubFunctions = new List<List<IFunction>>();
    private int minArity = -1;
    private int maxArity = -1;
    private double tickets = 1.0;
    private IOperator initializer;
    private IOperator manipulator;
    private int minTreeHeight = -1;
    private int minTreeSize = -1;

    public virtual string Name {
      get { return this.GetType().Name; }
    }

    public virtual string Description {
      get { return "Description for this function is missing (TODO)"; }
    }

    public int MinSubTrees {
      get {
        return minArity;
      }
      protected set {
        minArity = value;
        while (minArity > allowedSubFunctions.Count) allowedSubFunctions.Add(new List<IFunction>());
      }
    }

    public int MaxSubTrees {
      get {
        return maxArity;
      }
      protected set {
        maxArity = value;
        while (allowedSubFunctions.Count > maxArity) allowedSubFunctions.RemoveAt(allowedSubFunctions.Count - 1);
        while (maxArity > allowedSubFunctions.Count) allowedSubFunctions.Add(new List<IFunction>());
      }
    }


    public int MinTreeSize {
      get {
        if (minTreeSize <= 0) RecalculateMinimalTreeSize();
        return minTreeSize;
      }
    }

    public int MinTreeHeight {
      get {
        if (minTreeHeight <= 0) RecalculateMinimalTreeHeight();
        return minTreeHeight;
      }
    }

    public double Tickets {
      get { return tickets; }
      set {
        if (value < 0.0) throw new ArgumentException("Number of tickets must be positive");
        else tickets = value;
      }
    }

    public IOperator Initializer {
      get { return initializer; }
      set { initializer = value; }
    }

    public IOperator Manipulator {
      get { return manipulator; }
      set { manipulator = value; }
    }

    public virtual IFunctionTree GetTreeNode() {
      return new FunctionTreeBase(this);
    }

    public ICollection<IFunction> GetAllowedSubFunctions(int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      return allowedSubFunctions[index];
    }

    public void AddAllowedSubFunction(IFunction function, int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      if (allowedSubFunctions[index] == null) {
        allowedSubFunctions[index] = new List<IFunction>();
      }
      if (!allowedSubFunctions[index].Contains(function)) {
        allowedSubFunctions[index].Add(function);
      }
    }

    public void RemoveAllowedSubFunction(IFunction function, int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      allowedSubFunctions[index].Add(function);
    }

    public bool IsAllowedSubFunction(IFunction function, int index) {
      return GetAllowedSubFunctions(index).Contains(function);
    }

    private void RecalculateMinimalTreeSize() {
      minTreeSize = int.MaxValue;
      int sum = 1;
      int minSize = int.MaxValue;
      for (int i = 0; i < MinSubTrees; i++) {
        foreach (IFunction subFun in GetAllowedSubFunctions(i)) {
          minSize = Math.Min(minSize, subFun.MinTreeSize);
        }
        sum += minSize;
      }
      minTreeSize = sum;
    }

    private void RecalculateMinimalTreeHeight() {
      minTreeHeight = int.MaxValue;
      int height = 0;
      int minHeight = int.MaxValue;
      for (int i = 0; i < MinSubTrees; i++) {
        foreach (IFunction subFun in GetAllowedSubFunctions(i)) {
          minHeight = Math.Min(minHeight, subFun.MinTreeHeight);
        }
        height = Math.Max(height, minHeight);
      }
      minTreeHeight = height + 1;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      FunctionBase clone = (FunctionBase)base.Clone(clonedObjects);
      clone.initializer = (IOperator)Auxiliary.Clone(initializer, clonedObjects);
      clone.manipulator = (IOperator)Auxiliary.Clone(manipulator, clonedObjects);
      clone.maxArity = maxArity;
      clone.minArity = minArity;
      clone.minTreeHeight = minTreeHeight;
      clone.minTreeSize = minTreeSize;
      clone.tickets = tickets;
      for (int i = 0; i < MaxSubTrees; i++) {
        var allowedSubFunctionsForSlot = new List<IFunction>();
        foreach (IFunction f in GetAllowedSubFunctions(i)) {
          allowedSubFunctionsForSlot.Add((IFunction)Auxiliary.Clone(f, clonedObjects));
        }
        clone.allowedSubFunctions.Add(allowedSubFunctionsForSlot);
      }
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute minSubTreesAttr = document.CreateAttribute("MinSubTrees");
      minSubTreesAttr.Value = XmlConvert.ToString(MinSubTrees);
      XmlAttribute maxSubTreesAttr = document.CreateAttribute("MaxSubTrees");
      maxSubTreesAttr.Value = XmlConvert.ToString(MaxSubTrees);
      node.Attributes.Append(minSubTreesAttr);
      node.Attributes.Append(maxSubTreesAttr);
      for (int i = 0; i < MaxSubTrees; i++) {
        XmlNode slotNode = document.CreateElement("AllowedSubFunctions");
        XmlAttribute slotAttr = document.CreateAttribute("Slot");
        slotAttr.Value = XmlConvert.ToString(i);
        slotNode.Attributes.Append(slotAttr);
        node.AppendChild(slotNode);
        foreach (IFunction f in GetAllowedSubFunctions(i)) {
          slotNode.AppendChild(PersistenceManager.Persist(f, document, persistedObjects));
        }
      }
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      MinSubTrees = XmlConvert.ToInt32(node.Attributes["MinSubTrees"].Value);
      MaxSubTrees = XmlConvert.ToInt32(node.Attributes["MaxSubTrees"].Value);
      foreach (XmlNode allowedSubFunctionsNode in node.SelectNodes("AllowedSubFunctions")) {
        int slot = XmlConvert.ToInt32(allowedSubFunctionsNode.Attributes["Slot"].Value);
        foreach (XmlNode fNode in allowedSubFunctionsNode.ChildNodes) {
          AddAllowedSubFunction((IFunction)PersistenceManager.Restore(fNode, restoredObjects), slot);
        }
      }
    }
  }
}
