using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class ContextFactory {
    [ThreadStatic]
    private static HiveDataContext _hiveDataContext;

    public static HiveDataContext Context { 
      get {
        if(_hiveDataContext == null) 
          _hiveDataContext = new HiveDataContext();
        return _hiveDataContext;        
      } 
    }
  }
}
