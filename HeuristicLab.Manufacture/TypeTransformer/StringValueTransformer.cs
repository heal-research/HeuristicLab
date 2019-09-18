using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace ParameterTest {
  public class StringValueTransformer : BaseTransformer {
    public override IItem FromData(ParameterData obj, Type targetType) =>
      //item.Cast<StringValue>().Value = CastValue<string>(obj.Default);
      new StringValue() { Value = CastValue<string>(obj.Default) };

    public override void SetValue(IItem item, ParameterData data) =>
      item.Cast<StringValue>().Value = CastValue<string>(data.Default);

    public override ParameterData ToData(IItem value) {
      ParameterData data = base.ToData(value);
      data.Default = value.Cast<StringValue>().Value;
      return data;
    }

  }
}
