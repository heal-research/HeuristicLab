using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class IntJsonItem : IntervalRestrictedValueJsonItem<int> { }
  public class IntArrayJsonItem : IntervalRestrictedArrayJsonItem<int> { }
  public class IntRangeJsonItem : RangedJsonItem<int> { }
  public class IntMatrixJsonItem : IntervalRestrictedMatrixJsonItem<int> {
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
