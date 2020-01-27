using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {

  public class IntValueConverter : ValueTypeValueConverter<IntValue, int> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntValue);
  }

  public class DoubleValueConverter : ValueTypeValueConverter<DoubleValue, double> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleValue);
  }

  public class PercentValueConverter : ValueTypeValueConverter<PercentValue, double> {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentValue);
  }

  public class BoolValueConverter : ValueTypeValueConverter<BoolValue, bool> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolValue);
  }

  public class DateTimeValueConverter : ValueTypeValueConverter<DateTimeValue, DateTime> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DateTimeValue);
  }

  public abstract class ValueTypeValueConverter<ValueType, T> : BaseConverter
    where ValueType : ValueTypeValue<T>
    where T : struct {

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) =>
      ((ValueType)item).Value = CastValue<T>(data.Value);

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new JsonItem() {
        Name = "[OverridableParamName]",
        Value = ((ValueType)value).Value,
        Range = new object[] { GetMinValue(typeof(T)), GetMaxValue(typeof(T)) }
      };
  }
}
