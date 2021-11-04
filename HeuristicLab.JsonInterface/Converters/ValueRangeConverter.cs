using System;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {

  public class IntRangeConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(IntRange).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IntRange range = item as IntRange;
      IntRangeJsonItem cdata = data as IntRangeJsonItem;
      if(data.Active) {
        range.Start = cdata.MinValue;
        range.End = cdata.MaxValue;
      }
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IntRange range = value as IntRange;
      return new IntRangeJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        MinValue = range.Start,
        MaxValue = range.End,
        Minimum = int.MinValue,
        Maximum = int.MaxValue
      };
    }
  }

  public class DoubleRangeConverter : BaseConverter {
    public override int Priority => 1;

    public override bool CanConvertType(Type t) =>
      typeof(DoubleRange).IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      DoubleRange range = item as DoubleRange;
      DoubleRangeJsonItem cdata = data as DoubleRangeJsonItem;
      if (data.Active) {
        range.Start = cdata.MinValue;
        range.End = cdata.MaxValue;
      }
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      DoubleRange range = value as DoubleRange;
      return new DoubleRangeJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        MinValue = range.Start,
        MaxValue = range.End,
        Minimum = double.MinValue,
        Maximum = double.MaxValue
      };
    }
  }
}
