using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class MatrixJsonItemBase<T> : JsonItem<T[][], T>, IMatrixJsonItem {
    public virtual bool RowsResizable { get; set; }
    public virtual bool ColumnsResizable { get; set; }

    public override void SetFromJObject(JObject jObject) {
      base.SetFromJObject(jObject);
      RowsResizable = (jObject[nameof(IMatrixJsonItem.RowsResizable)]?.ToObject<bool>()).GetValueOrDefault();
      ColumnsResizable = (jObject[nameof(IMatrixJsonItem.ColumnsResizable)]?.ToObject<bool>()).GetValueOrDefault();
    }
  }
}
