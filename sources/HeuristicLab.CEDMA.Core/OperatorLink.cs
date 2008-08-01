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
using System.Xml;

namespace HeuristicLab.CEDMA.Core {
  public class OperatorLink : OperatorBase {
    private long id;
    public long Id {
      get { return id; }
    }

    private IOperator myOperator;
    public IOperator Operator {
      get { return myOperator; }
      set { myOperator = value; }
    }

    public OperatorLink() : base() { } // for cloning and persistence

    public OperatorLink(long id, IOperator op)
      : base() {
      this.id = id;
      this.myOperator = op;
      Name = myOperator.Name;
    }

    public override void Abort() {
      throw new NotSupportedException();
    }

    public override IOperation Apply(IScope scope) {
      throw new NotSupportedException();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OperatorLink clone = (OperatorLink)base.Clone(clonedObjects);
      clone.id = id;
      clone.myOperator = Operator;
      return clone;
    }

    public override IView CreateView() {
      return myOperator.CreateView();
    }

    public override IOperation Execute(IScope scope) {
      throw new NotSupportedException();
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute idAttr = document.CreateAttribute("OperatorId");
      idAttr.Value = id.ToString();
      node.Attributes.Append(idAttr);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      id = long.Parse(node.Attributes["OperatorId"].Value);
      base.Populate(node, restoredObjects);
    }
  }
}
