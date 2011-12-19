#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  [Item("HeatMap", "Represents a heat map of double values.")]
  [StorableClass]
  public class HeatMap : DoubleMatrix {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Gradient; }
    }

    private string title;
    public string Title {
      get { return title; }
      set {
        if (value == null) value = string.Empty;
        if (title != value) {
          title = value;
          OnTitleChanged();
        }
      }
    }
    private double minimum;
    public double Minimum {
      get { return minimum; }
      set {
        if (minimum != value) {
          minimum = value;
          if (minimum >= maximum) Maximum = minimum + 1.0;
          OnMinimumChanged();
        }
      }
    }
    private double maximum;
    public double Maximum {
      get { return maximum; }
      set {
        if (maximum != value) {
          maximum = value;
          if (maximum <= minimum) Minimum = maximum - 1.0;
          OnMaximumChanged();
        }
      }
    }

    #region Storable Properties
    [Storable(Name = "Title")]
    private string StorableTitle {
      get { return title; }
      set { title = value; }
    }
    [Storable(Name = "Minimum")]
    private double StorableMinimum {
      get { return minimum; }
      set { minimum = value; }
    }
    [Storable(Name = "Maximum")]
    private double StorableMaximum {
      get { return maximum; }
      set { maximum = value; }
    }
    #endregion

    [StorableConstructor]
    protected HeatMap(bool deserializing) : base(deserializing) { }
    protected HeatMap(HeatMap original, Cloner cloner)
      : base(original, cloner) {
      this.title = original.title;
      this.minimum = original.minimum;
      this.maximum = original.maximum;
    }
    public HeatMap()
      : base() {
      this.title = "Heat Map";
      this.minimum = 0.0;
      this.maximum = 1.0;
    }
    public HeatMap(int rows, int columns)
      : base(rows, columns) {
      this.title = "Heat Map";
      this.minimum = 0.0;
      this.maximum = 1.0;
    }
    public HeatMap(int rows, int columns, string title)
      : base(rows, columns) {
      this.title = title == null ? string.Empty : title;
      this.minimum = 0.0;
      this.maximum = 1.0;
    }
    public HeatMap(double[,] elements)
      : base(elements) {
      this.title = "Heat Map";
      this.minimum = 0.0;
      this.maximum = 1.0;
    }
    public HeatMap(double[,] elements, string title)
      : base(elements) {
      this.title = title == null ? string.Empty : title;
      this.minimum = 0.0;
      this.maximum = 1.0;
    }
    public HeatMap(double[,] elements, string title, double minimum, double maximum)
      : base(elements) {
      this.title = title == null ? string.Empty : title;
      if (minimum >= maximum) throw new ArgumentException("Minimum is larger than or equal to maximum");
      this.minimum = minimum;
      this.maximum = maximum;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new HeatMap(this, cloner);
    }

    public override string ToString() {
      return Title;
    }

    public event EventHandler TitleChanged;
    protected virtual void OnTitleChanged() {
      var handler = TitleChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler MinimumChanged;
    protected virtual void OnMinimumChanged() {
      var handler = MinimumChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler MaximumChanged;
    protected virtual void OnMaximumChanged() {
      var handler = MaximumChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
