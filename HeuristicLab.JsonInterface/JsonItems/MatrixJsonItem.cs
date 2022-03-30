using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public abstract class MatrixJsonItem<T> : ValueJsonItem<T[][]>{
    public virtual bool RowsResizable { get; set; }
    public virtual bool ColumnsResizable { get; set; }

    IList<string> rows = new List<string>();
    public IEnumerable<string> RowNames {
      get => rows;
      set => rows = new List<string>(value);
    }

    IList<string> cols = new List<string>();
    public IEnumerable<string> ColumnNames {
      get => cols;
      set => cols = new List<string>(value);
    }

    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);
      RowsResizable = (jObject[nameof(IMatrixJsonItem.RowsResizable)]?.ToObject<bool>()).GetValueOrDefault();
      ColumnsResizable = (jObject[nameof(IMatrixJsonItem.ColumnsResizable)]?.ToObject<bool>()).GetValueOrDefault();
      RowNames = jObject[nameof(IMatrixJsonItem.RowNames)]?.ToObject<IEnumerable<string>>();
      ColumnNames = jObject[nameof(IMatrixJsonItem.ColumnNames)]?.ToObject<IEnumerable<string>>();
    }

    public MatrixJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
