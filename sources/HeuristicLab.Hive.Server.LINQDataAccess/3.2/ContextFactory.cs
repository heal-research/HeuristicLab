using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public class ContextFactory {
    [ThreadStatic]
    private static HiveDataContext _hiveDataContext = null;

    [ThreadStatic]
    private static SqlTransaction _currentTransaction = null;

    public static HiveDataContext Context { 
      get {
        if(_hiveDataContext == null) 
          _hiveDataContext = new HiveDataContext();
        return _hiveDataContext;        
      } 
      set {        
        _hiveDataContext = value;
      }
    }

    public static SqlTransaction CurrentTransaction { 
      get {
        return _currentTransaction;
      } set {
        _currentTransaction = value;
      } 
    }
  }
}
