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
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Parameters {
  public abstract class ParameterChangeHandler<TItem> where TItem : class, IItem {
    protected Action handler;

    protected ParameterChangeHandler(IValueParameter<TItem> parameter, Action handler) {
      if (!(parameter is IFixedValueParameter<TItem>))
        parameter.ValueChanged += ParameterOnValueChanged;
      this.handler = handler;
    }

    protected virtual void ParameterOnValueChanged(object sender, EventArgs e) {
      handler();
    }
  }

  public class ParameterChangeHandler<TItem, TValue> : ParameterChangeHandler<TItem>
    where TValue : struct
    where TItem : ValueTypeValue<TValue> {
    private ValueTypeValue<TValue> last;

    protected ParameterChangeHandler(IValueParameter<TItem> parameter, Action handler)
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

    public static ParameterChangeHandler<TItem, TValue> Create(IValueParameter<TItem> parameter, Action handler)
      => new ParameterChangeHandler<TItem, TValue>(parameter, handler);
  }

  public class BoolValueParameterChangeHandler : ParameterChangeHandler<BoolValue, bool> {
    private BoolValueParameterChangeHandler(IValueParameter<BoolValue> parameter, Action handler) : base(parameter, handler) { }
    public static new BoolValueParameterChangeHandler Create(IValueParameter<BoolValue> parameter, Action handler)
      => new BoolValueParameterChangeHandler(parameter, handler);
  }
  public class IntValueParameterChangeHandler : ParameterChangeHandler<IntValue, int> {
    private IntValueParameterChangeHandler(IValueParameter<IntValue> parameter, Action handler) : base(parameter, handler) { }
    public static new IntValueParameterChangeHandler Create(IValueParameter<IntValue> parameter, Action handler)
      => new IntValueParameterChangeHandler(parameter, handler);
  }
  public class DoubleValueParameterChangeHandler : ParameterChangeHandler<DoubleValue, double> {
    private DoubleValueParameterChangeHandler(IValueParameter<DoubleValue> parameter, Action handler) : base(parameter, handler) { }
    public static new DoubleValueParameterChangeHandler Create(IValueParameter<DoubleValue> parameter, Action handler)
      => new DoubleValueParameterChangeHandler(parameter, handler);
  }
  public class PercentValueParameterChangeHandler : ParameterChangeHandler<PercentValue, double> {
    private PercentValueParameterChangeHandler(IValueParameter<PercentValue> parameter, Action handler) : base(parameter, handler) { }
    public static new PercentValueParameterChangeHandler Create(IValueParameter<PercentValue> parameter, Action handler)
      => new PercentValueParameterChangeHandler(parameter, handler);
  }
  public class DateTimeValueParameterChangeHandler : ParameterChangeHandler<DateTimeValue, DateTime> {
    private DateTimeValueParameterChangeHandler(IValueParameter<DateTimeValue> parameter, Action handler) : base(parameter, handler) { }
    public static new DateTimeValueParameterChangeHandler Create(IValueParameter<DateTimeValue> parameter, Action handler)
      => new DateTimeValueParameterChangeHandler(parameter, handler);
  }
  public class TimeSpanValueParameterChangeHandler : ParameterChangeHandler<TimeSpanValue, TimeSpan> {
    private TimeSpanValueParameterChangeHandler(IValueParameter<TimeSpanValue> parameter, Action handler) : base(parameter, handler) { }
    public static new TimeSpanValueParameterChangeHandler Create(IValueParameter<TimeSpanValue> parameter, Action handler)
      => new TimeSpanValueParameterChangeHandler(parameter, handler);
  }
  public class EnumValueParameterChangeHandler<TEnum> : ParameterChangeHandler<EnumValue<TEnum>, TEnum> where TEnum : struct, IComparable {
    private EnumValueParameterChangeHandler(IValueParameter<EnumValue<TEnum>> parameter, Action handler) : base(parameter, handler) { }
    public static new EnumValueParameterChangeHandler<TEnum> Create(IValueParameter<EnumValue<TEnum>> parameter, Action handler)
      => new EnumValueParameterChangeHandler<TEnum>(parameter, handler);
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
}
