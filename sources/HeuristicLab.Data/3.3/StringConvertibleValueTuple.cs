#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [StorableClass]
  [Item("StringConvertibleValueTuple<,>", "A generic abstract base class for representing multiple values of a string convertible value.")]
  public abstract class StringConvertibleValueTuple<T, U> : Item, IStringConvertibleValueTuple
    where T : class, IDeepCloneable, IStringConvertibleValue
    where U : class, IDeepCloneable, IStringConvertibleValue {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.ValueType; }
    }

    [Storable]
    protected bool readOnly;
    public virtual bool ReadOnly {
      get { return readOnly; }
    }

    [Storable(Name = "Values")]
    protected Tuple<T, U> values;

    protected T Item1 { get { return values.Item1; } }
    IStringConvertibleValue IStringConvertibleValueTuple.Item1 { get { return Item1; } }
    protected U Item2 { get { return values.Item2; } }
    IStringConvertibleValue IStringConvertibleValueTuple.Item2 { get { return Item2; } }

    [StorableConstructor]
    protected StringConvertibleValueTuple(bool deserializing)
      : base(deserializing) {
    }
    protected StringConvertibleValueTuple(StringConvertibleValueTuple<T, U> original, Cloner cloner)
      : base(original, cloner) {
      T item1 = (T)cloner.Clone(original.values.Item1);
      U item2 = (U)cloner.Clone(original.values.Item2);
      values = Tuple.Create<T, U>(item1, item2);
      RegisterEventHandler();
    }

    public StringConvertibleValueTuple(T item1, U item2)
      : base() {
      values = Tuple.Create<T, U>(item1, item2);
      readOnly = false;
      RegisterEventHandler();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandler();
    }


    public abstract StringConvertibleValueTuple<T, U> AsReadOnly();

    public override string ToString() {
      return string.Format("Item1: {0}, Item2: {1}", Item1, Item2);
    }

    private void RegisterEventHandler() {
      Item1.ValueChanged += Item_ValueChanged;
      Item2.ValueChanged += Item_ValueChanged;
    }

    private void Item_ValueChanged(object sender, EventArgs e) {
      OnValueChanged();
    }

    public event EventHandler ValueChanged;
    protected virtual void OnValueChanged() {
      if (ValueChanged != null)
        ValueChanged(this, EventArgs.Empty);
      OnToStringChanged();
    }
  }
}
