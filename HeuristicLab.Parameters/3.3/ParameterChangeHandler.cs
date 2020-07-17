#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Parameters {
  public class ParameterChangeHandler<TItem> where TItem : class, IItem {
    protected Action handler;

    protected ParameterChangeHandler(IValueParameter<TItem> parameter, Action handler) {
      if (!(parameter is IFixedValueParameter<TItem>))
        parameter.ValueChanged += ParameterOnValueChanged;
      this.handler = handler;
    }

    protected virtual void ParameterOnValueChanged(object sender, EventArgs e) {
      handler();
    }

    public static ParameterChangeHandler<TItem> Create(IValueParameter<TItem> parameter, Action handler) {
      return new ParameterChangeHandler<TItem>(parameter, handler);
    }
  }

  public class ValueTypeValueParameterChangeHandler<TItem, TValue> : ParameterChangeHandler<TItem>
    where TValue : struct
    where TItem : ValueTypeValue<TValue> {
    private ValueTypeValue<TValue> last;

    protected ValueTypeValueParameterChangeHandler(IValueParameter<TItem> parameter, Action handler)
      : base(parameter, handler) {
      last = parameter.Value;
      if (last != null && !last.ReadOnly)
        last.ValueChanged += ParameterValueOnValueChanged;
    }

    protected override void ParameterOnValueChanged(object sender, EventArgs e) {
      if (last != null) last.ValueChanged -= ParameterValueOnValueChanged;
      last = ((IValueParameter<TItem>)sender).Value;
      if (last != null && !last.ReadOnly)
        last.ValueChanged += ParameterValueOnValueChanged;
      base.ParameterOnValueChanged(sender, e);
    }

    protected void ParameterValueOnValueChanged(object sender, EventArgs e) {
      handler();
    }

    public static ValueTypeValueParameterChangeHandler<TItem, TValue> Create(IValueParameter<TItem> parameter, Action handler)
      => new ValueTypeValueParameterChangeHandler<TItem, TValue>(parameter, handler);
  }
  public class ValueTypeArrayParameterChangeHandler<TItem, TValue> : ParameterChangeHandler<TItem>
    where TValue : struct
    where TItem : ValueTypeArray<TValue> {
    private ValueTypeArray<TValue> last;

    protected ValueTypeArrayParameterChangeHandler(IValueParameter<TItem> parameter, Action handler)
      : base(parameter, handler) {
      last = parameter.Value;
      if (last != null && !last.ReadOnly)
        last.ToStringChanged += ParameterValueOnValueChanged;
    }

    protected override void ParameterOnValueChanged(object sender, EventArgs e) {
      if (last != null) last.ToStringChanged -= ParameterValueOnValueChanged;
      last = ((IValueParameter<TItem>)sender).Value;
      if (last != null && !last.ReadOnly)
        last.ToStringChanged += ParameterValueOnValueChanged;
      base.ParameterOnValueChanged(sender, e);
    }

    protected void ParameterValueOnValueChanged(object sender, EventArgs e) {
      handler();
    }

    public static ValueTypeArrayParameterChangeHandler<TItem, TValue> Create(IValueParameter<TItem> parameter, Action handler)
      => new ValueTypeArrayParameterChangeHandler<TItem, TValue>(parameter, handler);
  }

  public class ValueTypeMatrixParameterChangeHandler<TItem, TValue> : ParameterChangeHandler<TItem>
    where TValue : struct
    where TItem : ValueTypeMatrix<TValue> {
    private ValueTypeMatrix<TValue> last;

    protected ValueTypeMatrixParameterChangeHandler(IValueParameter<TItem> parameter, Action handler)
      : base(parameter, handler) {
      last = parameter.Value;
      if (last != null && !last.ReadOnly)
        last.ToStringChanged += ParameterValueOnValueChanged;
    }

    protected override void ParameterOnValueChanged(object sender, EventArgs e) {
      if (last != null) last.ToStringChanged -= ParameterValueOnValueChanged;
      last = ((IValueParameter<TItem>)sender).Value;
      if (last != null && !last.ReadOnly)
        last.ToStringChanged += ParameterValueOnValueChanged;
      base.ParameterOnValueChanged(sender, e);
    }

    protected void ParameterValueOnValueChanged(object sender, EventArgs e) {
      handler();
    }

    public static ValueTypeMatrixParameterChangeHandler<TItem, TValue> Create(IValueParameter<TItem> parameter, Action handler)
      => new ValueTypeMatrixParameterChangeHandler<TItem, TValue>(parameter, handler);
  }

  public class BoolValueParameterChangeHandler : ValueTypeValueParameterChangeHandler<BoolValue, bool> {
    private BoolValueParameterChangeHandler(IValueParameter<BoolValue> parameter, Action handler) : base(parameter, handler) { }
    public static new BoolValueParameterChangeHandler Create(IValueParameter<BoolValue> parameter, Action handler)
      => new BoolValueParameterChangeHandler(parameter, handler);
  }
  public class IntValueParameterChangeHandler : ValueTypeValueParameterChangeHandler<IntValue, int> {
    private IntValueParameterChangeHandler(IValueParameter<IntValue> parameter, Action handler) : base(parameter, handler) { }
    public static new IntValueParameterChangeHandler Create(IValueParameter<IntValue> parameter, Action handler)
      => new IntValueParameterChangeHandler(parameter, handler);
  }
  public class DoubleValueParameterChangeHandler : ValueTypeValueParameterChangeHandler<DoubleValue, double> {
    private DoubleValueParameterChangeHandler(IValueParameter<DoubleValue> parameter, Action handler) : base(parameter, handler) { }
    public static new DoubleValueParameterChangeHandler Create(IValueParameter<DoubleValue> parameter, Action handler)
      => new DoubleValueParameterChangeHandler(parameter, handler);
  }
  public class PercentValueParameterChangeHandler : ValueTypeValueParameterChangeHandler<PercentValue, double> {
    private PercentValueParameterChangeHandler(IValueParameter<PercentValue> parameter, Action handler) : base(parameter, handler) { }
    public static new PercentValueParameterChangeHandler Create(IValueParameter<PercentValue> parameter, Action handler)
      => new PercentValueParameterChangeHandler(parameter, handler);
  }
  public class DateTimeValueParameterChangeHandler : ValueTypeValueParameterChangeHandler<DateTimeValue, DateTime> {
    private DateTimeValueParameterChangeHandler(IValueParameter<DateTimeValue> parameter, Action handler) : base(parameter, handler) { }
    public static new DateTimeValueParameterChangeHandler Create(IValueParameter<DateTimeValue> parameter, Action handler)
      => new DateTimeValueParameterChangeHandler(parameter, handler);
  }
  public class TimeSpanValueParameterChangeHandler : ValueTypeValueParameterChangeHandler<TimeSpanValue, TimeSpan> {
    private TimeSpanValueParameterChangeHandler(IValueParameter<TimeSpanValue> parameter, Action handler) : base(parameter, handler) { }
    public static new TimeSpanValueParameterChangeHandler Create(IValueParameter<TimeSpanValue> parameter, Action handler)
      => new TimeSpanValueParameterChangeHandler(parameter, handler);
  }
  public class EnumValueParameterChangeHandler<TEnum> : ValueTypeValueParameterChangeHandler<EnumValue<TEnum>, TEnum> where TEnum : struct, IComparable {
    private EnumValueParameterChangeHandler(IValueParameter<EnumValue<TEnum>> parameter, Action handler) : base(parameter, handler) { }
    public static new EnumValueParameterChangeHandler<TEnum> Create(IValueParameter<EnumValue<TEnum>> parameter, Action handler)
      => new EnumValueParameterChangeHandler<TEnum>(parameter, handler);
  }
  public class BoolArrayParameterChangeHandler : ValueTypeArrayParameterChangeHandler<BoolArray, bool> {
    private BoolArrayParameterChangeHandler(IValueParameter<BoolArray> parameter, Action handler) : base(parameter, handler) { }
    public static new BoolArrayParameterChangeHandler Create(IValueParameter<BoolArray> parameter, Action handler)
      => new BoolArrayParameterChangeHandler(parameter, handler);
  }
  public class IntArrayParameterChangeHandler : ValueTypeArrayParameterChangeHandler<IntArray, int> {
    private IntArrayParameterChangeHandler(IValueParameter<IntArray> parameter, Action handler) : base(parameter, handler) { }
    public static new IntArrayParameterChangeHandler Create(IValueParameter<IntArray> parameter, Action handler)
      => new IntArrayParameterChangeHandler(parameter, handler);
  }
  public class DoubleArrayParameterChangeHandler : ValueTypeArrayParameterChangeHandler<DoubleArray, double> {
    private DoubleArrayParameterChangeHandler(IValueParameter<DoubleArray> parameter, Action handler) : base(parameter, handler) { }
    public static new DoubleArrayParameterChangeHandler Create(IValueParameter<DoubleArray> parameter, Action handler)
      => new DoubleArrayParameterChangeHandler(parameter, handler);
  }
  public class BoolMatrixParameterChangeHandler : ValueTypeMatrixParameterChangeHandler<BoolMatrix, bool> {
    private BoolMatrixParameterChangeHandler(IValueParameter<BoolMatrix> parameter, Action handler) : base(parameter, handler) { }
    public static new BoolMatrixParameterChangeHandler Create(IValueParameter<BoolMatrix> parameter, Action handler)
      => new BoolMatrixParameterChangeHandler(parameter, handler);
  }
  public class IntMatrixParameterChangeHandler : ValueTypeMatrixParameterChangeHandler<IntMatrix, int> {
    private IntMatrixParameterChangeHandler(IValueParameter<IntMatrix> parameter, Action handler) : base(parameter, handler) { }
    public static new IntMatrixParameterChangeHandler Create(IValueParameter<IntMatrix> parameter, Action handler)
      => new IntMatrixParameterChangeHandler(parameter, handler);
  }
  public class DoubleMatrixParameterChangeHandler : ValueTypeMatrixParameterChangeHandler<DoubleMatrix, double> {
    private DoubleMatrixParameterChangeHandler(IValueParameter<DoubleMatrix> parameter, Action handler) : base(parameter, handler) { }
    public static new DoubleMatrixParameterChangeHandler Create(IValueParameter<DoubleMatrix> parameter, Action handler)
      => new DoubleMatrixParameterChangeHandler(parameter, handler);
  }
  public class StringValueParameterChangeHandler : ParameterChangeHandler<StringValue> { // StringValue does not derive from ValueTypeValue
    private StringValue last;

    private StringValueParameterChangeHandler(IValueParameter<StringValue> parameter, Action handler) : base(parameter, handler) {
      last = parameter.Value;
      if (last != null && !last.ReadOnly)
        last.ValueChanged += ParameterValueOnValueChanged;
    }

    protected override void ParameterOnValueChanged(object sender, EventArgs e) {
      if (last != null) last.ValueChanged -= ParameterValueOnValueChanged;
      last = ((IValueParameter<StringValue>)sender).Value;
      if (last != null && !last.ReadOnly)
        last.ValueChanged += ParameterValueOnValueChanged;
      base.ParameterOnValueChanged(sender, e);
    }

    private void ParameterValueOnValueChanged(object sender, EventArgs e) {
      handler();
    }
    public static StringValueParameterChangeHandler Create(IValueParameter<StringValue> parameter, Action handler)
     => new StringValueParameterChangeHandler(parameter, handler);
  }

  public class ItemListParameterChangeHandler<T> : ParameterChangeHandler<ItemList<T>> where T : class,IItem {
    private ItemList<T> last;

    private ItemListParameterChangeHandler(IValueParameter<ItemList<T>> parameter, Action handler)
      : base(parameter, handler) {
      last = parameter.Value;
      if (last != null && !(last is ReadOnlyItemList<T>)) {
        last.PropertyChanged += ParameterValueOnListChanged;
      }
    }

    protected override void ParameterOnValueChanged(object sender, EventArgs e) {
      if (last != null && !(last is ReadOnlyItemList<T>))
        last.PropertyChanged -= ParameterValueOnListChanged;
      last = ((IValueParameter<ItemList<T>>)sender).Value;
      if (last != null && !(last is ReadOnlyItemList<T>))
        last.PropertyChanged += ParameterValueOnListChanged;
      base.ParameterOnValueChanged(sender, e);
    }

    private void ParameterValueOnListChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "Item[]") handler();
    }
    public static ItemListParameterChangeHandler<T> Create(IValueParameter<ItemList<T>> parameter, Action handler)
     => new ItemListParameterChangeHandler<T>(parameter, handler);
  }
}
