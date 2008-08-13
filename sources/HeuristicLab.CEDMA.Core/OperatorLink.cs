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
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {
  public class OperatorLink : OperatorBase {
    private long id;
    public long Id {
      get { return id; }
    }

    public IDatabase Database { get; set; }

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
      if(Operator == null) {
        if(Database == null) return null;
        OperatorEntry targetEntry = Database.GetOperator(Id);
        IOperator target = (IOperator)PersistenceManager.RestoreFromGZip(targetEntry.RawData);
        PatchOperatorLinks(target);
        Operator = target;
      }
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

    private void PatchOperatorLinks(IOperatorGraph opGraph) {
      foreach(IOperator op in opGraph.Operators) {
        PatchOperatorLinks(op);
      }
    }

    private void PatchOperatorLinks(IOperator op) {
      if(op is OperatorLink) {
        OperatorLink link = op as OperatorLink;
        link.Database = Database;
        //if(downloaded.ContainsKey(link.Id)) {
        //  link.Operator = downloaded[link.Id];
        //} else {
        //  OperatorEntry targetEntry = Database.GetOperator(link.Id);
        //  IOperator target = (IOperator)PersistenceManager.RestoreFromGZip(targetEntry.RawData);
        //  downloaded.Add(link.Id, target);
        //  PatchOperatorLinks(target, downloaded);
        //  link.Operator = target;
        //}
      } else if(op is CombinedOperator) {
        PatchOperatorLinks(((CombinedOperator)op).OperatorGraph);
      }
      // also patch operator links contained (indirectily) in variables
      foreach(VariableInfo varInfo in op.VariableInfos) {
        IVariable var = op.GetVariable(varInfo.ActualName);
        if(var != null && var.Value is IOperatorGraph) {
          PatchOperatorLinks((IOperatorGraph)var.Value);
        } else if(var != null && var.Value is IOperator) {
          PatchOperatorLinks((IOperator)var.Value);
        }
      }
    }
  }
}
