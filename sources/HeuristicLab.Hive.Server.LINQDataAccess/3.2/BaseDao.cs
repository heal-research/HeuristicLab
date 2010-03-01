using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class BaseDao {
    public static HiveDataContext Context {
      get {
        return ContextFactory.Context;
      }
    }
  }
}
