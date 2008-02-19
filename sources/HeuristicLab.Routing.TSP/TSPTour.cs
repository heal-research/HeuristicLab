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
using HeuristicLab.Permutation;

namespace HeuristicLab.Routing.TSP {
  public class TSPTour : ItemBase, IVisualizationItem {
    private DoubleMatrixData myCoordinates;
    public DoubleMatrixData Coordinates {
      get { return myCoordinates; }
      set { myCoordinates = value; }
    }
    private Permutation.Permutation myTour;
    public Permutation.Permutation Tour {
      get { return myTour; }
      set { myTour = value; }
    }


    public TSPTour() { }
    public TSPTour(DoubleMatrixData coordinates, Permutation.Permutation tour) {
      myCoordinates = coordinates;
      myTour = tour;
    }


    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      TSPTour clone = (TSPTour)base.Clone(clonedObjects);
      clone.myCoordinates = (DoubleMatrixData)Auxiliary.Clone(Coordinates, clonedObjects);
      clone.myTour = Tour;
      return clone;
    }

    public override IView CreateView() {
      return new TSPTourView(this);
    }

    public event EventHandler CoordinatesChanged;
    protected virtual void OnCoordinatesChanged() {
      if (CoordinatesChanged != null)
        CoordinatesChanged(this, new EventArgs());
    }
    public event EventHandler TourChanged;
    protected virtual void OnTourChanged() {
      if (TourChanged != null)
        TourChanged(this, new EventArgs());
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Coordinates", Coordinates, document, persistedObjects));
      node.AppendChild(PersistenceManager.Persist("Tour", Tour, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myCoordinates = (DoubleMatrixData)PersistenceManager.Restore(node.SelectSingleNode("Coordinates"), restoredObjects);
      myTour = (Permutation.Permutation)PersistenceManager.Restore(node.SelectSingleNode("Tour"), restoredObjects);
    }
    #endregion
  }
}
