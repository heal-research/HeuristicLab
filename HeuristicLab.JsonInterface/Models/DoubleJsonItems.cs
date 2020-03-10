using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class DoubleJsonItem : IntervalRestrictedValueJsonItem<double> { }
  public class DoubleArrayJsonItem : IntervalRestrictedArrayJsonItem<double> { }
  public class DoubleRangeJsonItem : RangedJsonItem<double> { }

  public class DoubleMatrixJsonItem : IntervalRestrictedMatrixJsonItem<double> {
    /*
    protected override bool IsInRange() {
      for (int c = 0; c < Value.Length; ++c) {
        for (int r = 0; r < Value[c].Length; ++r) {
          if (Value[c][r] < Range.First() && Range.Last() < Value[c][r])
            return false;
        }
      }
      return true;
    }
    */
  }
}
