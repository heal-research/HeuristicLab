using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class IntMatrixConverter : ValueTypeMatrixConverter<IntMatrix, int> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntMatrix);
  }

  public class DoubleMatrixConverter : ValueTypeMatrixConverter<DoubleMatrix, double> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleMatrix);
  }

  public class PercentMatrixConverter : ValueTypeMatrixConverter<PercentMatrix, double> {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentMatrix);
  }

  public class BoolMatrixConverter : ValueTypeMatrixConverter<BoolMatrix, bool> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolMatrix);
  }

  public abstract class ValueTypeMatrixConverter<MatrixType, T> : BaseConverter
    where MatrixType : ValueTypeMatrix<T> 
    where T : struct 
  {
    public override void InjectData(IItem item, JsonItem data, IJsonItemConverter root) => 
      CopyMatrixData(item as MatrixType, data.Value);

    public override void Populate(IItem value, JsonItem item, IJsonItemConverter root) {
      item.Name = "[OverridableParamName]";
      item.Value = ((MatrixType)value).CloneAsMatrix();
    }

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
