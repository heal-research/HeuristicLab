using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.Manufacture {
  public class ValueTypeArrayConverter<ArrayType, T> : BaseConverter
    where ArrayType : ValueTypeArray<T>
    where T : struct 
  {
    public override void InjectData(IItem item, JsonItem data) => 
      CopyArrayData(item.Cast<ArrayType>(), CastValue<T[]>(data.Default));

    public override JsonItem ExtractData(IItem value) => 
      new JsonItem() {
        Default = value.Cast<ArrayType>().CloneAsArray()
      };

    #region Helper
    private void CopyArrayData(ArrayType array, T[] data) {
      for (int i = 0; i < data.Length; ++i) {
        array[i] = data[i];
      }
    }
    #endregion
  }
}
