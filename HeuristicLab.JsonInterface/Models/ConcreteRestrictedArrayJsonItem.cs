using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {
  public abstract class ConcreteRestrictedArrayJsonItem<T> : ArrayJsonItem<T>, IConcreteRestrictedJsonItem<T> {
    public IEnumerable<T> ConcreteRestrictedItems { get; set; }

    protected override bool Validate() {
      bool res = true;
      foreach(var x in Value) {
        bool tmp = false;
        foreach(var restrictedItem in ConcreteRestrictedItems) {
          tmp = tmp || x.Equals(restrictedItem); //if one tmp is true, it stays true (match found)
        }
        res = res && tmp; //if one tmp is false, res will set false
      }
      return res;
    }
  }
}
