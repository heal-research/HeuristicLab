using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {

  public class IntArrayConverter : ValueTypeArrayConverter<IntArray, int> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntArray);
  }

  public class DoubleArrayConverter : ValueTypeArrayConverter<DoubleArray, double> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleArray);
  }

  public class PercentArrayConverter : ValueTypeArrayConverter<PercentArray, double> {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentArray);
  }

  public class BoolArrayConverter : ValueTypeArrayConverter<BoolArray, bool> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolArray);
  }

  public abstract class ValueTypeArrayConverter<ArrayType, T> : BaseConverter
    where ArrayType : ValueTypeArray<T>
    where T : struct 
  {
    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) => 
      CopyArrayData(((ArrayType)item), CastValue<T[]>(data.Value));

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new JsonItem() {
        Name = "[OverridableParamName]",
        Value = ((ArrayType)value).CloneAsArray()
      };

    #region Helper
    private void CopyArrayData(ArrayType array, T[] data) {
      var colInfo = array.GetType().GetProperty("Length");
      colInfo.SetValue(array, data.Length);
      for (int i = 0; i < data.Length; ++i) {
        array[i] = data[i];
      }
    }
    #endregion
  }
}
