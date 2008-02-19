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

namespace HeuristicLab.Logging {
  public class Log : ItemBase, IVisualizationItem {
    private ItemList myItems;
    public ItemList Items {
      get { return myItems; }
      set { myItems = value; }
    }


    public Log() { }
    public Log(ItemList items) {
      myItems = items;
    }


    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Log clone = (Log)base.Clone(clonedObjects);
      clone.myItems = (ItemList)Auxiliary.Clone(Items, clonedObjects);
      return clone;
    }

    public override IView CreateView() {
      return new LogView(this);
    }

    public event EventHandler ItemsChanged;
    protected virtual void OnItemsChanged() {
      if (ItemsChanged != null)
        ItemsChanged(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Items", Items, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myItems = (ItemList)PersistenceManager.Restore(node.SelectSingleNode("Items"), restoredObjects);
    }
    #endregion
  }
}
