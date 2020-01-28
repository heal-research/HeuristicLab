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

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((IntValue)item).Value = ((IntJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new IntJsonItem() {
        Name = "[OverridableParamName]",
        Value = ((IntValue)value).Value,
        Range = new int[] { int.MinValue, int.MaxValue }
      };
  }

  public class DoubleValueConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleValue);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((DoubleValue)item).Value = ((DoubleJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleJsonItem() {
        Name = "[OverridableParamName]",
        Value = ((DoubleValue)value).Value,
        Range = new double[] { double.MinValue, double.MaxValue }
      };
  }

  public class PercentValueConverter : BaseConverter {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentValue);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((PercentValue)item).Value = ((DoubleJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleJsonItem() {
        Name = "[OverridableParamName]",
        Value = ((PercentValue)value).Value,
        Range = new double[] { double.MinValue, double.MaxValue }
      };
  }

  public class BoolValueConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolValue);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((BoolValue)item).Value = ((BoolJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new BoolJsonItem() {
        Name = "[OverridableParamName]",
        Value = ((BoolValue)value).Value,
        Range = new bool[] { false, true }
      };
  }

  public class DateTimeValueConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DateTimeValue);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((DateTimeValue)item).Value = ((DateTimeJsonItem)data).Value;

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DateTimeJsonItem() {
        Name = "[OverridableParamName]",
        Value = ((DateTimeValue)value).Value,
        Range = new DateTime[] { DateTime.MinValue, DateTime.MaxValue }
      };
  }
}
