using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace ParameterTest {
  public class ValueTypeArrayTransformer<ArrayType, T> : BaseTransformer
    where ArrayType : ValueTypeArray<T>
    where T : struct 
  {
    public override IItem FromData(ParameterData obj, Type targetType) =>
      Instantiate<ArrayType>(CastValue<T[]>(obj.Default));

    public override void SetValue(IItem item, ParameterData data) {
      T[] arr = CastValue<T[]>(data.Default);
      ArrayType valueArray = item.Cast<ArrayType>();
      for (int i = 0; i < arr.Length; ++i) {
        valueArray[i] = arr[i];
      }
    }
      //item.Cast<StringValue>().Value = CastValue<string>(data.Default);

    public override ParameterData ToData(IItem value) {
      ParameterData data = base.ToData(value);
      data.Default = value.Cast<ArrayType>().CloneAsArray();
      return data;
    }
  }
}
