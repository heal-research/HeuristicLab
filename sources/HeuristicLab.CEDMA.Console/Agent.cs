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

namespace HeuristicLab.CEDMA.Console {
  public class Agent : ItemBase, IAgent {
    public IDatabase Database { get; set; }
    public long Id { get; set; }
    public string Name { get; set; }
    public AgentStatus Status { get; set; }
    private OperatorGraph operatorGraph;

    public IOperatorGraph OperatorGraph {
      get { return operatorGraph; }
    }

    public Agent()
      : base() {
      operatorGraph = new OperatorGraph();
    }

    public Agent(IDatabase database, long id) : this() {
      Database = database;
      Id = id;
    }

    public void Save() {
      AgentEntry entry = new AgentEntry(Id, Name, Status, DbPersistenceManager.Save(this));
      Database.Update(entry);
    }

    public void Activate() {
      Status = AgentStatus.Waiting;
      Save();
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("OperatorGraph", operatorGraph, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      operatorGraph = (OperatorGraph)PersistenceManager.Restore(node.SelectSingleNode("OperatorGraph"), restoredObjects);
    }
    #endregion

  }
}
