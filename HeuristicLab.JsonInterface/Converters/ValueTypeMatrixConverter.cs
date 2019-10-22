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
      CopyMatrixData(item.Cast<MatrixType>(), CastValue<T[,]>(data.Value));

    public override JsonItem ExtractData(IItem value) =>
      new JsonItem() {
        Value = value.Cast<MatrixType>().CloneAsMatrix()
      };

    #region Helper
    private void CopyMatrixData(MatrixType matrix, T[,] data) {
      var rowInfo = matrix.GetType().GetProperty("Rows");
      rowInfo.SetValue(matrix, data.GetLength(0));
      var colInfo = matrix.GetType().GetProperty("Columns");
      colInfo.SetValue(matrix, data.GetLength(1));
      //matrix.Cast<dynamic>().Columns = data.GetLength(1);
      for (int x = 0; x < data.GetLength(0); ++x) {
        for (int y = 0; y < data.GetLength(1); ++y) {
          matrix[x, y] = data[x, y];
        }
      }
    }
    #endregion
  }
}
