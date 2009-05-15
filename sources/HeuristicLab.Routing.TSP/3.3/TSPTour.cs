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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Represent the tour of a TSP.
  /// </summary>
  public class TSPTour : ItemBase, IVisualizationItem {

    [Storable]
    private DoubleMatrixData myCoordinates;
    /// <summary>
    /// Gets or sets the coordinates of the current instance.
    /// </summary>
    public DoubleMatrixData Coordinates {
      get { return myCoordinates; }
      set { myCoordinates = value; }
    }

    [Storable]
    private Permutation.Permutation myTour;
    /// <summary>
    /// Gets or sets the current permutation/tour of the current instance.
    /// </summary>
    public Permutation.Permutation Tour {
      get { return myTour; }
      set { myTour = value; }
    }


    /// <summary>
    /// Initializes a new instance of <see cref="TSPTour"/>.
    /// </summary>
    public TSPTour() { }
    /// <summary>
    /// Initializes a new instance of <see cref="TSPTour"/> with the given <paramref name="coordinates"/>
    /// and the given <paramref name="tour"/>.
    /// </summary>
    /// <param name="coordinates">The coordinates of the TSP.</param>
    /// <param name="tour">The tour the current instance should represent.</param>
    public TSPTour(DoubleMatrixData coordinates, Permutation.Permutation tour) {
      myCoordinates = coordinates;
      myTour = tour;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Uses <see cref="Auxiliary.Clone"/> method of class <see cref="Auxiliary"/> to clone
    /// the coordinates.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="TSPTour"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      TSPTour clone = (TSPTour)base.Clone(clonedObjects);
      clone.myCoordinates = (DoubleMatrixData)Auxiliary.Clone(Coordinates, clonedObjects);
      clone.myTour = Tour;
      return clone;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TSPTourView"/> to display the current instance.
    /// </summary>
    /// <returns>The created view as <see cref="TSPTourView"/>.</returns>
    public override IView CreateView() {
      return new TSPTourView(this);
    }

    /// <summary>
    /// Occurs when the coordinates of the current instance have been changed.
    /// </summary>
    public event EventHandler CoordinatesChanged;
    /// <summary>
    /// Fires a new <c>CoordinatesChanged</c> event.
    /// </summary>
    protected virtual void OnCoordinatesChanged() {
      if (CoordinatesChanged != null)
        CoordinatesChanged(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when the tour of the current instance has been changed.
    /// </summary>
    public event EventHandler TourChanged;
    /// <summary>
    /// Fires a new <c>TourChanged</c> event.
    /// </summary>
    protected virtual void OnTourChanged() {
      if (TourChanged != null)
        TourChanged(this, new EventArgs());
    }    
  }
}
