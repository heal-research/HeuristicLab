using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {

  public class IntValueConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntValue);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((IntValue)item).Value = ((IntJsonItem)data).Value;

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
    public override Type ConvertableType => typeof(DoubleValue);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((DoubleValue)item).Value = ((DoubleJsonItem)data).Value;

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
    public override Type ConvertableType => typeof(PercentValue);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((PercentValue)item).Value = ((DoubleJsonItem)data).Value;

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
    public override Type ConvertableType => typeof(BoolValue);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((BoolValue)item).Value = ((BoolJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new BoolJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((BoolValue)value).Value
      };
  }

  public class DateTimeValueConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DateTimeValue);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((DateTimeValue)item).Value = ((DateTimeJsonItem)data).Value;

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
