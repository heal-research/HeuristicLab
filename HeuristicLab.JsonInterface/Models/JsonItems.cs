using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class IntJsonItem : JsonItem<int> {}
  public class IntArrayJsonItem: JsonItem<int[], int> { }
  public class IntRangeJsonItem : JsonItem<int[], int> { }
  public class IntMatrixJsonItem : JsonItem<int[][], int> {
    protected override bool IsInRange() {
      for (int c = 0; c < Value.Length; ++c) {
        for (int r = 0; r < Value[c].Length; ++r) {
          if (Value[c][r] < Range.First() && Range.Last() < Value[c][r])
            return false;
        }
      }
      return true;
    }
  }

  public class DoubleJsonItem: JsonItem<double> {}
  public class DoubleArrayJsonItem: JsonItem<double[], double> { }
  public class DoubleRangeJsonItem : JsonItem<double[], double> { }
  public class DoubleMatrixJsonItem : JsonItem<double[][], double> {
    protected override bool IsInRange() {
      for(int c = 0; c < Value.Length; ++c) {
        for(int r = 0; r < Value[c].Length; ++r) {
          if (Value[c][r] < Range.First() && Range.Last() < Value[c][r])
            return false;
        }
      }
      return true;
    }
  }

  public class BoolJsonItem: JsonItem<bool> { }
  public class BoolArrayJsonItem : JsonItem<bool[], bool> { }
  public class BoolMatrixJsonItem : JsonItem<bool[][], bool> { }

  public class StringJsonItem: JsonItem<string> {}

  public class DateTimeJsonItem: JsonItem<DateTime> {}
}
