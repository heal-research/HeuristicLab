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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Logging {
  /// <summary>
  /// Class for point xy charts.
  /// </summary>
  public class PointXYChart : ItemBase, IVisualizationItem {
    [Storable]
    private ItemList myValues;
    /// <summary>
    /// Gets or sets the values of the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="OnValuesChanged"/> in the setter.</remarks>
    public ItemList Values {
      get { return myValues; }
      set {
        if (value != myValues) {
          myValues = value;
          OnValuesChanged();
        }
      }
    }

    [Storable]
    private BoolData myConnectDots;
    /// <summary>
    /// Gets or sets the flag whether the dots should be connected or not.
    /// </summary>
    /// <remarks>Calls <see cref="OnConnectDotsChanged"/> in the setter.</remarks>
    public BoolData ConnectDots {
      get { return myConnectDots; }
      set {
        if (value != myConnectDots) {
          myConnectDots = value;
          OnConnectDotsChanged();
        }
      }
    }
 
         
    /// <summary>
    /// Initializes a new instance of <see cref="PointXYChart"/> with the <c>ConnectDots</c> flag set to 
    /// <c>true</c>.
    /// </summary>
    public PointXYChart() {
      myConnectDots = new BoolData(true);
    }
    /// <summary>
    /// Initializes a new instance of <see cref="PointXYChart"/> with the given <paramref name="connectDots"/>
    /// flag and the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="connectDots">The flag whether the dots should be connected or not.</param>
    /// <param name="values">The values with which the current instance should be initialized.</param>
    public PointXYChart(bool connectDots,ItemList values) {
      myConnectDots = new BoolData(connectDots);
      myValues = values;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="PointXYChart"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      PointXYChart clone = (PointXYChart)base.Clone(clonedObjects);
      clone.myValues = (ItemList)Auxiliary.Clone(Values, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Occurs when the values of the current instance have been changed.
    /// </summary>
    public event EventHandler ValuesChanged;
    /// <summary>
    /// Fires a <c>ValuesChanged</c> event.
    /// </summary>
    protected virtual void OnValuesChanged() {
      if (ValuesChanged != null)
        ValuesChanged(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when the boolean flag has been changed.
    /// </summary>
    public event EventHandler ConnectDotsChanged;
    /// <summary>
    /// Fires a <c>ConnectDotsChanged</c> event.
    /// </summary>
    protected virtual void OnConnectDotsChanged() {
      if (ConnectDotsChanged != null)
        ConnectDotsChanged(this, new EventArgs());
    }
  }
}
