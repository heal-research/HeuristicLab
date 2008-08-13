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
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Core {
  public class DatabaseOperatorLibrary : ItemBase, IOperatorLibrary {

    private OperatorGroup group;
    public IOperatorGroup Group {
      get { return group; }
    }

    private IDatabase database;
    public IDatabase Database {
      get { return database; }
      set { this.database = value; }
    }

    private Dictionary<IOperator, long> knownOperators;

    public DatabaseOperatorLibrary()
      : base() {
      group = new OperatorGroup();
      knownOperators = new Dictionary<IOperator, long>();
    }

    public void Save() {
      Dictionary<IOperator, long> newKnownOperators = new Dictionary<IOperator, long>();
      foreach(IOperator op in group.Operators) {
        if(knownOperators.ContainsKey(op)) {
          // update
          long id = knownOperators[op];
          Database.UpdateOperator(id, op.Name, PersistenceManager.SaveToGZip(op));
          knownOperators.Remove(op);
          newKnownOperators.Add(op, id);
        } else {
          // create new
          long id = Database.InsertOperator(op.Name, PersistenceManager.SaveToGZip(op));
          newKnownOperators.Add(op, id);
        }
      }
      // delete operators from the table that are not present in the group anymore (remaining entries)
      foreach(long id in knownOperators.Values) {
        Database.DeleteOperator(id);
      }

      knownOperators = newKnownOperators;
    }

    public void Restore() {
      foreach(IOperator op in knownOperators.Keys) {
        group.RemoveOperator(op);
      }
      knownOperators.Clear();
      if(database == null) return;
      foreach(OperatorEntry e in Database.GetOperators()) {
        IOperator op = (IOperator)PersistenceManager.RestoreFromGZip(e.RawData);
        knownOperators.Add(op, e.Id);
        group.AddOperator(op);
      }

      // set the DB source for each operator
      foreach(IOperator op in group.Operators) {
        OperatorLinkPatcher.LinkDatabase(op, Database);
      }
    }

    private IOperator FindOperator(long id) {
      foreach(KeyValuePair<IOperator, long> p in knownOperators) if(p.Value == id) return p.Key;
      return null;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      throw new NotSupportedException();
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      throw new NotSupportedException();
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      throw new NotSupportedException();
    }

    public override IView CreateView() {
      Restore();
      return new DatabaseOperatorLibraryView(this);
    }

    internal long GetId(IOperator op) {
      return knownOperators[op];
    }
  }
}
