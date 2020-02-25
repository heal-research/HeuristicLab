using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class NamedMatrixJsonItemBase<T> : MatrixJsonItemBase<T>, INamedMatrixJsonItem {
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

    public void AddColumn(string col) {
      cols.Add(col);
    }

    public void AddRow(string row) {
      rows.Add(row);
    }

    public override void SetFromJObject(JObject jObject) {
      base.SetFromJObject(jObject);
      RowNames = jObject[nameof(INamedMatrixJsonItem.RowNames)]?.ToObject<IEnumerable<string>>();
      ColumnNames = jObject[nameof(INamedMatrixJsonItem.ColumnNames)]?.ToObject<IEnumerable<string>>();
    }
  }
}
