using System;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {

  public class IntValueConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(IntValue).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if(data.Active)
        ((IntValue)item).Value = ((IntJsonItem)data).Value;
    }
      

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new IntJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((IntValue)value).Value,
        Minimum = int.MinValue,
        Maximum = int.MaxValue
      };
  }

  public class DoubleValueConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(DoubleValue).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if (data.Active)
        ((DoubleValue)item).Value = ((DoubleJsonItem)data).Value;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((DoubleValue)value).Value,
        Minimum = double.MinValue,
        Maximum = double.MaxValue
      };
  }

  public class PercentValueConverter : BaseConverter {
    public override int Priority => 2;

    public override bool CanConvertType(Type t) =>
      typeof(PercentValue).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if (data.Active)
        ((PercentValue)item).Value = ((DoubleJsonItem)data).Value;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((PercentValue)value).Value,
        Minimum = 0.0d,
        Maximum = 1.0d
      };
  }

  public class BoolValueConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(BoolValue).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if (data.Active)
        ((BoolValue)item).Value = ((BoolJsonItem)data).Value;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new BoolJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((BoolValue)value).Value
      };
  }

  public class DateTimeValueConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(DateTimeValue).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if (data.Active)
        ((DateTimeValue)item).Value = ((DateTimeJsonItem)data).Value;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DateTimeJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((DateTimeValue)value).Value,
        Minimum = DateTime.MinValue,
        Maximum = DateTime.MaxValue
      };
  }
}
