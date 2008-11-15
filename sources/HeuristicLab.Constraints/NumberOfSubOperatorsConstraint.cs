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
using System.Diagnostics;
using HeuristicLab.Data;
using System.Xml;

namespace HeuristicLab.Constraints {
  public class NumberOfSubOperatorsConstraint : ConstraintBase {
    private IntData minOperators;
    private IntData maxOperators;

    public IntData MaxOperators {
      get { return maxOperators; }
    }

    public IntData MinOperators {
      get { return minOperators; }
    }

    public override string Description {
      get { return "Number of sub-operators has to be between " + MinOperators.ToString() + " and " + MaxOperators.ToString() + "."; }
    }

    public NumberOfSubOperatorsConstraint()
      : this(0,0) {
    }

    public NumberOfSubOperatorsConstraint(int min, int max) : base() {
      minOperators = new IntData(min);
      maxOperators = new IntData(max);
    }

    public override bool Check(IItem data) {
      IOperator op = data as IOperator;
      if (data == null) return false;

      return (op.SubOperators.Count >= minOperators.Data && op.SubOperators.Count <= maxOperators.Data);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      NumberOfSubOperatorsConstraint clone = new NumberOfSubOperatorsConstraint();
      clonedObjects.Add(Guid, clone);
      clone.maxOperators.Data = maxOperators.Data;
      clone.minOperators.Data = minOperators.Data;
      return clone;
    }

    public override IView CreateView() {
      return new NumberOfSubOperatorsConstraintView(this);
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode minNode = PersistenceManager.Persist("min", minOperators, document, persistedObjects);
      XmlNode maxNode = PersistenceManager.Persist("max", maxOperators, document, persistedObjects);
      node.AppendChild(minNode);
      node.AppendChild(maxNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      minOperators = (IntData)PersistenceManager.Restore(node.SelectSingleNode("min"), restoredObjects);
      maxOperators = (IntData)PersistenceManager.Restore(node.SelectSingleNode("max"), restoredObjects);
    }
    #endregion persistence

  }
}
