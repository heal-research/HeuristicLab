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
  public class PointXYChart : ItemBase, IVisualizationItem {
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

    private BoolData myConnectDots;
    public BoolData ConnectDots {
      get { return myConnectDots; }
      set {
        if (value != myConnectDots) {
          myConnectDots = value;
          OnConnectDotsChanged();
        }
      }
    }
 
         
   
    public PointXYChart() {
      myConnectDots = new BoolData(true);
    }
    public PointXYChart(bool connectDots,ItemList values) {
      myConnectDots = new BoolData(connectDots);
      myValues = values;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      PointXYChart clone = (PointXYChart)base.Clone(clonedObjects);
      clone.myValues = (ItemList)Auxiliary.Clone(Values, clonedObjects);
      return clone;
    }

    public override IView CreateView() {
      return new PointXYChartView(this);
    }

    public event EventHandler ValuesChanged;
    protected virtual void OnValuesChanged() {
      if (ValuesChanged != null)
        ValuesChanged(this, new EventArgs());
    }
    public event EventHandler ConnectDotsChanged;
    protected virtual void OnConnectDotsChanged() {
      if (ConnectDotsChanged != null)
        ConnectDotsChanged(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("ConnectDots",ConnectDots,document,persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Values", Values, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myConnectDots = (BoolData)PersistenceManager.Restore(node.SelectSingleNode("ConnectDots"), restoredObjects);
      myValues = (ItemList)PersistenceManager.Restore(node.SelectSingleNode("Values"), restoredObjects);
    }
    #endregion
  }
}
