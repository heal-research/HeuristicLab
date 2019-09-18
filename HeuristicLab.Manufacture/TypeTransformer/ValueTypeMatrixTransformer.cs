using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using HeuristicLab.Data;
using HeuristicLab.Core;

namespace ParameterTest {
  public class ValueTypeMatrixTransformer<MatrixType, T> : BaseTransformer
    where MatrixType : ValueTypeMatrix<T> 
    where T : struct 
  {
    public override IItem FromData(ParameterData obj, Type targetType) {
      /*T[,] arr = CastValue<T[,]>(obj.Default);
      MatrixType matrix = item.Cast<MatrixType>();
      for (int x = 0; x < arr.GetLength(0); ++x){
        for (int y = 0; y < arr.GetLength(1); ++y) {
          matrix[x, y] = arr[x, y];
        }
      }*/
      return Instantiate<MatrixType>(CastValue<T[,]>(obj.Default));
    }

    public override void SetValue(IItem item, ParameterData data) {
      T[,] arr = CastValue<T[,]>(data.Default);
      MatrixType matrix = item.Cast<MatrixType>();
      for (int x = 0; x < arr.GetLength(0); ++x) {
        for (int y = 0; y < arr.GetLength(1); ++y) {
          matrix[x, y] = arr[x, y];
        }
      }
    }

    public override ParameterData ToData(IItem value) {
      ParameterData data = base.ToData(value);
      data.Default = value.Cast<MatrixType>().CloneAsMatrix();
      return data;
    }
  }
}
