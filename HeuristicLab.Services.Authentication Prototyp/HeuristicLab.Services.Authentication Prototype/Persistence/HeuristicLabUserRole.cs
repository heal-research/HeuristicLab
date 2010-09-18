using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Persistence {
  partial class HeuristicLabUserRole {
    public override bool Equals(object obj) {
        if(this.ID == null ^ ((HeuristicLabUserRole)obj).ID == null )
        {
          return false;
        }
        else if (this.ID == null && ((HeuristicLabUserRole)obj).ID == null)
        {
         return base.Equals(obj);
        }
        return ((HeuristicLabUserRole)obj).ID.Equals(this.ID);
    }
  }
}
