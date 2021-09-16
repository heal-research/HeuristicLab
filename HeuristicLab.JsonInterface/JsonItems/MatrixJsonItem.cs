using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("2DCBBBA5-B29F-4E97-A3BC-99F7C9E15759")]
  public abstract class MatrixJsonItem<T> : ValueJsonItem<T[][]>, IMatrixJsonItem {
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

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      RowsResizable = (jObject[nameof(IMatrixJsonItem.RowsResizable)]?.ToObject<bool>()).GetValueOrDefault();
      ColumnsResizable = (jObject[nameof(IMatrixJsonItem.ColumnsResizable)]?.ToObject<bool>()).GetValueOrDefault();
      RowNames = jObject[nameof(IMatrixJsonItem.RowNames)]?.ToObject<IEnumerable<string>>();
      ColumnNames = jObject[nameof(IMatrixJsonItem.ColumnNames)]?.ToObject<IEnumerable<string>>();
    }

    public MatrixJsonItem() { }

    [StorableConstructor]
    protected MatrixJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
