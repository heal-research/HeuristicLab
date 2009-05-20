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
  /// Class to represent a Linechart.
  /// </summary>
  public class Linechart : ItemBase, IVisualizationItem {
    [Storable]
    private IntData myNumberOfLines;
    /// <summary>
    /// Gets or sets the number of lines of the current instance.
    /// </summary>
    /// <remarks>Calls <see cref="OnNumberOfLinesChanged"/> in the setter.</remarks>
    public int NumberOfLines {
      get { return myNumberOfLines.Data; }
      set {
        if (value != myNumberOfLines.Data) {
          myNumberOfLines.Data = value;
          OnNumberOfLinesChanged();
        }
      }
    }
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


    /// <summary>
    /// Initializes a new instance of <see cref="Linechart"/>.
    /// </summary>
    public Linechart() {
      myNumberOfLines = new IntData(0);
    }
    /// <summary>
    /// Initializes a new instance of <see cref="Linechart"/> with the given <paramref name="numberOfLines"/>
    /// and the given <paramref name="values"/>.
    /// </summary>
    /// <param name="numberOfLines">The number of lines with which to initialize the current instance.</param>
    /// <param name="values">The values with which to initialize the current instance.</param>
    public Linechart(int numberOfLines, ItemList values) {
      myNumberOfLines = new IntData(numberOfLines);
      myValues = values;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="Auxiliary.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already clone objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="Linechart"/>.</returns>
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Linechart clone = (Linechart)base.Clone(clonedObjects);
      clone.myNumberOfLines = (IntData)Auxiliary.Clone(myNumberOfLines, clonedObjects);
      clone.myValues = (ItemList)Auxiliary.Clone(Values, clonedObjects);
      return clone;
    }

    /// <summary>
    /// Creates an instance of <see cref="LinechartView"/> to represent the current instance visually.
    /// </summary>
    /// <returns>The created view as <see cref="LinechartView"/>.</returns>
    public override IView CreateView() {
      return new LinechartView(this);
    }

    /// <summary>
    /// Occurs when the number of lines has been changed.
    /// </summary>
    public event EventHandler NumberOfLinesChanged;
    /// <summary>
    /// Fires a new <c>NumberOfLinesChanged</c> event.
    /// </summary>
    protected virtual void OnNumberOfLinesChanged() {
      if (NumberOfLinesChanged != null)
        NumberOfLinesChanged(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when the values have been changed.
    /// </summary>
    public event EventHandler ValuesChanged;
    /// <summary>
    /// Fires a new <c>ValuesChanged</c> event.
    /// </summary>
    protected virtual void OnValuesChanged() {
      if (ValuesChanged != null)
        ValuesChanged(this, new EventArgs());
    }
  }
}
