using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.Interfaces;

namespace HeuristicLab.DataAccess.ADOHelper {
  public class ManyToManyRelation: PersistableObject {
    public Guid Id2 { get; set; }

    public IList<object> AdditionalAttributes { get; set; }
  }
}
