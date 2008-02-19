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
  public class Linechart : ItemBase, IVisualizationItem {
    private IntData myNumberOfLines;
    public int NumberOfLines {
      get { return myNumberOfLines.Data; }
      set {
        if (value != myNumberOfLines.Data) {
          myNumberOfLines.Data = value;
          OnNumberOfLinesChanged();
        }
      }
    }
    private ItemList myValues;
    public ItemList Values {
      get { return myValues; }
      set {
        if (value != myValues) {
          myValues = value;
          OnValuesChanged();
        }
      }
    }


    public Linechart() {
      myNumberOfLines = new IntData(0);
    }
    public Linechart(int numberOfLines, ItemList values) {
      myNumberOfLines = new IntData(numberOfLines);
      myValues = values;
    }


    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Linechart clone = (Linechart)base.Clone(clonedObjects);
      clone.myNumberOfLines = (IntData)Auxiliary.Clone(myNumberOfLines, clonedObjects);
      clone.myValues = (ItemList)Auxiliary.Clone(Values, clonedObjects);
      return clone;
    }

    public override IView CreateView() {
      return new LinechartView(this);
    }

    public event EventHandler NumberOfLinesChanged;
    protected virtual void OnNumberOfLinesChanged() {
      if (NumberOfLinesChanged != null)
        NumberOfLinesChanged(this, new EventArgs());
    }
    public event EventHandler ValuesChanged;
    protected virtual void OnValuesChanged() {
      if (ValuesChanged != null)
        ValuesChanged(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("NumberOfLines", myNumberOfLines, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Values", Values, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myNumberOfLines = (IntData)PersistenceManager.Restore(node.SelectSingleNode("NumberOfLines"), restoredObjects);
      myValues = (ItemList)PersistenceManager.Restore(node.SelectSingleNode("Values"), restoredObjects);
    }
    #endregion
  }
}
