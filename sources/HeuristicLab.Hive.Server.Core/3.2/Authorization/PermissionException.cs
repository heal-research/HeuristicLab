using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.Core {
  public class PermissionException : FaultException {
    public PermissionException()
      : base("Current user has insufficent rights for this action!") {
    }

    public PermissionException(string msg)
      : base("Current user has insufficent rights for this action: " + msg) {
    }
  }
}
