using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  public abstract class BaseDao<TBusiness, TDatabaseEntity> {
    public static HiveDataContext Context {
      get {
        return ContextFactory.Context;
      }
    }

    public abstract TDatabaseEntity DtoToEntity(TBusiness source, TDatabaseEntity target);
    public abstract TBusiness EntityToDto(TDatabaseEntity source, TBusiness target);

  }
}
