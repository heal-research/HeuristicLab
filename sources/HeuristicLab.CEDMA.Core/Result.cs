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

namespace HeuristicLab.CEDMA.Core {
  public class Result : ItemBase, IResult {
    public IDatabase Database { get; set; }
    public long Id { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public IItem Item { get; set; }
    public Result()
      : base() {
    }

    public Result(IDatabase database, long id)
      : this() {
      Database = database;
      Id = id;
    }


    public ICollection<IResult> SubResults {
      get {
        List<IResult> results = new List<IResult>();
        foreach(ResultEntry entry in Database.GetSubResults(Id)) {
          Result result = (Result)DbPersistenceManager.Restore(entry.RawData);
          result.Database = Database;
          result.Id = entry.Id;
          result.Summary = entry.Summary;
          result.Description = entry.Description;
          results.Add(result);
        }
        return results;
      }
    } 

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Item", Item, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Item = (IItem)PersistenceManager.Restore(node.SelectSingleNode("Item"), restoredObjects);
    }
    #endregion

    public override IView CreateView() {
      return Item.CreateView();
    }
  }
}
