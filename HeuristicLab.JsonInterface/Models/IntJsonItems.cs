using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class IntJsonItem : JsonItem<int> {
    /*I
    public int MinValue { get; set; } 
    public int MaxValue { get; set; }
    */
  }
  public class IntArrayJsonItem : ArrayJsonItemBase<int> { }
  public class IntRangeJsonItem : ArrayJsonItemBase<int> {
    public override bool Resizable { get => false; set { } }
  }
  public class IntMatrixJsonItem : MatrixJsonItemBase<int> {

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
}
