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
    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IntMatrix mat = item as IntMatrix;
      IntMatrixJsonItem d = data as IntMatrixJsonItem;
      CopyMatrixData(mat, d.Value);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new IntMatrixJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = Transform((IntMatrix)value),
        Minimum = int.MinValue,
        Maximum = int.MaxValue
      };
  }

  public class DoubleMatrixConverter : ValueTypeMatrixConverter<DoubleMatrix, double> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleMatrix);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      DoubleMatrix mat = item as DoubleMatrix;
      DoubleMatrixJsonItem d = data as DoubleMatrixJsonItem;
      CopyMatrixData(mat, d.Value);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleMatrixJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = Transform((DoubleMatrix)value),
        Minimum = double.MinValue,
        Maximum = double.MaxValue,
        RowNames = ((DoubleMatrix)value).RowNames,
        ColumnNames = ((DoubleMatrix)value).ColumnNames
      };
  }

  public class PercentMatrixConverter : ValueTypeMatrixConverter<PercentMatrix, double> {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentMatrix);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      PercentMatrix mat = item as PercentMatrix;
      DoubleMatrixJsonItem d = data as DoubleMatrixJsonItem;
      CopyMatrixData(mat, d.Value);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleMatrixJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = Transform((PercentMatrix)value),
        Minimum = 0.0d,
        Maximum = 1.0d
      };
  }

  public class BoolMatrixConverter : ValueTypeMatrixConverter<BoolMatrix, bool> {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolMatrix);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      BoolMatrix mat = item as BoolMatrix;
      BoolMatrixJsonItem d = data as BoolMatrixJsonItem;
      CopyMatrixData(mat, d.Value);
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new BoolMatrixJsonItem() {
        //Name = "[OverridableParamName]",
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = Transform((BoolMatrix)value)
      };
  }

  public abstract class ValueTypeMatrixConverter<MatrixType, T> : BaseConverter
    where MatrixType : ValueTypeMatrix<T> 
    where T : struct 
  {
    #region Helper
    protected void CopyMatrixData(MatrixType matrix, T[][] data) {
      var cols = data.Length;
      var rows = data.Length > 0 ? data[0].Length : 0;

      var rowInfo = matrix.GetType().GetProperty("Rows");
      rowInfo.SetValue(matrix, rows);
      var colInfo = matrix.GetType().GetProperty("Columns");
      colInfo.SetValue(matrix, cols);

      for (int x = 0; x < rows; ++x) {
        for (int y = 0; y < cols; ++y) {
          matrix[x, y] = data[y][x];
        }
      }
    }

    protected T[][] Transform(MatrixType matrix) {
      T[][] m = new T[matrix.Columns][];
      for (int column = 0; column < matrix.Columns; ++column) {
        m[column] = new T[matrix.Rows];
        for (int row = 0; row < matrix.Rows; ++row) {
          m[column][row] = matrix[row, column];
        }
      }
      return m;
    }
    #endregion
  }
}
