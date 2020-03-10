using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public abstract class ConcreteRestrictedValueJsonItem<T> : ValueJsonItem<T>, IConcreteRestrictedJsonItem<T> {
    public IEnumerable<T> ConcreteRestrictedItems { get; set; }

    protected override bool Validate() {
      if (ConcreteRestrictedItems == null) return true;
      foreach (var x in ConcreteRestrictedItems)
        if (Value.Equals(x)) return true;
      return false;
    }
  }
}
