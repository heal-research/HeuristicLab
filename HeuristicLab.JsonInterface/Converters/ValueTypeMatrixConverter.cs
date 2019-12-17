using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class ValueTypeMatrixConverter<MatrixType, T> : BaseConverter
    where MatrixType : ValueTypeMatrix<T> 
    where T : struct 
  {
    public override void InjectData(IItem item, JsonItem data) => 
      CopyMatrixData(item.Cast<MatrixType>(), data.Value);

    public override JsonItem ExtractData(IItem value) =>
      new JsonItem() {
        Name = "[OverridableParamName]",
        Value = value.Cast<MatrixType>().CloneAsMatrix()
      };

    #region Helper
    private void CopyMatrixData(MatrixType matrix, object data) {
      if (data is Array arr) {
        var rows = arr.Length;
        var cols = arr.Length > 0 && arr.GetValue(0) is JArray jarr ? jarr.Count : 0;

        var rowInfo = matrix.GetType().GetProperty("Rows");
        rowInfo.SetValue(matrix, rows);
        var colInfo = matrix.GetType().GetProperty("Columns");
        colInfo.SetValue(matrix, cols);

        for (int x = 0; x < rows; ++x) {
          jarr = (JArray)arr.GetValue(x);
          for (int y = 0; y < cols; ++y) {
            matrix[x, y] = jarr[y].ToObject<T>();
          }
        }
      }
    }
    #endregion
  }
}
