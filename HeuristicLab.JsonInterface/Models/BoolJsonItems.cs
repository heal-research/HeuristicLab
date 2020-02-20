using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class BoolJsonItem : JsonItem<bool> { }

  public class BoolArrayJsonItem : ArrayJsonItemBase<bool> { }

  public class BoolMatrixJsonItem : MatrixJsonItemBase<bool> { }
}
