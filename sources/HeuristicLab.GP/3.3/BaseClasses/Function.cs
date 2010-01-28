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
using System.Linq;

namespace HeuristicLab.GP {
  public abstract class Function : ItemBase, IFunction {
    private List<List<IFunction>> allowedSubFunctions = new List<List<IFunction>>();
    private int minArity = -1;
    private int maxArity = -1;
    private double tickets = 1.0;
    private IOperator initializer;
    private IOperator manipulator;
    private int minTreeHeight = -1;
    private int minTreeSize = -1;

    private string name;
    public virtual string Name {
      get { return name; }
      set {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException();
        if (value != name) {
          name = value;
          FireChanged();
        }
      }
    }

    protected Function() {
      name = this.GetType().Name;
    }

    public virtual string Description {
      get { return "Description for this function is missing (TODO)"; }
    }

    public int MinSubTrees {
      get {
        return minArity;
      }
      protected internal set {
        if (value < 0) throw new ArgumentException();
        if (minArity != value) {
          minArity = value;
          while (minArity > allowedSubFunctions.Count) allowedSubFunctions.Add(new List<IFunction>());
          ResetCachedValues();
          FireChanged();
        }
      }
    }

    public int MaxSubTrees {
      get {
        return maxArity;
      }
      protected internal set {
        if (value < 0) throw new ArgumentException();
        if (value < minArity) throw new ArgumentException();
        if (value != maxArity) {
          maxArity = value;
          while (allowedSubFunctions.Count > maxArity) allowedSubFunctions.RemoveAt(allowedSubFunctions.Count - 1);
          while (maxArity > allowedSubFunctions.Count) {
            if (allowedSubFunctions.Count > 0) {
              // copy the list of allowed sub-functions from the previous slot
              allowedSubFunctions.Add(new List<IFunction>(allowedSubFunctions[allowedSubFunctions.Count - 1]));
            } else {
              // add empty list
              allowedSubFunctions.Add(new List<IFunction>());
            }
          }
          ResetCachedValues();
          FireChanged();
        }
      }
    }


    public int MinTreeSize {
      get {
        if (minTreeSize <= 0) {
          RecalculateMinimalTreeSize();
          FireChanged();
        }
        Debug.Assert(minTreeSize > 0);
        return minTreeSize;
      }
    }

    public int MinTreeHeight {
      get {
        if (minTreeHeight <= 0) {
          RecalculateMinimalTreeHeight();
          FireChanged();
        }
        Debug.Assert(minTreeHeight > 0);
        return minTreeHeight;
      }
    }

    public double Tickets {
      get { return tickets; }
      set {
        if (value < 0.0) throw new ArgumentException("Number of tickets must be positive");
        if (value != tickets) {
          tickets = value;
          FireChanged();
        }
      }
    }

    public IOperator Initializer {
      get { return initializer; }
      set {
        if (initializer != value) {
          initializer = value;
          FireChanged();
        }
      }
    }

    public IOperator Manipulator {
      get { return manipulator; }
      set {
        if (manipulator != value) {
          manipulator = value;
          FireChanged();
        }
      }
    }

    public virtual IFunctionTree GetTreeNode() {
      return new FunctionTree(this);
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
      ResetCachedValues();
      FireChanged();
    }
    public void RemoveAllowedSubFunction(IFunction function, int index) {
      if (index < 0 || index > MaxSubTrees) throw new ArgumentException("Index outside of allowed range. index = " + index);
      if (allowedSubFunctions[index].Contains(function)) {
        allowedSubFunctions[index].Remove(function);
        ResetCachedValues();
        FireChanged();
      }
    }

    private void ResetCachedValues() {
      minTreeHeight = -1;
      minTreeSize = -1;
    }

    public bool IsAllowedSubFunction(IFunction function, int index) {
      return GetAllowedSubFunctions(index).Contains(function);
    }

    private void RecalculateMinimalTreeSize() {
      if (MinSubTrees == 0) minTreeSize = 1;
      else {
        minTreeSize = int.MaxValue; // prevent infinite recursion        
        minTreeSize = 1 + (from slot in Enumerable.Range(0, MinSubTrees)
                           let minForSlot = (from function in GetAllowedSubFunctions(slot)
                                             where function != this
                                             select function.MinTreeSize).DefaultIfEmpty(0).Min()
                           select minForSlot).Sum();
      }
    }

    private void RecalculateMinimalTreeHeight() {
      if (MinSubTrees == 0) minTreeHeight = 1;
      else {
        minTreeHeight = int.MaxValue;
        minTreeHeight = 1 + (from slot in Enumerable.Range(0, MinSubTrees)
                             let minForSlot = (from function in GetAllowedSubFunctions(slot)
                                               where function != this
                                               select function.MinTreeHeight).DefaultIfEmpty(0).Min()
                             select minForSlot).Max();
      }
    }

    public override IView CreateView() {
      return new FunctionView(this);
    }

    public override string ToString() {
      return name;
    }

    #region persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Function clone = (Function)base.Clone(clonedObjects);
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
    #endregion
  }
}
