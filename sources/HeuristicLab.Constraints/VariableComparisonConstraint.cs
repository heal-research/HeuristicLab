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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  public class VariableComparisonConstraint : ConstraintBase {
    private StringData leftVarName;
    public StringData LeftVarName {
      get { return leftVarName; }
      set { leftVarName = value; }
    }

    private StringData rightVarName;
    public StringData RightVarName {
      get { return rightVarName; }
      set { rightVarName = value; }
    }

    private IntData comparer;
    public IntData Comparer {
      get { return comparer; }
      set { comparer = value; }
    }

    public override string Description {
      get {
        return @"Compares items in a ConstrainedItemList";
      }
    }

    public VariableComparisonConstraint() {
      leftVarName = new StringData();
      rightVarName = new StringData();
      comparer = new IntData(-1);
    }

    public override bool Check(IItem data) {
      ConstrainedItemList list = (data as ConstrainedItemList);
      if (list == null) throw new InvalidOperationException("ERROR in VariableComparisonConstraint: Can only be applied on ConstrainedItemLists");
      IComparable left = null;
      IItem right = null;
      foreach (IItem item in list) {
        Variable tmp = (item as Variable);
        if (tmp != null && tmp.Name.Equals(leftVarName.Data)) {
          left = (tmp.Value as IComparable);
          if (left == null) throw new InvalidCastException("ERROR in VariableComparisonConstraint: Value of the variable on the left side needs to be of type IComparable");
        } else if (tmp != null && tmp.Name.Equals(rightVarName.Data)) {
          right = tmp;
        }
      }
      if (left != null && right != null) {
        switch (comparer.Data) {
          case 0:
            return left.CompareTo(right) < 0;
          case 1:
            return left.CompareTo(right) <= 0;
          case 2:
            return left.CompareTo(right) == 0;
          case 3:
            return left.CompareTo(right) >= 0;
          case 4:
            return left.CompareTo(right) > 0;
          default:
            throw new InvalidOperationException("ERROR in VariableComparisonConstraint: Comparer undefined");
        }
      }
      return true;
    }

    public override IView CreateView() {
      return new VariableComparisonConstraintView(this);
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      VariableComparisonConstraint clone = new VariableComparisonConstraint();
      clonedObjects.Add(Guid, clone);
      clone.LeftVarName = (StringData)Auxiliary.Clone(LeftVarName, clonedObjects);
      clone.RightVarName = (StringData)Auxiliary.Clone(RightVarName, clonedObjects);
      clone.Comparer = (IntData)Auxiliary.Clone(Comparer, clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode leftNode = PersistenceManager.Persist("LeftVarName", LeftVarName, document, persistedObjects);
      node.AppendChild(leftNode);
      XmlNode rightNode = PersistenceManager.Persist("RightVarName", RightVarName, document, persistedObjects);
      node.AppendChild(rightNode);
      XmlNode comparerNode = PersistenceManager.Persist("Comparer", Comparer, document, persistedObjects);
      node.AppendChild(comparerNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      leftVarName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("LeftVarName"), restoredObjects);
      rightVarName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("RightVarName"), restoredObjects);
      comparer = (IntData)PersistenceManager.Restore(node.SelectSingleNode("Comparer"), restoredObjects);
    }
    #endregion
  }
}
