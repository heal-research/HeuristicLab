using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public class StringJsonItem : ConcreteRestrictedValueJsonItem<string> { }
  public class StringArrayJsonItem : ConcreteRestrictedArrayJsonItem<string> { }
}
