using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class BoolJsonItem : ValueJsonItem<bool> {
    protected override bool Validate() => true;
  }

  public class BoolArrayJsonItem : ArrayJsonItem<bool> {
    protected override bool Validate() => true;
  }

  public class BoolMatrixJsonItem : MatrixJsonItem<bool> {
    protected override bool Validate() => true;
  }
}
