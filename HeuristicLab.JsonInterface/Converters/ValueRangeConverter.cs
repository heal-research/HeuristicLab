using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.JsonInterface {

  public class IntRangeConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntRange);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IntRange range = item as IntRange;
      IntRangeJsonItem cdata = data as IntRangeJsonItem;
      range.Start = cdata.MinValue;
      range.End = cdata.MaxValue;
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
    public override Type ConvertableType => typeof(DoubleRange);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      DoubleRange range = item as DoubleRange;
      DoubleRangeJsonItem cdata = data as DoubleRangeJsonItem;
      range.Start = cdata.MinValue;
      range.End = cdata.MaxValue;
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
