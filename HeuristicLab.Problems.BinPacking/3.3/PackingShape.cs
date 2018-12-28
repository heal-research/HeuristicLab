#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HEAL.Fossil;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking {
  [StorableType("FA24378B-4353-4CC9-AEFD-4B002EF5B139")]
  public abstract class PackingShape<T> : Item, IPackingShape, IParameterizedItem
    where T : class, IPackingPosition {
    public static Type PositionType {
      get { return typeof(T); }
    }

    public abstract bool EnclosesPoint(T myPosition, T checkedPoint);
    public abstract bool Encloses(T checkedPosition, PackingShape<T> checkedShape);
    public abstract bool Overlaps(T myPosition, T checkedPosition, PackingShape<T> checkedShape);
    public abstract int Volume { get; }
    public abstract T Origin { get; }

    protected PackingShape()
      : base() {
      Parameters = new ParameterCollection();
    }

    [StorableConstructor]
    protected PackingShape(StorableConstructorFlag _) : base(_) { }
    protected PackingShape(PackingShape<T> original, Cloner cloner) {
      this.Parameters = new ParameterCollection(original.Parameters.Select(p => cloner.Clone(p)));
    }

    public virtual void CollectParameterValues(IDictionary<string, IItem> values) {
      foreach (IValueParameter param in Parameters.OfType<IValueParameter>()) {
        var children = GetCollectedValues(param);
        foreach (var c in children) {
          if (String.IsNullOrEmpty(c.Key))
            values.Add(param.Name, c.Value);
          else values.Add(param.Name + "." + c.Key, c.Value);
        }
      }
    }

    protected virtual IEnumerable<KeyValuePair<string, IItem>> GetCollectedValues(IValueParameter param) {
      if (param.Value == null) yield break;
      if (param.GetsCollected) yield return new KeyValuePair<string, IItem>(String.Empty, param.Value);
      var parameterizedItem = param.Value as IParameterizedItem;
      if (parameterizedItem != null) {
        var children = new Dictionary<string, IItem>();
        parameterizedItem.CollectParameterValues(children);
        foreach (var child in children) yield return child;
      }
    }

    public abstract void ApplyHorizontalOrientation();
    public abstract int CompareTo(object obj);

    [Storable]
    public IKeyedItemCollection<string, IParameter> Parameters { get; private set; }
  }
}
