#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableClass]
  public abstract class ShapeInfo : DeepCloneable, IShapeInfo {
    [StorableConstructor]
    protected ShapeInfo(bool deserializing) : base() { }
    protected ShapeInfo(ShapeInfo original, Cloner cloner)
      : base(original, cloner) {
      location = original.location;
    }

    protected ShapeInfo() : base() { }

    [Storable]
    private Point location;
    public Point Location {
      get { return this.location; }
      set {
        if (this.location != value) {
          this.location = value;
          this.OnChanged();
        }
      }
    }

    public abstract IEnumerable<string> Connectors { get; }

    public event EventHandler Changed;
    protected virtual void OnChanged() {
      EventHandler handler = this.Changed;
      if (handler != null) this.Changed(this, EventArgs.Empty);
    }
  }
}
